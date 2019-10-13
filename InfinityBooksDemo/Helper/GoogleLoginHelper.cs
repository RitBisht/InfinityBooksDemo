using InfinityBooksDemo.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

namespace InfinityBooksDemo.Helper
{
    class GoogleLoginHelper
    {
        public async Task<UserGoogleProfile> StoreGoogleUserData(string code, string clientId, string clientSecret, string redirectUri)
        {
            #region Getting User Details from Google Api

            if(code==null)
            {
                return null;
            }

            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(ConfigurationManager.AppSettings["GoogleMapUrl"])
            };

            //string requestUrl = $"oauth2/v4/token?code={code}&client_id=175142243483-4p6n97bgei5s1ueenmufdj09md20ha4r.apps.googleusercontent.com&client_secret=gVFIilYg9lyBXStAfiv0QAJ2&grant_type=authorization_code&redirect_uri=http://localhost:54418/Login/GoogleDataProcess&state=abcdef";


            var requestUrl = $"oauth2/v4/token?code={code}&client_id={clientId}&client_secret={clientSecret}&grant_type=authorization_code&redirect_uri={redirectUri}&state=abcdef";

            var dict = new Dictionary<string, string>
            {
                { "Content-Type", "application/x-www-form-urlencoded" }
            };
            var req = new HttpRequestMessage(HttpMethod.Post, requestUrl) { Content = new FormUrlEncodedContent(dict) };
            var response =  httpClient.SendAsync(req).Result;
            var token = JsonConvert.DeserializeObject<GmailToken>(await response.Content.ReadAsStringAsync());
            var obj = await GetuserProfile(token.AccessToken);
            #endregion
             return obj;
        }

        private async Task<UserGoogleProfile> GetuserProfile(string accesstoken)
        {
            string googleMapUrl = ConfigurationManager.AppSettings["GoogleMapUrl"];
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(googleMapUrl)
            };
            string url = googleMapUrl + $"/oauth2/v1/userinfo?alt=json&access_token={accesstoken}";
            var response = httpClient.GetAsync(url).Result;
            return JsonConvert.DeserializeObject<UserGoogleProfile>(await response.Content.ReadAsStringAsync());
        }

    }
}
