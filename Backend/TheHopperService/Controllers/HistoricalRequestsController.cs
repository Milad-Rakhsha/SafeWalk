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
using TheHopperService.Models;

namespace TheHopperService.Controllers
{
    public class HistoricalRequestsController : ApiController
    {
        private HopperEntities db = new HopperEntities();

        // GET: api/HistoricalRequests
        public IQueryable<HistoricalRequest> GetHistoricalRequests()
        {
            return db.HistoricalRequests;
        }

        // GET: api/HistoricalRequests/5
        [ResponseType(typeof(HistoricalRequest))]
        public IHttpActionResult GetHistoricalRequest(int id)
        {
            HistoricalRequest historicalRequest = db.HistoricalRequests.Find(id);
            if (historicalRequest == null)
            {
                return NotFound();
            }

            return Ok(historicalRequest);
        }

        // PUT: api/HistoricalRequests/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutHistoricalRequest(int id, HistoricalRequest historicalRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != historicalRequest.ID)
            {
                return BadRequest();
            }

            db.Entry(historicalRequest).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HistoricalRequestExists(id))
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

        // POST: api/HistoricalRequests
        [ResponseType(typeof(HistoricalRequest))]
        public IHttpActionResult PostHistoricalRequest(HistoricalRequest historicalRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.HistoricalRequests.Add(historicalRequest);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = historicalRequest.ID }, historicalRequest);
        }

        // DELETE: api/HistoricalRequests/5
        [ResponseType(typeof(HistoricalRequest))]
        public IHttpActionResult DeleteHistoricalRequest(int id)
        {
            HistoricalRequest historicalRequest = db.HistoricalRequests.Find(id);
            if (historicalRequest == null)
            {
                return NotFound();
            }

            db.HistoricalRequests.Remove(historicalRequest);
            db.SaveChanges();

            return Ok(historicalRequest);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool HistoricalRequestExists(int id)
        {
            return db.HistoricalRequests.Count(e => e.ID == id) > 0;
        }
    }
}