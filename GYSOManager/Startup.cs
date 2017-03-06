using Owin;
using Nancy.Owin;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace GYSOManager
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseNancy();
        }
    }

    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            StaticConfiguration.DisableErrorTraces = false;

            base.ApplicationStartup(container, pipelines);
            Nancy.Security.Csrf.Enable(pipelines);

            CreateTables();
        }


        private void CreateTables()
        {
            using (var db = new GYSOContext())
            {
                db.Database.EnsureCreated();
            }
        }
    }
}