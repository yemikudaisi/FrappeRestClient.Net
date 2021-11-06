// <copyright file="Cookies.cs" company="Yemi Kudaisi">
// Copyright (c) Yemi Kukudaisi. All rights reserved.
// </copyright>

namespace FrappeRestClient.Net
{
    /// <summary>
    /// Handles all Frappe Framework cookies related functions.
    /// </summary>
    public class Cookies
    {
        /// <summary>
        /// Contains the keys of common Frappe Framework cookie
        /// field name.
        /// </summary>
        public static class FieldNames
        {
            /// <summary>
            /// Gets the session ID cookie field name.
            /// </summary>
            public static string Sid { get => "sid"; }

            /// <summary>
            /// Gets the system user cookie field name.
            /// </summary>
            public static string SystemUser { get => "system_user"; }

            /// <summary>
            /// Gets the full name cookie field name.
            /// </summary>
            public static string FullName { get => "full_name"; }

            /// <summary>
            /// Gets the user ID cookie field name.
            /// </summary>
            public static string UserId { get => "user_id"; }

            /// <summary>
            /// Gets the user image path cookie image.
            /// </summary>
            public static string UserImage { get => "user_image"; }
        }
    }
}
