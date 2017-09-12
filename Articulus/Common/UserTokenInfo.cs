using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class UserTokenInfo
    {
        private static UserTokenInfo instance = null;
        private static object lockThis = new object();

        private UserTokenInfo() { }

        public static UserTokenInfo GetInstance
        {
            get
            {
                lock (lockThis)
                {
                    if (instance == null)
                        instance = new UserTokenInfo();

                    return instance;
                }
            }
        }

        public string access_token { get; set; }
        public string expiry { get; set; }
        public string Email { get; set; }

        public bool isTokenValid()
        {
            if (Convert.ToDateTime(expiry) > DateTime.Now)
                return true;
            return false;
        }

        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public List<string> Topic { get; set; } = new List<string>();
        public List<string> Website { get; set; } = new List<string>();
    }

    public class UserTokenInfoSerialized
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string userName { get; set; }
    }
}
