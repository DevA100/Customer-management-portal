namespace IncidentProject.Models
{
    public class LoginResponse
    {
        public int successCode { get; set; }
        public string response { get; set; }
        public int errorCode { get; set; }

        LoginResponse lp = new LoginResponse()
        {
            response = "",
            successCode = 00,
            errorCode = 404

        };
    }
}
