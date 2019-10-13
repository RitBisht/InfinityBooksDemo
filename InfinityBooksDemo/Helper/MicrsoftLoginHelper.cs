using InfinityBooksDemo.Models;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

namespace InfinityBooksDemo.Helper
{
    class MicrsoftLoginHelper
    {    
        public async Task<UserFacebookProfile> StoreMicrosoftUserData(string code, string facebookClientId, string facebookClientSecret, string redirectUri)
        {
            if(code==null)
            {
                return null;
            }
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(ConfigurationManager.AppSettings["FacebookMapUrl"] + "/v2.8/")
            };

            var requestUrl = $"/oauth/access_token?authorize?type=client_cred&client_id={facebookClientId}&redirect_uri={redirectUri}&code={code}&client_secret={facebookClientSecret}&fields=id,name,email,first_name,last_name,age_range,birthday,gender,locale";
          
            var req = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            var response = httpClient.SendAsync(req).Result;
            var token= JsonConvert.DeserializeObject<FacebookToken>(await response.Content.ReadAsStringAsync());

            var detail =GetuserProfile(token.AccessToken).Result;

            return detail;
        }

        public async Task<UserFacebookProfile> GetuserProfile(string accesstoken)
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(ConfigurationManager.AppSettings["FacebookMapUrl"])
            };
            string url = $"/me?access_token={accesstoken}&fields=id,name,email,first_name,last_name,age_range,birthday,gender,locale";
            var response = httpClient.GetAsync(url).Result;
            var res =response.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<UserFacebookProfile>(response.Content.ReadAsStringAsync().Result);
        }
    }
}
