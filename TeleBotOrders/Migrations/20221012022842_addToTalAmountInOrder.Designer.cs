// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TeleBotOrders;

#nullable disable

namespace TeleBotOrders.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    [Migration("20221012022842_addToTalAmountInOrder")]
    partial class addToTalAmountInOrder
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("TeleBotOrders.Cafe", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long>("MenuId")
                        .HasColumnType("bigint");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long?>("OrderId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("MenuId");

                    b.HasIndex("OrderId")
                        .IsUnique();

                    b.ToTable("Cafes");
                });

            modelBuilder.Entity("TeleBotOrders.Dish", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<int>("Count")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<double>("Discount")
                        .HasColumnType("double precision");

                    b.Property<long?>("MenuId")
                        .HasColumnType("bigint");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long?>("OrderId")
                        .HasColumnType("bigint");

                    b.Property<string>("PathImage")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<double>("Price")
                        .HasColumnType("double precision");

                    b.HasKey("Id");

                    b.HasIndex("MenuId");

                    b.HasIndex("OrderId");

                    b.ToTable("Dishes");
                });

            modelBuilder.Entity("TeleBotOrders.Menu", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long?>("OrderId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("OrderId")
                        .IsUnique();

                    b.ToTable("Menus");
                });

            modelBuilder.Entity("TeleBotOrders.Order", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<double>("TotalAmount")
                        .HasColumnType("double precision");

                    b.HasKey("Id");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("TeleBotOrders.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<bool>("IsInit")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long?>("OrderId")
                        .HasColumnType("bigint");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("TeleBotOrders.Cafe", b =>
                {
                    b.HasOne("TeleBotOrders.Menu", "Menu")
                        .WithMany()
                        .HasForeignKey("MenuId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TeleBotOrders.Order", "Order")
                        .WithOne("Cafe")
                        .HasForeignKey("TeleBotOrders.Cafe", "OrderId");

                    b.Navigation("Menu");

                    b.Navigation("Order");
                });

            modelBuilder.Entity("TeleBotOrders.Dish", b =>
                {
                    b.HasOne("TeleBotOrders.Menu", "Menu")
                        .WithMany("Dishes")
                        .HasForeignKey("MenuId");

                    b.HasOne("TeleBotOrders.Order", "Order")
                        .WithMany("Dishes")
                        .HasForeignKey("OrderId");

                    b.Navigation("Menu");

                    b.Navigation("Order");
                });

            modelBuilder.Entity("TeleBotOrders.Menu", b =>
                {
                    b.HasOne("TeleBotOrders.Order", "Order")
                        .WithOne("Menu")
                        .HasForeignKey("TeleBotOrders.Menu", "OrderId");

                    b.Navigation("Order");
                });

            modelBuilder.Entity("TeleBotOrders.User", b =>
                {
                    b.HasOne("TeleBotOrders.Order", "Order")
                        .WithMany("Users")
                        .HasForeignKey("OrderId");

                    b.Navigation("Order");
                });

            modelBuilder.Entity("TeleBotOrders.Menu", b =>
                {
                    b.Navigation("Dishes");
                });

            modelBuilder.Entity("TeleBotOrders.Order", b =>
                {
                    b.Navigation("Cafe")
                        .IsRequired();

                    b.Navigation("Dishes");

                    b.Navigation("Menu")
                        .IsRequired();

                    b.Navigation("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
