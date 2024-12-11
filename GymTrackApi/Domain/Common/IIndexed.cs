namespace Domain.Common;

public interface IIndexed
{
	int Index { get; }
}

public static class IndexedExtensions
{
	public static int GetNextIndex(this IReadOnlyCollection<IIndexed> indexed) =>
		indexed.Count > 0 ? indexed.Select(i => i.Index).Max() + 1 : 0;
}