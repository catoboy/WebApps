﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL;
using Model;

namespace Gruppeoppgave1.Controllers
{
    public class HomeController : Controller
    {
        private IDatabaseLogikkBLL _databaseLogikkBll;
        private Reise reise = new Reise();

        // GET: Home
        public HomeController()
        {
            _databaseLogikkBll = new DatabaseLogikkBLL();
        }

        public HomeController(IDatabaseLogikkBLL stub)
        {
            _databaseLogikkBll = stub;
        }

        public ActionResult Index()
        {
            var tider = reise.getAlleTider();
            var reiseModel = new KundeReise();
            reiseModel.reiseTidene = reise.GetSelectListItems(tider);

            return View(reiseModel);
        }

        [HttpPost]
        public ActionResult Index(KundeReise reiseInput)
        {
            var tider = reise.getAlleTider();
            reiseInput.reiseTidene = reise.GetSelectListItems(tider);
            if (ModelState.IsValid)
            {
                Session["Reise"] = reiseInput;

                return RedirectToAction("Reisevalg");
            }
            return View(reiseInput);
        }

        public ActionResult ReiseValg()
        {
            return View(Session["Reise"]);
        }

        [HttpPost]
        public ActionResult ReiseInfo(KundeReise info)
        {
            if (info.kunde == null)
            {
                info.reise = new Reise
                {
                    Fra = Session["Fra"].ToString(),
                    Til = Session["Til"].ToString(),
                    Dato = Session["Dato"].ToString(),
                    Tid = Request["Tid"],
                    Pris = Double.Parse(Request["Pris"]),
                    Spor = Request["Spor"],
                    Tog = Request["Tog"],
                    Bytter = int.Parse(Request["Bytter"]),
                    Avgang = Request["Avgang"],
                    Ankomst = Request["Ankomst"]
                };
                Session["Reisen"] = info;
                return View(Session["Reisen"]);
            } 
            info.reise = ((KundeReise)Session["Reisen"]).reise; 
            var id = _databaseLogikkBll.lagreBillett(info);
            Session["ID"] = id; 
            return RedirectToAction("Billett", Session["ID"]);
        }


        public ActionResult Billett()
        {
            var billettID = Session["ID"];
            var valgtBillett = _databaseLogikkBll.getBillett((int)billettID);
            return View(valgtBillett);
        }
        
        public ActionResult Admin()
        {
            IEnumerable<Kunde> allebilleter = _databaseLogikkBll.getAlleKunder();
            return View(allebilleter);
        }

        public ActionResult listAdmin()
        {
            IEnumerable<Admin> alleAdmins = _databaseLogikkBll.getAlleAdmin();
            return View(alleAdmins);
        }

        [HttpPost]
        public ActionResult lagAdmin(Admin admin)
        {
            _databaseLogikkBll.lagAdmin(admin);
            return RedirectToAction("listAdmin");
        }

        public ActionResult lagAdmin()
        {
            return View();
        }

        public ActionResult slettAdmin(int ID)
        { 
            _databaseLogikkBll.slettAdmin(ID);
            return RedirectToAction("listAdmin");
        }

        public ActionResult SlettKunde(int ID)
        {
            _databaseLogikkBll.slettKunde(ID);
            return RedirectToAction("Admin");
        }

        public ActionResult EditKunde(int ID)
        {
            Kunde valgtkunde = _databaseLogikkBll.editKunde(ID);
            return View(valgtkunde);
        }

        [HttpPost]
        public ActionResult EditKunde(Kunde kunde)
        {
            _databaseLogikkBll.editKunde(kunde);
            return RedirectToAction("Admin");
        }

        public ActionResult AdminReise(int ID)
        {
            Reise valgtreise = _databaseLogikkBll.seReise(ID);
            return View(valgtreise);
        }

        [HttpPost]
        public ActionResult AdminReise(Reise reise)
        {
            _databaseLogikkBll.seReise(reise);
            return RedirectToAction("Admin");
        }

        public ActionResult SlettReise(int ID)
        {
            _databaseLogikkBll.slettReise(ID);
            return RedirectToAction("Admin");
        }
        
        public ActionResult Login()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult Autorisasjon(Admin admin)
        { 
            if (_databaseLogikkBll.Autorisasjon(admin))
            {
                Session["loginID"] = admin.ID;
                return RedirectToAction("listAdmin");
            }
            else
            {
                return View("Login", admin); 
            }
        }

    }
}
