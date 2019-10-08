using Dapper;
using InfinityBooksFunctionApp.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace InfinityBooksFunctionApp.Helper
{
    class QueryBuilder<T>
    {
        public Dictionary<string, PropertyInfo> PropertiesCache;
        public Dictionary<string, PropertyInfo> SelectPropertiesCache;
        public Dictionary<string, PropertyInfo> ValidPropertiesCache;



        //public  Select(string Entity,string primaryKey, IEnumerable<KeyValuePair<string, string>> parameters)
        //{

        //}

        public QueryBuilder()
        {
            BuildPropertiesCache();
        }


        public Query ConstructInsertQuery(JObject fields,string Entity)
        {
            Dictionary<string, object> desFields = JsonConvert.DeserializeObject<Dictionary<string, object>>(fields.ToString());
            desFields = desFields.Where(x => PropertiesCache.ContainsKey(x.Key.ToUpper()))
                         .ToDictionary(x => x.Key, x => x.Value);
            Query resultQuery = new Query();
            StringBuilder query = new StringBuilder();
            query.Append("INSERT INTO ");
            query.Append(Entity);
            query.Append(" (");
            query.Append(StringHelper.JoinValues(desFields.Keys));
            query.Append(") ");
            query.Append("OUTPUT ");
            query.Append(StringHelper.JoinValues(PropertiesCache.Keys, "inserted."));
            query.Append(" VALUES (");
            query.Append(StringHelper.JoinValues(desFields.Keys, "@"));
            query.Append(")");

            resultQuery.query = query.ToString();
            resultQuery.dynamicParams = new DynamicParameters();
            foreach (var item in desFields)
            {
                resultQuery.dynamicParams.Add(item.Key.ToUpper(), item.Value);
            }
            return resultQuery;
        }

        public Query ConstructUpdateQuery(JObject fields, string PrimaryKey, string Entity, IEnumerable<KeyValuePair<string, string>> parameters)
        {
            Query resultQuery = new Query();
            if (string.IsNullOrEmpty((string)fields[PrimaryKey]) && parameters.Count() == 0)
            {
                return null;
            }
            else if (!string.IsNullOrEmpty((string)fields[PrimaryKey]) && parameters.Count() == 0)
            {
                parameters = new[] { new KeyValuePair<string, string>(PrimaryKey, (string)fields[PrimaryKey]) };
            }
            Dictionary<string, object> desFields = JsonConvert.DeserializeObject<Dictionary<string, object>>(fields.ToString());
            if (desFields.ContainsKey("createdBy"))
            {
                desFields.Remove("createdBy");
            }
            if (desFields.ContainsKey("createdAt"))
            {
                desFields.Remove("createdAt");
            }

            desFields = desFields.Where(x => PropertiesCache.ContainsKey(x.Key.ToUpper()))
                         .ToDictionary(x => x.Key, x => x.Value);            
            
            Tuple<string, DynamicParameters> result = ConstructWhereClause(parameters);

            desFields.Remove(PrimaryKey);

            if (string.IsNullOrEmpty(result.Item1))
            {
                return null;
            }
            StringBuilder query = new StringBuilder();
            query.Append("UPDATE ");
            query.Append(Entity);
            query.Append(" SET ");         
            query.Append(string.Join(",", desFields.Keys.Select(c => { c = c + " =@Updated" + c; return c; }).ToList()));
            query.Append(" OUTPUT ");
            query.Append(StringHelper.JoinValues(PropertiesCache.Keys, "inserted."));
            query.Append(result.Item1);

            resultQuery.dynamicParams = result.Item2;
            resultQuery.query = query.ToString();
            foreach (var item in desFields)
            {
                resultQuery.dynamicParams.Add("Updated" + item.Key, item.Value);
            }
            return resultQuery;
        }

        public Query ConstructSelectQuery(IEnumerable<KeyValuePair<string, string>> parameters, string defaultOrderBy, string viewName, string accessControlclause, string pagingSortKey,string Entity,int pageNumber, int maxRows)
        {
            Tuple<string, DynamicParameters> result = ConstructWhereClause(parameters, accessControlclause);

            string orderByClause = ConstructSortingClause(parameters);
            if (string.IsNullOrEmpty(orderByClause) && !string.IsNullOrEmpty(defaultOrderBy))
            {
                if (!defaultOrderBy.ToLower().Contains("order by"))
                {
                    defaultOrderBy = " ORDER BY " + defaultOrderBy;
                }
                orderByClause = " " + defaultOrderBy;
            }

            StringBuilder query = new StringBuilder();
            query.Append("SELECT ");

            if (maxRows > 0 && pageNumber > 0 && (!string.IsNullOrEmpty(pagingSortKey) || !string.IsNullOrEmpty(orderByClause)))
            {
                int startRowNo = (pageNumber - 1) * maxRows + 1;
                int endRowNo = startRowNo + maxRows;
                query.Append(" TOP(" + endRowNo + ") ");
            }

            if (string.IsNullOrEmpty(viewName))
                query.Append(ConstructSelectClause(parameters));
            else
                query.Append(" *");

            if (maxRows > 0 && pageNumber > 0 && (!string.IsNullOrEmpty(pagingSortKey) || !string.IsNullOrEmpty(orderByClause)))
            {
                var pagingOrderByClause = orderByClause;
                if (string.IsNullOrEmpty(pagingOrderByClause))
                {
                    pagingOrderByClause = " ORDER BY " + pagingSortKey;
                }
                query.Append(", ROW_NUMBER() OVER(" + pagingOrderByClause + ") as paging_row_number ");
            }


            query.Append(" FROM ");
            if (!string.IsNullOrEmpty(viewName))
                query.Append(viewName);
            else
                query.Append(Entity);

            query.Append(" V ");

            query.Append(result.Item1);

            query.Append(orderByClause);

            if (maxRows > 0 && pageNumber > 0 && !string.IsNullOrEmpty(pagingSortKey))
            {
                int startRowNo = (pageNumber - 1) * maxRows + 1;
                int endRowNo = startRowNo + maxRows;
                query = new StringBuilder("SELECT P.* FROM (" + query.ToString() + ") AS P WHERE P.paging_row_number BETWEEN " + startRowNo + " AND " + endRowNo);
            }

            return new Query
            {
                query = query.ToString(),
                dynamicParams = result.Item2
            };
        }

        public Query ConstructDeleteQuery(IEnumerable<KeyValuePair<string, string>> parameters,string Entity)
        {
            Tuple<string, DynamicParameters> result = ConstructWhereClause(parameters);
            if (string.IsNullOrEmpty(result.Item1))
            {
                return null;
            }
            StringBuilder query = new StringBuilder();
            query.Append("DELETE FROM ");
            query.Append(Entity);
            query.Append(" ");
            query.Append(result.Item1);
            return new Query
            {
                query = query.ToString(),
                dynamicParams = result.Item2
            };
        }

        public Tuple<string, DynamicParameters> ConstructWhereClause(IEnumerable<KeyValuePair<string, string>> parameters, string accessControlclause = null)
        {
            StringBuilder whereClause = new StringBuilder();
            DynamicParameters dynamicParameters = new DynamicParameters();
            if (!IsQueryStringExist(parameters) && string.IsNullOrEmpty(accessControlclause))
            {
                return new Tuple<string, DynamicParameters>(whereClause.ToString(), dynamicParameters);
            }

            int paramCount = 0;

            List<string> conditions = new List<string>();
            foreach (var field in parameters)
            {
                StringBuilder condition = new StringBuilder();
                if (!IsValidWhereCondition(field))
                {
                    continue;
                }
                string[] splitKey = field.Key.Split('.');
                string op;
                if (field.Value.Equals("null", StringComparison.OrdinalIgnoreCase))
                {
                    op = "IS NULL";
                }
                else
                {
                    op = Operator.GetOperator(splitKey.Count() > 1 ? splitKey[1] : "EQ").Expression;
                }
                condition.Append(splitKey[0]);
                condition.Append(" ");
                condition.Append(op);
                condition.Append(" ");
                if (op.Equals("IS NULL", StringComparison.OrdinalIgnoreCase)
                     || op.Equals("IS NOT NULL", StringComparison.OrdinalIgnoreCase))
                {
                    conditions.Add(condition.ToString());
                }
                else if (!string.IsNullOrEmpty(field.Value) && field.Value.StartsWith("{$expr{") && field.Value.EndsWith("}}"))
                {
                    condition.Append(field.Value.Substring(7, field.Value.Length - 9));
                    condition.Append(" ");

                    conditions.Add(condition.ToString());
                }
                else if (op.Equals("IN", StringComparison.OrdinalIgnoreCase))
                {
                    string value = field.Value;
                    string latestValue = null;
                    if (!value.All(Char.IsDigit))
                    {
                        StringBuilder sb;
                        string[] splitParam = value.Split(',');
                        foreach (var param in splitParam)
                        {
                            sb = new StringBuilder();
                            latestValue += sb.Append("'").Append(param).Append("'").Append(",").ToString();
                        }
                        condition.Append("(");
                        condition.Append(latestValue.Remove(latestValue.Length - 1));
                        condition.Append(")");
                    }
                    else
                    {
                        condition.Append("(");
                        condition.Append(value);
                        condition.Append(")");
                    }
                    conditions.Add(condition.ToString());
                }
                else if (op.Equals("NOT IN", StringComparison.OrdinalIgnoreCase))
                {
                    string value = field.Value;
                    string latestValue = null;
                    if (!value.All(Char.IsDigit))
                    {
                        StringBuilder sb;
                        string[] splitParam = value.Split(',');
                        foreach (var param in splitParam)
                        {
                            sb = new StringBuilder();
                            latestValue += sb.Append("'").Append(param).Append("'").Append(",").ToString();
                        }
                        condition.Append("(");
                        condition.Append(latestValue.Remove(latestValue.Length - 1));
                        condition.Append(")");
                    }
                    else
                    {
                        condition.Append("(");
                        condition.Append(value);
                        condition.Append(")");
                    }
                    conditions.Add(condition.ToString());
                }
                else if (!string.IsNullOrEmpty(field.Value))
                {
                    paramCount++;
                    string paramKey = "PARAM" + paramCount;

                    string value = field.Value;
                    if (op.Equals("LIKE", StringComparison.OrdinalIgnoreCase)
                        || op.Equals("NOT LIKE", StringComparison.OrdinalIgnoreCase))
                    {
                        value = "%" + field.Value + "%";
                    }

                    condition.Append("@");
                    condition.Append(paramKey);
                    condition.Append(" ");

                    dynamicParameters.Add(paramKey, value);
                    conditions.Add(condition.ToString());
                }

            }
            whereClause.Append(conditions.Count > 0 || !string.IsNullOrEmpty(accessControlclause) ? " WHERE " : "");
            whereClause.Append(string.Join(" AND ", conditions));
            if (!string.IsNullOrEmpty(accessControlclause))
            {
                if (conditions.Count > 0)
                    whereClause.Append(" AND ");
                whereClause.Append(accessControlclause);
            }
            return new Tuple<string, DynamicParameters>(whereClause.ToString(), dynamicParameters);
        }

        private string ConstructSortingClause(IEnumerable<KeyValuePair<string, string>> parameters)
        {
            StringBuilder sortingClause = new StringBuilder();
            sortingClause.Append(" ORDER BY ");
            if (!IsQueryStringExist(parameters))
            {
                return string.Empty;
            }

            List<string> validatedFields = new List<string>();
            foreach (var item in parameters)
            {
                Operator op = Operator.GetOperator(item.Key);
                if (op == null)
                {
                    continue;
                }
                string values = StringHelper.JoinValues(FilterValidFields(item.Value.Split(',').ToList()), null, " " + op.Expression);
                if (!string.IsNullOrEmpty(values))
                    validatedFields.Add(values);
            }
            sortingClause.Append(StringHelper.JoinValues(validatedFields));
            return validatedFields.Count > 0 ? sortingClause.ToString() : string.Empty;
        }

        private string ConstructSelectClause(IEnumerable<KeyValuePair<string, string>> parameters)
        {
            List<string> selectFields = GetParameterValues(parameters, "select.fields");
            //Return all fields if there is no specif field request in the API.
            if (selectFields == null || selectFields.Count == 0)
                return StringHelper.JoinValues(SelectPropertiesCache.Keys); ;

            List<string> fields = new List<string>();
            //Iterate all select Fields and get all fields name only.
            if (selectFields != null && selectFields.Count > 0)
                selectFields.ForEach(x => x.Split(',').ToList().ForEach(i => fields.Add(i)));

            //Validate request fields with original fields and return after validation.
            return string.Join(",", FilterValidFields(fields));
        }

        protected List<string> GetParameterValues(IEnumerable<KeyValuePair<string, string>> queryStrings, string key)
        {
            if (queryStrings == null)
            {
                return null;
            }

            var match = queryStrings.Where(kv => string.Compare(kv.Key, key, true) == 0).Select(x => x.Value).ToList();

            return match;
        }

        protected bool IsQueryStringExist(IEnumerable<KeyValuePair<string, string>> fields)
        {
            return fields != null && fields.Count() > 0;
        }

        private bool IsValidProperty(string property)
        {
            return ValidPropertiesCache.ContainsKey(property.ToUpper());
        }

        private bool IsValidWhereCondition(KeyValuePair<string, string> condition)
        {
            if (condition.Key.Contains('.'))
            {
                string[] splitKeys = condition.Key.Split('.');
                if (Operator.IsValidOperator(splitKeys[1])
                   && IsValidProperty(splitKeys[0]))
                {
                    return true;
                }
            }
            else if (IsValidProperty(condition.Key))
            {
                return true;
            }
            return false;
        }

        private List<string> FilterValidFields(List<string> fields)
        {
            return fields.Select(x => x.ToUpper()).Intersect(ValidPropertiesCache.Keys).ToList();
        }

        private void BuildPropertiesCache()
        {
            Type type = typeof(T);
            PropertiesCache = new Dictionary<string, PropertyInfo>();
            SelectPropertiesCache = new Dictionary<string, PropertyInfo>();
            ValidPropertiesCache = new Dictionary<string, PropertyInfo>();
            foreach (PropertyInfo pInfo in type.GetProperties())
            {
                //if (!Attribute.IsDefined(pInfo, typeof(SecurePropertyAttribute)) && !Attribute.IsDefined(pInfo, typeof(NotMappedAttribute)))
                if (!Attribute.IsDefined(pInfo, typeof(ValidationAttribute)) && (!(pInfo.PropertyType.Name.Substring(0, 4) == "List") && (!Attribute.IsDefined(pInfo, typeof(NotMappedAttribute)))))
                    SelectPropertiesCache.Add(pInfo.Name.ToUpper(), pInfo);
                if (!Attribute.IsDefined(pInfo, typeof(NotMappedAttribute)))
                    PropertiesCache.Add(pInfo.Name.ToUpper(), pInfo);
                //if (!Attribute.IsDefined(pInfo, typeof(NotMappedAttribute)))
                if (!(pInfo.PropertyType.Name.Substring(0, 4) == "List"))
                    ValidPropertiesCache.Add(pInfo.Name.ToUpper(), pInfo);
            }
        }
        
    }
}
