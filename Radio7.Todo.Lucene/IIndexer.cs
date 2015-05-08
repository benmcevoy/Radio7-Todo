using System.Collections.Generic;
using Lucene.Net.Analysis;
using Lucene.Net.Store;

namespace Radio7.Todo.Lucene
{
    public interface IIndexer<T>
    {
        int IndexBatchSize { get; }

        Directory Directory { get; }

        Analyzer Analyzer { get; }

        IEnumerable<T> GetDocumentsToIndex();

        void SetDocumentsIndexed(IEnumerable<T> documents);
    }
}