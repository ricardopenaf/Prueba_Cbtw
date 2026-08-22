using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketReservation.Domain.Entities;

namespace TicketReservation.Infrastructure.Persistence.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.ToTable("Events", t => t.HasCheckConstraint("CK_Events_AvailableSeats", "[AvailableSeats] >= 0"));

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Code)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(e => e.Code).IsUnique();

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Venue)
            .IsRequired()
            .HasMaxLength(200);
    }
}
