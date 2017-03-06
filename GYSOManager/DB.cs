using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace GYSOManager
{
    public class GYSOContext : DbContext
    {
        public DbSet<Registration> Registrations { get; set; }

        public DbSet<Team> Teams { get; set; }

        public DbSet<Grade> Grades { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(ConfigurationManager.ConnectionStrings["Default"].ConnectionString);
        }
    }

    public enum Sex
    {
        Male = 0,
        Female = 1
    }

    public class Grade
    {
        public int GradeId { get; set; }

        public string Label { get; set; }

        public List<Team> Teams { get; set; }
    }

    public class Team
    {
        public int TeamId { get; set; }

        public string Name { get; set; }

        public int GradeId { get; set; }

        public Grade Grade { get; set; }

        public Sex Sex { get; set; }

        public string CoachName { get; set; }

        public string CoachEmail { get; set; }

        public string CoachPhone { get; set; }

        public string LeadCoachName { get; set; }

        public string LeadCoachEmail { get; set; }

        public string LeadCoachPhone { get; set; }

        public List<Registration> Members { get; set; }
    }

    public class Registration
    {
        public int RegistrationId { get; set; }

        public DateTime RegistrationDate { get; set; }

        public int GradeId { get; set; }

        public Grade Grade { get; set; }

        public string name { get; set; }

        public Sex sex { get; set; }

        public string birthday { get; set; }

        public string athletic { get; set; }

        public string soccerexperience { get; set; }

        public string playwith { get; set; }

        public string parentname { get; set; }

        public string parentphone1 { get; set; }

        public string parentphone2 { get; set; }

        public string parentemail { get; set; }

        public string emergencyname { get; set; }

        public string emergencyphone1 { get; set; }

        public string emergencyphone2 { get; set; }

        public bool sponsorcheck { get; set; }

        public bool coachcheck { get; set; }

        public bool scholarshipcheck { get; set; }

        public string signature { get; set; }

        public string signaturedate { get; set; }

        public int? TeamId { get; set; }

        public Team Team { get; set; }
    }
}