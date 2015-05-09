using Cormo.Injects;
using Radio7.Todo.Lucene;

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
}