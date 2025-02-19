using Domain.Models.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.User;

internal sealed class UserRefreshTokenConfiguration : IEntityTypeConfiguration<UserRefreshToken>
{
	public void Configure(EntityTypeBuilder<UserRefreshToken> builder)
	{
		builder
			.ToTable("UserRefreshTokens", Schemas.AUTHENTICATION)
			.HasKey(token => token.Id);
	}
}