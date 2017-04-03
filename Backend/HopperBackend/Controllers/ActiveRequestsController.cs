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
using HopperBackend.Models;
using Microsoft.Azure.Mobile.Server.Config;

namespace HopperBackend.Controllers
{

    [MobileAppController]
    public class ActiveRequestsController : ApiController
    {
        private HopperEntities db = new HopperEntities();

        // GET: api/ActiveRequests
        public IQueryable<ActiveRequest> GetActiveRequests()
        {
            return db.ActiveRequests;
        }

        // GET: api/ActiveRequests/5
        [ResponseType(typeof(ActiveRequest))]
        public IHttpActionResult GetActiveRequest(int id)
        {
            ActiveRequest activeRequest = db.ActiveRequests.Find(id);
            if (activeRequest == null)
            {
                return NotFound();
            }

            return Ok(activeRequest);
        }

        // PUT: api/ActiveRequests/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutActiveRequest(int id, ActiveRequest activeRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != activeRequest.ID)
            {
                return BadRequest();
            }

            db.Entry(activeRequest).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ActiveRequestExists(id))
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

        // POST: api/ActiveRequests
        [ResponseType(typeof(ActiveRequest))]
        public IHttpActionResult PostActiveRequest(ActiveRequest activeRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ActiveRequests.Add(activeRequest);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (ActiveRequestExists(activeRequest.ID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = activeRequest.ID }, activeRequest);
        }

        // DELETE: api/ActiveRequests/5
        [ResponseType(typeof(ActiveRequest))]
        public IHttpActionResult DeleteActiveRequest(int id)
        {
            ActiveRequest activeRequest = db.ActiveRequests.Find(id);
            if (activeRequest == null)
            {
                return NotFound();
            }

            db.ActiveRequests.Remove(activeRequest);
            db.SaveChanges();

            return Ok(activeRequest);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ActiveRequestExists(int id)
        {
            return db.ActiveRequests.Count(e => e.ID == id) > 0;
        }
    }
}