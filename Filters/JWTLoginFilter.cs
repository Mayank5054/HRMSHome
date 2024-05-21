using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using HRMS.Security;
using Newtonsoft.Json.Linq;
namespace HRMS.Filters
{
    public class JWTLoginFilter : ActionFilterAttribute, IAuthenticationFilter
    {
        public void OnAuthentication(AuthenticationContext filterContext)
        {
          var request = filterContext.HttpContext.Request;

            //request.Headers["Authorization"]
            var authenticationTokenHeader = request.Cookies["jwtToken"];
            if(authenticationTokenHeader == null)
            {
                filterContext.Result = new RedirectResult("~/JWT/Login");
            }
            else
            {
                var token = authenticationTokenHeader.Value;
                var validationResult = JWT.ValidateToken(token);
                if (validationResult != null)
                {
                    filterContext.HttpContext.User = validationResult;
                }
                else
                {
                    filterContext.Result = new RedirectResult("~/JWT/Login");
                }
              
            }
        }

        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
            
        }


       
    }
}