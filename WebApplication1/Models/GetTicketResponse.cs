namespace IncidentProject.Models
{
    public class GetTicketResponse
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public string AccountNo { get; set; }
        public string Branch { get; set; }
        public string ComType { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Date { get; set; }
        public string Status { get; set; }
    }
}
