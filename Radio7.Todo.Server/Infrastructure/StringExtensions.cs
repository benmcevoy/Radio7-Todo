using Microsoft.Security.Application;

namespace Radio7.Todo.Server.Infrastructure
{
    public static class StringExtensions
    {
        public static string HtmlEncode(this string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return value;

            return Encoder.HtmlEncode(value);
        }
    }
}