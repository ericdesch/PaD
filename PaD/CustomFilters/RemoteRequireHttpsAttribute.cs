using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PaD.CustomFilters
{
    public class RemoteRequireHttpsAttribute : RequireHttpsAttribute
    {
        //TODO: do this as a rewrite rule. Change enabled="false" to "true" when deploying
        //<rewrite>
        //  <rules>
        //    <rule name = "SSL_ENABLED" enabled="false" stopProcessing="true">
        //      <match url = "(.*)" />
        //      < conditions >
        //        < add input="{HTTPS}" pattern="^OFF$" />
        //      </conditions>
        //      <action type = "Redirect" url="https://{HTTP_HOST}/{R:1}" appendQueryString="true" redirectType="Permanent" />
        //    </rule>
        //  </rules>
        //</rewrite>

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("RemoteRequireHttpsAttribute: filterContext is null");
            }

            if (filterContext.HttpContext != null)
            {
                if (filterContext.HttpContext.Request.IsSecureConnection)
                {
                    return;
                }

                var currentUrl = filterContext.HttpContext.Request.Url;
                if (currentUrl.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.CurrentCultureIgnoreCase))
                {
                    return;
                }

                if (string.Equals(filterContext.HttpContext.Request.Headers["X-Forwarded-Proto"], "https", StringComparison.InvariantCultureIgnoreCase))
                {
                    return;
                }

                if (filterContext.HttpContext.Request.IsLocal)
                {
                    return;
                }

                var val = ConfigurationManager.AppSettings["RequireSSL"].Trim();
                var requireSsl = bool.Parse(val);
                if (!requireSsl)
                {
                    return;
                }
            }

            base.OnAuthorization(filterContext);
        }
    }
}