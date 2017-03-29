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
    public class RiderReviewsController : ApiController
    {
        private HopperEntities db = new HopperEntities();

        // GET: api/RiderReviews
        public IQueryable<RiderReview> GetRiderReviews()
        {
            return db.RiderReviews;
        }

        // GET: api/RiderReviews/5
        [ResponseType(typeof(RiderReview))]
        public IHttpActionResult GetRiderReview(int id)
        {
            RiderReview riderReview = db.RiderReviews.Find(id);
            if (riderReview == null)
            {
                return NotFound();
            }

            return Ok(riderReview);
        }

        // PUT: api/RiderReviews/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutRiderReview(int id, RiderReview riderReview)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != riderReview.ID)
            {
                return BadRequest();
            }

            db.Entry(riderReview).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RiderReviewExists(id))
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

        // POST: api/RiderReviews
        [ResponseType(typeof(RiderReview))]
        public IHttpActionResult PostRiderReview(RiderReview riderReview)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.RiderReviews.Add(riderReview);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = riderReview.ID }, riderReview);
        }

        // DELETE: api/RiderReviews/5
        [ResponseType(typeof(RiderReview))]
        public IHttpActionResult DeleteRiderReview(int id)
        {
            RiderReview riderReview = db.RiderReviews.Find(id);
            if (riderReview == null)
            {
                return NotFound();
            }

            db.RiderReviews.Remove(riderReview);
            db.SaveChanges();

            return Ok(riderReview);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RiderReviewExists(int id)
        {
            return db.RiderReviews.Count(e => e.ID == id) > 0;
        }
    }
}