﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using OfficeFileAccessor;

#nullable disable

namespace OfficeFileAccessor.Migrations
{
    [DbContext(typeof(OfficeFileAccessorContext))]
    partial class OfficeFileAccessorContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("OfficeFileAccessor.AppUsers.Entities.ApplicationUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("mail");

                    b.Property<DateTime>("LastUpdateDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("last_update_date")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<string>("Organization")
                        .HasColumnType("text")
                        .HasColumnName("organization");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("password");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("user_name");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("application_user");

                    b.HasData(
                        new
                        {
                            Id = -1,
                            Email = "default@example.com",
                            LastUpdateDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            PasswordHash = "AQAAAAIAAYagAAAAENkEv7oRtqJlHD11CP4+/psO/+8t7vQpFUhE8rUHN4YED6OLvfeCLilx1aKOhwaNpA==",
                            UserName = "DefaultUser"
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
