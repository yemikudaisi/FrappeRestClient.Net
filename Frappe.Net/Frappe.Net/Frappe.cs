using Frappe.Net.Authorization;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Security.Authentication;
using System.Threading.Tasks;
using Tiny.RestClient;

namespace Frappe.Net
{
    public class Frappe
    {
        string _baseUrl;
        string _userName;
        string _password;
        TinyRestClient client;
        bool _isAuthenticated;
        bool _isPassword;
        bool _isToken;
        bool _isAccessToken;

        public Frappe(string baseUrl) {
            _baseUrl = baseUrl;
            client = new TinyRestClient(new HttpClient(), $"{baseUrl}/api/method");
            //client.Settings.DefaultHeaders.Add("Accept", "application/json");
            //client.Settings.DefaultHeaders.Add("Content-Type", "application/json");
        }

        public async Task<Frappe> UseTokenAsync(string apiSecret, string apiKey) {
            client.Settings.DefaultHeaders.Add("Authorization", $"token {apiKey}:{apiSecret}");
            // validates the token by getting logged user
            try
            {
                await this.GetLoggedUserAsync();
            }
            catch (HttpException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    client.Settings.DefaultHeaders.Remove("Authorization");
                    throw new AuthenticationException("Authentication failed");
                }
            }
            return this;

        }

        public async Task<Frappe> UsePasswordAsync(string email, string password)
        {
            try
            {
                await client.PostRequest("login", new EmailPasswordPair() { usr = email, pwd = password })
                    .ExecuteAsStringAsync();
            }
            catch (HttpException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new AuthenticationException("Authentication failed");
                }
            }

            this._isPassword = true;
            this._isAuthenticated = true;
            return this;
        }

        public async Task<string> GetLoggedUserAsync() {
            var response =  await client.GetRequest("frappe.auth.get_logged_user")
                    .ExecuteAsStringAsync();
            return ToDynamic(response).message;
        }

        private dynamic ToDynamic(string json) {
            return JObject.Parse(json);
        }

    }
}
