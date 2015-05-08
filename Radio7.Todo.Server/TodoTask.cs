using System;
using Radio7.Todo.Lucene;

namespace Radio7.Todo.Server
{
    [LuceneDocument("Id")]
    public class TodoTask
    {
        [LuceneField("Id")]
        public Guid Id { get; set; }
        [LuceneField("Raw")]
        public string Raw { get; set; }
        [LuceneField("Title")]
        public string Title { get; set; }
        [LuceneField("Body")]
        public string Body { get; set; }
        [LuceneField("CreateDateTime")]
        public DateTime CreateDateTime { get; set; }
        [LuceneField("IsDone")]
        public bool IsDone { get; set; }

        public string[] Tags { get; set; }
    }
}