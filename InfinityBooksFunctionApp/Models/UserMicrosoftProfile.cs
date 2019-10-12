using Dapper;

namespace InfinityBooksFunctionApp.Models
{
    public class UserMicrosoftProfile
    {
        public string id { get; set; }
        public string givenName { get; set; }
        public string surname { get; set; }
        public string userPrincipalName { get; set; }
        public string email { get; set; }
    }
}
