
using System;
using System.Threading.Tasks;


namespace HIM.Services.Transport
{
    public class CredentialManager
    {
        public Task _initTask;
        private bool _initialized = true;
        private string _password;
        private string _username;

        public CredentialManager(string username, string password)
        {
            _initTask = Task.CompletedTask;
            Username = username;
            Password = password;
        }

        public bool IntegratedAuth => string.IsNullOrEmpty(Username);

        public string Password
        {
            get
            {
                if (!_initialized)
                {
                    _initTask.Wait();
                }
                return _password;
            }
            set => _password = value;
        }

        public string Username
        {
            get
            {
                if (!_initialized)
                {
                    _initTask.Wait();
                }
                return _username;
            }
            set => _username = value;
        }


    }
}