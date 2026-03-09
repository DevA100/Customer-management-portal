using System;
using System.ComponentModel.DataAnnotations;

namespace IncidentProject.Models
{
    public class UserRequest
    {

        public string Id { get; set; }
        [Required]
        public string staffemail { get; set; }

        [Required]
        public string password { get; set; }
        public int status { get; set; }
        public string last_login_date { get; set; }
        public string name { get; set; }
        public string branch { get; set; }
        public string loggedinuser { get; set; }
    }
}
