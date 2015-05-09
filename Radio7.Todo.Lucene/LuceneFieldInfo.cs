using System.Reflection;

namespace Radio7.Todo.Lucene
{
    public class LuceneFieldInfo
    {
        public PropertyInfo PropertyInfo { get; set; }

        public LuceneFieldAttribute LuceneFieldAttribute { get; set; }

        public string Name
        {
            get { return LuceneFieldAttribute.Name ?? PropertyInfo.Name; }
        }

        public override string ToString()
        {
            return LuceneFieldAttribute.ToString();
        }
    }
}
