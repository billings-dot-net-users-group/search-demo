using System.Collections.Generic;

namespace BillingsDotNet.Search
{
    public interface ISearchResult
    {
        IEnumerable<IResultDocument> Documents { get; } 
        string Source { get; }
        string Query { get; }
        int Total { get; }
        decimal Duration { get; }
        IDictionary<string, Dictionary<string, int>> Facets { get; }
    }
}