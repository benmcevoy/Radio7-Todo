using System;
using Lucene.Net.Index;

namespace Radio7.Todo.Lucene
{
    [AttributeUsage(AttributeTargets.Class)]
    public class LuceneDocumentAttribute : Attribute
    {
        public LuceneDocumentAttribute(string documentIdFieldName)
        {
            DocumentIdFieldName = documentIdFieldName;
        }

        public string DocumentIdFieldName { get; private set; }

        public Term IdTerm { get { return new Term(DocumentIdFieldName); } }
    }
}
