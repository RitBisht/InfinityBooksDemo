using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace InfinityBooksFunctionApp.Helper
{
    class GeneralHelper<T>
    {
        private HttpRequestMessage _req;
        private string _Entity;
        private string _PrimaryKey;
        public GeneralHelper(HttpRequestMessage req, string Entity, string PrimaryKey)
        {
            _req = req;
            _Entity = Entity;
            _PrimaryKey = PrimaryKey;
        }
       public async Task<HttpResponseMessage> PerformOperationAsync(string id)
        {
            QueryHelper<T> queryHelper = new QueryHelper<T>();
            var reqType = _req.Method.ToString();
            if (string.Equals(reqType, "GET"))
            {
                IEnumerable<KeyValuePair<string, string>> queryParams = queryHelper.GetReqQueryParam(_req, _PrimaryKey, id);
                var resultData = queryHelper.Select(queryParams, null, null, null, null, _Entity);                
                return resultData != null && resultData.Count > 0 ? _req.CreateResponse(HttpStatusCode.OK, resultData) : _req.CreateResponse(HttpStatusCode.Unauthorized, "Data Not Found");
            }

            if (string.Equals(reqType, "POST"))
            {
                IEnumerable<KeyValuePair<string, string>> queryParams = queryHelper.GetReqQueryParam(_req, _PrimaryKey, id);
                var jsonString = await _req.Content.ReadAsStringAsync();
                JObject objectData = JsonConvert.DeserializeObject<JObject>(jsonString);
                var resultData = queryHelper.Insert(objectData, _Entity,_PrimaryKey);
                return resultData != null && resultData.Count > 0 ? _req.CreateResponse(HttpStatusCode.OK, resultData) : _req.CreateResponse(HttpStatusCode.Unauthorized, "Data Not Found");
            }

            if (string.Equals(reqType, "PUT"))
            {
                IEnumerable<KeyValuePair<string, string>> queryParams = queryHelper.GetReqQueryParam(_req, _PrimaryKey, id);
                JObject objectData = await _req.Content.ReadAsAsync<JObject>();
                var resultData = queryHelper.Update(objectData, _PrimaryKey, _Entity, queryParams);
                return resultData != null && resultData.Count > 0 ? _req.CreateResponse(HttpStatusCode.OK, resultData) : _req.CreateResponse(HttpStatusCode.Unauthorized, "Data Not Found");
            }

            if (string.Equals(reqType, "DELETE"))
            {
                IEnumerable<KeyValuePair<string, string>> queryParams = queryHelper.GetReqQueryParam(_req, _PrimaryKey, id);
                JObject objectData = await _req.Content.ReadAsAsync<JObject>();
                var resultData = queryHelper.Delete(queryParams, _Entity);
                return resultData != 0 ? _req.CreateResponse(HttpStatusCode.OK) : _req.CreateResponse(HttpStatusCode.NotFound);
            }

            return _req.CreateResponse(HttpStatusCode.BadRequest);
        }

    }
}
