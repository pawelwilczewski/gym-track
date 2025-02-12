using Domain.Common;
using Domain.Common.Ownership;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Application.Persistence;

public interface IUserAssetRepository<TAsset> where TAsset : AggregateRoot, IOwned
{
	IQueryable<TAsset> Readable { get; }
	IQueryable<TAsset> Modifiable { get; }

	EntityEntry<TAsset> Add(TAsset entity);
	EntityEntry<TAsset> Remove(TAsset entity);
}