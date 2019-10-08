using Dapper;
using InfinityBooksFunctionApp.Models;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;

namespace InfinityBooksFunctionApp.Helper
{
    class QueryHelper<T>
    {
        private static string connectionString;
        QueryBuilder<T> builder;
        public QueryHelper()
        {
            connectionString = "Server=tcp:infibooksserver.database.windows.net,1433;Initial Catalog=infiBooksDatabase;Persist Security Info=False;User ID=infiadmin;Password=Newserver!23;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            builder = new QueryBuilder<T>();
        }
        #region SelectTableData
        public List<T> Select(IEnumerable<KeyValuePair<string, string>> parameters, string defaultOrderBy, string viewName, string accessControlclause, string pagingSortKey, string Entity)
        {
            Query query=builder.ConstructSelectQuery(parameters, defaultOrderBy, viewName, accessControlclause, pagingSortKey, Entity, 0, 0);
            var userData= RunQuery(query).ToList();
            PopulateAdditionalFields(userData);
            return userData;
        }
        #endregion

        #region UpdateTableData
        public List<T> Update(JObject fields, string PrimaryKey, string Entity, IEnumerable<KeyValuePair<string, string>> parameters)
        {
            var query=builder.ConstructUpdateQuery(fields,PrimaryKey,Entity,parameters);
            return RunQuery(query).ToList();
        }
        #endregion

        #region InsertTableData
        public List<T> Insert(JObject fields,string Entity)
        {
            var query = builder.ConstructInsertQuery(fields,Entity);
            return RunQuery(query).ToList();
        }
        #endregion

        #region DeleteTableData
        public int Delete(IEnumerable<KeyValuePair<string, string>> parameters, string Entity)
        {
            var query=builder.ConstructDeleteQuery(parameters, Entity);            
            return ExecuteQuery(query);
        }
        #endregion

        #region QueryRunnerForGet/Put/Post
        public IEnumerable<T> RunQuery(Query query)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                return connection.Query<T>(query.query, query.dynamicParams as object, commandTimeout: null, commandType: null);
            }
        }
        #endregion

        #region QueryExecuteDelete
        public int ExecuteQuery(Query query)
        {
            using (var connection = new SqlConnection(connectionString))
            {                    
                 return  connection.Execute(query.query, query.dynamicParams as object, commandTimeout: null, commandType: null);            
            }
        }
        #endregion

        public virtual void PopulateAdditionalFields(List<T> resultData)
        {

        }


        public virtual void ManageChildUpdate(List<T> resultData)
        {

        }

        public IEnumerable<KeyValuePair<string,string>> GetReqQueryParam(HttpRequestMessage request,string PrimaryKey,string PrimaryKeyValue)
        {
            IEnumerable<KeyValuePair<string, string>> queryParams = request.GetQueryNameValuePairs();
            if (!string.IsNullOrEmpty(PrimaryKeyValue) && !string.IsNullOrEmpty(PrimaryKey) && PrimaryKeyValue != "{id?}")
            {
                List<KeyValuePair<string, string>> queryParamList = new List<KeyValuePair<string, string>>();
                queryParamList.AddRange(queryParams);
                queryParamList.Add(new KeyValuePair<string, string>(PrimaryKey, PrimaryKeyValue));
                queryParams = queryParamList.AsEnumerable();
            }
            return queryParams;

        }

    }
}
