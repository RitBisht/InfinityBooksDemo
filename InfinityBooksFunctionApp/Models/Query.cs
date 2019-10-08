using Dapper;

namespace InfinityBooksFunctionApp.Models
{
    public class Query
    {
        public string query { get; set; }
        public DynamicParameters dynamicParams { get; set; }
    }
}
