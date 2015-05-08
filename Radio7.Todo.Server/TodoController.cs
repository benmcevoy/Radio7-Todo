using System.Collections.Generic;
using System.Web.Http;
using Cormo.Injects;
using Cormo.Web.Api;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Radio7.Todo.Lucene;

namespace Radio7.Todo.Server
{
    [RestController]
    public class TodoController
    {
        [Inject]
        Indexer<TodoTask> _index;
        [Inject]
        Searcher<TodoTask> _searcher;
        [Inject]
        Parser _parser;

        [Route("todo/"), HttpPost]
        public TodoTask Post(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return null;

            var todo = _parser.Parse(raw);

            _index.Index(new[] { todo });

            return todo;
        }

        static readonly BooleanQuery _query = new BooleanQuery { new BooleanClause(new TermQuery(new Term("IsDone", "false")), Occur.MUST) };

        [Route("todo/"), HttpGet]
        public IEnumerable<TodoTask> Get()
        {
            return _searcher.Search(_query);
        }
    }
}