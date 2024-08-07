﻿// <auto-generated />
using System;
using Birdsoft.SecuIntegrator24.DataAccessObject;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace SecuIntegrator24DAO.Migrations
{
    [DbContext(typeof(TradingDbContext))]
    [Migration("20240803153327_20240803")]
    partial class _20240803
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.7");

            modelBuilder.Entity("Birdsoft.SecuIntegrator24.DataAccessObject.Trading", b =>
                {
                    b.Property<string>("Code")
                        .HasColumnType("TEXT")
                        .HasColumnOrder(0);

                    b.Property<DateTime>("TradingDate")
                        .HasColumnType("TEXT")
                        .HasColumnOrder(1);

                    b.Property<decimal>("Change")
                        .HasColumnType("TEXT");

                    b.Property<char?>("ChangeIndicator")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("ChangeValue")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("ClosingPrice")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("FinalAskPrice")
                        .HasColumnType("TEXT");

                    b.Property<ulong>("FinalAskVolume")
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("FinalBidPrice")
                        .HasColumnType("TEXT");

                    b.Property<ulong>("FinalBidVolume")
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("HighestPrice")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("LowestPrice")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("OpeningPrice")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("PERatio")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("TradeValue")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("TradeVolume")
                        .HasColumnType("TEXT");

                    b.Property<ulong>("Transaction")
                        .HasColumnType("INTEGER");

                    b.HasKey("Code", "TradingDate");

                    b.ToTable("Tradings");
                });
#pragma warning restore 612, 618
        }
    }
}
