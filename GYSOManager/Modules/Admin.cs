using Nancy;
using Nancy.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Nancy.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Newtonsoft.Json;

namespace GYSOManager.Modules
{
    public class AdminModule : NancyModule
    {
        public AdminModule()
        {
            Before.AddItemToStartOfPipeline(ctx =>
            {
                string adminpassword = ConfigurationManager.AppSettings["adminpassword"];

                if (!Request.Cookies.ContainsKey("token"))
                {
                    return Response.AsRedirect("/login?error=Please log in");
                }

                var token = Request.Cookies["token"];
                if (token != LoginModule.GetHashSha256(adminpassword))
                {
                    return Response.AsRedirect("/login?error=Please log in");
                }
                return null;
            });

            Get["/admin"] = _ =>
            {
                bool expanded = false;
                if (Request.Query.ContainsKey("expanded"))
                {
                    expanded = bool.Parse(Request.Query["expanded"]);
                }

                var settings = Settings.Read();

                using (var ctx = new GYSOContext())
                {
                    var currentYear = DateTime.Now.Year;

                    var teams = ctx.Teams.Include(x => x.Grade).ToList();

                    var gradeTeams = teams.GroupBy(x => x.GradeId).ToDictionary(x => x.Key, y => y.ToList());

                    var players = ctx.Registrations
                        .Include(x => x.Grade)
                        .Include(x => x.Team)
                        .ToList();

                    return View["admin", new
                    {
                        GradeTeams = gradeTeams,
                        Expanded = expanded,
                        RegistrationOpen = settings.RegistrationOpen,
                        Teams = teams,
                        Players = players.Where(x => x.RegistrationDate.Year == currentYear)
                    }];
                }
            };
            Get["/admin/updatemember"] = _ =>
            {
                var playerid = int.Parse(Request.Query["playerid"]);
                var teamid = int.Parse(Request.Query["teamid"]);

                using (var ctx = new GYSOContext())
                {
                    Registration player = ctx.Registrations.ToList().Where(x => x.RegistrationId == playerid).FirstOrDefault();
                    if (player != null)
                    {
                        player.TeamId = teamid > 0 ? teamid : null;
                        ctx.SaveChanges();
                    }
                }

                return Response.AsRedirect("/admin");
            };
            Get["/admin/removeplayer"] = _ =>
            {
                var playerid = int.Parse(Request.Query["playerid"]);

                using (var ctx = new GYSOContext())
                {
                    var player = ctx.Registrations.ToList().Where(x => x.RegistrationId == playerid).FirstOrDefault();
                    if (player != null)
                    {
                        ctx.Remove(player);
                        ctx.SaveChanges();
                    }
                }

                return Response.AsRedirect("/admin");
            };
            Post["/admin/updatesettings"] = _ =>
            {
                var settings = this.Bind<Settings>();
                Settings.Save(settings);

                return Response.AsRedirect("/admin");
            };
            Get["/admin/addteam"] = _ =>
            {
                return View["addteam", new Team()];
            };
            Post["/admin/addteam"] = _ =>
            {
                try
                {
                    var team = this.Bind<Team>();
                    using (var ctx = new GYSOContext())
                    {
                        ctx.Teams.Add(team);
                        ctx.SaveChanges();
                    }
                }
                catch (Exception)
                {

                    throw;
                }

                return Response.AsRedirect("/admin");
            };
            Get["/admin/editteam"] = _ =>
            {
                var teamid = int.Parse(Request.Query["teamid"]);

                using (var ctx = new GYSOContext())
                {
                    var team = ctx.Teams.Where((Func<Team, bool>)(x => x.TeamId == teamid)).First();
                    return View["addteam", team];
                }
            };
            Post["/admin/editteam"] = _ =>
            {
                var team = this.Bind<Team>();

                using (var ctx = new GYSOContext())
                {
                    ctx.Update(team);
                    ctx.SaveChanges();
                }

                return Response.AsRedirect("/admin");
            };
            Get["/admin/removeteam"] = _ =>
            {
                var teamid = int.Parse(Request.Query["teamid"]);

                using (var ctx = new GYSOContext())
                {
                    var team = ctx.Teams.ToList().Where(x => x.TeamId == teamid).FirstOrDefault();
                    if (team != null)
                    {
                        ctx.Remove(team);
                        ctx.SaveChanges();
                    }
                }

                return Response.AsRedirect("/admin");
            };
        }
    }

    public class Settings
    {
        private static string SettingsFilepath = ConfigurationManager.AppSettings["settingsfile"];

        public bool RegistrationOpen = true;

        public static Settings Read()
        {
            if (File.Exists(SettingsFilepath))
            {
                return JsonConvert.DeserializeObject<Settings>(File.ReadAllText(SettingsFilepath));
            }

            return new Settings();
        }

        public static void Save(Settings settings)
        {
            File.WriteAllText(SettingsFilepath, JsonConvert.SerializeObject(settings));
        }
    }
}