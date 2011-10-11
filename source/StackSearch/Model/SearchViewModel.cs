using System.Collections.Generic;
using System.Linq;
using BillingsDotNet.Search;

namespace StackSearch.Model
{
    public class SearchViewModel
    {
        public SearchViewModel()
        {
            Results = Enumerable.Empty<ISearchResult>();
        }

        public string f { get; set; }

        public string q { get; set; }

        public ISearchResult Result { get; set; }

        public IEnumerable<ISearchResult> Results { get; set; }
    }
}