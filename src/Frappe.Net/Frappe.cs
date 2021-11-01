using Frappe.Net.Authorization;
using log4net;
using log4net.Config;
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
        public Db Db { get =>_db; }
        public bool IsAuthenticated { get => _isAuthenticated; set => _isAuthenticated = value; }
        public TinyRestClient Client { get => client; set => client = value; }

        private static readonly ILog log = LogManager.GetLogger(typeof(Frappe));

        /// <summary>
        /// Class contructor
        /// 
        /// </summary>
        /// <param name="baseUrl">Base url to the frappe site</param>
        /// <param name="debug">if true Frappe will logs all requests in debug console</param>
        public Frappe(string baseUrl, bool debug=false) {
            _baseUrl = baseUrl;
            client = new TinyRestClient(new HttpClient(), $"{baseUrl}/api/method");
            if (debug)
                client.Settings.Listeners.AddDebug();
            _db = new Db(this);
            BasicConfigurator.Configure();
        }

        public void ChangeRoute(string route) {
            var headers = client.Settings.DefaultHeaders;
            client = new TinyRestClient(new HttpClient(), $"{_baseUrl}{route}");
            foreach (var h in headers) {
                var e = h.Value.GetEnumerator();
                e.MoveNext();
                string value = (string)e.Current;
                client.Settings.DefaultHeaders.Add(h.Key, value);
            }
        }

        public void ResetRoute()
        {
            ChangeRoute("/api/method");
        }

        /// <summary>
        /// Login to Frappe sight using access token
        /// 
        /// </summary>
        /// <param name="accessToken">The apps access token</param>
        /// <returns></returns>
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

        /// <summary>
        /// Add authorization token by default to all request headers
        /// whithout validation
        /// </summary>
        /// <param name="apiKey">User API key</param>
        /// <param name="apiSecret">User API secret</param>
        /// <returns></returns>
        public Frappe SetToken(string apiKey, string apiSecret)
        {
            ClearAuthorization();
            client.Settings.DefaultHeaders.Add("Authorization", $"token {apiKey}:{apiSecret}");
            this._isToken = true;
            return this;
        }

        /// <summary>
        /// Adds authorization token to by default to all request headers and
        /// validates it by getting the current logged in user
        /// 
        /// </summary>
        /// <param name="apiKey">User API key</param>
        /// <param name="apiSecret">User API secret</param>
        /// <returns></returns>
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

        /// <summary>
        /// Login to a Frappe Framework site with traditional username/email
        /// and password
        /// 
        /// </summary>
        /// <param name="email">Username or email</param>
        /// <param name="password">user password</param>
        /// <returns></returns>
        public async Task<Frappe> UsePasswordAsync(string email, string password)
        {
            ClearAuthorization();
            try
            {
                await client.PostRequest("login", new EmailPasswordPair() { usr = email, pwd = password })
                    .ExecuteAsStringAsync();
            }
            catch (HttpException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    var msg = "Invalid login credential";
                    log.Error($"{msg} >>> {e.Message}");
                    throw new AuthenticationException();
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

        /// <summary>
        /// Gets the currently logged user
        /// </summary>
        /// <returns>The name of the logged in user</returns>
        public async Task<string> GetLoggedUserAsync() {
            var response =  await client.GetRequest("frappe.auth.get_logged_user")
                    .ExecuteAsStringAsync();
            return ToObject(response).message;
        }

        /// <summary>
        /// Clear authorization header and reset flags
        /// 
        /// </summary>
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

        public async Task<string> PingAsync()
        {
            var response = await client.GetRequest("frappe.ping").ExecuteAsStringAsync();
            return ToObject(response).message.ToString();
        }
    }
}
