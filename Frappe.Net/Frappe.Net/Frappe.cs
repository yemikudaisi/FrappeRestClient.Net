using Frappe.Net.Authorization;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Security.Authentication;
using System.Threading.Tasks;
using Tiny.RestClient;

namespace Frappe.Net
{
    /// <summary>
    /// Entry class in to the frappe REST client
    /// manages all communication to the remote Frappe Framework deployment
    /// 
    /// </summary>
    public class Frappe : JsonObjectParser
    {
        string _baseUrl;
        TinyRestClient client;
        private bool _isAuthenticated;
        private bool _isPassword;
        private bool _isToken;
        private bool _isAccessToken;
        private Db _db;
        private const string METHOD_PATH = "method/";
        public Db Db { get =>_db; }
        public bool IsAuthenticated { get => _isAuthenticated; set => _isAuthenticated = value; }

        public Frappe(string baseUrl) {
            _baseUrl = baseUrl;
            client = new TinyRestClient(new HttpClient(), $"{baseUrl}/api");
            _db = new Db(client);
        }

        public async Task<Frappe> UseAccessTokenAsync(string accessToken)
        {
            // TODO: Setup frappe access token service
            ClearAuthorization();
            client.Settings.DefaultHeaders.Add("Authorization", $"Bearer {accessToken}");

            // validation
            try
            {
                await this.GetLoggedUserAsync();
                this._isToken = true;
            }
            catch (HttpException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new AuthenticationException("Invalid login credential");
                }
                if (e.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    throw new AuthenticationException("Server error");
                }
            }
            return this;
        }

        public async Task<Frappe> UseTokenAsync(string apiKey, string apiSecret) {
            ClearAuthorization();
            client.Settings.DefaultHeaders.Add("Authorization", $"token {apiKey}:{apiSecret}");

            // validation
            try
            {
                await this.GetLoggedUserAsync();
            }
            catch (HttpException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new AuthenticationException("Invalid login credential");
                }
                if (e.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    throw new AuthenticationException("Server error");
                }
            }

            _isAccessToken = true;
            _isAuthenticated = true;
            return this;
        }

        public async Task<Frappe> UsePasswordAsync(string email, string password)
        {
            ClearAuthorization();
            try
            {
                await client.PostRequest(getUri("login"), new EmailPasswordPair() { usr = email, pwd = password })
                    .ExecuteAsStringAsync();
            }
            catch (HttpException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new AuthenticationException("Invalid login credential");
                }
                if (e.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    throw new AuthenticationException("Server error");
                }
            }

            this._isPassword = true;
            this._isAuthenticated = true;
            return this;
        }

        public async Task<string> GetLoggedUserAsync() {
            var response =  await client.GetRequest(getUri("frappe.auth.get_logged_user"))
                    .ExecuteAsStringAsync();
            return ToObject(response).message;
        }

        private void ClearAuthorization() {
            client.Settings.DefaultHeaders.Remove("Authorization");
            _isAuthenticated = false;
            _isToken = false;
            _isPassword = false;
            _isAccessToken = false;
        }

        public void Logout()
        {
            ClearAuthorization();
        }

        private string getUri(string method) {
            return METHOD_PATH + method;
        }
    }
}
