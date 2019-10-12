using InfinityBooksDemo.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;

namespace InfinityBooksDemo.Controllers
{
    public class ProductsController : Controller
    {
        // GET: Products
        [HttpGet]
        public async Task<ActionResult> Products(string searchString = null)
        {
                HttpCookie userIdcookieValue = Request.Cookies["userId"];
                IEnumerable<Product> products = null;

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:7071/api/");
                    //HTTP GET
                    HttpResponseMessage res=null;
                    if(searchString != null)
                    {
                        var responseTask = client.GetAsync("product?searchString=" + searchString);
                        responseTask.Wait();
                        res = responseTask.Result;
                    }
                    else
                    {
                        var responseTask = client.GetAsync("product");
                        responseTask.Wait();
                        res = responseTask.Result;
                    }
                    //var responseTask = client.GetAsync("product");
                    //responseTask.Wait();

                    //var result = responseTask.Result;
                    if (res.IsSuccessStatusCode)
                    {
                        var readTask = JsonConvert.DeserializeObject<List<Product>>(await res.Content.ReadAsStringAsync());
                        products = readTask;
                    }
                    else //web api sent error response 
                    {
                        //log response status here..

                        products = Enumerable.Empty<Product>();

                        ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                    }
                }

                return View(products);          
        }


        [HttpGet]
        [Route("id")]
        public async Task<ActionResult> ProductDetail(string id)
        {
                Product product = null;

                if (!string.IsNullOrEmpty(id))
                {
                    using (var client = new HttpClient())
                    {
                        IEnumerable<Product> products = null;
                        client.BaseAddress = new Uri("http://localhost:7071/api/");
                        //HTTP GET
                        var responseTask = client.GetAsync(string.Concat("product/", id));
                        responseTask.Wait();

                        var result = responseTask.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            var readTask = JsonConvert.DeserializeObject<List<Product>>(await result.Content.ReadAsStringAsync());
                            products = readTask;
                        }
                        else //web api sent error response 
                        {
                            //log response status here..

                            products = Enumerable.Empty<Product>();

                            ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                        }
                        if (products != null && products.Count() > 0)
                        {
                            product = products.First();
                        }
                    }
                }
                return View(product);          
        }
    }
}