using Domain.Common.Ownership;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Persistence.Configurations.ValueConverters;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class OwnerValueConverter() : ValueConverter<Owner, Guid?>(
	owner => owner is Owner.User ? ((Owner.User)owner).UserId : null,
	id => id == null ? new Owner.Public() : new Owner.User(id.Value));