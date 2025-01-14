﻿using InfinityBooksDemo.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Text;
using System.Configuration;
using InfinityBooksDemo.Service;

namespace InfinityBooksDemo.Controllers
{
    public class UserCartController : Controller
    {
        // GET: Cart
        [HttpGet]
        public async Task<ActionResult> CartGetAsync()
        {
            if (Request.Cookies.AllKeys.Contains("userId") && Session["userId"] != null && string.Equals(Request.Cookies["userId"].Value, Session["userId"].ToString()))
            {

                IEnumerable<Cart> cartProduct = null;

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(ConfigurationManager.AppSettings["Azfunctionurl"]);
                    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", KeyVaultService.InfiniteApiKey);
                    string requestUri = string.Concat("cart/?userId=", Request.Cookies["userId"].Value);
                    try
                    {
                        #region Get user cart data
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
                            cartProduct = Enumerable.Empty<Cart>();
                            ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                        }
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        @ViewBag.InternalErrorMessage = ex.Message;
                    }
                }
                return View("Cart", cartProduct);
            }
            return RedirectToAction("UserLogin", "Login");
        }

        [HttpGet]
        public async Task<ActionResult> CartPostAsync(string id)
        {

            if (Request.Cookies.AllKeys.Contains("userId") && Session["userId"] != null && string.Equals(Request.Cookies["userId"].Value, Session["userId"].ToString()))
            {
                if (!string.IsNullOrEmpty(id))
                {
                    IEnumerable<Cart> cartProduct;
                    using (var client = new HttpClient())
                    {

                        client.BaseAddress = new Uri(ConfigurationManager.AppSettings["Azfunctionurl"]);
                        client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", KeyVaultService.InfiniteApiKey);

                        var json = JsonConvert.SerializeObject(new Cart() { productId = Convert.ToInt32(id), statusTypeId = 1, userId = Convert.ToInt32(Request.Cookies["userId"].Value), quantity = 1, createdAt = DateTime.Now });

                        var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
                        try
                        {
                            #region Add product in user
                            var responseTask = client.PostAsync("cart/", stringContent);
                            responseTask.Wait();

                            var result = responseTask.Result;
                            if (result.IsSuccessStatusCode)
                            {
                                var readTask = JsonConvert.DeserializeObject<List<Cart>>(await result.Content.ReadAsStringAsync());
                                cartProduct = readTask;
                            }
                            else
                            {
                                cartProduct = Enumerable.Empty<Cart>();

                                ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                            }
                            #endregion
                        }
                        catch (Exception ex)
                        {
                            @ViewBag.InternalErrorMessage = ex.Message;
                        }
                    }
                    return RedirectToAction("CartGetAsync");
                }
            }
            return RedirectToAction("UserLogin", "Login");
        }

        [HttpGet]
        public async Task<ActionResult> CartPutAsync(Cart data)
        {
            if (Request.Cookies.AllKeys.Contains("userId") && Session["userId"] != null && string.Equals(Request.Cookies["userId"].Value, Session["userId"].ToString()))
            {
                IEnumerable<Cart> cartProduct = null;
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(ConfigurationManager.AppSettings["Azfunctionurl"]);
                    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", KeyVaultService.InfiniteApiKey);
                    #region Update product data in cart
                    if (string.Equals(data.operation, "INCREMENT"))
                    {
                        data.quantity++;
                    }
                    else
                    {
                        data.quantity--;
                    }
                    var json = JsonConvert.SerializeObject(data);
                    var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

                    try
                    {
                        var responseTask = client.PutAsync("cart/", stringContent);
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
                    #endregion
                    catch (Exception ex)
                    {
                        ViewBag.InternalErrorMessage = ex.Message;
                    }
                }
                return RedirectToAction("CartGetAsync");
            }
            return RedirectToAction("UserLogin", "Login");
        }

        [HttpGet]
        public async Task<ActionResult> CartDescProductQtyAsync(Cart data)
        {
            if (Request.Cookies.AllKeys.Contains("userId") && Session["userId"] != null && string.Equals(Request.Cookies["userId"].Value, Session["userId"].ToString()))
            {
                IEnumerable<Cart> cartProduct = null;

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(ConfigurationManager.AppSettings["Azfunctionurl"]);
                    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", KeyVaultService.InfiniteApiKey);

                    var json = JsonConvert.SerializeObject(data);

                    var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");                   
                    try
                    {
                        var responseTask = client.PutAsync("cart/", stringContent);
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
                  
                    catch (Exception ex)
                    {
                        @ViewBag.InternalErrorMessage = ex.Message;
                    }
                }
                return View(cartProduct);
            }
            return RedirectToAction("UserLogin", "Login");
        }

        [HttpGet]
        public async Task<ActionResult> CartDeleteAsync(string id)
        {
            if (Request.Cookies.AllKeys.Contains("userId") && Session["userId"] != null && string.Equals(Request.Cookies["userId"].Value, Session["userId"].ToString()))
            {
                if (!string.IsNullOrEmpty(id))
                {
                    IEnumerable<Cart> cartProduct = null;

                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(ConfigurationManager.AppSettings["Azfunctionurl"]);
                        client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", KeyVaultService.InfiniteApiKey);
                        #region Delete product from cart
                        try
                        {
                            var responseTask = client.DeleteAsync("cart/" + id);
                            responseTask.Wait();

                            var result = responseTask.Result;
                            if (result.IsSuccessStatusCode)
                            {
                                return RedirectToAction("CartGetAsync");
                            }
                            else
                            {
                                cartProduct = Enumerable.Empty<Cart>();
                                ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                                return RedirectToAction("CartGetAsync");
                            }
                        }
                        #endregion
                        catch (Exception ex)
                        {
                            @ViewBag.InternalErrorMessage = ex.Message;
                        }
                    }
                }
            }
            return RedirectToAction("UserLogin", "Login");
        }
    }
}