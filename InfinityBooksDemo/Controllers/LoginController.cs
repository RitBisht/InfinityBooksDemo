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
using InfinityBooksDemo.Service;

namespace InfinityBooksDemo.Controllers
{
    public class LoginController : Controller
    {

        private string googleClientId;
        private string googleclientSecret;
        private string facebookClientId;
        private string facebookClientSecret;
        private string microsoftClientId;
        private string microsoftClientSecret;
        private string redirectUri;

        public LoginController()
        {
            googleClientId = KeyVaultService.GoogleClientId;
            redirectUri = ConfigurationManager.AppSettings["RedirectUri"];
            googleclientSecret = KeyVaultService.GoogleClientSecret;
            facebookClientId = KeyVaultService.FacebookClientId;
            facebookClientSecret = KeyVaultService.FacebookClientSecret;
            microsoftClientId = ConfigurationManager.AppSettings["MicrosoftClientId"];
            microsoftClientSecret = ConfigurationManager.AppSettings["MicrosoftClientSecret"];
        }

        public async System.Threading.Tasks.Task<ActionResult> UserLogin(string email, string password)
        {
            TelemetryClient tClient = new TelemetryClient(new TelemetryConfiguration("5b3786e8-3c01-4f85-8891-4970e8113389"));

            tClient.TrackPageView("UserLogin");
            IEnumerable<User> user = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["Azfunctionurl"]);
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    return View();
                }
                var json = JsonConvert.SerializeObject(new User() { emailId = email, password = password });

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
                    ViewBag.Loginned = true;
                    return RedirectToAction("Products", "Products");
                }
                if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    ViewBag.ErrorMessage = "Invalid User or Password";
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


        public RedirectResult UserMicrosoftLogin()
        {
            string microsoftLoginuri = ConfigurationManager.AppSettings["MicrosoftLoginUri"].Replace("{clientId}", microsoftClientId).Replace("{redirectUri}", string.Concat(redirectUri, "MicrosoftDataProcess"));
            return Redirect(microsoftLoginuri);
        }

        public async Task<ActionResult> MicrosoftDataProcess(string code)
        {
            GoogleLoginHelper googleHelper = new GoogleLoginHelper();
            var userData = googleHelper.StoreGoogleUserData(code, googleClientId, googleclientSecret, string.Concat(redirectUri, "GoogleDataProcess")).Result;

            if (userData != null)
            {
                IEnumerable<User> user = null;
                using (var client = new HttpClient())
                {
                    if (userData != null)
                    {
                        client.BaseAddress = new Uri(ConfigurationManager.AppSettings["Azfunctionurl"]);
                        var json = JsonConvert.SerializeObject(userData);

                        var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
                        var responseTask = client.PostAsync("saveGoogleUser", stringContent);
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
                            ViewBag.Loginned = true;
                            return RedirectToAction("Products", "Products");
                        }
                        if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        {
                            ViewBag.ErrorMessage = "Invalid User or Password";
                        }
                        else
                        {
                            user = Enumerable.Empty<User>();

                            ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                        }
                    }
                }
            }
            ViewBag.ErrorMessage = "Invalid User or Password";
            return View("UserLogin");
        }

        public RedirectResult UserGoogleLogin()
        {
            string googleLoginuri = KeyVaultService.GoogleLoginUrl.Replace("{clientId}", googleClientId).Replace("{redirectUri}", string.Concat(redirectUri, "GoogleDataProcess"));
            return Redirect(googleLoginuri);
        }


        public async Task<ActionResult> GoogleDataProcess(string code)
        {
            GoogleLoginHelper googleHelper = new GoogleLoginHelper();
            var userData = googleHelper.StoreGoogleUserData(code, googleClientId, googleclientSecret, string.Concat(redirectUri, "GoogleDataProcess")).Result;
            if (userData != null)
            {
                IEnumerable<User> user = null;
                using (var client = new HttpClient())
                {
                    if (userData != null)
                    {
                        client.BaseAddress = new Uri(ConfigurationManager.AppSettings["Azfunctionurl"]);
                        var json = JsonConvert.SerializeObject(userData);

                        var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
                        var responseTask = client.PostAsync("saveGoogleUser", stringContent);
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
                            ViewBag.Loginned = true;
                            return RedirectToAction("Products", "Products");
                        }
                        if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        {
                            ViewBag.ErrorMessage = "Invalid User or Password";
                        }
                        else
                        {
                            user = Enumerable.Empty<User>();

                            ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                        }
                    }
                }
            }
            ViewBag.ErrorMessage = "Invalid User or Password";
            return View("UserLogin");
        }

        public RedirectResult UserFacebookLogin()
        {
            string facebookLoginuri = KeyVaultService.FacebookLoginUrl.Replace("{clientId}", facebookClientId).Replace("{redirectUri}", string.Concat(redirectUri, "FacebookDataProcess"));
            return Redirect(facebookLoginuri);
        }


        public async Task<ActionResult> FacebookDataProcess(string code)
        {
            FacebookLoginHelper facebookHelper = new FacebookLoginHelper();
            var userData = facebookHelper.StoreFacebookUserData(code, facebookClientId, facebookClientSecret, string.Concat(redirectUri, "FacebookDataProcess")).Result;

            if (userData != null)
            {
                IEnumerable<User> user = null;
                using (var client = new HttpClient())
                {
                    if (userData != null)
                    {
                        client.BaseAddress = new Uri(ConfigurationManager.AppSettings["Azfunctionurl"]);
                        var json = JsonConvert.SerializeObject(userData);

                        var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
                        var responseTask = client.PostAsync("saveFacebookUser", stringContent);
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
                            ViewBag.Loginned = true;
                            return RedirectToAction("Products", "Products");
                        }
                        if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        {
                            ViewBag.ErrorMessage = "Invalid User or Password";
                        }
                        else
                        {
                            user = Enumerable.Empty<User>();

                            ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                        }
                    }
                }
            }
            ViewBag.ErrorMessage = "Invalid User or Password";
            return View("UserLogin");
        }
    }
}