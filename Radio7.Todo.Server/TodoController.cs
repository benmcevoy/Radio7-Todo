using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Web;
using System.Web.Http;
using Cormo.Injects;
using Cormo.Web.Api;
using Lucene.Net.Index;
using Radio7.Todo.Server.Infrastructure;

namespace Radio7.Todo.Server
{
    [RestController]
    public class TodoController
    {
        [Inject] TodoTaskIndexer  _indexer;
        [Inject] TodoTaskSearcher _searcher;
        [Inject] Parser _parser;
        [Inject] CookieService _cookieService;
        [Inject, Value] string CookieValue;

        [Route("todo/"), HttpPost]
        public async Task<TodoTask> Post(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return null;

            var todo = _parser.Parse(raw);

            await _indexer.IndexAsync(new[] { todo });

            return todo;
        }

        [Route("todo/done"), HttpPost]
        public async Task Done(Guid id)
        {
            var doc = (_searcher.Search(new Term("Id", id.ToString("N"))) ?? Enumerable.Empty<TodoTask>()).FirstOrDefault();

            if (doc == null) return;

            doc.IsDone = true;
            doc.CompletedDateTime = DateTime.UtcNow;

            await _indexer.IndexAsync(new[] { doc });
        }

        [Route("todo/"), HttpGet]
        public IEnumerable<TodoTask> Get()
        {
            return _searcher.Search(new Term("IsDone", "False")) ?? Enumerable.Empty<TodoTask>();
        }

        [Route("todo/export"), HttpGet]
        public IEnumerable<TodoTask> Export()
        {
            return _searcher.Search() ?? Enumerable.Empty<TodoTask>();
        }

        [Route("todo/authenticate"), HttpPost, AllowAnonymous]
        public void Authenticate(string token)
        {
            if (token.Equals(CookieValue, StringComparison.Ordinal))
            {
                _cookieService.Create(new HttpContextWrapper(HttpContext.Current));
            }
        }
    }
}