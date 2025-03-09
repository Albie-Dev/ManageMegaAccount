using System.Diagnostics.CodeAnalysis;

namespace MMA.Domain
{
    public static class CollectionExtension
    {
        public static bool IsNullOrEmpty<T>([NotNullWhen(false)] this ICollection<T>? collection)
        {
            return collection == null || !collection.Any();
        }

        public static string ToMultilineString(this List<string> list)
        {
            return string.Join(Environment.NewLine, list);
        }
    }
}