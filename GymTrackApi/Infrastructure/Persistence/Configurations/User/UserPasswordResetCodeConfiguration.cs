using Domain.Models.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.User;

internal sealed class UserPasswordResetCodeConfiguration : IEntityTypeConfiguration<UserPasswordResetCode>
{
	public void Configure(EntityTypeBuilder<UserPasswordResetCode> builder)
	{
		builder
			.ToTable("UserPasswordResetCodes", Schemas.AUTHENTICATION)
			.HasKey(code => code.UserId);
	}
}