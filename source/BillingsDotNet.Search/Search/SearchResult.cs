using System.Collections.Generic;

namespace BillingsDotNet.Search
{
    public class SearchResult : ISearchResult
    {
        public IEnumerable<IResultDocument> Documents { get; set; }

        public string Source { get; set; }

        public string Query { get; set; }

        public int Total { get; set; }

        public decimal Duration { get; set; }

        public IDictionary<string, Dictionary<string, int>> Facets { get; set; }
    }
}