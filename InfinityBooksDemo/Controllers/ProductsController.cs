using InfinityBooksDemo.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Configuration;
using InfinityBooksDemo.Service;
using System.Net.Http.Headers;

namespace InfinityBooksDemo.Controllers
{
    public class ProductsController : Controller
    {
        // GET: Products
        [HttpGet]
        public async Task<ActionResult> Products(string searchString = null)
        {
            IEnumerable<Product> products = null;
            using (var client = new HttpClient())
            {                
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["Azfunctionurl"]);
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", KeyVaultService.InfiniteApiKey);

                //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("subc");
                HttpResponseMessage res = null;
                if (searchString != null)
                {
                    var responseTask = client.GetAsync("product/?searchString=" + searchString);
                    responseTask.Wait();
                    res = responseTask.Result;
                }
                else
                {
                    var responseTask = client.GetAsync("product/");
                    responseTask.Wait();
                    res = responseTask.Result;
                }
                if (res.IsSuccessStatusCode)
                {
                    var readTask = JsonConvert.DeserializeObject<List<Product>>(await res.Content.ReadAsStringAsync());
                    products = readTask;
                }
                else
                {
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
                    client.BaseAddress = new Uri(ConfigurationManager.AppSettings["Azfunctionurl"]);
                    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", KeyVaultService.InfiniteApiKey);
                    
                    var responseTask = client.GetAsync(string.Concat("product/", id));
                    responseTask.Wait();

                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = JsonConvert.DeserializeObject<List<Product>>(await result.Content.ReadAsStringAsync());
                        products = readTask;
                    }
                    else
                    {
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