using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using InfinityBooksFunctionApp.Helper;
using InfinityBooksFunctionApp.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace InfinityBooksFunctionApp
{
    public static class UserAuthentication
    {
        
        [FunctionName("UserAuthentication")]
        public static async Task<HttpResponseMessage> UserAuth([HttpTrigger(AuthorizationLevel.Anonymous, "get","post","put", Route = "auth/{id?}")]HttpRequestMessage req, TraceWriter log,string id=null)
        {
            string PrimaryKey = "id";
            string Entity = "infi.Users";
            string PrimaryKeyValue = id;


            GeneralHelper<User> generalHelper = new GeneralHelper<User>(req,Entity,PrimaryKey);

            return await generalHelper.PerformOperationAsync(PrimaryKeyValue);          
        }


        [FunctionName("ProductDetails")]
        public static async Task<HttpResponseMessage> ProductOps([HttpTrigger(AuthorizationLevel.User, "get", "post", "put","delete", Route = "product/{id?}")]HttpRequestMessage req, TraceWriter log, string id = null)
        {
            string PrimaryKey = "productId";
            string Entity = "infi.Products";
            string PrimaryKeyValue = id;

            GeneralHelper<Product> generalHelper = new GeneralHelper<Product>(req, Entity, PrimaryKey);

            return await generalHelper.PerformOperationAsync(PrimaryKeyValue);
        }

        [FunctionName("SaveGoogleUser")]
        public static async Task<HttpResponseMessage> SaveGoogleUser([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "saveGoogleUser")]HttpRequestMessage req, TraceWriter log)
        {
            string code = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "code", true) == 0)
                .Value;
            GoogleLoginHelper googleLoginHelper = new GoogleLoginHelper();
            return await googleLoginHelper.StoreGoogleUserData(req, code);

        }


    }
}
