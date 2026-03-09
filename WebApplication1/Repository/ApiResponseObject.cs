using System.Net;

namespace IncidentProject.Repository
{
    public class ApiResponseObject
    {
        public string responseContent { get; set; }
        public HttpStatusCode statusCode { get; set; }
        public object StatusCode { get; set; }
    }
}