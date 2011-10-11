using System;
using BillingsDotNet;
using BillingsDotNet.Search;

namespace Indexer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var indexes = new IIndex[]
            {
                new IndexTankIndex("http://:GKwE+QM84IReDp@dkjak.api.indextank.com", "billingsdotnet"), 
                new LuceneIndex(@"C:\Users\john\Projects\Billings .NET Users Group\201109 - All About Search\source\StackSearch\App_Data\Lucene"),
            };

            var importer = new StackExchangeImporter(@"C:\Users\john\Projects\Billings .NET Users Group\201109 - All About Search\data\092011 Super User");
            foreach (var index in indexes)
            {
                Console.WriteLine("Indexing documents to " + index.Name);
                importer.Import(index, 50000);
            }
        }
    }
}