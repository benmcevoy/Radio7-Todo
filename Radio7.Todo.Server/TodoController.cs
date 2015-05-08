using System;
using System.Web.Http;
using Cormo.Web.Api;

namespace Radio7.Todo.Server
{
    [RestController]
    public class TodoController
    {
        [Route("todo/"), HttpPost]
        public TodoTask Post(string raw)
        {
            return new TodoTask()
            {
                Id = Guid.NewGuid(),
                CreateDateTime = DateTime.Now,

            };
        }

        [Route("todo/"), HttpGet]
        public TodoTask[] Get()
        {
            return new[]
            {
                new TodoTask()
                {
                    Id = Guid.NewGuid(),
                    CreateDateTime = DateTime.Now,

                }
            };
        }
    }
}