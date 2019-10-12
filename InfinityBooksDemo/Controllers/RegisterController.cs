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
using System.Configuration;

namespace InfinityBooksDemo.Controllers
{
    public class RegisterController : Controller
    {
        [HttpGet]
        public async System.Threading.Tasks.Task<ActionResult> UserRegister()
        {
            return View();
        }
        // GET: Login
        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> UserRegisterPost(UserProfile userProfile)
        {           
            IEnumerable<User> user = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["Azfunctionurl"]);
               
                var json = JsonConvert.SerializeObject(userProfile);

                var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
                //HTTP GET
                var responseTask = client.PostAsync("userProfile", stringContent);
                //var responseTask = client.GetAsync("cart");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = JsonConvert.DeserializeObject<List<User>>(await result.Content.ReadAsStringAsync());
                    user = readTask;
                    Response.Cookies.Add(new HttpCookie("userId", Convert.ToString(user.First().id)));
                    return RedirectToAction("Products","Products");
                }
                if(result.StatusCode==System.Net.HttpStatusCode.Unauthorized)
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
            return RedirectToAction("UserLogin","Login");
        }
    }
}