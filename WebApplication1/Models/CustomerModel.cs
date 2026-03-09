using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IncidentProject.Models
{
    public class CustomerModel
    {
        public int Id { get; set; }

        [Required]
        public string Category { get; set; }

        [Required]
        public string AccountNo { get; set; }
        public string AccountName { get; set; }

        [Required]
        public string Branch { get; set; }

        [Required]
        public string ComType { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public string DateLogged { get; set; }
        public string DateAssignedForProcessing { get; set; }
        public string DateClosed { get; set; }

        public string Status { get; set; }

        public string TicketNo { get; set; }

        public string LoggedInUser { get; set; }

        public string Comments { get; set; }
        public string DocPath { get; set; }
    }
}
