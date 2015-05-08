using System;
using Lucene.Net.Documents;

namespace Radio7.Todo.Lucene
{
    [AttributeUsage(AttributeTargets.Property)]
    public class LuceneFieldAttribute : Attribute
    {
        public LuceneFieldAttribute(string name, 
            Field.Store store = Field.Store.YES,
            Field.Index index = Field.Index.NOT_ANALYZED, 
            Field.TermVector termVector = Field.TermVector.NO, 
            float fuzziness = 0.5f, 
            float boost = 1f)
        {
            Store = store;
            Index = index;
            TermVector = termVector;
            Name = name;
            Fuzziness = fuzziness;
            Boost = boost;
        }

        public override string ToString()
        {
            return Name;
        }

        public string Name { get; private set; }        
        
        public Field.Store Store { get; private set; }
        
        public Field.Index Index { get; private set; }
        
        public Field.TermVector TermVector { get; private set; }

        public float Fuzziness { get; private set; }

        public float Boost { get; private set; }
    }
}
