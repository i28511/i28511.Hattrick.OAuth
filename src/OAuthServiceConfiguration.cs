using System;

namespace i28511.Hattrick.OAuth
{
    /// <summary>
    /// Represents the configuration for the OAuth service.
    /// </summary>
    public class OAuthServiceConfiguration
    {
        /// <summary>
        /// Gets or sets the unique identifier for the application that is using the OAuth service.
        /// It is provided by the OAuth provider.
        /// </summary>
        public string ConsumerKey { get; set; }

        /// <summary>
        /// Gets or sets the secret key that is used to authenticate the application that is using the OAuth service.
        /// It is provided by the OAuth provider.
        /// </summary>
        public string ConsumerSecret { get; set; }

        /// <summary>
        /// Gets or sets the URL that the OAuth provider will redirect to after the user has authorized the application.
        /// </summary>
        public Uri CallbackUrl { get; set; }
    }

}