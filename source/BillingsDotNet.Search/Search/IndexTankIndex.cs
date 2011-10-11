using System;
using System.Collections.Generic;
using System.Linq;
using IndexTankDotNet;
using StackExchange.DataDump;

namespace BillingsDotNet.Search
{
    public class IndexTankIndex : IIndex
    {
        private const string name = "IndexTank";

        class IndexTankResult : ISearchResult
        {
            private readonly IndexTankDotNet.SearchResult result;

            public IndexTankResult(IndexTankDotNet.SearchResult result)
            {
                this.result = result;
            }

            public IEnumerable<IResultDocument> Documents { get { return result.ResultDocuments.Select(doc => new IndexTankDocument(doc)); } }

            public decimal Duration { get { return result.SearchTime; } }

            public IDictionary<string, Dictionary<string, int>> Facets { get { return result.Facets; } } 

            public string Query { get { return result.QueryText; } }

            public string Source { get { return name; } }

            public int Total { get { return result.Matches; } }
        }

        class IndexTankDocument : IResultDocument
        {
            private readonly IndexTankDotNet.ResultDocument resultDocument;

            public IndexTankDocument(IndexTankDotNet.ResultDocument resultDocument)
            {
                this.resultDocument = resultDocument;
            }

            public IDictionary<string, string> Fields { get { return resultDocument.Fields; } }

            public string Id { get { return resultDocument.DocumentId; } }

            public IDictionary<string, string> Snippets { get { return resultDocument.Snippets; } } 
        }

        private readonly Index index;
        private readonly int batchSize = 500;

        public IndexTankIndex(string url, string name)
        {
            index = new IndexTankClient(url).GetIndex(name);
        }

        public string Name
        {
            get { return name; }
        }

        public void Add(IEnumerable<Post> posts)
        {
            var batch = new List<Document>(batchSize);
            foreach (var post in posts)
            {
                var doc = new Document(post.Id.ToString());

                doc.AddTimestamp(post.CreationDate);
                
                doc.AddField("title", post.Title);
                doc.AddField("text", post.Body);
                
                var answerCount = post.AnswerCount ?? 0;
                doc.AddVariable(0, answerCount);
                AddRangeFacet(doc, "answers", answerCount, new[] { 10, 20, 30, 40, 50 });

                var favoriteCount = post.FavoriteCount ?? 0;
                doc.AddVariable(1, favoriteCount);
                AddRangeFacet(doc, "favorites", favoriteCount, new[] { 10, 20, 30, 40, 50 });

                var commentCount = post.CommentCount ?? 0;
                doc.AddVariable(2, commentCount);
                AddRangeFacet(doc, "comments", commentCount, new[] { 10, 20, 30, 40, 50 });
                
                batch.Add(doc);

                if (batch.Count == batch.Capacity)
                {
                    index.AddDocuments(batch);
                    batch.Clear();
                }
            }

            if (batch.Count > 0)
                index.AddDocuments(batch);
        }

        private void AddRangeFacet(Document doc, string facetName, int count, int[] ranges)
        {
            var facet = (string) null;
            for (int i = 0; i < ranges.Length; i++)
            {
                var upperBound = ranges[i];
                if (count < upperBound)
                {
                    if (i == 0)
                        facet = "Less than " + upperBound;
                    else
                        facet = String.Format("{0} to {1}", ranges[i - 1], upperBound - 1);

                    break;
                }

                if (i == ranges.Length - 1)
                    facet = "More than " + ranges[i];
            }

            doc.AddCategory(facetName, facet);
        }

        public ISearchResult Search(string query)
        {
            return Search(query, null);
        }

        public ISearchResult Search(string query, string facet)
        {
            var indexQuery = new Query(query)
                .WithCategories()
                .WithFields("title")
                .WithSnippetFromFields("text")
                .Take(20);

            if (!String.IsNullOrEmpty(facet))
            {
                var parts = facet.Split(':');
                indexQuery.WithCategoryFilter(parts[0], parts[1]);
            }

            return new IndexTankResult(index.Search(indexQuery));
        }
    }
}