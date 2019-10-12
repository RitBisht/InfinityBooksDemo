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
using Microsoft.Graph;
using Microsoft.Identity.Client;
using System;
using System.Configuration;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Protocols;

namespace InfinityBooksFunctionApp.Helper
{
    class MicrosoftLoginHelper
    {
        private  static FacebookAuthenticationOptions fbAuthOption;       

        private readonly HttpClient _httpClient;

        
    
        public async Task<HttpResponseMessage> StoreMicrosftUserData(HttpRequestMessage request,string code)
        {
            string accessToken= await GetTokenAsync();
            var graphClient = new GraphServiceClient(
                new DelegateAuthenticationProvider(
                    (requestMessage) =>
                    {
                        // Get back the access token.
                       // string accessToken = 

                        if (!String.IsNullOrEmpty(accessToken))
                        {
                            // Configure the HTTP bearer Authorization Header
                            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", accessToken);
                        }
                        else
                        {
                            throw new Exception("Invalid authorization context");
                        }

                        return (Task.FromResult(0));
                    }
                    ));

            try
            {
                var me = await graphClient.Me.Request().Select("DisplayName").GetAsync();
               // ViewBag.Message = me.DisplayName;
            }
            catch (Exception ex)
            {
                // Skip any exception, so far
            }






            //PublicClientApplication clientApp = new PublicClientApplication(" ");
            //GraphServiceClient graphClient1 = new GraphServiceClient("https://graph.microsoft.com/v1.0", new DelegateAuthenticationProvider(async (requestMessage) => {
            //    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", await GetTokenAsync());
            //}));

            //var currentUser = await graphClient.Me.Request().GetAsync();
            //if (code==null)
            //{
            //    request.CreateResponse(HttpStatusCode.InternalServerError);
            //}

            //var httpClient = new HttpClient
            //{
            //    BaseAddress = new Uri("https://graph.faceboook.com")
            //};

            //var requestUrl = $"/oauth2/v4/access_token?authorize?client_id=750890845352322&redirect_url=http://localhost:77021/api/saveFacebookUser&scope=publish_stream,manage_pages&code={code}&client_secret=5589ef076c7f9c1027648c505a4113ad";
            //var dict = new Dictionary<string, string>
            //{
            //    { "Content-Type", "application/x-www-form-urlencoded" }
            //};
            //var req = new HttpRequestMessage(HttpMethod.Post, requestUrl) { Content = new FormUrlEncodedContent(dict) };
            //var response = await _httpClient.SendAsync(req);
            //var result=JsonConvert.DeserializeObject<UserGoogleProfile>(await response.Content.ReadAsStringAsync());



          
            return null;
        }

        public async Task<string> GetTokenAsync()
        {
            var tenantId = "fd41ee0d-0d97-4102-9a50-c7c3c5470454";
            var clientId = "2f459090-fa00-48c2-acf0-12ad6cc6f552";
            var clientSecret = "l/?NHgZvYL.NTB6x[sp0tVEpif8dlZL5";

            // Configure app builder
            var authority = $"https://login.microsoftonline.com/{tenantId}";
            var app = ConfidentialClientApplicationBuilder
                .Create(clientId)
                .WithClientSecret(clientSecret)
                .WithAuthority(new Uri(authority))
                .Build();

            // Acquire tokens for Graph API
            var scopes = new[] { "https://graph.microsoft.com/.default" };
            var authenticationResult = await app.AcquireTokenForClient(scopes).ExecuteAsync();
            return authenticationResult.AccessToken;
        }

        //public async Task<UserGoogleProfile> GetuserProfile(string accesstoken)
        //{
        //    var httpClient = new HttpClient
        //    {
        //        BaseAddress = new Uri("https://www.googleapis.com")
        //    };
        //    string url = $"https://www.googleapis.com/oauth2/v1/userinfo?alt=json&access_token={accesstoken}";
        //    var response = await httpClient.GetAsync(url);
        //    return JsonConvert.DeserializeObject<UserGoogleProfile>(await response.Content.ReadAsStringAsync());
        //}

    }
}
