using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;

namespace Radio7.Todo.Lucene
{
    public interface ISearcher<out T> where T : new()
    {
        IEnumerable<T> Search(Term term);
        IEnumerable<T> Search();
        IEnumerable<T> Search(string query);
    }

    public class Searcher<T> : ISearcher<T> where T : new()
    {
        private readonly ISearchConfig _searchConfig;

        public Searcher(ISearchConfig searchConfig)
        {
            _searchConfig = searchConfig;
        }

        public virtual IEnumerable<T> Search(Term term)
        {
            return Search(ToExactTermQuery(term));
        }

        public virtual IEnumerable<T> Search()
        {
            return Search(new MatchAllDocsQuery());
        }

        public virtual IEnumerable<T> Search(string query)
        {
            query = ToSafeQuery(query);

            return Search(ToWildCardQuery(query, new T().GetLuceneFieldInfos()));
        }

        private IEnumerable<T> Search(Query query)
        {
            try
            {
                Debug.WriteLine(query);

                var searcher = new IndexSearcher(_searchConfig.Directory);
                var topDocs = searcher
                    .Search(query, _searchConfig.SearchResultLimit);

                return topDocs.ScoreDocs
                    .Select(scoreDoc =>
                        searcher.Doc(scoreDoc.Doc).ToResult<T>());
            }
            catch (NoSuchDirectoryException)
            {
                return Enumerable.Empty<T>();
            }
        }

        protected Query ToWildCardQuery(string query, IEnumerable<LuceneFieldInfo> fields)
        {
            var terms = query.ToLowerInvariant().Split(new[] { " ,." }, StringSplitOptions.RemoveEmptyEntries);
            var wildCardQuery = new BooleanQuery();
            var enumeratedFields = fields.ToList();

            foreach (var term in terms)
            {
                var booleanQuery = new BooleanQuery();

                foreach (var field in enumeratedFields)
                {
                    var subQuery = new FuzzyQuery(
                        new Term(field.LuceneFieldAttribute.Name, term),
                        field.LuceneFieldAttribute.Fuzziness)
                    {
                        Boost = field.LuceneFieldAttribute.Boost
                    };

                    booleanQuery.Add(subQuery, Occur.SHOULD);
                }

                wildCardQuery.Add(booleanQuery, Occur.MUST);
            }

            return wildCardQuery;
        }

        protected Query ToExactTermQuery(Term term)
        {
            return new BooleanQuery { new BooleanClause(new TermQuery(term), Occur.MUST) };
        }

        protected static string ToSafeQuery(string query)
        {
            // http://lucene.apache.org/core/2_9_4/queryparsersyntax.html#Escaping%20Special%20Characters
            // remove any special lucene characters
            // + - && || ! ( ) { } [ ] ^ " ~ * ? : \
            return Regex.Replace(query, @"[\?\*\|\+\-!\(\)\{\}\[\]&:\#\\""~\^]", "");
        }
    }
}
