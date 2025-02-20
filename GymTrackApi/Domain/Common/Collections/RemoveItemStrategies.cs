namespace Domain.Common.Collections;

internal static class RemoveItemStrategies
{
	public static void RemoveSoonestExpiring<T>(ListWithMaxCapacity<T> list) where T : IExpiring
	{
		list.Remove(list.MinBy(item => item.ExpiresAt)!);
	}
}