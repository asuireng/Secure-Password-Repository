﻿using Microsoft.Owin.Security;
using System;
using System.Net;
using System.Runtime.Caching;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Secure_Password_Repository.Filters
{
    /// <summary>
    /// Verify that the encryption key still exists in cache - otherwise return an error
    /// Upon every action, ensure that the hashed password is still in cache, otherwise password decryption is not possible
    /// </summary>

    public class UserEncryptionKeyCacheVerification : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //get the current user
            IPrincipal user = filterContext.HttpContext.User;

            //make sure user isautenticated
            if (user.Identity.IsAuthenticated)

                //if the encryption key doesnt exist in cache or the session ID doesnt match (occurs on a double long) then log the user out
                if (MemoryCache.Default.Get(user.Identity.Name) == null || !string.Equals((MemoryCache.Default.Get(user.Identity.Name + "SessionID") ?? string.Empty).ToString(), filterContext.HttpContext.Session.SessionID.ToString(), StringComparison.OrdinalIgnoreCase))
                {

                    //log out the user
                    AuthenticationManager.SignOut();
                    filterContext.HttpContext.Response.Clear();
                    //return a 401 error
                    filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;

                    //if the request was AJAX based, then just return the error
                    if(filterContext.HttpContext.Request.IsAjaxRequest())
                        filterContext.HttpContext.Response.End();
                    //otherwise return back to the login screen
                    else
                        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "Controller", "Account" }, { "Action", "Login" }, { "ReturnUrl", "Password" } });

                }

        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.Current.GetOwinContext().Authentication;
            }
        }

    }

}