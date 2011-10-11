using System.Collections.Generic;

namespace BillingsDotNet.Search
{
    public interface IResultDocument
    {
        IDictionary<string, string> Fields { get; }

        string Id { get; }
        IDictionary<string, string> Snippets { get; }
    }
}