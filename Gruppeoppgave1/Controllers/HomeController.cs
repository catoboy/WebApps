﻿using Gruppeoppgave1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gruppeoppgave1.Controllers
{
    public class HomeController : Controller
    {
		private DB db = new DB();

        // GET: Home
        public ActionResult Index()
        {
            var tider = getAlleTider();
            var reiseModel = new KundeReise();
            reiseModel.reiseTidene = GetSelectListItems(tider);
           
            return View(reiseModel);
        }

        [HttpPost]
        public ActionResult Index(KundeReise reiseInput)
        {
            var tider = getAlleTider();
            reiseInput.reiseTidene = GetSelectListItems(tider);
            Session["Reise"] = reiseInput;

            return RedirectToAction("Reisevalg");
        }

        public ActionResult ReiseValg()
        {
            return View(Session["Reise"]);
        }

        public ActionResult Kunde ()
        { 
             return View();
        }

        public ActionResult Reiser()
        { 
            return View(Session["Reisen"]);
        }
     
        [HttpPost]
        public ActionResult ReiseInfo(KundeReise info)
        {
            if (info.kunde == null)
            {
                var fra = Session["Fra"].ToString();
                var til = Session["Til"].ToString();
                var dato = Session["Dato"].ToString();
                var tid = Request["Tid"];
                double pris = Double.Parse(Request["Pris"]);
                var spor = Request["Spor"];
                var tog = Request["Tog"];
                int bytter = int.Parse(Request["Bytter"]);
                var avgang = Request["Avgang"];
                var ankomst = Request["Ankomst"];

                Reise reise = new Reise
                {
                    Fra = fra,
                    Til = til,
                    Dato = dato,
                    Tid = tid,
                    Pris = pris,
                    Spor = spor,
                    Tog = tog,
                    Bytter = bytter,
                    Avgang = avgang,
                    Ankomst = ankomst
                };

                KundeReise reisen = new KundeReise
                {
                    kunde = null,
                    reise = reise
                };
                Session["Reisen"] = reisen;

                return View(Session["Reisen"]);
            }

            Billett billet = new Billett();
            info.reise = ((KundeReise)Session["Reisen"]).reise;
            billet.Reise = info.reise;
            billet.Kunde = info.kunde;
            db.Billett.Add(billet);
            db.Reise.Add(info.reise);
            db.Kunde.Add(info.kunde);
            db.SaveChanges();
            Session["ID"] = billet.ID;

            return RedirectToAction("Billett", Session["ID"]);
        }

        public ActionResult Billett()
        {
            var billettID = Session["ID"];
            DB db = new DB();
            var valgtBillett = db.Billett.Find(billettID);

            return View(valgtBillett);
        }
        
        private IEnumerable<String> getAlleTider()
        {
            return new List<String>
            {
                "00:00",
                "01:00",
                "02:00",
                "03:00",
                "04:00",
                "05:00",
                "06:00",
                "07:00",
                "08:00",
                "09:00",
                "10:00",
                "11:00",
                "12:00",
                "13:00",
                "14:00",
                "15:00",
                "16:00",
                "17:00",
                "18:00",
                "19:00",
                "20:00",
                "21:00",
                "22:00",
                "23:00",
            };
        }

        private IEnumerable<SelectListItem> GetSelectListItems (IEnumerable<String> tider)
        {
            var valgteListe = new List<SelectListItem>();
            foreach (var tid in tider)
            {
                valgteListe.Add(new SelectListItem
                {
                    Value = tid,
                    Text = tid
                });
            }
            return valgteListe;
        }    

    }
}
