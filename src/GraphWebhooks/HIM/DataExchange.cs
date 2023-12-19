using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HIM.Services.Transport
{
    public class DataExchange
    {
        public const string EndpointKey = "Endpoint";
        public readonly string EndpointDefault = "https://test-him.ondataport.de/app/api";
        public static string SessionKey { get; private set; }

        private readonly ILogger<DataExchange> _logger;
        private readonly CredentialManager _credentialManager;
        public const string UserAgent = "WF-Application";
        const string CitrixXmlNamespace = "http://citrix.com/authentication/response/1";

        public DataExchange(ILogger<DataExchange> logger, CredentialManager credentialManager)
        {
            _logger = logger;
            _credentialManager = credentialManager;
        }

        private static bool DoMatch(string url, Uri responseUri)
        {
            var uri = new Uri(url);
            return uri.Equals(responseUri);
        }

        internal async Task<T> GetJsonAsync<T>(string uri, bool forceIntegratedAuth = false)
        {
            uri = GetUri(uri);

            _logger.LogDebug($"Get data from webservice {uri}");

            var data = await GetStringAsync(uri, hc => hc.GetAsync(uri), false, forceIntegratedAuth);
            if (_logger.IsEnabled(LogLevel.Trace))
            {
                _logger.LogTrace("Response JSON: " + data);
            }
            var retVal = JsonConvert.DeserializeObject<T>(data);

            return retVal;
        }
        internal async Task<byte[]> GetBinaryAsync(string uri, string mediaType)
        {
            uri = GetUri(uri);

            _logger.LogDebug($"Get data from url {uri}");

            var data = await GetDataAsync(uri, hc => hc.GetAsync(uri), mediaType);
            var retVal = await data.ReadAsByteArrayAsync();

            return retVal;
        }
        internal async Task<Stream> GetStreamAsync(string uri)
        {
            uri = GetUri(uri);

            _logger.LogDebug($"Get stream from url {uri}");

            var data = await GetDataAsync(uri, hc => hc.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead), null, false);
            var retVal = await data.ReadAsStreamAsync();

            return retVal;
        }
        internal async Task PostStreamAsync(string uri, HttpContent httpContent, CancellationToken cancellationToken = default)
        {
            uri = GetUri(uri);

            _logger.LogDebug($"Post content to url {uri}");

            var data = await GetDataAsync(uri, hc => hc.PostAsync(uri, httpContent), null, false);
        }

        internal async Task<string> GetStringAsync(string uri, bool isJournal = false, bool forceIntegratedAuth = false)
        {
            uri = GetUri(uri);

            _logger.LogDebug($"Get data from webservice {uri} as string");

            return await GetStringAsync(uri, hc => hc.GetAsync(uri), isJournal, forceIntegratedAuth);
        }

        internal async Task PatchAsync(string uri, object patchContent, CancellationToken cancellationToken)
        {
            uri = GetUri(uri);
            _logger.LogDebug($"Patch data to webservice {uri}");

            var json = JsonConvert.SerializeObject(patchContent);

            await GetStringAsync(uri, hc => hc.PatchAsync(uri, new StringContent(json, Encoding.UTF8, "application/json"), cancellationToken));
        }

        internal async Task<T> PostJsonAsync<T>(string uri, object postContent, CancellationToken cancellationToken)
        {
            uri = GetUri(uri);
            _logger.LogDebug($"Post data to webservice {uri}");

            var json = JsonConvert.SerializeObject(postContent);

            var data = await GetStringAsync(uri, hc => hc.PostAsync(uri, new StringContent(json, Encoding.UTF8, "application/json"), cancellationToken));
            var retVal = JsonConvert.DeserializeObject<T>(data);

            return retVal;
        }

        private string GetUri(string relativeUri)
        {
            if (relativeUri.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
            {
                return relativeUri;
            }
            string url = EndpointDefault;
            return url + relativeUri;
        }

        internal HttpClient GetHttpClient(string uri, bool forceIntegratedAuth, out HttpClientHandler handler)
        {
            handler = new HttpClientHandler();

            #region Authentication

            if (forceIntegratedAuth || _credentialManager.IntegratedAuth)
            {
                _logger.LogDebug("Integrated authentication is used");
                handler.UseDefaultCredentials = true;
            }
            else
            {
                if (_credentialManager.Username.Contains("\\"))
                {
                    handler.Credentials = new NetworkCredential(_credentialManager.Username, _credentialManager.Password);
                }
                else
                {
                    handler.Credentials = new NetworkCredential(_credentialManager.Username, _credentialManager.Password, Environment.UserDomainName);
                }
            }

            handler.PreAuthenticate = true;

            #endregion Authentication

            if (handler.SupportsAutomaticDecompression)
            {
                handler.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            }

            if (handler.CookieContainer == null) handler.CookieContainer = new CookieContainer();
            if (!forceIntegratedAuth && !string.IsNullOrEmpty(SessionKey))
            {
                _logger.LogDebug($"Zuvex session is used");
                var baseUrl = new Uri(new Uri(uri), "/");

                handler.CookieContainer.Add(baseUrl, new Cookie("NSC_TMAS", SessionKey));
            }

            var client = new HttpClient(handler);
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue(DataExchange.UserAgent, "1"));
            client.DefaultRequestHeaders.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("(WinUI)"));

            return client;
        }

        public async Task AskForCredentials()
        {
            throw new Exception("AskForCredentials");
        }

        private async Task AuthenticateZuvex(string url)
        {
            SessionKey = null;
            if (_credentialManager.IntegratedAuth)
            {
                await AskForCredentials();
            }
            var hc = GetHttpClient(url, false ,out HttpClientHandler handler);

            _logger.LogDebug($"Username and Password is used {_credentialManager.Username} for {url}");

            var baseUrl = new Uri(new Uri(url), "/");
            var initialResp = await hc.GetAsync(url);
            var html = await initialResp.Content.ReadAsStringAsync();
            if (html.StartsWith('{'))
            {
                Debugger.Break();
            }
            var start = html.IndexOf("action=\"") + 8;
            var end = html.IndexOf("\"", start);
            var loginUrl = html.Substring(start, end - start);
            start = html.IndexOf("value=\"") + 7;
            end = html.IndexOf("\"", start);
            var SAMLRequest = html.Substring(start, end - start);
            start = html.IndexOf("value=\"", end) + 7;
            end = html.IndexOf("\"", start);
            var RelayState = html.Substring(start, end - start);

            var data = new[]
                    {
                        new KeyValuePair<string, string>("SAMLRequest", SAMLRequest),
                        new KeyValuePair<string, string>("RelayState", RelayState),
                    };
            var cnt = new FormUrlEncodedContent(data);

            var samlLoginreq = new HttpRequestMessage();
            samlLoginreq.Content = cnt;
            samlLoginreq.Headers.Add("Origin", baseUrl.ToString());
            samlLoginreq.Headers.Referrer = baseUrl;
            samlLoginreq.RequestUri = new Uri(loginUrl);
            samlLoginreq.Method = HttpMethod.Post;
            var samlLogin = await hc.SendAsync(samlLoginreq);
            var samlResp = await samlLogin.Content.ReadAsStringAsync();


            var uri = new Uri(samlLogin.RequestMessage.RequestUri, "/nf/auth/getAuthenticationRequirements.do");
            var response = await hc.PostAsync(uri, new StringContent(""));
            response.EnsureSuccessStatusCode();
            var xml = await response.Content.ReadAsStringAsync();
            var xDoc = XDocument.Parse(xml);
            var xStateContext = xDoc.Descendants(XName.Get("StateContext", CitrixXmlNamespace)).FirstOrDefault();
            var xDomain = xDoc.Descendants(XName.Get("InitialSelection", CitrixXmlNamespace)).FirstOrDefault();
            var xPostBack = xDoc.Descendants(XName.Get("PostBack", CitrixXmlNamespace)).FirstOrDefault();
            var domain = xDomain?.Value;
            var stateContext = xStateContext?.Value;
            var postBack = xPostBack?.Value;


            var formData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("login", _credentialManager.Username),
                new KeyValuePair<string, string>("passwd", _credentialManager.Password),
                new KeyValuePair<string, string>("domain", domain),
                new KeyValuePair<string, string>("loginBtn", ""),
                new KeyValuePair<string, string>("StateContext", stateContext)
            };
            uri = new Uri(response.RequestMessage.RequestUri, postBack);
            response = await hc.PostAsync(uri, new FormUrlEncodedContent(formData));
            response.EnsureSuccessStatusCode();
            xml = await response.Content.ReadAsStringAsync();
            xDoc = XDocument.Parse(xml);
            var xRedirectUrl = xDoc.Descendants(XName.Get("RedirectURL", CitrixXmlNamespace)).FirstOrDefault();
            var redirectUrl = xRedirectUrl?.Value;
            var xResult = xDoc.Descendants(XName.Get("Result", CitrixXmlNamespace)).FirstOrDefault();
            if (xResult?.Value == "update-credentials")
            {
                await AlertUIAsync("Das Kennwort für den Zugang muss geändert werden. Sie werden dazu auf die Webseite geleitet.");
            }
            if (xResult?.Value == "more-info")
            {
                await AlertUIAsync("Die Benutzerdaten wurden vom Zuvex-System abgelehnt. Bitte prüfen Sie die Zugangsdaten. Falls der Fehler erneut auftritt, wenden Sie sich an den Administrator.");
                return;
            }

            var postAssertion = await hc.GetAsync(redirectUrl);
            var postAssertionContent = await postAssertion.Content.ReadAsStringAsync();
            start = postAssertionContent.IndexOf("action=\"") + 8;
            end = postAssertionContent.IndexOf("\"", start);
            var samlAuthUrl = postAssertionContent.Substring(start, end - start);
            start = postAssertionContent.IndexOf("value=\"") + 7;
            end = postAssertionContent.IndexOf("\"", start);
            SAMLRequest = postAssertionContent.Substring(start, end - start);
            start = postAssertionContent.IndexOf("value=\"", end) + 7;
            end = postAssertionContent.IndexOf("\"", start);
            RelayState = postAssertionContent.Substring(start, end - start);
            data = new[]
                    {
                        new KeyValuePair<string, string>("SAMLResponse", SAMLRequest),
                        new KeyValuePair<string, string>("RelayState", RelayState),
                    };
            cnt = new FormUrlEncodedContent(data);


            var baseUrlIdp = new Uri(response.RequestMessage.RequestUri, "/");

            var samlAuthreq = new HttpRequestMessage();
            samlAuthreq.Content = cnt;
            samlAuthreq.Headers.Add("Origin", baseUrlIdp.ToString());
            samlAuthreq.Headers.Referrer = baseUrlIdp;
            samlAuthreq.RequestUri = new Uri(samlAuthUrl);
            samlAuthreq.Method = HttpMethod.Post;
            var samlAuth = await hc.SendAsync(samlAuthreq);
            var samlAuthResp = await samlAuth.Content.ReadAsStringAsync();


            //start = samlAuthResp.IndexOf("href=\"") + 6;
            //end = samlAuthResp.IndexOf("\"", start);
            //var selfAuthUrl = samlAuthResp.Substring(start, end - start);

            //var selfAuthResp= await hc.GetAsync(selfAuthUrl);
            //var samlAuthResp2 = await selfAuthResp.Content.ReadAsStringAsync();
            if (samlAuth.Headers.TryGetValues("Set-Cookie", out IEnumerable<string> setCookieHeader))
            {
                foreach (var setCookieString in setCookieHeader)
                {
                    if (setCookieString.StartsWith("NSC_TMAS"))
                    {
                        var parts = setCookieString.Split(';');
                        var parts2 = parts[0].Split('=');
                        SessionKey = parts2[1];
                        break;
                    }
                }
            }
            //var cookies = handler.CookieContainer.GetCookies(baseUrl);
            //SessionKey = cookies["NSC_TMAS"].Value;

            if (SessionKey == null) throw new UnauthorizedAccessException();
        }

        private Task AlertUIAsync(string message)
        {
            throw new Exception(message);
        }

        private async Task<string> GetStringAsync(string url, Func<HttpClient, Task<HttpResponseMessage>> requestDelegate, bool isJournal = false, bool forceIntegratedAuth = false)
        {
            var content = await GetDataAsync(url, requestDelegate, isJournal ? "text/html" : "application/json", !forceIntegratedAuth, forceIntegratedAuth);
            if (content == null) return string.Empty;
            return await content.ReadAsStringAsync();
        }

        private async Task<HttpContent> GetDataAsync(string url, Func<HttpClient, Task<HttpResponseMessage>> requestDelegate, string expectedType, bool interactive = true, bool forceIntegratedAuth = false)
        {
            if (forceIntegratedAuth && interactive) throw new ArgumentException("Forced Integrated Auth cannot be used with interactive set to true");
            var hc = GetHttpClient(url, forceIntegratedAuth, out HttpClientHandler handler);

            //var response = await hc.GetAsync(url);
            var response = await requestDelegate(hc);


            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                if (response.RequestMessage.Method == HttpMethod.Get)
                {
                    if (interactive)
                    {
                        //_credentialManager.Clear();
                    }
                }
                else
                {
                    //server should send 403 forbidden
                    response.EnsureSuccessStatusCode();
                }
            }
            else
            {
                await CheckForServerErrorMessage(response);

                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "HTTP request error");
                    throw;
                }

                if (response.Content.Headers.ContentType == null && DoMatch(url, response.RequestMessage.RequestUri))
                    return null;

                if ((expectedType == null || response.Content.Headers.ContentType.MediaType == expectedType)
                    && DoMatch(url, response.RequestMessage.RequestUri))
                {
                    //received data
                    return response.Content;
                }
            }

            do
            {
                if (!interactive) throw new UnauthorizedAccessException();
                await AuthenticateZuvex(url);
            } while (SessionKey == null);

            return await GetDataAsync(url, requestDelegate, expectedType, interactive);
        }

        private async Task CheckForServerErrorMessage(HttpResponseMessage response)
        {
            string errorMsg =null;
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                try
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var jsonResponse = JObject.Parse(content);
                    if (jsonResponse?["errors"] != null)
                    {
                        foreach (var errorScope in jsonResponse["errors"])
                        {
                            foreach (var errorList in errorScope)
                            {
                                foreach (var error in errorList)
                                {
                                    errorMsg = error.Value<string>();
                                }
                            }
                            
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
            if (errorMsg != null)
            {
                throw new Exception(errorMsg);
            }
        }

        internal async Task DeleteAsync(string uri, CancellationToken cancellationToken)
        {
            uri = GetUri(uri);

            _logger.LogDebug($"Delete data from webservice {uri} as string");

            await GetStringAsync(uri, hc => hc.DeleteAsync(uri, cancellationToken), false);
        }
    }
}