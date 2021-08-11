﻿// <auto-generated />
using System;
using BenchmarkApp.Server.Database.SQL.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace BenchmarkApp.Server.Migrations
{
    [DbContext(typeof(SqlDatabaseContext))]
    partial class SqlDatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityByDefaultColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.2");

            modelBuilder.Entity("BenchmarkApp.Server.Database.SQL.Entities.SqlFriendshipEntity", b =>
                {
                    b.Property<Guid>("FriendAId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("FriendBId")
                        .HasColumnType("uuid");

                    b.HasKey("FriendAId", "FriendBId");

                    b.HasIndex("FriendAId");

                    b.HasIndex("FriendBId");

                    b.ToTable("Friendships");
                });

            modelBuilder.Entity("BenchmarkApp.Server.Database.SQL.Entities.SqlUserEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("Age")
                        .HasColumnType("integer");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<string>("FirstName")
                        .HasColumnType("text");

                    b.Property<int>("Gender")
                        .HasColumnType("integer");

                    b.Property<string>("Identifier")
                        .HasColumnType("text");

                    b.Property<string>("LastName")
                        .HasColumnType("text");

                    b.Property<string>("UserName")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("BenchmarkApp.Server.Database.SQL.Entities.SqlFriendshipEntity", b =>
                {
                    b.HasOne("BenchmarkApp.Server.Database.SQL.Entities.SqlUserEntity", "FriendA")
                        .WithMany("FriendShips")
                        .HasForeignKey("FriendAId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BenchmarkApp.Server.Database.SQL.Entities.SqlUserEntity", "FriendB")
                        .WithMany()
                        .HasForeignKey("FriendBId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("FriendA");

                    b.Navigation("FriendB");
                });

            modelBuilder.Entity("BenchmarkApp.Server.Database.SQL.Entities.SqlUserEntity", b =>
                {
                    b.Navigation("FriendShips");
                });
#pragma warning restore 612, 618
        }
    }
}
