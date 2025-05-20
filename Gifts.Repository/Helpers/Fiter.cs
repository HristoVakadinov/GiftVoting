using System;
using System.Collections.Generic;

namespace Gifts.Repository.Helpers
{
    public class Filter
    {
        public static Filter Empty => new Filter();

        public Dictionary<string, object> Conditions { get; set; } = new Dictionary<string, object>();
        public Filter AddCondition(string field, object value)
        {
            Conditions[field] = value;
            return this;
        }
    }
}