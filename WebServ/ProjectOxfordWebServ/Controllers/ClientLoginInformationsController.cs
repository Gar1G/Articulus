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

namespace ProjectOxfordWebServ.Controllers
{
    public class ClientLoginInformationsController : ApiController
    {
        private ProjectOxfordWebServContext db = new ProjectOxfordWebServContext();

        // GET: api/ClientLoginInformations
        // Return All info in Table
        public IQueryable<ClientLoginInformation> GetClientLoginInformations()
        {
            return db.ClientLoginInformations;
        }

        // GET: api/ClientLoginInformations/5
        // Returns A specific Users Data
        [ResponseType(typeof(ClientLoginInformation))]
        public IHttpActionResult GetClientLoginInformation(string Email)
        {
            var query = (from a in db.ClientLoginInformations
                         where a.Email == Email
                         select a).SingleOrDefault();

            if (query == null)
            {
                return NotFound();
            }

            return Ok(query);
        }

        // POST: api/ClientLoginInformations
        public IHttpActionResult PostClientLoginInformation(ClientLoginInformation clientLoginInformation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ClientLoginInformations.Add(clientLoginInformation);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = clientLoginInformation.Id }, clientLoginInformation);
        }

        // DELETE: api/ClientLoginInformations/5
        [ResponseType(typeof(ClientLoginInformation))]
        public IHttpActionResult DeleteClientLoginInformation(int id)
        {
            ClientLoginInformation clientLoginInformation = db.ClientLoginInformations.Find(id);
            if (clientLoginInformation == null)
            {
                return NotFound();
            }

            db.ClientLoginInformations.Remove(clientLoginInformation);
            db.SaveChanges();

            return Ok(clientLoginInformation);
        }

    }
}