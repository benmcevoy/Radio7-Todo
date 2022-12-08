using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Web;
using System.Web.Http;
using Lucene.Net.Index;
using Radio7.Todo.Server.Infrastructure;
using Radio7.Todo.Lucene;

namespace Radio7.Todo.Server
{
    public class TodoController : ApiController
    {
        private readonly IIndexer<TodoTask>  _indexer;
        private readonly ISearcher<TodoTask> _searcher;
        private readonly Parser _parser;
        private readonly CookieService _cookieService;
        private readonly string _cookieValue;

        public TodoController(Parser parser, CookieService cookieService, Producers producers, TodoTaskIndexer indexer, TodoTaskSearcher searcher)
        {
            _indexer = indexer;
            _searcher = searcher;
            _parser = parser;
            _cookieService = cookieService;
            _cookieValue = producers.CreateCookieValue();
        }

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
            if (token.Equals(_cookieValue, StringComparison.Ordinal))
            {
                _cookieService.Create(new HttpContextWrapper(HttpContext.Current));
            }
        }
    }
}