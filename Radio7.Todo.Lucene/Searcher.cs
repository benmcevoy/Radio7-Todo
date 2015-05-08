using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Lucene.Net.Index;
using Lucene.Net.Search;

namespace Radio7.Todo.Lucene
{
    public class Searcher<T> where T : new()
    {
        private readonly ISearchConfig _searchConfig;

        public Searcher(ISearchConfig searchConfig)
        {
            _searchConfig = searchConfig;
        }

        public virtual IEnumerable<T> Search(Query query)
        {
            var searcher = new IndexSearcher(_searchConfig.Directory);
            var topDocs = searcher
                .Search(query, _searchConfig.SearchResultLimit);

            return topDocs.ScoreDocs
                .Select(scoreDoc =>
                    searcher.Doc(scoreDoc.Doc).ToResult<T>());
        }

        public virtual IEnumerable<T> Search()
        {
            var searcher = new IndexSearcher(_searchConfig.Directory);
            var topDocs = searcher
                .Search(new MatchAllDocsQuery(), _searchConfig.SearchResultLimit);

            return topDocs.ScoreDocs
                .Select(scoreDoc =>
                    searcher.Doc(scoreDoc.Doc).ToResult<T>());
        }

        public virtual IEnumerable<T> Search(string query)
        {
            query = ToSafeQuery(query);

            var searcher = new IndexSearcher(_searchConfig.Directory);
            var topDocs = searcher
                .Search(ToWildCardQuery(query, new T().GetLuceneFieldInfos()), _searchConfig.SearchResultLimit);

            return topDocs.ScoreDocs
                .Select(scoreDoc =>
                    searcher.Doc(scoreDoc.Doc).ToResult<T>());
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
            };

            return wildCardQuery;
        }

        protected static string ToSafeQuery(string query)
        {
            // search all
            if (string.IsNullOrWhiteSpace(query)) return "*";

            // http://lucene.apache.org/core/2_9_4/queryparsersyntax.html#Escaping%20Special%20Characters
            // remove any special lucene characters
            // + - && || ! ( ) { } [ ] ^ " ~ * ? : \
            return Regex.Replace(query, @"[\?\*\|\+\-!\(\)\{\}\[\]&:\#\\""~\^]", "");
        }
    }
}
