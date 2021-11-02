namespace FrappeRestClient.Net.Authorization
{
    /// <summary>
    /// Helper class for holding Frappe based email password pair
    /// </summary>
    public class EmailPasswordPair
    {
        /// <summary>
        /// Gets or sets the username or email.
        /// </summary>
        public string usr { get; set; }

        /// <summary>
        /// Gets or sets user password.
        /// </summary>
        public string pwd { get; set; }
    }
}
