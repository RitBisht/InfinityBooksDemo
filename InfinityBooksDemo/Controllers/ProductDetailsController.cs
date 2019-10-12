using InfinityBooksDemo.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace InfinityBooksDemo.Controllers
{
    public class ProductDetailsController : Controller
    {
        // GET: ProductDetails
        
        public async System.Threading.Tasks.Task<ActionResult> ProductDetailsAsync(string id)
        {
            IEnumerable<Product> products = null;

            if (id != null)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:7071/api/");
                    //HTTP GET
                    var responseTask = client.GetAsync("product/" + id);
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
                }
            }
            return View(products);
        }
    }
}