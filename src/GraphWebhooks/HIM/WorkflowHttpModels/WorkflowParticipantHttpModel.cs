using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIM.Services.DataModels.WorkflowHttpModels
{
    [Obsolete]
    public class WorkflowParticipantHttpModel
    {
        //        GET /read/api/workflows/57cf8ab7-d880-4bfe-9808-2f6fc23dc27a/ccparticipants? tenantUrl = https://fhhportal.ondataport.de/websites/0061HIM&noCache=1656665197351 HTTP/1.1
        //Accept: application/json, text/plain, */*
        //Accept-Encoding: gzip, deflate, br
        //Accept-Language: en-US,en;q=0.9,de;q=0.8,de-DE;q=0.7,en-GB;q=0.6
        //Cache-Control: no-cache
        //Connection: keep-alive
        //Content-Type: application/json
        //Cookie: NSC_TMAS=e75501f1b6dc381fcd0fd334b94981a3; NSC_PERS=c8097eddd5454d73b6a19679d2130ed9
        //DNT: 1
        //Host: him.ondataport.de
        //Origin: https://fhhportal.ondataport.de
        //Pragma: no-cache
        //Referer: https://fhhportal.ondataport.de/
        //Sec-Fetch-Dest: empty
        //Sec-Fetch-Mode: cors
        //Sec-Fetch-Site: same-site
        //User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/103.0.5060.66 Safari/537.36 Edg/103.0.1264.44
        //sec-ch-ua: " Not;A Brand";v="99", "Microsoft Edge";v="103", "Chromium";v="103"
        //sec-ch-ua-mobile: ?0
        //sec-ch-ua-platform: "Windows"




        //https://him.ondataport.de/read/api/workflows/57cf8ab7-d880-4bfe-9808-2f6fc23dc27a/ccparticipants?tenantUrl=https://fhhportal.ondataport.de/websites/0061HIM&noCache=1656665197351
        //{"data":{"permanentCcParticipants":[{"id":310330,"principalType":0,"displayName":"Tandler, Matthias","eMail":"matthias.tandler@microsoft.com","login":"fhhnet\\TandleMa","shortcut":"-/- Zuvex (Extern)","isWorkflowAdministrator":false,"isParticipant":false,"isParticipantWithoutStartRights":false,"lastUpdate":"2022-06-30T11:29:57.994128+02:00","sid":"S-1-5-21-2000478354-764733703-1177238915-1049388"}],"temporaryCcParticipants":[]},"timeStamp":"637922691972899674"}


        public List<UserHttpModel> permanentCcParticipants { get; set; }
        public List<UserHttpModel> temporaryCcParticipants { get; set; }
    }
}
