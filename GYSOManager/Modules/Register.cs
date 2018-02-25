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
        public RegisterModule()
            : base("/")
        {
            Get["/"] = _ =>
            {
                return Response.AsRedirect("/register-english");
            };
            Get["/register-english"] = _ =>
            {
                string bypass = Request.Query["bypass"];

                var settings = Settings.Read();
                if (!settings.RegistrationOpen && string.IsNullOrEmpty(bypass))
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
                var reg = this.Bind<Registration>();
                WriteRegistration(reg);
                return Response.AsRedirect("/register/complete?name=" + reg.name);
            };
            Get["/register-spanish"] = _ =>
            {
                string bypass = Request.Query["bypass"];

                var settings = Settings.Read();
                if (!settings.RegistrationOpen && string.IsNullOrEmpty(bypass))
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
                var reg = this.Bind<Registration>();
                WriteRegistration(reg);
                return Response.AsRedirect("/register/complete?name=" + reg.name);
            };
            Get["/register/complete"] = _ =>
            {
                string playerName = Request.Query["name"];
                if (string.IsNullOrEmpty(playerName))
                {
                    playerName = "Player";
                }

                var settings = Settings.Read();

                return View["register-finished", new
                {
                    HigherPrice = settings.HigherPrice,
                    PlayerName = playerName
                }];
            };
        }

        private void WriteRegistration(Registration reg)
        {
            this.ValidateCsrfToken();

            reg.RegistrationDate = DateTime.Now;
            using (var db = new GYSOContext())
            {
                db.Add(reg);
                db.SaveChanges();
            }
        }
    }
}