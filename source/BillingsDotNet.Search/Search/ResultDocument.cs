using System.Collections.Generic;

namespace BillingsDotNet.Search
{
    public class ResultDocument : IResultDocument
    {
        public IDictionary<string, string> Fields { get; set; }

        public string Id { get; set; }

        public IDictionary<string, string> Snippets { get; set; }
    }
}