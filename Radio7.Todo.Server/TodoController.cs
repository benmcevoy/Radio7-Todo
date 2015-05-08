using System;
using System.Linq;
using System.Web.Http;
using Cormo.Injects;
using Cormo.Web.Api;
using Radio7.Todo.Lucene;

namespace Radio7.Todo.Server
{
    [RestController]
    public class TodoController
    {
        [Inject] Indexer<TodoTask> _index;
        [Inject] Searcher<TodoTask> _searcher;

        [Route("todo/"), HttpPost]
        public TodoTask Post(string raw)
        {
            var todo = new TodoTask()
            {
                Id = Guid.NewGuid(),
                CreateDateTime = DateTime.Now,
                Raw = raw
            };

            _index.Index(new[] { todo });

            return todo;
        }

        [Route("todo/"), HttpGet]
        public TodoTask[] Get()
        {
            return _searcher.Search().ToArray();
        }
    }
}