using InfinityBooksDemo.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using InfinityBooksDemo.Helper;
using System.Configuration;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;

namespace InfinityBooksDemo.Controllers
{
    public class LoginController : Controller
    {
        
        public async System.Threading.Tasks.Task<ActionResult> UserLogin(string email, string password)
        {
            TelemetryClient tClient = new TelemetryClient(new TelemetryConfiguration("5b3786e8-3c01-4f85-8891-4970e8113389"));

            tClient.TrackPageView("UserLogin");
            IEnumerable<User> user = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["Azfunctionurl"]);
                if(string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    ViewBag.ErrorMessage = "Field Missing";
                    return View();
                }
                var json = JsonConvert.SerializeObject(new User() {emailId= email, password=password });

                var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
                //HTTP GET
                var responseTask = client.PostAsync("auth", stringContent);
                //var responseTask = client.GetAsync("cart");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {                    
                    var readTask = JsonConvert.DeserializeObject<List<User>>(await result.Content.ReadAsStringAsync());
                    user = readTask;
                    Response.Cookies.Add(new HttpCookie("userId", Convert.ToString(readTask.First().id)));
                    Response.Cookies["userId"].Expires = DateTime.Now.AddSeconds(600);
                    Request.Cookies.Add(new HttpCookie("userId", Convert.ToString(readTask.First().id)));
                    Session.Add("userId", Convert.ToString(readTask.First().id));
                    return RedirectToAction("Products","Products");
                }
                if(result.StatusCode==System.Net.HttpStatusCode.Unauthorized)
                {
                    ModelState.AddModelError(string.Empty, "User is not authorize");
                }
                else
                {                   
                    user = Enumerable.Empty<User>();

                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            ViewBag.ErrorMessage = "Invalid User or Password";
            return View();
        }

        public ActionResult UserGoogleLoginHelper(string code)
        {
            GoogleLoginHelper helper = new GoogleLoginHelper();
            var res = helper.StoreGoogleUserData(code);
            return null;
        }

        public async Task<ActionResult> UserGoogleLogin()
        {
           // Request.Re
            WebRequestHandler handler = new WebRequestHandler();
            handler.CookieContainer = new System.Net.CookieContainer();
            handler.UseCookies = true;
            handler.UseDefaultCredentials = true;
            IEnumerable<User> user = null;
            using (var client = new HttpClient(handler))
            {
                client.BaseAddress = new Uri("http://localhost:7071/api/googleVerification");
                //HTTP GET
                var responseTask = client.GetAsync("googleVerification");
                //var responseTask = client.GetAsync("cart");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    //int count = handler.CookieContainer.Count;
                    //Cookie serverCookie = handler.CookieContainer.
                    //                      GetCookies(new Uri("http://localhost:7071/api/auth"))
                    //                      ["userId"];

                    var res = result.Content.ReadAsByteArrayAsync().Result;
                    var bytesAsString = Encoding.UTF8.GetString(res);
                    return base.Content(bytesAsString, "text/html");

                    //var readTask = JsonConvert.DeserializeObject<List<User>>(await result.Content.ReadAsStringAsync());
                    //user = readTask;
                    //Response.Cookies.Add(new HttpCookie("userId", Convert.ToString(readTask.First().id)));
                    //return RedirectToAction("Products", "Products");
                }
                if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    ModelState.AddModelError(string.Empty, "User is not authorize");
                }
                else //web api sent error response 
                {
                    //log response status here..

                    user = Enumerable.Empty<User>();

                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            return View();
        }

        public async Task<ActionResult> UserFacebookLogin()
        {
            WebRequestHandler handler = new WebRequestHandler();
            handler.CookieContainer = new System.Net.CookieContainer();
            handler.UseCookies = true;
            handler.UseDefaultCredentials = true;
            IEnumerable<User> user = null;
            using (var client = new HttpClient(handler))
            {
                client.BaseAddress = new Uri("http://localhost:7071/api/");
                //HTTP GET
                var responseTask = client.GetAsync("facebookVerification");
                //var responseTask = client.GetAsync("cart");
                //responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    //int count = handler.CookieContainer.Count;
                    //Cookie serverCookie = handler.CookieContainer.
                    //                      GetCookies(new Uri("http://localhost:7071/api/auth"))
                    //                      ["userId"];
                    var res = result.Content.ReadAsByteArrayAsync().Result;
                    var bytesAsString = Encoding.UTF8.GetString(res);
                    return base.Content(bytesAsString, "text/html");
                    var readTask = JsonConvert.DeserializeObject<List<User>>(bytesAsString);
                    user = readTask;
                    Response.Cookies.Add(new HttpCookie("userId", Convert.ToString(readTask.First().id)));
                    return RedirectToAction("Products", "Products");
                }
                if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    ModelState.AddModelError(string.Empty, "User is not authorize");
                }
                else //web api sent error response 
                {
                    //log response status here..

                    user = Enumerable.Empty<User>();

                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            return View();
        }
    }
}