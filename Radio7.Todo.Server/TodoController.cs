using System;
using System.Collections.Generic;
using System.Linq;
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
        [Inject] Indexer<TodoTask> _index;
        [Inject] Searcher<TodoTask> _searcher;
        [Inject] Parser _parser;

        [Route("todo/"), HttpPost]
        public TodoTask Post(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return null;

            var todo = _parser.Parse(raw);

            _index.Index(new[] { todo });

            return todo;
        }

        [Route("todo/done"), HttpPost]
        public void Done(Guid id)
        {
            var doc = (_searcher.Search(new Term("Id", id.ToString("N"))) ?? Enumerable.Empty<TodoTask>()).FirstOrDefault();

            if (doc == null) return;

            doc.IsDone = true;

            _index.Index(new[] { doc });
        }

        [Route("todo/"), HttpGet]
        public IEnumerable<TodoTask> Get()
        {
            return _searcher.Search(new Term("IsDone", "False")) ?? Enumerable.Empty<TodoTask>();
        }
    }
}