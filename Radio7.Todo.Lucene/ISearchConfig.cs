using Lucene.Net.Analysis;
using Lucene.Net.Store;

namespace Radio7.Todo.Lucene
{
    public interface ISearchConfig
    {
        Directory Directory { get; }

        Analyzer Analyzer { get; }

        int SearchResultLimit { get; }
    }
}