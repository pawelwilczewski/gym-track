using Domain.Models.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.User;

internal sealed class UserEmailConfirmationCodeConfiguration : IEntityTypeConfiguration<UserEmailConfirmationCode>
{
	public void Configure(EntityTypeBuilder<UserEmailConfirmationCode> builder)
	{
		builder
			.ToTable("UserEmailConfirmationCodes", Schemas.AUTHENTICATION)
			.HasKey(code => code.EmailConfirmationCode);
	}
}