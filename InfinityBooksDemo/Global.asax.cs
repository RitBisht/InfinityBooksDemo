using InfinityBooksDemo.Service;
using Microsoft.Azure.KeyVault;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace InfinityBooksDemo
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            var keyVault = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(KeyVaultService.GetToken));
            KeyVaultService.GoogleClientId = keyVault.GetSecretAsync(WebConfigurationManager.AppSettings["GoogleVaultClientIdUrl"]).Result.Value;
            KeyVaultService.GoogleClientSecret = keyVault.GetSecretAsync(WebConfigurationManager.AppSettings["GoogleVaultClientSecretUrl"]).Result.Value;
            KeyVaultService.GoogleLoginUrl = keyVault.GetSecretAsync(WebConfigurationManager.AppSettings["GoogleVaultLoginUrl"]).Result.Value;
            KeyVaultService.GoogleMapUrl = keyVault.GetSecretAsync(WebConfigurationManager.AppSettings["GoogleVaultMapUrl"]).Result.Value;
            KeyVaultService.FacebookClientId = keyVault.GetSecretAsync(WebConfigurationManager.AppSettings["FacebookVaultClientIdUrl"]).Result.Value;
            KeyVaultService.FacebookClientSecret = keyVault.GetSecretAsync(WebConfigurationManager.AppSettings["FacebookVaultClientSecretUrl"]).Result.Value;
            KeyVaultService.FacebookLoginUrl = keyVault.GetSecretAsync(WebConfigurationManager.AppSettings["FacebookVaultLoginUrl"]).Result.Value;
            KeyVaultService.FacebookMapUrl = keyVault.GetSecretAsync(WebConfigurationManager.AppSettings["FacebookVaultMapUrl"]).Result.Value;
            KeyVaultService.InfiniteApiKey = keyVault.GetSecretAsync(WebConfigurationManager.AppSettings["InfinityBooksApisKey"]).Result.Value;
            KeyVaultService.Blobkey = keyVault.GetSecretAsync(WebConfigurationManager.AppSettings["BlobKeyVaultUrl"]).Result.Value;
        }
    }
}
