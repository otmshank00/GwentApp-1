using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace GwentApp.App_Code
{
    public class WebAPIIsLocalAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// Whether a SecurityException should be thrown if the request is not local.
        /// </summary>
        public bool ThrowSecurityException
        {
            get;
            set;
        }

        protected override bool IsAuthorized(HttpActionContext httpActionContext)
        {
            // Check whether the request originated from a local IP address.
            bool isLocal = httpActionContext.RequestContext.IsLocal;

            // If the request isn't local, and the developer wants an exception to be thrown.
            if (!isLocal && ThrowSecurityException)
            {
                // Throw the exception.
                throw new SecurityException();
            }

            // Return whether the request was local or not.
            return isLocal;
        }
    }
}