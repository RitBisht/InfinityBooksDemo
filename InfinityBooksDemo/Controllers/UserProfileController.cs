using InfinityBooksDemo.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;

namespace InfinityBooksDemo.Controllers
{
    public class UserProfileController : Controller
    {
        [HttpGet]
        public async System.Threading.Tasks.Task<ActionResult> UserRegister()
        {
            return View("Register");
        }
        // GET: Login
        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> UserRegisterPost(UserProfile userProfile)
        {
            WebRequestHandler handler = new WebRequestHandler();
            handler.CookieContainer = new System.Net.CookieContainer();
            handler.UseCookies = true;
            handler.UseDefaultCredentials = true;
            IEnumerable<UserProfile> user = null;
            using (var client = new HttpClient(handler))
            {
                if (userProfile != null)
                {                   
                    client.BaseAddress = new Uri("http://localhost:7071/api/");

                    var json = JsonConvert.SerializeObject(userProfile);

                    var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
                    //HTTP GET
                    var responseTask = client.PostAsync("userProfile", stringContent);
                    responseTask.Wait();

                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = JsonConvert.DeserializeObject<List<UserProfile>>(await result.Content.ReadAsStringAsync());
                        user = readTask;
                        Response.Cookies.Add(new HttpCookie("userId", Convert.ToString(user.First().id)));
                        return RedirectToAction("Products", "Products");
                    }
                    if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        ModelState.AddModelError(string.Empty, "User is not authorize");
                    }
                    else //web api sent error response 
                    {
                        //log response status here..

                        user = Enumerable.Empty<UserProfile>();

                        ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                    }
                }
            }
            return RedirectToAction("UserLogin", "Login");
        }

        public async Task<ActionResult> UserProfileGet()       
        {
            if (Request.Cookies.AllKeys.Contains("userId") && !string.Equals(Request.Cookies["userId"].Value, '0'))
            {
                    IEnumerable<UserProfile> user;
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri("http://localhost:7071/api/");
                        var responseTask = client.GetAsync("userProfile?userId="+ Request.Cookies["userId"].Value);
                        responseTask.Wait();

                        var result = responseTask.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            var readTask = JsonConvert.DeserializeObject<List<UserProfile>>(await result.Content.ReadAsStringAsync());
                            user = readTask;
                            Response.Cookies.Add(new HttpCookie("userId", Convert.ToString(user.First().id)));
                            return View("profile",user.First());
                        }
                        if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        {
                            ModelState.AddModelError(string.Empty, "User is not authorize");
                        }
                        else //web api sent error response 
                        {
                            //log response status here..
                            user = Enumerable.Empty<UserProfile>();

                            ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                        }
                    }
                
            }
            return RedirectToAction("UserLogin", "Login");
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> UserProfilePut(UserProfile userProfile)
        {
            if (Request.Cookies.AllKeys.Contains("userId") && !string.Equals(Request.Cookies["userId"].Value, '0'))
            {
                IEnumerable<UserProfile> user;
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:7071/api/");
                    userProfile.userId = Convert.ToInt32(Request.Cookies["userId"].Value);
                    userProfile.address.userId = userProfile.userId;
                    var json = JsonConvert.SerializeObject(userProfile);

                    var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
                    //HTTP GET
                    var responseTask = client.PutAsync("userProfile", stringContent);
                    responseTask.Wait();

                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = JsonConvert.DeserializeObject<List<UserProfile>>(await result.Content.ReadAsStringAsync());
                        user = readTask;
                        Response.Cookies.Add(new HttpCookie("userId", Convert.ToString(user.First().id)));
                        return RedirectToAction("Products", "Products");
                    }
                    if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        ModelState.AddModelError(string.Empty, "User is not authorize");
                    }
                    else
                    {
                        user = Enumerable.Empty<UserProfile>();
                        ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                    }
                }

            }
            return RedirectToAction("UserLogin", "Login");
        }
    }
}