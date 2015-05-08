using System;

namespace Radio7.Todo.Server
{
    public class TodoTask
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string[] Tags { get; set; }
        public DateTime CreateDateTime { get; set; }
        public bool IsDone { get; set; }
    }
}