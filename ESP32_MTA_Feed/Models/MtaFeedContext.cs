using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;
using Pomelo.EntityFrameworkCore.MySql;

namespace ESP32_MTA_Feed.Models;

public partial class MtaFeedContext : DbContext
{
    private readonly IConfiguration _configuration;

    public MtaFeedContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public virtual DbSet<SubwayStop> SubwayStops { get; set; }

          protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var conStrBuilder = new MySqlConnector.MySqlConnectionStringBuilder(_configuration.GetConnectionString("MtaFeed"))
            {
                Password = _configuration["DATABASE_PASS"],
                UserID = _configuration["DATABASE_USER"]
            };

            var serverVersion = new MySqlServerVersion(new Version(8, 0, 36)); // Change the version as per your MySQL server version
            optionsBuilder.UseMySql(conStrBuilder.ConnectionString, serverVersion);
        }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<SubwayStop>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("SUBWAY_STOPS");

            entity.Property(e => e.LocationType)
                .HasMaxLength(5)
                .HasColumnName("location_type");
            entity.Property(e => e.ParentStation)
                .HasMaxLength(5)
                .HasColumnName("parent_station");
            entity.Property(e => e.StopId)
                .HasMaxLength(5)
                .HasColumnName("stop_id");
            entity.Property(e => e.StopLat)
                .HasMaxLength(50)
                .HasColumnName("stop_lat");
            entity.Property(e => e.StopLon)
                .HasMaxLength(50)
                .HasColumnName("stop_lon");
            entity.Property(e => e.StopName)
                .HasMaxLength(255)
                .HasColumnName("stop_name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
