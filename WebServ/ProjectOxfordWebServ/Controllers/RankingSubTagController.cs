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
    public class RankingSubTagController : ApiController
    {
        private ProjectOxfordWebServContext db = new ProjectOxfordWebServContext();

        // GET: api/Ranking_Sub_Tag
        public IQueryable<RankingSubTag> GetRanking_Sub_Tag()
        {
            return db.Ranking_Sub_Tag;
        }

       
    }
}