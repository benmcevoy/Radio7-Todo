using System;
using System.Collections;
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

            foreach (var field in fields)
            {
                if (field.PropertyInfo.PropertyType.IsArray() || field.PropertyInfo.PropertyType.IsEnumerableOfT())
                {
                    SerializeEnumerable(luceneDocument, document, field);
                    continue;
                }

                var value = Serialize(document, field.PropertyInfo);
                luceneDocument.AddField(field, value);
            }

            return luceneDocument;
        }

        private static void SerializeEnumerable<T>(Document luceneDocument, T document, LuceneFieldInfo field)
        {
            var enumerable = (IEnumerable)(field.PropertyInfo.GetValue(document));

            if (enumerable == null) return;

            foreach (var item in enumerable)
            {
                // TODO: limited to value types that are convertible to string
                // should really have a typeconverter attribute on the fieldInfo object as well to allow specific serialization
                var value = (string)Convert.ChangeType(item, typeof(string));
                luceneDocument.AddField(field, value);
            }
        }

        private static Document AddField(this Document luceneDocument, LuceneFieldInfo field, string value)
        {
            if (value == null) return luceneDocument;

            luceneDocument.Add(new Field(
                field.Name,
                value,
                field.LuceneFieldAttribute.Store,
                field.LuceneFieldAttribute.Index,
                field.LuceneFieldAttribute.TermVector));

            return luceneDocument;
        }

        public static T ToResult<T>(this Document document) where T : new()
        {
            var result = new T();
            var fields = result.GetLuceneFieldInfos();

            foreach (var field in fields)
            {
                if (field.PropertyInfo.PropertyType.IsArray() || field.PropertyInfo.PropertyType.IsEnumerableOfT())
                {
                    TryDeserializeEnumerable(result, document, field);
                    continue;
                }

                var luceneField = document.GetField(field.Name);
                if (luceneField == null) continue;

                object value;
                if (TryDeserialize(luceneField.StringValue, field.PropertyInfo.PropertyType, out value))
                {
                    field.PropertyInfo.SetValue(result, value);
                }
            }

            return result;
        }

        private static void TryDeserializeEnumerable<T>(T result, Document document, LuceneFieldInfo field)
        {
            var luceneFields = document.GetFields(field.Name);
            if (luceneFields == null) return;

            var fieldType = field.PropertyInfo.PropertyType;

            if (fieldType.IsArray())
            {
                var argumentType = fieldType.GetElementType();
                var collection = new ArrayList();

                foreach (var luceneField in luceneFields)
                {
                    object value;
                    if (TryDeserialize(luceneField.StringValue, argumentType, out value))
                    {
                        collection.Add(value);
                    }
                }

                field.PropertyInfo.SetValue(result, collection);
                return;
            }

            if (fieldType.IsEnumerableOfT())
            {
                var argumentType = fieldType.GetGenericArguments().First();
                var listType = typeof(List<>);
                var concreteType = listType.MakeGenericType(argumentType);
                var collection = (IList)Activator.CreateInstance(concreteType);

                foreach (var luceneField in luceneFields)
                {
                    object value;
                    if (TryDeserialize(luceneField.StringValue, argumentType, out value))
                    {
                        collection.Add(value);
                    }
                }

                field.PropertyInfo.SetValue(result, collection);
            }
        }

        public static Term GetLuceneDocumentIdTerm<T>(this T document)
        {
            var type = typeof(T);

            if (!DocumentCache.ContainsKey(type))
            {
                var luceneDocumentAttribute = type.GetCustomAttribute<LuceneDocumentAttribute>();

                if (luceneDocumentAttribute == null)
                {
                    throw new InvalidOperationException(
                        string.Format("document of type {0} has no LuceneDocumentAttribute", type.Name));
                }

                DocumentCache[type] = luceneDocumentAttribute;
            }

            var field = DocumentCache[type].DocumentIdFieldName;
            var fieldValue = Serialize(document, type.GetProperty(field));

            return new Term(field, fieldValue);
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

        private static string Serialize<T>(T document, PropertyInfo propertyInfo)
        {
            var typeName = propertyInfo.PropertyType.Name;

            switch (typeName)
            {
                case "DateTime": return DateTime.Parse(propertyInfo.GetValue(document).ToString()).ToString("yyyyMMddHHmmssfff");
                case "Guid": return Guid.Parse(propertyInfo.GetValue(document).ToString()).ToString("N");

                default: return (string)Convert.ChangeType(propertyInfo.GetValue(document), typeof(string));
            }
        }

        private static bool TryDeserialize(string value, Type type, out object result)
        {
            result = value;

            var typeName = type.Name;

            if (type.IsGenericType) typeName = type.GetGenericArguments()[0].Name;

            try
            {
                switch (typeName)
                {
                    case "DateTime":
                        DateTime tempDate;

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
                result = "TryDeserialize failed for " + type.Name;
            }

            return false;
        }
    }
}