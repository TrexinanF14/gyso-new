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
using System.Collections.Concurrent;

namespace GYSOManager.Modules
{
    public class PlayerListModule : NancyModule
    {
        public PlayerListModule()
            : base("/")
        {
            Get["/player-list"] = _ =>
            {
                return View["player-list"];
            };
            Post["/player-list"] = _ =>
            {
                string email = Request.Form["email"];
                EmailPlayerList(email);
                return Response.AsRedirect("/player-list-complete");
            };
            Get["/player-list-complete"] = _ =>
            {
                return View["player-list-complete"];
            };
        }

        private void EmailPlayerList(string email)
        {
            using (var ctx = new GYSOContext())
            {
                string playerStr = ctx.Registrations
                    .ToList()
                    .Where(x => x.parentemail == email)
                    .Where(x => x.RegistrationDate.Year == DateTime.Now.Year)
                    .Select(x => x.name)
                    .Aggregate("", (next, agg) => agg + Environment.NewLine + next);

                if (string.IsNullOrWhiteSpace(playerStr))
                {
                    playerStr = "No players found";
                }

                Email.SendMessage(email, "You have registered the following players: " + playerStr);
            }

        }
    }
}