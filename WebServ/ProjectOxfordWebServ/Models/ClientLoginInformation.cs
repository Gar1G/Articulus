using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectOxfordWebServ.Models
{
    public class ClientLoginInformation
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Salt { get; set; }
        public string HashedSaltedPassword { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}