using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfinityBooksFunctionApp.Helper
{
    public class StringHelper
    {
        public static string JoinValues<TKey, TValue>(Dictionary<TKey, TValue>.KeyCollection source, string prefix = null, string suffix = null)
        {
            if (source == null || source.Count == 0)
            {
                return null;
            }
            StringBuilder joinedString = new StringBuilder();
            for (int i = 0; i < source.Count; i++)
            {
                if (string.IsNullOrEmpty(source.ElementAt(i).ToString()))
                    continue;
                if (!string.IsNullOrEmpty(prefix))
                    joinedString.Append(prefix);
                joinedString.Append(source.ElementAt(i));
                if (!string.IsNullOrEmpty(suffix))
                    joinedString.Append(suffix);
                if (i < source.Count - 1)
                    joinedString.Append(",");
            }
            return joinedString.ToString();
        }
        public static string JoinValues(List<string> source, string prefix = null, string suffix = null)
        {
            if (source == null || source.Count == 0)
            {
                return null;
            }
            StringBuilder joinedString = new StringBuilder();
            for (int i = 0; i < source.Count; i++)
            {
                if (string.IsNullOrEmpty(source[i]))
                    continue;
                if (!string.IsNullOrEmpty(prefix))
                    joinedString.Append(prefix);
                joinedString.Append(source[i]);
                if (!string.IsNullOrEmpty(suffix))
                    joinedString.Append(suffix);
                if (i < source.Count - 1)
                    joinedString.Append(",");
            }
            return joinedString.ToString();
        }
    }
}
