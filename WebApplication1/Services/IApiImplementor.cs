using CustomerIncidentProject.Models;
using IncidentProject.Models;
using IncidentProject.Repository;
using System.Threading.Tasks;

namespace IncidentProject.Services
{
    public interface IApiImplementor
    {
        Task<ApiResponseObject> PostApiService(string baseurl, string apiurl, object model);
        Task<ApiResponseObject> PostEmailApiService(string baseurl, string apiurl, EmailRequest model);
        Task<ApiResponseObject> GetApiService(string baseurl, string apiurl, CustomerAccountRequest model);
        
    }
}
