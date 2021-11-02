namespace Frappe.Net
{
    using System;
    using System.Net.Http;
    using System.Security.Authentication;
    using System.Threading.Tasks;
    using global::FrappeRestClient.Net.Authorization;
    using log4net;
    using log4net.Config;
    using Newtonsoft.Json.Linq;
    using Tiny.RestClient;

    /// <summary>
    /// Entry class in to the frappe REST client
    /// manages all communication to the remote Frappe Framework deployment.
    /// </summary>
    public class FrappeRestClient : JsonObjectParser
    {
        private string baseUrl;
        private TinyRestClient client;
        private bool isAuthenticated;
        private bool isPassword;
        private bool isToken;
        private bool isAccessToken;
        private Db db;

        /// <summary>
        /// Gets the DB object.
        /// </summary>
        public Db Db { get => this.db; }

        /// <summary>
        /// Gets a value indicating whether the Frappe client is authenticated.
        /// </summary>
        public bool IsAuthenticated { get => this.isAuthenticated;}

        /// <summary>
        /// Gets or sets an instance of <see cref="TinyRestClient">TinyRestClient</see>.
        /// </summary>
        /// <see cref="TinyRestClient"/>
        public TinyRestClient Client { get => this.client; set => this.client = value; }

        private static readonly ILog log = LogManager.GetLogger(typeof(FrappeRestClient));

        /// <summary>
        /// Initializes a new instance of the <see cref="FrappeRestClient"/> class.
        /// </summary>
        /// <param name="baseUrl">Base url to the frappe site.</param>
        /// <param name="debug">If true, Frappe will logs all requests in debug console.</param>
        public FrappeRestClient(string baseUrl, bool debug=false)
        {
            this.baseUrl = baseUrl;
            this.client = new TinyRestClient(new HttpClient(), $"{baseUrl}/api/method");
            if (debug)
            {
                this.client.Settings.Listeners.AddDebug();
            }

            this.db = new Db(this);
            BasicConfigurator.Configure();
        }

        /// <summary>
        /// Changes the route from the default route
        /// to supplied route.
        /// </summary>
        /// <param name="route">The new route for subsequent calls.</param>
        public void ChangeRoute(string route)
        {
            if (this.isAccessToken || this.isToken)
            {
                var headers = this.client.Settings.DefaultHeaders;
                this.client = new TinyRestClient(new HttpClient(), $"{this.baseUrl}{route}");
                foreach (var h in headers)
                {
                    var e = h.Value.GetEnumerator();
                    e.MoveNext();
                    string value = (string)e.Current;
                    this.client.Settings.DefaultHeaders.Add(h.Key, value);
                }
            }
            else if (this.isPassword) {
                throw new InvalidOperationException("Username and password used. Please create new Frappe client instance");
            }
        }

        /// <summary>
        /// Reset the API route back to default (/api/method/).
        /// </summary>
        public void ResetRoute()
        {
            this.ChangeRoute("/api/method");
        }

        /// <summary>
        /// Login to Frappe sight using access token.
        /// </summary>
        /// <param name="accessToken">The apps access token.</param>
        /// <returns>The current instance of FrappeRestClient for fluent code.</returns>
        public async Task<FrappeRestClient> UseAccessTokenAsync(string accessToken)
        {
            // TODO: Setup frappe access token service on a Frappe Site for test
            this.ClearAuthorization();
            this.client.Settings.DefaultHeaders.Add("Authorization", $"Bearer {accessToken}");

            // validation
            try
            {
                await this.GetLoggedUserAsync();
                this.isToken = true;
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
        /// whithout validation.
        /// </summary>
        /// <param name="apiKey">User API key.</param>
        /// <param name="apiSecret">User API secret.</param>
        /// <returns>The current instance of FrappeRestClient for fluent code.</returns>
        public FrappeRestClient SetToken(string apiKey, string apiSecret)
        {
            this.ClearAuthorization();
            this.client.Settings.DefaultHeaders.Add("Authorization", $"token {apiKey}:{apiSecret}");
            this.isToken = true;
            return this;
        }

        /// <summary>
        /// Adds authorization token to by default to all request headers and
        /// validates it by getting the current logged in user.
        /// </summary>
        /// <param name="apiKey">User API key.</param>
        /// <param name="apiSecret">User API secret.</param>
        /// <returns>The current instance of FrappeRestClient for fluent code.</returns>
        public async Task<FrappeRestClient> UseTokenAsync(string apiKey, string apiSecret) {
            this.ClearAuthorization();
            this.client.Settings.DefaultHeaders.Add("Authorization", $"token {apiKey}:{apiSecret}");

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

            this.isAccessToken = true;
            this.isAuthenticated = true;
            return this;
        }

        /// <summary>
        /// Login to a Frappe Framework site with traditional username/email
        /// and password.
        /// </summary>
        /// <param name="email">Username or email.</param>
        /// <param name="password">user password.</param>
        /// <returns>The current instance of FrappeRestClient for fluent code.</returns>
        public async Task<FrappeRestClient> UsePasswordAsync(string email, string password)
        {
            this.ClearAuthorization();
            try
            {
                await this.client.PostRequest("login", new EmailPasswordPair() { usr = email, pwd = password})
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

            this.isPassword = true;
            this.isAuthenticated = true;
            return this;
        }

        /// <summary>
        /// Gets the currently logged user.
        /// </summary>
        /// <returns>The name of the logged in user.</returns>
        public async Task<string> GetLoggedUserAsync() {
            var response =  await client.GetRequest("frappe.auth.get_logged_user")
                    .ExecuteAsStringAsync();
            return ToObject(response).message.ToString();
        }

        /// <summary>
        /// Clear authorization header and reset flags.
        /// </summary>
        private void ClearAuthorization() {
            this.client.Settings.DefaultHeaders.Remove("Authorization");
            this.ResetFlags();
        }

        /// <summary>
        /// Resets all flags to false.
        /// </summary>
        private void ResetFlags() {
            this.isAuthenticated = false;
            this.isToken = false;
            this.isPassword = false;
            this.isAccessToken = false;
        }

        /// <summary>
        /// Logout a frappe user
        /// </summary>
        public void Logout()
        {
            ClearAuthorization();
        }

        /// <summary>
        /// Pings the frappe site
        /// </summary>
        /// <returns>Returns the reponse from the Frappe site</returns>
        public async Task<string> PingAsync()
        {
            var response = await client.GetRequest("frappe.ping").ExecuteAsStringAsync();
            return ToObject(response).message.ToString();
        }
    }
}
