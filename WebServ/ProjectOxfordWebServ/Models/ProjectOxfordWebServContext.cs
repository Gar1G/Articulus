using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ProjectOxfordWebServ.Models
{
    public class ProjectOxfordWebServContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public ProjectOxfordWebServContext() : base("name=ProjectOxfordWebServContext")
        {
        }

        public System.Data.Entity.DbSet<ProjectOxfordWebServ.Models.ClientLoginInformation> ClientLoginInformations { get; set; }

        public System.Data.Entity.DbSet<ProjectOxfordWebServ.Models.UserByGlobalDistributionUpdate> D_ut_by_d_t_Time_Period_Update { get; set; }

        public System.Data.Entity.DbSet<ProjectOxfordWebServ.Models.GlobalTagDataAllUsers> GlobalTagDataAllUsers { get; set; }

        public System.Data.Entity.DbSet<ProjectOxfordWebServ.Models.SumUserDataGlobal> sum_nt_g { get; set; }

        public System.Data.Entity.DbSet<ProjectOxfordWebServ.Models.TopicPreferences> TopicPreferences { get; set; }

        public System.Data.Entity.DbSet<ProjectOxfordWebServ.Models.WebsitePreferences> WebsitePreferences { get; set; }

        public System.Data.Entity.DbSet<ProjectOxfordWebServ.Models.RankingMainTag> Ranking_Main_Tag { get; set; }

        public System.Data.Entity.DbSet<ProjectOxfordWebServ.Models.UserSubTagEmotionData> UserSubTagEmotionDatas { get; set; }

        public System.Data.Entity.DbSet<ProjectOxfordWebServ.Models.UserDistributionUpdate> D_ut_by_d_t_Time_Period_Update_sub_tags { get; set; }

        public System.Data.Entity.DbSet<ProjectOxfordWebServ.Models.SumUserDataSubTag> sum_nt_sub_tag { get; set; }

        public System.Data.Entity.DbSet<ProjectOxfordWebServ.Models.RankingSubTag> Ranking_Sub_Tag { get; set; }

    }
}
