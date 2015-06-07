using System;
using System.Collections.Generic;
using System.Linq;
using Radio7.Todo.Server.Infrastructure;

namespace Radio7.Todo.Server
{
    public class Parser
    {
        private const int NotFound = -1;

        public TodoTask Parse(string raw)
        {
            var title = GetTitle(raw);
            var body = GetBody(raw, title.Length);

            return new TodoTask
            {
                Id = Guid.NewGuid(),
                CreatedDateTime = DateTime.UtcNow,
                Raw = raw,
                Title = title.HtmlEncode(),
                Body = body.HtmlEncode(),
                IsDone = false,
                Tags = GetTags(raw)
            };
        }

        private static IEnumerable<string> GetTags(string raw)
        {
            var parts = raw.Split(new[] { '#' }, StringSplitOptions.RemoveEmptyEntries);

            if (!parts.Any()) yield break;

            foreach (var part in parts.Skip(1))
            {
                var tagValue = part.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();

                if (string.IsNullOrWhiteSpace(tagValue)) continue;

                yield return tagValue.ToUpperInvariant();
            }
        }

        private static string GetTitle(string raw)
        {
            var offsetPeriod = raw.IndexOf(".", StringComparison.Ordinal);
            var offsetNewLine = raw.IndexOf("\n", StringComparison.Ordinal);

            if (offsetNewLine == NotFound && offsetPeriod == NotFound)
            {
                return raw;
            }

            if (offsetNewLine == NotFound) return raw.Substring(0, offsetPeriod);

            return raw.Substring(0, offsetNewLine).Trim();
        }

        private static string GetBody(string raw, int offset)
        {
            if (offset >= raw.Length) return "";

            return raw.Substring(offset + 1).Trim();
        }
    }
}