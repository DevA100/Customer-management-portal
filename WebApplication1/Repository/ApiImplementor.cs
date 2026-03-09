using CustomerIncidentProject.Models;
using IncidentProject.Models;
using IncidentProject.Services;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Net;
using System.Threading.Tasks;

namespace IncidentProject.Repository
{
    public class ApiImplementor : IApiImplementor
    {
        public async Task<ApiResponseObject> GetApiService(string baseurl, string apiurl, CustomerAccountRequest model)
        {
            ApiResponseObject res = new ApiResponseObject();
            try
            {

                var client = new RestClient(baseurl);
                var restRequest = new RestRequest(apiurl, Method.Get);
                restRequest.AddHeader("Accept", "application/json");
                restRequest.AddParameter("accountNo",model.accountNo);

                restRequest.RequestFormat = DataFormat.Json;

                //ssl
                ServicePointManager.ServerCertificateValidationCallback +=
                (sender, certificate, chain, sslPolicyErrors) => true;

                var response = await client.ExecuteAsync(restRequest);

                if (response.StatusCode == HttpStatusCode.OK && response.Content != null)
                {

                    res.statusCode = response.StatusCode;
                    res.responseContent = response.Content;
                }
                else
                {
                    res.statusCode = response.StatusCode;
                    res.responseContent = response.Content;
                }
                return res;
            }
            catch (Exception ex)
            {
                
                res.statusCode = System.Net.HttpStatusCode.InternalServerError;
            }
            return res;
        }

        public async Task<ApiResponseObject> PostApiService(string baseurl, string apiurl, object model)
        {
            ApiResponseObject res = new ApiResponseObject();
            try
            {              
                var client = new RestClient(baseurl);
                var restRequest = new RestRequest(apiurl, Method.Post);
                restRequest.AddHeader("Accept", "application/json");
                restRequest.RequestFormat = DataFormat.Json;
                restRequest.AddJsonBody(model);

                //ssl
                ServicePointManager.ServerCertificateValidationCallback +=
                (sender, certificate, chain, sslPolicyErrors) => true;

                var response = await client.ExecuteAsync(restRequest);

                if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                {

                    res.statusCode = response.StatusCode;
                    res.responseContent = response.Content;
                }
                else
                {
                    res.statusCode = response.StatusCode;
                    res.responseContent = response.Content;
                }
                return res;
            }
            catch (Exception ex)
            {
                // log here
                res.statusCode = System.Net.HttpStatusCode.InternalServerError;
            }
            return res;
        }

        public async Task<ApiResponseObject> PostEmailApiService(string baseurl, string apiurl, EmailRequest model)
        {
            ApiResponseObject res = new ApiResponseObject();
            try
            {
                var client = new RestClient(baseurl);
                var restRequest = new RestRequest(apiurl, Method.Post);




                restRequest.AlwaysMultipartFormData = true;
                restRequest.AddParameter("Recepient", model.Recepient);
                restRequest.AddParameter("Title", model.Title);
                restRequest.AddParameter("Body", model.Body);
                
                ServicePointManager.ServerCertificateValidationCallback +=
                (sender, certificate, chain, sslPolicyErrors) => true;

                var response = await client.ExecuteAsync(restRequest);

                if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
                {

                    res.statusCode = response.StatusCode;
                    res.responseContent = response.Content;
                }
                else
                {
                    res.statusCode = response.StatusCode;
                    res.responseContent = response.Content;
                }
                return res;



            }
            catch (Exception ex)
            {
                
                res.statusCode = System.Net.HttpStatusCode.InternalServerError;
            }
            return res;
        }
    }
}
