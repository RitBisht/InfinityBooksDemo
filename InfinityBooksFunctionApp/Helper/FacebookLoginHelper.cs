using InfinityBooksFunctionApp.Models;
using Microsoft.Owin.Security.Facebook;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace InfinityBooksFunctionApp.Helper
{
    class FacebookLoginHelper
    {

        private readonly HttpClient _httpClient;

        public FacebookLoginHelper()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://graph.facebook.com/v2.8/")

            };
          
        }
    
        public async Task<HttpResponseMessage> StoreFacebookUserData(HttpRequestMessage request,string code)
        {
            string Entity = "infi.Users";
            string PrimaryKey = "id";
            QueryHelper<User> userQueryHelper = new QueryHelper<User>();
            QueryHelper<UserProfile> userProfileQueryHelper = new QueryHelper<UserProfile>();
            if (code==null)
            {
                request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            #region Fetching FaceBook user detail



            var requestUrl = $"/oauth/access_token?authorize?type=client_cred&client_id=750890845352322&redirect_uri=http://localhost:7071/api/saveFacebookUser&code={code}&client_secret=5589ef076c7f9c1027648c505a4113ad&fields=id,name,email,first_name,last_name,age_range,birthday,gender,locale";
            //var requestUrl = $"/oauth/access_token?authorize?type=client_cred&client_id=750890845352322&redirect_uri=http://localhost:7071/api/saveFacebookUser&code={code}&client_secret=5589ef076c7f9c1027648c505a4113ad&fields=id,name,email,first_name,last_name,age_range,birthday,gender,locale";
           
            var req = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            var response = await _httpClient.SendAsync(req);
            var token= JsonConvert.DeserializeObject<FacebookToken>(await response.Content.ReadAsStringAsync());

            var detail = await GetuserProfile(token.AccessToken);
            #endregion

            #region Checking user existence

            List<KeyValuePair<string, string>> parameter = new List<KeyValuePair<string, string>>();
            parameter.Add(new KeyValuePair<string, string>("emailId", detail.Email));
            List<User> userDetail = userQueryHelper.Select(parameter, null, null, null, null, "infi.Users");
            if (userDetail == null || userDetail.Count == 0)
            {
                User newUser = new User() { username = detail.UserName, status = 1, emailId = detail.Email, accountTypeId=1,createdAt = DateTime.UtcNow };
                UserProfile newUserProfile = new UserProfile() { emailId = detail.Email, gender = detail.Gender, firstName = detail.Name, lastName = detail.LastName, createdAt = DateTime.UtcNow };
                userProfileQueryHelper.Insert(JObject.FromObject(newUserProfile), "infi.UserProfiles", PrimaryKey);
                userDetail.AddRange(userQueryHelper.Insert(JObject.FromObject(newUser), Entity, PrimaryKey));
            }

            #endregion

            return userDetail != null && userDetail.Count > 0 ? request.CreateResponse(HttpStatusCode.OK, userDetail) : request.CreateResponse(HttpStatusCode.BadRequest);
        }

        public async Task<UserFacebookProfile> GetuserProfile(string accesstoken)
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://graph.faceboook.com")
            };
            string url = $"https://graph.facebook.com/me?access_token={accesstoken}&fields=id,name,email,first_name,last_name,age_range,birthday,gender,locale";
            var response = await httpClient.GetAsync(url);
            var res = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<UserFacebookProfile>(await response.Content.ReadAsStringAsync());
        }
    }
}
