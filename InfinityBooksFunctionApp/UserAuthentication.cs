using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using InfinityBooksFunctionApp.Helper;
using InfinityBooksFunctionApp.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InfinityBooksFunctionApp
{
    public static class UserAuthentication
    {
        
        [FunctionName("UserAuthentication")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get","post","put", Route = "auth/{id?}")]HttpRequestMessage req, TraceWriter log,string id=null)
        {
            string PrimaryKey = "id";
            string Entity = "infi.Users";
            QueryHelper<User> queryHelper = new QueryHelper<User>();        

            var reqType = req.Method.ToString();
            if(string.Equals(reqType,"GET"))
            {
                IEnumerable<KeyValuePair<string, string>> queryParams=queryHelper.GetReqQueryParam(req, PrimaryKey, id);
                var UserData=queryHelper.Select(queryParams, null, null, null, null, Entity);
                return UserData != null && UserData.Count > 0 ? req.CreateResponse(HttpStatusCode.OK, UserData) : req.CreateResponse(HttpStatusCode.Unauthorized, "User Not Found");
            }

            if (string.Equals(reqType, "POST"))
            {
                IEnumerable<KeyValuePair<string, string>> queryParams = queryHelper.GetReqQueryParam(req, PrimaryKey, id);
                var jsonString = await req.Content.ReadAsStringAsync();                
                JObject objectData=JsonConvert.DeserializeObject<JObject>(jsonString);
                List<User> UserData = queryHelper.Insert(objectData, Entity);
                return UserData != null && UserData.Count > 0 ? req.CreateResponse(HttpStatusCode.OK, UserData) : req.CreateResponse(HttpStatusCode.Unauthorized, "User Not Found");
            }

            if (string.Equals(reqType, "PUT"))
            {
                IEnumerable<KeyValuePair<string, string>> queryParams = queryHelper.GetReqQueryParam(req, PrimaryKey, id);
                JObject objectData = await req.Content.ReadAsAsync<JObject>();
                var UserData = queryHelper.Update(objectData, PrimaryKey, Entity,queryParams);
                return UserData != null && UserData.Count > 0 ? req.CreateResponse(HttpStatusCode.OK, UserData) : req.CreateResponse(HttpStatusCode.Unauthorized, "User Not Found");
            }

            return req.CreateResponse(HttpStatusCode.BadRequest);
        }


        [FunctionName("ProductDetails")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "put","delete", Route = "auth/{id?}")]HttpRequestMessage req, TraceWriter log, string id = null)
        {
            string PrimaryKey = "id";
            string Entity = "infi.Products";
            QueryHelper<User> queryHelper = new QueryHelper<User>();

            var reqType = req.Method.ToString();
            if (string.Equals(reqType, "GET"))
            {
                IEnumerable<KeyValuePair<string, string>> queryParams = queryHelper.GetReqQueryParam(req, PrimaryKey, id);
                var UserData = queryHelper.Select(queryParams, null, null, null, null, Entity);
                return UserData != null && UserData.Count > 0 ? req.CreateResponse(HttpStatusCode.OK, UserData) : req.CreateResponse(HttpStatusCode.Unauthorized, "User Not Found");
            }

            if (string.Equals(reqType, "POST"))
            {
                IEnumerable<KeyValuePair<string, string>> queryParams = queryHelper.GetReqQueryParam(req, PrimaryKey, id);
                var jsonString = await req.Content.ReadAsStringAsync();
                JObject objectData = JsonConvert.DeserializeObject<JObject>(jsonString);
                List<User> UserData = queryHelper.Insert(objectData, Entity);
                return UserData != null && UserData.Count > 0 ? req.CreateResponse(HttpStatusCode.OK, UserData) : req.CreateResponse(HttpStatusCode.Unauthorized, "User Not Found");
            }

            if (string.Equals(reqType, "PUT"))
            {
                IEnumerable<KeyValuePair<string, string>> queryParams = queryHelper.GetReqQueryParam(req, PrimaryKey, id);
                JObject objectData = await req.Content.ReadAsAsync<JObject>();
                var UserData = queryHelper.Update(objectData, PrimaryKey, Entity, queryParams);
                return UserData != null && UserData.Count > 0 ? req.CreateResponse(HttpStatusCode.OK, UserData) : req.CreateResponse(HttpStatusCode.Unauthorized, "User Not Found");
            }

            return req.CreateResponse(HttpStatusCode.BadRequest);
        }
    }
}
