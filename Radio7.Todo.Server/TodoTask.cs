using System;
using System.Collections.Generic;
using Radio7.Todo.Lucene;

namespace Radio7.Todo.Server
{
    [LuceneDocument("Id")] public class TodoTask
    {
        [LuceneField] public Guid Id { get; set; }
        [LuceneField] public string Raw { get; set; }
        [LuceneField] public string Title { get; set; }
        [LuceneField] public string Body { get; set; }
        [LuceneField] public DateTime CreateDateTime { get; set; }
        [LuceneField] public bool IsDone { get; set; }
        [LuceneField] public IEnumerable<string> Tags { get; set; }
        [LuceneField] public DateTime? CompletedDateTime { get; set; }
    }
}