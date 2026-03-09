namespace IncidentProject.Models
{
    public class BarResponse
    {
        public BarResponse()
        {
            Branches = new string[] { };
            Values = new int[] { };
        }

        public string[] Branches { get; set; }
        public int[] Values { get; set; }
    }
}
