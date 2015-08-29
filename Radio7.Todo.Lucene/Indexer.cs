using System.Threading.Tasks;
using System.Collections.Generic;
using Lucene.Net.Index;
using System;

namespace Radio7.Todo.Lucene
{
    public interface IIndexer<T>
    {
        void Index(IEnumerable<T> documents);

        Task IndexAsync(IEnumerable<T> documents);
    }

    public class Indexer<T> : IIndexer<T>
    {
        private readonly ISearchConfig _indexConfig;

        public Indexer(ISearchConfig config)
        {
            _indexConfig = config;
        }

        public virtual void Index(IEnumerable<T> documents)
        {
            // TODO: commit in batches
            // TODO: read the docs on indexwriter
            using (var indexWriter = new IndexWriter(_indexConfig.Directory, _indexConfig.Analyzer, IndexWriter.MaxFieldLength.LIMITED))
            {
                foreach (var document in documents)
                {
                    Index(indexWriter, document);
                }

                indexWriter.Commit();
            }
        }

        public Task IndexAsync(IEnumerable<T> documents)
        {
            return Task.Factory.StartNew(() =>
            {
                using (var indexWriter = new IndexWriter(_indexConfig.Directory, _indexConfig.Analyzer, IndexWriter.MaxFieldLength.LIMITED))
                {
                    foreach (var document in documents)
                    {
                        Index(indexWriter, document);
                    }

                    indexWriter.Commit();
                }
            });
        }

        protected virtual void Index(IndexWriter indexWriter, T document)
        {
            indexWriter.UpdateDocument(document.GetLuceneDocumentIdTerm(), document.ToLuceneDocument());
        }
    }
}
