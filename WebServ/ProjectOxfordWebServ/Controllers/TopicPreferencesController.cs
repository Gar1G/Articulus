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
    public class TopicPreferencesController : ApiController
    {
        private ProjectOxfordWebServContext db = new ProjectOxfordWebServContext();

        // GET: api/TopicPreferences
        public IQueryable<TopicPreferences> GetTopicPreferences()
        {
            return db.TopicPreferences;
        }

        // GET: api/TopicPreferences/5
        [ResponseType(typeof(TopicPreferences))]
        public IHttpActionResult GetTopicPreferences(string Email)
        {
            var query = from a in db.TopicPreferences
                        where a.Email == Email
                        select a;

            List<String> Topic_Pref = new List<string>();

            foreach (var a in query)
            {

                Topic_Pref.Add(a.Topic);

            }

            return Ok(Topic_Pref);
        }

        // POST: api/TopicPreferences
        [ResponseType(typeof(TopicPreferences))]
        public IHttpActionResult PostTopicPreferences(TopicPreferences topicPreferences)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var querytable = db.TopicPreferences.Where(a => a.Email == topicPreferences.Email && a.Topic == topicPreferences.Topic).Count();

            if (querytable >= 1)
            {
                return Ok();
            }
            else
            {
                db.TopicPreferences.Add(topicPreferences);
            }
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = topicPreferences.Id }, topicPreferences);
        }

        // DELETE: api/TopicPreferences/5
        public IHttpActionResult DeleteTopicPreferences(string Topic,string Email)
        {

            var query = (from a in db.TopicPreferences
                         where a.Email == Email && a.Topic == Topic
                         select a).SingleOrDefault();

            if (query == null)
            {
                return Ok(false);
            }

            db.TopicPreferences.Remove(query);
            db.SaveChanges();

            return Ok(true);
        }

    
    }
}