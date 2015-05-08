using System.IO;
using System.Web.Hosting;
using Cormo.Injects;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Store;
using Radio7.Todo.Lucene;
using Directory = Lucene.Net.Store.Directory;
using Version = Lucene.Net.Util.Version;

namespace Radio7.Todo.Server
{
    public class Producers
    {
        [Produces]
        public Indexer<TodoTask> CreateIndexer()
        {
            return new Indexer<TodoTask>(new TodoTaskSearchConfig());
        }

        [Produces]
        public Searcher<TodoTask> CreateSearcher()
        {
            return new Searcher<TodoTask>(new TodoTaskSearchConfig());
        }
    }

    public class TodoTaskSearchConfig : ISearchConfig
    {
        public TodoTaskSearchConfig()
        {
            var path = HostingEnvironment.MapPath("~/app_data/index");
            Directory = new SimpleFSDirectory(new DirectoryInfo(path));
            Analyzer = new StandardAnalyzer(Version.LUCENE_30);
            SearchResultLimit = 20;
        }

        public Directory Directory { get; private set; }
        public Analyzer Analyzer { get; private set; }
        public int SearchResultLimit { get; private set; }
    }
}