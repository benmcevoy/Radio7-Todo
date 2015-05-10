using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using Cormo.Injects;
using Cormo.Web.Api;
using Lucene.Net.Index;
using Radio7.Todo.Lucene;
using Radio7.Todo.Server.Infrastructure;

namespace Radio7.Todo.Server
{
    [RestController]
    public class TodoController
    {
        [Inject] IIndexer<TodoTask> _indexer;
        [Inject] ISearcher<TodoTask> _searcher;
        [Inject] Parser _parser;
        [Inject] CookieService _cookieService;

        [Route("todo/"), HttpPost]
        public TodoTask Post(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return null;

            var todo = _parser.Parse(raw);

            _indexer.Index(new[] { todo });

            return todo;
        }

        [Route("todo/done"), HttpPost]
        public void Done(Guid id)
        {
            var doc = (_searcher.Search(new Term("Id", id.ToString("N"))) ?? Enumerable.Empty<TodoTask>()).FirstOrDefault();

            if (doc == null) return;

            doc.IsDone = true;

            _indexer.Index(new[] { doc });
        }

        [Route("todo/"), HttpGet]
        public IEnumerable<TodoTask> Get()
        {
            return _searcher.Search(new Term("IsDone", "False")) ?? Enumerable.Empty<TodoTask>();
        }

        [Route("todo/authenticate"), HttpPost, AllowAnonymous]
        public void Authenticate(string token)
        {
            var key = ConfigurationManager.AppSettings["Radio7.Todo.Server.CookieService.CookieValue"];

            if (token.Equals(key, StringComparison.Ordinal))
            {
                _cookieService.Create(new HttpContextWrapper(HttpContext.Current));
            }
        }
    }
}