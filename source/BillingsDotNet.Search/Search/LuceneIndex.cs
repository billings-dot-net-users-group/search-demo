using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using StackExchange.DataDump;
using Directory = System.IO.Directory;

namespace BillingsDotNet.Search
{
    public class LuceneIndex : IIndex
    {
        class LuceneDocument : IResultDocument
        {
            public IDictionary<string, string> Fields { get; set; }

            public string Id { get; set; }

            public IDictionary<string, string> Snippets { get; set; }
        }

        private readonly string path;

        public LuceneIndex(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            this.path = path;
        }

        public string Name
        {
            get { return "Lucene.NET"; }
        }

        public void Add(IEnumerable<Post> posts)
        {
            var directory = FSDirectory.Open(new DirectoryInfo(path));
            var analyzer = new StandardAnalyzer(Version.LUCENE_29);
            var writer = new IndexWriter(directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED);

            foreach (var post in posts)
            {
                var doc = new Document();
                doc.Add(new Field("docid", post.Id.ToString(), Field.Store.YES, Field.Index.NO));
                doc.Add(new Field("title", post.Title, Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("text", post.Body, Field.Store.YES, Field.Index.ANALYZED));

                writer.AddDocument(doc);
            }

            writer.Optimize();
            writer.Commit();
            writer.Close();
        }

        public ISearchResult Search(string query)
        {
            var timer = new Stopwatch();
            timer.Start();

            var directory = FSDirectory.Open(new DirectoryInfo(path));
            var analyzer = new StandardAnalyzer(Version.LUCENE_29);
            var searcher = new IndexSearcher(directory, true);

            var queryParser = new QueryParser(Version.LUCENE_29, "text", analyzer);
            var result = searcher.Search(queryParser.Parse(query), 20);

            var docs = (from scoreDoc in result.scoreDocs
                        let doc = searcher.Doc(scoreDoc.doc)
                        let fields = new Dictionary<string, string> { { "title", doc.Get("title") }, { "text", doc.Get("text") } }
                        select new LuceneDocument { Id = scoreDoc.doc.ToString(), Fields = fields }).ToList();

            var ret = new SearchResult { Query = query, Total = result.totalHits, Documents = docs, Source = Name };

            searcher.Close();
            directory.Close();

            timer.Stop();
            ret.Duration = (decimal) timer.Elapsed.TotalSeconds;

            return ret;
        }
    }
}