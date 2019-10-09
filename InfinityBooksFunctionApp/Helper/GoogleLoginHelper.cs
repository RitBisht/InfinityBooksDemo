using InfinityBooksFunctionApp.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace InfinityBooksFunctionApp.Helper
{
    class GoogleLoginHelper
    {
        public async Task<HttpResponseMessage> StoreGoogleUserData(HttpRequestMessage request,string code)
        {
            string Entity = "infi.Users";
            string PrimaryKey = "id";
            QueryHelper<User> userQueryHelper = new QueryHelper<User>();
            if (string.IsNullOrEmpty(code))
            {
                request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://www.googleapis.com")
            };
            var requestUrl = $"oauth2/v4/token?code={code}&client_id=175142243483-4p6n97bgei5s1ueenmufdj09md20ha4r.apps.googleusercontent.com&client_secret=gVFIilYg9lyBXStAfiv0QAJ2&grant_type=authorization_code&redirect_uri=http://localhost:7071/api/saveGoogleUser&state=abcdef";

            var dict = new Dictionary<string, string>
            {
                { "Content-Type", "application/x-www-form-urlencoded" }
            };
            var req = new HttpRequestMessage(HttpMethod.Post, requestUrl) { Content = new FormUrlEncodedContent(dict) };
            var response = await httpClient.SendAsync(req);
            var token = JsonConvert.DeserializeObject<GmailToken>(await response.Content.ReadAsStringAsync());
            //Session["user"] = token.AccessToken;
            var obj = await GetuserProfile(token.AccessToken);

            #region Checking user existence

            
            List<KeyValuePair<string, string>> parameter=new List<KeyValuePair<string, string>>();
            parameter.Add(new KeyValuePair<string, string>("emailId", obj.Email));
            List<User> userDetail=userQueryHelper.Select(parameter,null,null,null,null,"infi.Users");
            if(userDetail==null || userDetail.Count==0)
            {
                User newUser = new User() { username=obj.GivenName,status=1,emailId=obj.Email,createdAt=DateTime.UtcNow};
                userDetail.AddRange(userQueryHelper.Insert(JObject.FromObject(newUser), Entity, PrimaryKey));
            }


            #endregion

            return request.CreateResponse(HttpStatusCode.OK,);
        }

        public async Task<UserGoogleProfile> GetuserProfile(string accesstoken)
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://www.googleapis.com")
            };
            string url = $"https://www.googleapis.com/oauth2/v1/userinfo?alt=json&access_token={accesstoken}";
            var response = await httpClient.GetAsync(url);
            return JsonConvert.DeserializeObject<UserGoogleProfile>(await response.Content.ReadAsStringAsync());
        }

    }
}
