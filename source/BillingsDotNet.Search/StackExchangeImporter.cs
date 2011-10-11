using System.IO;
using System.Linq;
using BillingsDotNet.Search;
using StackExchange.DataDump;

namespace BillingsDotNet
{
    public class StackExchangeImporter
    {
        private readonly string path;

        public StackExchangeImporter(string path)
        {
            this.path = path;
        }

        public void Import(IIndex index, int count)
        {
            var reader = new DataDumpReader<Post>();
            var xml = Path.Combine(path, "posts.xml");
            index.Add(reader.Read(xml)
                .Where(p => p.PostTypeId == PostTypeId.Question)
                .Take(count));
        }
    }
}