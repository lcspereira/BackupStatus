using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using BackupStatus.Models;

namespace BackupStatus.WebAPI.Controllers
{
    public class UpdateStatusController : ApiController
    {
        private HostModel db = new HostModel();

        // GET: api/UpdateStatus/<name>
        [ResponseType(typeof(Host))]
        public IHttpActionResult GetHost(string param)
        {
            Host host = db.Hosts.Where(h => h.Name == param).FirstOrDefault();
            //Host host = db.Hosts.Find(id);
            if (host == null)
            {
                return NotFound();
            }

            return Ok(host);
        }

        // PUT: api/UpdateStatus/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutHost(int param, Host host)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (param != host.Id)
            {
                return BadRequest();
            }

            if (db.Hosts.Any (e => ((e.Name == host.Name) && (e.LastStatusUpdate == host.LastStatusUpdate))))
            {
                return NotFound();
            }

            db.Entry(host).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HostExists(param))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool HostExists(int id)
        {
            return db.Hosts.Count(e => e.Id == id) > 0;
        }
    }
}