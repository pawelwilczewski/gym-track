using System.Collections;

namespace Domain.Common.Collections;

public sealed class ListWithMaxCapacity<T> : IList<T>, IReadOnlyList<T>
{
	private readonly List<T> list = [];

	public int Count => list.Count;
	public bool IsReadOnly => false;

	public int MaxCapacity { get; }

	private readonly RemoveToMakeSpaceStrategy removeItemStrategy;

	public ListWithMaxCapacity(int maxCapacity, RemoveToMakeSpaceStrategy removeItemToMakeSpaceStrategy)
	{
		MaxCapacity = maxCapacity >= 0
			? maxCapacity
			: throw new ArgumentOutOfRangeException(nameof(maxCapacity), "MaxCapacity must be non-negative.");

		removeItemStrategy = removeItemToMakeSpaceStrategy
			?? throw new ArgumentNullException(nameof(removeItemToMakeSpaceStrategy));
	}

	public IEnumerator<T> GetEnumerator() => list.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	public void Add(T item)
	{
		EnsureSpaceForNewItem();
		list.Add(item);
	}

	public void Insert(int index, T item)
	{
		EnsureSpaceForNewItem();
		list.Insert(index, item);
	}

	private void EnsureSpaceForNewItem()
	{
		while (list.Count >= MaxCapacity)
		{
			var previousCount = list.Count;
			removeItemStrategy(this);

			if (list.Count >= previousCount)
			{
				throw new InvalidOperationException("The remove strategy did not remove an item.");
			}
		}
	}

	public void CopyTo(T[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);

	public bool Remove(T item) => list.Remove(item);
	public void RemoveAt(int index) => list.RemoveAt(index);
	public void Clear() => list.Clear();

	public bool Contains(T item) => list.Contains(item);
	public int IndexOf(T item) => list.IndexOf(item);

	public T this[int index]
	{
		get => list[index];
		set => list[index] = value;
	}

	public delegate void RemoveToMakeSpaceStrategy(ListWithMaxCapacity<T> list);
}