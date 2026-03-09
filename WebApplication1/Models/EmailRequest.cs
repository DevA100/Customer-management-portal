namespace IncidentProject.Models
{
    public class EmailRequest
    {
        public string Recepient { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Attachments { get; set; }
    }
}
