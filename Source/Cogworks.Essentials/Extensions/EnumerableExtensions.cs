using Cogworks.Essentials.Constants.StringConstants;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cogworks.Essentials.Extensions
{
    public static class EnumerableExtensions
    {
        public static bool HasAny(this IEnumerable items)
            => items != null && items.Cast<object>().Any();

        public static bool HasAny<T>(this IEnumerable<T> items)
            => items != null && items.GetEnumerator().MoveNext();

        public static TOutput FirstOrDefaultOfType<TInput, TOutput>(this IEnumerable publishedContents)
            => publishedContents.OfType<TOutput>().FirstOrDefault();

        public static string JoinIfNotNull<TInput, TResult>(this IEnumerable<TInput> items, Func<TInput, TResult> func,
            string separator = Separators.Space)
            => !items.HasAny()
                ? string.Empty
                : string.Join(separator, items.Select(func));

        public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> items)
            => items.HasAny()
                ? items
                    .Select((item, index) => (item, index))
                    .ToList()
                : Enumerable.Empty<(T item, int index)>();

        public static IEnumerable<T> OrderBySequence<T, TId>(this IEnumerable<T> source,
            IEnumerable<TId> order,
            Func<T, TId> idSelector)
        {
            var lookup = source.ToDictionary(idSelector, t => t);

            foreach (var id in order)
            {
                yield return lookup[id];
            }
        }

        public static IEnumerable<IEnumerable<T>> ChunkBy<T>(this IEnumerable<T> items, int chunkSize)
            => items
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
    }
}
