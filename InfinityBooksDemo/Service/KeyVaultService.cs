using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;

namespace InfinityBooksDemo.Service
{
    public class KeyVaultService
    {   

        public static string GoogleLoginUrl { get; set; }
        public static string GoogleMapUrl { get; set; }
        public static string GoogleClientSecret { get; set; }
        public static string GoogleClientId { get; set; }
        public static string FacebookLoginUrl { get; set; }
        public static string FacebookClientSecret { get; set; }
        public static string FacebookClientId { get; set; }
        public static string FacebookMapUrl { get; set; }

        public static async Task<string> GetToken(string authority,string resource,string scope)
        {
            var authContext = new AuthenticationContext(authority);
            ClientCredential clientCred = new ClientCredential(WebConfigurationManager.AppSettings["clientId"], WebConfigurationManager.AppSettings["clientSecret"]);

            AuthenticationResult result = authContext.AcquireTokenAsync(resource, clientCred).Result;
            if(result==null)
            {
                throw new InvalidOperationException("Failed to obtain the JWT token");
            }
            return result.AccessToken;

        }
    }
}