namespace Domain.Common;

public interface IDisplayOrdered
{
	int DisplayOrder { get; }
}

public static class DisplayOrderedExtensions
{
	public static int GetNextDisplayOrder(this IReadOnlyCollection<IDisplayOrdered> items) =>
		items.Count > 0 ? items.Select(i => i.DisplayOrder).Max() + 1 : 0;

	public static IEnumerable<T> SortedByDisplayOrder<T>(this IEnumerable<T> items) where T : IDisplayOrdered =>
		items.OrderBy(i => i.DisplayOrder);
}