using Domain.Common.ValueObjects;

namespace Domain.Common;

public interface IIndexed<out T> where T : IValueObject<int, T>
{
	T Index { get; }
}

public static class IndexedExtensions
{
	public static T GetNextIndex<T>(this IReadOnlyCollection<IIndexed<T>> indexed) where T : IValueObject<int, T> =>
		T.From(indexed.Count > 0 ? indexed.Select(i => i.Index.Value).Max() + 1 : 0);
}