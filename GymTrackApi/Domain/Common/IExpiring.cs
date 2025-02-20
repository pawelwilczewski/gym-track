using Domain.Common.Collections;

namespace Domain.Common;

public interface IExpiring
{
	DateTime ExpiresAt { get; }
}
