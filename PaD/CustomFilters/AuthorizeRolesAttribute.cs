using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.WebPages;

using PaD.Models;

namespace PaD.CustomFilters
{
    public class AuthorizeRolesAttribute : AuthorizeAttribute
    {
        private Role[] _roles;
        public AuthorizeRolesAttribute(params Role[] roles)
        {
            _roles = roles;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            //// Admin is allowed to do anything.
            //if (httpContext.User.IsInRole(Role.Admin.Description()))
            //{
            //    return true;
            //}

            foreach (Role role in _roles)
            {
                if (httpContext.User.IsInRole(role.Description()))
                {
                    return true;
                }
            }

            return false;
        }
    }
}