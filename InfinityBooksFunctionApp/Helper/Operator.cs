using System;
using System.Collections.Generic;

namespace InfinityBooksFunctionApp.Helper
{
    public class Operator
        {
            private static Dictionary<string, Operator> OperatorMap = new Dictionary<string, Operator>();
            private static List<Operator> OperatorList = new List<Operator>();

            static Operator()
            {
                //Comparision Operators
                RegisterOperator("EQ", "=");
                RegisterOperator("NE", "!=");
                RegisterOperator("GE", ">=");
                RegisterOperator("GT", ">");
                RegisterOperator("LE", "<=");
                RegisterOperator("LT", "<");
                RegisterOperator("LIKE", "LIKE");
                RegisterOperator("NOTLIKE", "NOT LIKE");
                RegisterOperator("ISNOTNULL", "IS NOT NULL");
                RegisterOperator("ISNULL", "IS NULL");

                //Sorting Operators
                RegisterOperator("ORDER.BY", "ASC");
                RegisterOperator("ORDER.ASC.BY", "ASC");
                RegisterOperator("ORDER.DESC.BY", "DESC");
                RegisterOperator("IN", "IN");
                RegisterOperator("NIN", "NOT IN");
            }

            private static void RegisterOperator(String name, String expression)
            {
                name = name.ToUpper();
                Operator op = new Operator(name, expression);
                OperatorList.Add(op);
                OperatorMap.Add(name, op);
            }

            /// <summary>
            /// 
            /// </summary>
            public String Name
            {
                get;
                set;
            }

            /// <summary>
            /// 
            /// </summary>
            public String Expression
            {
                get;
                set;
            }

            public Operator(String name, String value)
            {
                Name = name;
                Expression = value;
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            public static Operator GetOperator(string name)
            {
                name = name.ToUpper();

                if (OperatorMap.ContainsKey(name))
                {
                    return OperatorMap[name];
                }

                return null;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            public static bool IsValidOperator(string name)
            {
                return OperatorMap.ContainsKey(name.ToUpper());
            }
        }
}
