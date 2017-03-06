using Nancy;
using Nancy.Cookies;
using Nancy.ModelBinding;
using Nancy.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Data.SQLite;

namespace GYSOManager.Modules
{
    public class RegisterModule : NancyModule
    {
        public static List<DateTime> requests = new List<DateTime>();

        public RegisterModule()
            : base("/")
        {
            Get["/"] = _ =>
            {
                return Response.AsRedirect("/register-english");
            };
            Get["/register-english"] = _ =>
            {
                var settings = Settings.Read();
                if (!settings.RegistrationOpen)
                {
                    return View["register-closed"];
                }

                return View["register-english", new
                {
                    Error = ""
                }];
            };
            Post["/register-english"] = _ =>
            {
                WriteRegistration();
                return Response.AsRedirect("/register/complete");
            };
            Get["/register-spanish"] = _ =>
            {
                var settings = Settings.Read();
                if (!settings.RegistrationOpen)
                {
                    return View["register-closed"];
                }


                return View["register-spanish", new
                {
                    Error = ""
                }];
            };
            Post["/register-spanish"] = _ =>
            {
                WriteRegistration();
                return Response.AsRedirect("/register/complete");
            };
            Get["/register/complete"] = _ =>
            {
                return View["register-finished"];
            };
        }

        private void WriteRegistration()
        {
            this.ValidateCsrfToken();

            var reg = this.Bind<Registration>();
            reg.RegistrationDate = DateTime.Now;
            using (var db = new GYSOContext())
            {
                db.Add(reg);
                db.SaveChanges();
            }
        }
    }
}