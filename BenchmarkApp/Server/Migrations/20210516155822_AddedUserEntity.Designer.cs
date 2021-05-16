﻿// <auto-generated />
using System;
using BenchmarkApp.Server.Database.SQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace BenchmarkApp.Server.Migrations
{
    [DbContext(typeof(SqlDatabaseContext))]
    [Migration("20210516155822_AddedUserEntity")]
    partial class AddedUserEntity
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityByDefaultColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.2");

            modelBuilder.Entity("BenchmarkApp.Server.Database.SQL.Entities.FriendshipEntity", b =>
                {
                    b.Property<int>("FriendAId")
                        .HasColumnType("integer");

                    b.Property<int>("FriendBId")
                        .HasColumnType("integer");

                    b.HasKey("FriendAId", "FriendBId");

                    b.HasIndex("FriendBId");

                    b.ToTable("Friendships");
                });

            modelBuilder.Entity("BenchmarkApp.Server.Database.SQL.Entities.UserEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<int?>("UserEntityId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("UserEntityId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("BenchmarkApp.Server.Database.SQL.Entities.FriendshipEntity", b =>
                {
                    b.HasOne("BenchmarkApp.Server.Database.SQL.Entities.UserEntity", "FriendA")
                        .WithMany()
                        .HasForeignKey("FriendAId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BenchmarkApp.Server.Database.SQL.Entities.UserEntity", "FriendB")
                        .WithMany()
                        .HasForeignKey("FriendBId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("FriendA");

                    b.Navigation("FriendB");
                });

            modelBuilder.Entity("BenchmarkApp.Server.Database.SQL.Entities.UserEntity", b =>
                {
                    b.HasOne("BenchmarkApp.Server.Database.SQL.Entities.UserEntity", null)
                        .WithMany("Friends")
                        .HasForeignKey("UserEntityId");
                });

            modelBuilder.Entity("BenchmarkApp.Server.Database.SQL.Entities.UserEntity", b =>
                {
                    b.Navigation("Friends");
                });
#pragma warning restore 612, 618
        }
    }
}
