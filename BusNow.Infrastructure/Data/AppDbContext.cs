using BusNow.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusNow.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<TransportLine> TransportLines => Set<TransportLine>();
        public DbSet<Stop> Stops => Set<Stop>();
        public DbSet<Route> Routes => Set<Route>();
        public DbSet<RouteStop> RouteStops => Set<RouteStop>();
        public DbSet<Schedule> Schedules => Set<Schedule>();
        public DbSet<Vehicle> Vehicles => Set<Vehicle>();
        public DbSet<ArrivalPrediction> ArrivalPredictions => Set<ArrivalPrediction>();
        public DbSet<ServiceAlert> ServiceAlerts => Set<ServiceAlert>();
        public DbSet<FavoriteStop> FavoriteStops => Set<FavoriteStop>();
        public DbSet<FavoriteLine> FavoriteLines => Set<FavoriteLine>();
        public DbSet<UsageStatistic> UsageStatistics => Set<UsageStatistic>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<TransportLine>()
                .Property(x => x.LineNumber)
                .HasMaxLength(10);

            builder.Entity<TransportLine>()
                .Property(x => x.Type)
                .HasMaxLength(20);

            builder.Entity<Stop>()
                .Property(x => x.Name)
                .HasMaxLength(120);

            builder.Entity<Stop>()
                .Property(x => x.Code)
                .HasMaxLength(20);

            builder.Entity<RouteStop>()
                .HasIndex(x => new { x.RouteId, x.OrderIndex });

            builder.Entity<FavoriteStop>()
                .HasIndex(x => new { x.UserId, x.StopId })
                .IsUnique();

            builder.Entity<FavoriteLine>()
                .HasIndex(x => new { x.UserId, x.LineId })
                .IsUnique();

            builder.Entity<FavoriteStop>()
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<FavoriteLine>()
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ServiceAlert>()
                .Property(x => x.Title)
                .HasMaxLength(150);

            builder.Entity<ServiceAlert>()
                .HasOne(x => x.AffectedLine)
                .WithMany()
                .HasForeignKey(x => x.AffectedLineId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ServiceAlert>()
                .HasOne(x => x.AffectedRoute)
                .WithMany()
                .HasForeignKey(x => x.AffectedRouteId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Vehicle>()
                .HasOne(v => v.Route)
                .WithMany()
                .HasForeignKey(v => v.RouteId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
