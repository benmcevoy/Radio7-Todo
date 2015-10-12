using System.IO;
using System.Web.Hosting;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Store;
using Radio7.Todo.Lucene;
using Directory = Lucene.Net.Store.Directory;
using Version = Lucene.Net.Util.Version;
using Radio7.Unity.Decorators;

namespace Radio7.Todo.Server
{
    [Singleton]
    public class TodoTaskIndexer : Indexer<TodoTask>
    {
        public TodoTaskIndexer(ISearchConfig config)
            : base(config)
        {
        }
    }

    [Singleton]
    public class TodoTaskSearcher : Searcher<TodoTask>
    {
        public TodoTaskSearcher(ISearchConfig searchConfig)
            : base(searchConfig)
        {
        }
    }

    [Singleton]
    public class TodoTaskSearchConfig : ISearchConfig
    {
        public TodoTaskSearchConfig()
        {
            var path = HostingEnvironment.MapPath("~/app_data/index");
            Directory = new SimpleFSDirectory(new DirectoryInfo(path));
            Analyzer = new StandardAnalyzer(Version.LUCENE_30);
            SearchResultLimit = int.MaxValue;
        }

        public Directory Directory { get; private set; }
        public Analyzer Analyzer { get; private set; }
        public int SearchResultLimit { get; private set; }
    }
}