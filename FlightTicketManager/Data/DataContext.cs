using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using FlightTicketManager.Data.Entities;

namespace FlightTicketManager.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DbSet<Aircraft> Aircrafts { get; set; }

        public DbSet<City> Cities { get; set; }

        public DbSet<Flight> Flights { get; set; }

        public DbSet<Ticket> Tickets { get; set; }

        public DbSet<FlightHistory> FlightsHistory { get; set; }

        public DbSet<TicketHistory> TicketsHistory { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var valueComparer = new ValueComparer<List<string>>(
                (c1, c2) => c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList());

            modelBuilder.Entity<City>()
                .HasIndex(c => c.Name)
                .IsUnique();

            modelBuilder.Entity<Aircraft>()
                .Property(a => a.Seats)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v), 
                    v => JsonConvert.DeserializeObject<List<string>>(v))
                .Metadata
                .SetValueComparer(valueComparer);

            modelBuilder.Entity<Flight>()
                .Property(f => f.AvailableSeats)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<List<string>>(v))
                .Metadata
                .SetValueComparer(valueComparer);

            modelBuilder.Entity<Flight>()
                .HasMany(f => f.TicketsList)
                .WithOne(t => t.Flight)
                .IsRequired();

            modelBuilder.Entity<Ticket>()
                .Property(t => t.Price)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<TicketHistory>()
                .Property(t => t.Price)
                .HasColumnType("decimal(18, 2)");

            base.OnModelCreating(modelBuilder);
        }
    }
}
