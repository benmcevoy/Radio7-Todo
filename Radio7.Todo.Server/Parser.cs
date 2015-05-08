using System;

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
                CreateDateTime = DateTime.UtcNow,
                Raw = raw,
                Title = title,
                Body = body,
                IsDone = false
            };
        }

        private static string GetTitle(string raw)
        {
            var offsetPeriod = raw.IndexOf(".", StringComparison.Ordinal);
            var offsetNewLine = raw.IndexOf(Environment.NewLine, StringComparison.Ordinal);

            if (offsetNewLine == NotFound && offsetPeriod == NotFound)
            {
                return raw;
            }

            if (offsetNewLine == NotFound) return raw.Substring(0, offsetPeriod);

            return raw.Substring(0, offsetNewLine).Trim();
        }

        private static string GetBody(string raw, int offset)
        {
            return raw.Substring(offset + 1).Trim();
        }
    }
}