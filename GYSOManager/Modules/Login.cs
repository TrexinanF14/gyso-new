using Nancy;
using Nancy.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace GYSOManager.Modules
{
    public class LoginModule : NancyModule
    {
        public LoginModule()
        {
            string adminusername = ConfigurationManager.AppSettings["adminusername"];
            string adminpassword = ConfigurationManager.AppSettings["adminpassword"];

            Get["/login"] = _ =>
            {
                if (!Request.Cookies.ContainsKey("token"))
                {
                    return Response.AsRedirect("/login");
                }

                var token = Request.Cookies["token"];
                if (token == GetHashSha256(adminpassword))
                {
                    return Response.AsRedirect("/admin");
                }

                return View["login", new
                {
                    ErrorMessage = (string)Request.Query["error"]
                }];
            };
            Get["/logout"] = _ =>
            {
                var response = Response.AsRedirect("/login");
                response.WithCookie("token", "", DateTime.Now.AddDays(7));
                return response;
            };
            Post["/login"] = _ =>
            {


                string username = Request.Form["username"];
                string password = Request.Form["password"];
                if (username != adminusername || password != adminpassword)
                {
                    return Response.AsRedirect("/login?error=Invalid username or password");
                }

                this.ValidateCsrfToken();

                var response = Response.AsRedirect("/admin");
                response.WithCookie("token", GetHashSha256(password), DateTime.Now.AddDays(7));
                return response;
            };
        }

        public static string GetHashSha256(string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            SHA256Managed hashstring = new SHA256Managed();
            byte[] hash = hashstring.ComputeHash(bytes);
            string hashString = string.Empty;
            foreach (byte x in hash)
            {
                hashString += String.Format("{0:x2}", x);
            }
            return hashString;
        }
    }
}