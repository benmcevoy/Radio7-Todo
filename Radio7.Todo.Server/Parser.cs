using System;
using System.Collections.Generic;
using System.Linq;
using Radio7.Todo.Server.Infrastructure;
using Cormo.Injects;

namespace Radio7.Todo.Server
{
    public class Parser
    {
        [Inject] Markdown _markdown;
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
                // do the markdown rendering on WRITE so we so not have to on the many READS
                Body = _markdown.Render(body),
                IsDone = false,
                Tags = GetTags(raw).Distinct()
            };
        }

        private static IEnumerable<string> GetTags(string raw)
        {
            var parts = raw.Split(new[] { '#' }, StringSplitOptions.RemoveEmptyEntries);

            if (!parts.Any()) yield break;

            foreach (var part in parts)
            {
                var tagValue = part.Split(new[] { ' ', '.', '\n', '?', '!', ',' },
                    StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();

                if (string.IsNullOrWhiteSpace(tagValue)) continue;

                yield return tagValue.Trim().ToUpperInvariant();
            }
        }

        private static string GetTitle(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return "";

            var terminator = SentenceTerminators(raw)
                .Where(x => x.Item1)
                .Min(x => x.Item2);

            return (terminator == NotFound)
                ? raw.Trim()
                : raw.Substring(0, terminator).Trim();
        }

        private static IEnumerable<Tuple<bool, int>> SentenceTerminators(string raw)
        {
            // look for terminator
            yield return FindOffset(raw, ".");
            yield return FindOffset(raw, "?");
            yield return FindOffset(raw, "!");
            yield return FindOffset(raw, "\n");

            // return the fragment
            yield return new Tuple<bool, int>(true, raw.Length);
        }

        private static Tuple<bool, int> FindOffset(string value, string test)
        {
            var offset = value.IndexOf(test, StringComparison.Ordinal);

            return new Tuple<bool, int>(offset > NotFound, offset + 1);
        }

        private static string GetBody(string raw, int offset)
        {
            return (offset >= raw.Length)
                ? ""
                : raw.Substring(offset + 1).Trim();
        }
    }
}