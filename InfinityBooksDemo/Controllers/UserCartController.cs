using InfinityBooksDemo.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Text;

namespace InfinityBooksDemo.Controllers
{
    public class UserCartController : Controller
    {
        // GET: Cart
        [HttpGet]
        public async Task<ActionResult> CartGetAsync()
        {
            if (Request.Cookies.AllKeys.Contains("userId") && !string.Equals(Request.Cookies["userId"].Value, '0'))
            {
                IEnumerable<Cart> cartProduct = null;

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:7071/api/");
                    string requestUri = string.Concat("cart?userId=", Request.Cookies["userId"].Value);
                    var responseTask = client.GetAsync(requestUri);
                    responseTask.Wait();

                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = JsonConvert.DeserializeObject<List<Cart>>(await result.Content.ReadAsStringAsync());
                        cartProduct = readTask;
                    }
                    else //web api sent error response 
                    {
                        //log response status here..

                        cartProduct = Enumerable.Empty<Cart>();

                        ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                    }
                }
                return View("Cart", cartProduct);
            }
            return RedirectToAction("UserLogin", "Login");
        }

        [HttpGet]
        public async Task<ActionResult> CartPostAsync(string id)
        {

            if (Request.Cookies.AllKeys.Contains("userId") && !string.Equals(Request.Cookies["userId"].Value, '0'))
            {
                if (!string.IsNullOrEmpty(id))
                {
                    IEnumerable<Cart> cartProduct;
                    using (var client = new HttpClient())
                    {

                        client.BaseAddress = new Uri("http://localhost:7071/api/");

                        var json = JsonConvert.SerializeObject(new Cart() { productId = Convert.ToInt32(id), statusTypeId = 1, userId = Convert.ToInt32(Request.Cookies["userId"].Value), quantity=1 ,createdAt = DateTime.Now });

                        var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
                        //HTTP GET
                        var responseTask = client.PostAsync("cart", stringContent);
                        //var responseTask = client.GetAsync("cart");
                        responseTask.Wait();

                        var result = responseTask.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            var readTask = JsonConvert.DeserializeObject<List<Cart>>(await result.Content.ReadAsStringAsync());
                            cartProduct = readTask;
                        }
                        else //web api sent error response 
                        {
                            //log response status here..

                            cartProduct = Enumerable.Empty<Cart>();

                            ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                        }
                    }
                    return RedirectToAction("CartGetAsync");
                }
            }
            return RedirectToAction("UserLogin", "Login");
        }

        [HttpPut]
        public async Task<ActionResult> CartPutAsync(Cart data, string id = null)
        {
            if (Request.Cookies.AllKeys.Contains("userId") && !string.Equals(Request.Cookies["userId"].Value, '0'))
            {
                IEnumerable<Cart> cartProduct = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:7071/api/");

                var json = JsonConvert.SerializeObject(data);

                var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
                //HTTP GET
                var responseTask = client.PutAsync("cart", stringContent);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = JsonConvert.DeserializeObject<List<Cart>>(await result.Content.ReadAsStringAsync());
                    cartProduct = readTask;
                }
                else //web api sent error response 
                {
                    cartProduct = Enumerable.Empty<Cart>();

                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            return View(cartProduct);
            }
            return RedirectToAction("UserLogin", "Login");
        }

        [HttpGet]
        public async Task<ActionResult> CartDeleteAsync( string id)
        {
            if (Request.Cookies.AllKeys.Contains("userId") && !string.Equals(Request.Cookies["userId"].Value, '0'))
            {
                if (!string.IsNullOrEmpty(id))
                {
                    IEnumerable<Cart> cartProduct = null;

                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri("http://localhost:7071/api/");

                        //  var json = JsonConvert.SerializeObject(data);

                      //  var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
                        //HTTP GET
                        var responseTask = client.DeleteAsync("cart/"+id);
                        responseTask.Wait();

                        var result = responseTask.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            return RedirectToAction("CartGetAsync");
                        }
                        else //web api sent error response 
                        {
                            cartProduct = Enumerable.Empty<Cart>();
                            ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                            return RedirectToAction("CartGetAsync");
                        }
                    }
                }
            }
            return RedirectToAction("UserLogin", "Login");
        }
    }
}