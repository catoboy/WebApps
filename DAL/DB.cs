﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using Model;

namespace DAL
{
    public class DB : DbContext
    {
        public DB() : base("name=Vy")
        {
            Database.CreateIfNotExists();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        public override int SaveChanges()
        {
            var modifisertEntitet = ChangeTracker.Entries()
                .Where(p => p.State == EntityState.Modified).ToList();
            var now = DateTime.Now;


            foreach (var endring in modifisertEntitet)
            {
                var entitNavn = endring.Entity.GetType().Name;
                var primaerVerdi = GetPrimaerNokkel(endring);

                foreach (var prop in endring.OriginalValues.PropertyNames)
                {

                    var orginalVerdi = endring.GetDatabaseValues().GetValue<object>(prop).ToString();
                    var endretVerdi = endring.CurrentValues[prop].ToString();

                    if (orginalVerdi != endretVerdi)
                    {

                        var log = new Logging()
                        {
                            Entitet = entitNavn,
                            Nokkelverdi = primaerVerdi.ToString(),
                            Egenskap = prop,
                            GammelVerdi = orginalVerdi,
                            NyVerdi = endretVerdi,
                            DatoEndret = now
                        };
                        Logg.Add(log);
                    }
                }
            }
            return base.SaveChanges();
        } 

        private object GetPrimaerNokkel(DbEntityEntry entry)
        {
            var verdi =
                ((IObjectContextAdapter) this).ObjectContext.ObjectStateManager.GetObjectStateEntry(entry.Entity);
            return verdi.EntityKey.EntityKeyValues[0].Value;
        }


        public void setAdmin()
        {
            DB db = new DB();
            if (db.Admin.Find(1) == null)
            {
                var admin = new Admin
                {
                    Brukernavn = "admin",
                    Passord = "vsSDUXwrarYGhShJBLoDVxqPreD+GGQXVKKo0JOq4Eh2H8pw"
                };
                db.Admin.Add(admin);
                db.SaveChanges();
            }
        }



        public virtual DbSet<Kunde> Kunde { get; set; }
        public virtual DbSet<Billett> Billett { get; set; }
        public virtual DbSet<Reise> Reise { get; set; }
        public virtual DbSet<Kredittkort> Kredittkort { get; set; }
        public virtual DbSet<Logging> Logg { get; set; }
        public virtual DbSet<Admin> Admin { get; set; }

    }
}