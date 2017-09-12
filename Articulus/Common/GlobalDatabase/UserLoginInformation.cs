using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Newsify6.Common.GlobalDatabase
{ 
    /// <summary>
    /// Table will contain information about a specific user;
    /// The topic and website preferences will be stored as json strings;
    /// When posting to this table, which already contains the specific email, the old entry will be deleted and the new one will be stored;
    /// Thus, when updating any of the credentials, remember to make sure that all the other information is correct;
    /// </summary>
    public class UserLoginInformation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string TopicJsonStringList { get; set; }
        public string WebsiteJsonStringList { get; set; }
    }
}
