using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using BackupStatus.Models;
using FuzzySharp;

namespace BackupStatus.WebUI.Controllers
{
    public class HostsController : Controller
    {
        private HostModel db = new HostModel();

        private enum DuplicateStatusCode
        {
            OK = 0,
            DUP_NAME = 1,
            DUP_ADDR = 2
        }

        [HttpGet]
        public ActionResult Index(string queryName)
        {
            List<Host> hosts;


            if (queryName.IsEmpty())
            {
                hosts = (from h in db.Hosts
                         orderby h.ReturnCode descending
                         where h.LastStatusUpdate != null
                         select h).ToList();
            }
            else
            {
                var hostNames = from h in db.Hosts
                                select h.Name;

                var fuzzyAux = Process.ExtractOne(queryName, hostNames);

                if (fuzzyAux.Score >= 90)
                {
                    hosts = (from h in db.Hosts
                             where h.Name == fuzzyAux.Value
                             select h).ToList();
                }
                else
                {
                    hosts = new List<Host>();
                }
            }
            return View(hosts);
        }

        // GET: Hosts/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Host host = db.Hosts.Find(id);
            if (host == null)
            {
                return HttpNotFound();
            }
            return View(host);
        }

        // GET: Hosts/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Hosts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "Id,Name,Address,ReturnCode,LastStatusUpdate")] Host host)
        {
            if (ModelState.IsValid)
            {
                if (db.Hosts.Where(h => h.Name == host.Name).Any())
                {
                    ViewBag.ErrorMessage = "Já existe um host com este nome.";
                    return View(host);
                }
                else if (db.Hosts.Where(h => h.Address == host.Address).Any())
                {
                    ViewBag.ErrorMessage = "Já existe um host com este endereço.";
                    return View(host);
                }
                host.ReturnCode = StatusCode.UNKNOWN;
                db.Hosts.Add(host);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(host);
        }

        // GET: Hosts/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Host host = db.Hosts.Find(id);
            if (host == null)
            {
                return HttpNotFound();
            }
            return View(host);
        }

        // POST: Hosts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit([Bind(Include = "Id,Name,Address,ReturnCode,LastStatusUpdate")] Host host)
        {
            if (ModelState.IsValid)
            {
                var DupHosts = from h in db.Hosts
                               where (h.Name == host.Name || h.Address == host.Address) && h.Id != host.Id
                               select h;
                if (!DupHosts.Any())
                {
                    db.Entry(host).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else if (DupHosts.Where(h => h.Name == host.Name).Any())
                {
                    ViewBag.ErrorMessage = "Já existe um host com este nome.";
                    return View(host);
                }
                else if (DupHosts.Where(h => h.Address == host.Address).Any())
                {
                    ViewBag.ErrorMessage = "Já existe um host com este endereço.";
                    return View(host);
                }
                
            }
            return View(host);
        }

        // GET: Hosts/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Host host = db.Hosts.Find(id);
            if (host == null)
            {
                return HttpNotFound();
            }
            return View(host);
        }

        // POST: Hosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult DeleteConfirmed(int id)
        {
            Host host = db.Hosts.Find(id);
            db.Hosts.Remove(host);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
