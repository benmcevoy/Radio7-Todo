using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Lucene.Net.Documents;
using Lucene.Net.Index;

namespace Radio7.Todo.Lucene
{
    public static class IndexerExtensions
    {
        private static readonly Dictionary<Type, IEnumerable<LuceneFieldInfo>> FieldCache
            = new Dictionary<Type, IEnumerable<LuceneFieldInfo>>(8);

        private static readonly Dictionary<Type, LuceneDocumentAttribute> DocumentCache
            = new Dictionary<Type, LuceneDocumentAttribute>(8);

        public static Document ToLuceneDocument<T>(this T document)
        {
            var luceneDocument = new Document();
            var fields = document.GetLuceneFieldInfos();

            // TODO: ensure LuceneDocumentAttribute id field is always included

            foreach (var field in fields)
            {
                var value = ConvertToString(document, field.PropertyInfo);

                if (value == null) continue;

                luceneDocument.Add(new Field(
                    field.LuceneFieldAttribute.Name, 
                    value,
                    field.LuceneFieldAttribute.Store, 
                    field.LuceneFieldAttribute.Index,
                    field.LuceneFieldAttribute.TermVector));
            }

            return luceneDocument;
        }

        public static T ToResult<T>(this Document document) where T : new()
        {
            var result = new T();
            var fields = result.GetLuceneFieldInfos();

            foreach (var field in fields)
            {
                object value;
                var luceneField = document.GetField(field.LuceneFieldAttribute.Name);

                if (luceneField == null) continue;

                if (TryConvertToType(
                    luceneField.StringValue,
                    field.PropertyInfo.PropertyType,
                    out value))
                {
                    field.PropertyInfo.SetValue(result, value);
                }
            }

            return result;
        }

        public static Term GetLuceneDocumentIdTerm<T>(this T document)
        {
            var type = typeof(T);

            if (DocumentCache.ContainsKey(type)) return DocumentCache[type].IdTerm;

            var luceneDocumentAttribute = type.GetCustomAttribute<LuceneDocumentAttribute>();

            if (luceneDocumentAttribute == null)
            {
                throw new InvalidOperationException(
                    string.Format("document of type {0} has no LuceneDocumentAttribute", type.Name));
            }

            DocumentCache[type] = luceneDocumentAttribute;

            return DocumentCache[type].IdTerm;
        }

        public static IEnumerable<LuceneFieldInfo> GetLuceneFieldInfos<T>(this T document)
        {
            var type = typeof(T);

            if (FieldCache.ContainsKey(type)) return FieldCache[type];

            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            var fields = properties
                .Select(p => new LuceneFieldInfo
                    {
                        PropertyInfo = p,
                        LuceneFieldAttribute = p.GetCustomAttribute<LuceneFieldAttribute>()
                    })
                .Where(f => f.LuceneFieldAttribute != null);

            FieldCache[type] = fields;

            return FieldCache[type];
        }

        // this is basic serialization... 
        // why not just tojson and back on the T? well, we are doing it on a field level...
        private static string ConvertToString<T>(T document, PropertyInfo propertyInfo)
        {
            var typeName = propertyInfo.PropertyType.Name;

            switch (typeName)
            {
                case "DateTime": return DateTime.Parse(propertyInfo.GetValue(document).ToString()).ToString(CultureInfo.InvariantCulture);
                case "Boolean": return Boolean.Parse(propertyInfo.GetValue(document).ToString()).ToString();
                case "Guid": return Guid.Parse(propertyInfo.GetValue(document).ToString()).ToString("N");

                default: return (string)Convert.ChangeType(propertyInfo.GetValue(document), propertyInfo.PropertyType);
            }
        }

        private static bool TryConvertToType(string value, Type type, out object result)
        {
            result = value;

            var typeName = type.Name;

            if (type.IsGenericType) typeName = type.GetGenericArguments()[0].Name;

            try
            {
                switch (typeName)
                {
                    case "DateTime":
                        // ReSharper disable once RedundantAssignment
                        var tempDate = new DateTime(1900, 1, 1);

                        if (
                            !DateTime.TryParse(value, CultureInfo.CurrentCulture, DateTimeStyles.None,
                                out tempDate)) return false;

                        result = tempDate;
                        return true;

                    case "Guid":
                        result = Guid.Parse(value);
                        return true;

                    default:
                        // let .NET have a crack
                        result = Convert.ChangeType(value, type);
                        return true;
                }
            }
            catch
            {
                result = "TryConvertToType failed for " + type.Name;
            }

            return false;
        }
    }
}