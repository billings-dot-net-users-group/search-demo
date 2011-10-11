using System.Collections.Generic;
using StackExchange.DataDump;

namespace BillingsDotNet.Search
{
    public interface IIndex
    {
        string Name { get;  }
        void Add(IEnumerable<Post> posts);
        ISearchResult Search(string query);
    }
}