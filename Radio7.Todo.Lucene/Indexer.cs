using System.Collections.Generic;
using Lucene.Net.Index;

namespace Radio7.Todo.Lucene
{
    public class Indexer<T>
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
            using (
                var indexWriter = new IndexWriter(_indexConfig.Directory, _indexConfig.Analyzer, 
                    true, IndexWriter.MaxFieldLength.LIMITED))
            {
                foreach (var document in documents)
                {
                    Index(indexWriter, document);
                }

                indexWriter.Commit();
            }
        }

        protected virtual void Index(IndexWriter indexWriter, T document)
        {
            indexWriter.UpdateDocument(document.GetLuceneDocumentIdTerm(), document.ToLuceneDocument());
        }
    }
}
