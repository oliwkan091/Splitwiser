﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Splitwiser.EntityFramework;

#nullable disable

namespace Splitwiser.EntityFramework.Migrations.Splitwiser
{
    [DbContext(typeof(SplitwiserDbContext))]
    [Migration("20231214155248_addPaymentTable")]
    partial class addPaymentTable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Splitwiser.Models.Group.GroupEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("NEWID()");

                    b.Property<DateTime>("AddDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("GroupName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("Splitwiser.Models.GroupPaymentHistory.GroupPaymentHistoryEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("NEWID()");

                    b.Property<DateTime>("AddDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<double>("Amount")
                        .HasColumnType("float");

                    b.Property<Guid>("GroupId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("TransactionName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("Payments");
                });

            modelBuilder.Entity("Splitwiser.Models.GroupPaymentHistory.PaymentMemberEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("NEWID()");

                    b.Property<DateTime>("AddDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<Guid>("GroupId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("PaymentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("PaymentMembers");
                });

            modelBuilder.Entity("Splitwiser.Models.UserGroup.UserGroupEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("NEWID()");

                    b.Property<Guid>("GroupId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("UserGroups");
                });
#pragma warning restore 612, 618
        }
    }
}
