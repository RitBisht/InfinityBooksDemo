using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Dapper;
using InfinityBooksFunctionApp.Helper;
using InfinityBooksFunctionApp.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InfinityBooksFunctionApp
{
    public static class InfinityBooksOperations
    {
       [FunctionName("UserAuthentication")]
        public static async Task<HttpResponseMessage> UserAuth([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "auth")]HttpRequestMessage req, TraceWriter log)
        {
            string PrimaryKey = "id";

            string Entity = "infi.Users";
            QueryHelper<User> queryHelper = new QueryHelper<User>();
            var jsonString = await req.Content.ReadAsStringAsync();
            User objectData = JsonConvert.DeserializeObject<User>(jsonString);
            if(!string.IsNullOrEmpty(objectData.emailId) && !string.IsNullOrEmpty(objectData.password))
            {
                List<KeyValuePair<string, string>>  qParameters = new List<KeyValuePair<string, string>>();
                qParameters.Add(new KeyValuePair<string, string>("email", Convert.ToString(objectData.emailId)));
                qParameters.Add(new KeyValuePair<string, string>("password", Convert.ToString(objectData.password)));               
                var resultData = queryHelper.Select(qParameters, null, null, null, null, Entity);

                var cookie = new CookieHeaderValue("userId", Convert.ToString(resultData.First().id));

                cookie.Expires = DateTimeOffset.Now.AddDays(1);
                //cookie.Domain = Request.RequestUri.Host;
                cookie.Path = "/";

                if(resultData!=null)
                {
                    var resp = req.CreateResponse(HttpStatusCode.OK, resultData);

                    var cookieVal = new CookieHeaderValue("userId", Convert.ToString(resultData.First().id));
                    cookie.Expires = DateTimeOffset.Now.AddDays(1);
                    cookie.Path = "/";
                    resp.Headers.AddCookies(new CookieHeaderValue[] { cookieVal });
                    return resp;
                }
                else
                {
                    req.CreateResponse(HttpStatusCode.Unauthorized, "Data Not Found");
                }
                //return resultData != null && resultData.Count > 0 ? req.CreateResponse(HttpStatusCode.OK, resultData).Headers.AddCookies(new CookieHeaderValue[] { cookie}) : req.CreateResponse(HttpStatusCode.Unauthorized, "Data Not Found");
               
            }
            return req.CreateResponse(HttpStatusCode.Unauthorized, "Data Not Found");
          }


        [FunctionName("ProductDetails")]
        public static async Task<HttpResponseMessage> ProductOps([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "product/{id?}")]HttpRequestMessage req, TraceWriter log, string id = null)
        {
            string PrimaryKey = "productId";
            string Entity = "infi.Products";
            string PrimaryKeyValue = id;
            List<KeyValuePair<string, string>> qParameters;

            QueryHelper<Product> queryHelper = new QueryHelper<Product>();
            QueryHelper<ProductImage> productImageQueryHelper = new QueryHelper<ProductImage>();
            QueryHelper<ProductCategory> productCategoryQueryHelper = new QueryHelper<ProductCategory>();
            QueryHelper<ProductType> productTypeQueryHelper = new QueryHelper<ProductType>();


            IEnumerable<KeyValuePair<string, string>> queryParams = queryHelper.GetReqQueryParam(req, PrimaryKey, id);
            
            var searchParam = queryParams.Where(x => x.Key.ToUpper() == "SEARCHSTRING").Select(x => x.Value);
            if (searchParam!=null && searchParam.Count()>0)
            {
                //string searchParam = searchqueryCount.
                string query = string.Concat("select * from ", Entity, " where author LIKE '%", searchParam.First(), "%' OR productCode LIKE '%", searchParam.First(), "%' OR name LIKE '%", searchParam.First(), "%'");
                using (var connection = new SqlConnection(Environment.GetEnvironmentVariable("SQLConnectionString")))
                {
                    var product = connection.Query<Product>(query).ToList();
                    var prodIdList = product.Select(x => x.productId);
                    var idString = string.Join(",", prodIdList);
                    List<KeyValuePair<string, string>> tempqueryParam = new List<KeyValuePair<string, string>>();
                    tempqueryParam.AddRange(queryParams);
                    tempqueryParam.Add(new KeyValuePair<string, string>("productId.IN", idString));
                    queryParams = tempqueryParam;
                }
            }
            

            var resultData = queryHelper.Select(queryParams, null, null, null, null, Entity);

            if(resultData!=null && resultData.Count>0)
            {
                foreach (var product in resultData)
                {
                    qParameters = new List<KeyValuePair<string, string>>();
                    qParameters.Add(new KeyValuePair<string, string>("productId", Convert.ToString(product.productId)));
                    qParameters.Add(new KeyValuePair<string, string>("productCategoriesId", Convert.ToString(product.productCategoryId)));
                    qParameters.Add(new KeyValuePair<string, string>("productTypeId", Convert.ToString(product.productTypeId)));


                    var prodCatgory=productCategoryQueryHelper.Select(qParameters, null, null, null, null, "infi.ProductCategories");
                    product.productCategoryName = prodCatgory.First().name;

                    var prodType = productTypeQueryHelper.Select(qParameters, null, null, null, null, "infi.ProductType");
                    product.productTypeName = prodType.First().Name;

                    var imageDataResult=productImageQueryHelper.Select(qParameters, null, null, null, null, "infi.ProductImages");
                    if(imageDataResult!=null && imageDataResult.Count>0)
                    {
                        product.productImages = imageDataResult;
                    }
                }
            }
            return req.CreateResponse(HttpStatusCode.OK, resultData);
        }

        [FunctionName("UserProfile")]
        public static async Task<HttpResponseMessage> UserProfile([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "put", "delete", Route = "userProfile/{id?}")]HttpRequestMessage req, TraceWriter log, string id = null)
        {
            string PrimaryKey = "id";
            string Entity = "infi.UserProfiles";
            string PrimaryKeyValue = id;
            List<KeyValuePair<string, string>> qParameters;
            QueryHelper<Address> addressQueryHelper = new QueryHelper<Address>();

            QueryHelper<UserProfile> userProfileHelper = new QueryHelper<UserProfile>();
            QueryHelper<User> userHelper = new QueryHelper<User>();
            var reqType = req.Method.ToString();
            if (string.Equals(reqType, "GET"))
            {
                IEnumerable<KeyValuePair<string, string>> queryParams = userProfileHelper.GetReqQueryParam(req, PrimaryKey, id);
                var resultData = userProfileHelper.Select(queryParams, null, null, null, null, Entity);
                if (resultData != null && resultData.Count > 0)
                {
                    foreach (var userprofile in resultData)
                    {
                        qParameters = new List<KeyValuePair<string, string>>();
                        qParameters.Add(new KeyValuePair<string, string>("userId", Convert.ToString(userprofile.userId)));
                        qParameters.Add(new KeyValuePair<string, string>("addressTypeId","1"));                     


                        var addresses = addressQueryHelper.Select(qParameters, null, null, null, null, "infi.Addresses");
                        if (addresses != null && addresses.Count > 0)
                        {
                            userprofile.address = addresses.First();
                        }

                      
                    }
                }
                return resultData != null && resultData.Count > 0 ? req.CreateResponse(HttpStatusCode.OK, resultData) : req.CreateResponse(HttpStatusCode.Unauthorized, "Data Not Found");
            }

            if (string.Equals(reqType, "POST"))
            {
                IEnumerable<KeyValuePair<string, string>> queryParams = userProfileHelper.GetReqQueryParam(req, PrimaryKey, id);
                var jsonString = await req.Content.ReadAsStringAsync();
                var objectData = JsonConvert.DeserializeObject<UserProfile>(jsonString);
                if(!string.IsNullOrEmpty(objectData.emailId))
                {
                    qParameters = new List<KeyValuePair<string, string>>();
                    qParameters.Add(new KeyValuePair<string, string>("emailId", objectData.emailId));
                    //qParameters.Add(new KeyValuePair<string, string>("addressTypeId", "1"));
                    var userSearch=userHelper.Select(qParameters, null, null, null, null, "infi.Users");
                    if(userSearch==null || userSearch.Count==0)
                    {
                        req.CreateResponse(HttpStatusCode.Conflict);
                    }

                }

                var newUser = new User() { emailId = objectData.emailId, password = objectData.password, status = 1 };
                var userResult = userHelper.Insert(JObject.FromObject(newUser), "infi.Users", "id");

                objectData.userId = userResult.First().id;

                var resultData = userProfileHelper.Insert(JObject.FromObject(objectData), Entity, PrimaryKey);
               
                if (resultData != null)
                {
                    
                    if (objectData.address!=null)
                    {
                        var add = objectData.address;
                        add.userId = resultData.First().userId;
                        add.status = 1;
                        add.addressTypeId = 1;
                        addressQueryHelper.Insert(JObject.FromObject(add), "infi.addresses", "addressesId");
                    }
                }
                return resultData != null && resultData.Count > 0 ? req.CreateResponse(HttpStatusCode.OK, resultData) : req.CreateResponse(HttpStatusCode.Unauthorized, "Data Not Found");
            }

            if (string.Equals(reqType, "PUT"))
            {
                IEnumerable<KeyValuePair<string, string>> queryParams = userProfileHelper.GetReqQueryParam(req, PrimaryKey, id);
                var objectData = await req.Content.ReadAsAsync<UserProfile>();

                var resultData = userProfileHelper.Update(JObject.FromObject(objectData), PrimaryKey, Entity, queryParams);
                if (resultData != null)
                {
                    if (objectData.address != null)
                    {
                        qParameters = new List<KeyValuePair<string, string>>();
                        qParameters.Add(new KeyValuePair<string, string>("userId", Convert.ToString(resultData.First().userId)));
                        qParameters.Add(new KeyValuePair<string, string>("addressTypeId", "1"));
                        objectData.address.addressTypeId = 1;
                        var add = objectData.address;
                        if (add.addressesId != 0)
                        {
                            addressQueryHelper.Update(JObject.FromObject(add), "addressesId","infi.addresses",qParameters);
                        }
                        else
                        {
                            add.addressTypeId = 1;
                            addressQueryHelper.Insert(JObject.FromObject(add), "infi.addresses", "addressesId");
                        }
                    }
                }
                return resultData != null && resultData.Count > 0 ? req.CreateResponse(HttpStatusCode.OK, resultData) : req.CreateResponse(HttpStatusCode.Unauthorized, "Data Not Found");
            }
            return null;
           
        }


        [FunctionName("CartDetails")]
        public static async Task<HttpResponseMessage> CartOps([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "put", "delete", Route = "cart/{id?}")]HttpRequestMessage req, TraceWriter log, string id = null)
        {
            string PrimaryKey = "id";
            string Entity = "infi.Cart";
            string PrimaryKeyValue = id;
            List<KeyValuePair<string, string>> qParameters;
            QueryHelper<ProductImage> productImageQueryHelper = new QueryHelper<ProductImage>();
            QueryHelper<Product> productQueryHelper = new QueryHelper<Product>();

            QueryHelper<Cart> queryHelper = new QueryHelper<Cart>();
            var reqType = req.Method.ToString();
            if (string.Equals(reqType, "GET"))
            {
                IEnumerable<KeyValuePair<string, string>> queryParams = queryHelper.GetReqQueryParam(req, PrimaryKey, id);
                var resultData = queryHelper.Select(queryParams, null, null, null, null, Entity);
                if (resultData != null && resultData.Count > 0)
                {
                    foreach (var cartItem in resultData)
                    {
                        qParameters = new List<KeyValuePair<string, string>>();
                        qParameters.Add(new KeyValuePair<string, string>("productId", Convert.ToString(cartItem.productId)));
                        qParameters.Add(new KeyValuePair<string, string>("statusTypeId", "1"));


                        var products = productQueryHelper.Select(qParameters, null, null, null, null, "infi.Products");
                        if (products != null && products.Count > 0)
                        {
                            cartItem.productdetail = products.First();
                            var imageDataResult = productImageQueryHelper.Select(qParameters, null, null, null, null, "infi.ProductImages");
                            if (imageDataResult != null && imageDataResult.Count > 0)
                            {
                                cartItem.productdetail.productImages = imageDataResult;
                            }
                        }


                    }
                }
                return req.CreateResponse(HttpStatusCode.OK, resultData);
            }

            if (string.Equals(reqType, "POST"))
            {
                IEnumerable<KeyValuePair<string, string>> queryParams = queryHelper.GetReqQueryParam(req, PrimaryKey, id);
                var jsonString = await req.Content.ReadAsStringAsync();
                JObject objectData = JsonConvert.DeserializeObject<JObject>(jsonString);
                List<KeyValuePair<string, string>> tempParameters=new List<KeyValuePair<string, string>>();
                tempParameters.Add(new KeyValuePair<string, string>("userId",objectData["userId"].ToString()));
                tempParameters.Add(new KeyValuePair<string, string>("productId", objectData["productId"].ToString()));
                var searchResult=queryHelper.Select(tempParameters, null, null, null, null, Entity);
                List<Cart> resultData = null;
                if(searchResult!=null && searchResult.Count>0)
                {
                    var cartValue = searchResult.First();
                    cartValue.quantity++;
                    resultData = queryHelper.Update(JObject.FromObject(cartValue), PrimaryKey, Entity, tempParameters);
                }
                else
                {
                    resultData = queryHelper.Insert(objectData, Entity, PrimaryKey);
                }
                
                return resultData != null && resultData.Count > 0 ? req.CreateResponse(HttpStatusCode.OK, resultData) : req.CreateResponse(HttpStatusCode.NoContent, "Data Not Found");
            }

            if (string.Equals(reqType, "PUT"))
            {
                IEnumerable<KeyValuePair<string, string>> queryParams = queryHelper.GetReqQueryParam(req, PrimaryKey, id);
                JObject objectData = await req.Content.ReadAsAsync<JObject>();
                var resultData = queryHelper.Update(objectData, PrimaryKey, Entity, queryParams);
                return resultData != null && resultData.Count > 0 ? req.CreateResponse(HttpStatusCode.OK, resultData) : req.CreateResponse(HttpStatusCode.NoContent, "Data Not Found");
            }

            if (string.Equals(reqType, "DELETE"))
            {
                IEnumerable<KeyValuePair<string, string>> queryParams = queryHelper.GetReqQueryParam(req, PrimaryKey, id);
                JObject objectData = await req.Content.ReadAsAsync<JObject>();
                var resultData = queryHelper.Delete(queryParams, Entity);
                return resultData != 0 ? req.CreateResponse(HttpStatusCode.OK) : req.CreateResponse(HttpStatusCode.NotFound);
            }

            GeneralHelper<Product> generalHelper = new GeneralHelper<Product>(req, Entity, PrimaryKey);

            return await generalHelper.PerformOperationAsync(PrimaryKeyValue);
        }

        //[FunctionName("SaveGoogleUser")]
        //public static async Task<HttpResponseMessage> SaveGoogleUser([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "saveGoogleUser")]HttpRequestMessage req, TraceWriter log)
        //{
        //   Request.

        //}


        [FunctionName("SaveGoogleUser")]
        public static async Task<HttpResponseMessage> SaveGoogleUser([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "saveGoogleUser")]HttpRequestMessage req, TraceWriter log)
        {
            string code = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "code", true) == 0)
                .Value;
            GoogleLoginHelper googleLoginHelper = new GoogleLoginHelper();
            return googleLoginHelper.StoreGoogleUserData(req, code).Result;

        }
               

        [FunctionName("SaveFacebookUser")]
        public static async Task<HttpResponseMessage> SaveFacebookUser([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "saveFacebookUser")]HttpRequestMessage req, TraceWriter log)
        {
            string code = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "code", true) == 0)
                .Value;

            var param = req.GetQueryNameValuePairs();
            FacebookLoginHelper facebookLoginHelper = new FacebookLoginHelper();
            return await facebookLoginHelper.StoreFacebookUserData(req, code);

        }

        [FunctionName("MicrosoftVerification")]
        public static async Task<HttpResponseMessage> MicrosoftVerification([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "microsoftVerification")]HttpRequestMessage req, TraceWriter log)
        {
            //string code = req.GetQueryNameValuePairs()
            //    .FirstOrDefault(q => string.Compare(q.Key, "code", true) == 0)
            //    .Value;
            MicrosoftLoginHelper microsoftLoginHelper = new MicrosoftLoginHelper();
            return await microsoftLoginHelper.StoreMicrosftUserData(req, null);

        }


    }
}
