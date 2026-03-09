using System;

namespace IncidentProject.Models
{
    public class CustomerAccountResponse
    {
        public string accountNumber { get; set; }
        public string accountName { get; set; }
        public string branchSolid { get; set; }
        public string branchName { get; set; }
        public string customerId { get; set; }
        public string email { get; set; }
        public string mainAddress { get; set; }
        public string mainCity { get; set; }
        public string mainState { get; set; }
        public string mainCountry { get; set; }
        public string accountOfficerCode { get; set; }
        public object accountOfficerName { get; set; }
        public string mainPhone { get; set; }
        public string alternatePhone { get; set; }
        public string bankAccountType { get; set; }
        public DateTime openingDate { get; set; }
        public string bvn { get; set; }
        public DateTime dob { get; set; }
        public int accountStatus { get; set; }
        public string schmCode { get; set; }
        public string gender { get; set; }
    }
}
