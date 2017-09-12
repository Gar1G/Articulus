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
using ProjectOxfordWebServ.Models;

namespace ProjectOxfordWebService.Controllers
{
    public class WebsitePreferencesController : ApiController
    {
        private ProjectOxfordWebServContext db = new ProjectOxfordWebServContext();

        // GET: api/WebsitePreferences
        public IQueryable<WebsitePreferences> GetWebsitePreferences()
        {
            return db.WebsitePreferences;
        }

        // GET: api/WebsitePreferences/5
        [ResponseType(typeof(WebsitePreferences))]
        public IHttpActionResult GetWebsitePreferences(string Email)
        {
            var query = from a in db.WebsitePreferences
                        where a.Email == Email
                        select a;

            List<String> Web_Pref = new List<string>();

            foreach (var a in query)
            {

                Web_Pref.Add(a.Website);

            }

            return Ok(Web_Pref);
        }

        // POST: api/WebsitePreferences
        [ResponseType(typeof(WebsitePreferences))]
        public IHttpActionResult PostWebsitePreferences(WebsitePreferences websitePreferences)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var querytable = db.WebsitePreferences.Where(a => a.Email == websitePreferences.Email && a.Website == websitePreferences.Website).Count();

            if (querytable >= 1)
            {
                return Ok();
            }
            else
            {
                db.WebsitePreferences.Add(websitePreferences);
            }

            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = websitePreferences.Id }, websitePreferences);
        }

        // DELETE: api/WebsitePreferences/5
        [ResponseType(typeof(WebsitePreferences))]
        public IHttpActionResult DeleteWebsitePreferences(string Email, string Website)
        {
            var query = (from a in db.WebsitePreferences
                         where a.Email == Email && a.Website == Website
                         select a).SingleOrDefault();

            if (query == null)
            {
                return Ok(false);
            }

            db.WebsitePreferences.Remove(query);
            db.SaveChanges();

            return Ok(true);
        }

       
    }
}