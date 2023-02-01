using System;

namespace i28511.Hattrick.OAuth.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class OAuthException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public OAuthError Error { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="error"></param>
        public OAuthException(OAuthError error)
            : base(error.ErrorDescription)
        {
            Error = error;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="error"></param>
        /// <param name="innerException"></param>
        public OAuthException(OAuthError error, Exception innerException)
            : base(error.ErrorDescription, innerException)
        {
            Error = error;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class OAuthError
    {
        /// <summary>
        /// 
        /// </summary>
        public string Error { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ErrorDescription { get; set; }
    }
}