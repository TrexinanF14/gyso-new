using Nancy;
using Nancy.Cookies;
using Nancy.ModelBinding;
using Nancy.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

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
                return View["register-english", new
                {
                    Error = ""
                }];
            };
            Post["/register-english"] = _ =>
            {
                var reg = this.Bind<Registration>();

                return "hello";
            };
        }
    }

    public enum Grade
    {
        Kindergarten = 0,
        First = 1,
        SecondThird = 2,
        FourthFith = 3,
        Middle = 4
    }

    public class Registration
    {
        public string name;

        public Grade grade;

        public string sex;

        public string birthday;

        public string athletic;

        public string soccerexperience;

        public string playwith;

        public string parentname;

        public string parentphone1;

        public string parentphone2;

        public string parentemail;

        public string emergencyname;

        public string emergencyphone1;

        public string emergencyphone2;

        public string signature;

        public string signaturedate;
    }
}