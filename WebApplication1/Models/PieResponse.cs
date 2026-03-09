namespace IncidentProject.Models
{
    public class PieResponse
    {
        public PieResponse()
        {
            Categories = new string[] { };
            Values = new int[] { };
        }

        public string[] Categories { get; set; }
        public int[] Values { get; set; }
    }
}
