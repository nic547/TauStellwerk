﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PiStellwerk.Database;

namespace PiStellwerk.Database.Migrations
{
    [DbContext(typeof(StwDbContext))]
    partial class StwDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.7");

            modelBuilder.Entity("PiStellwerk.Data.DccFunction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("EngineId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<byte>("Number")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("EngineId");

                    b.ToTable("DccFunction");
                });

            modelBuilder.Entity("PiStellwerk.Data.ECoSEngineData", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("ECoSEngineData");
                });

            modelBuilder.Entity("PiStellwerk.Data.Engine", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<ushort>("Address")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("ECoSEngineDataId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("LastUsed")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<byte>("SpeedSteps")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Tags")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("TopSpeed")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ECoSEngineDataId");

                    b.ToTable("Engines");
                });

            modelBuilder.Entity("PiStellwerk.Data.EngineImage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("EngineId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Filename")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsGenerated")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Width")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("EngineId");

                    b.ToTable("EngineImages");
                });

            modelBuilder.Entity("PiStellwerk.Data.DccFunction", b =>
                {
                    b.HasOne("PiStellwerk.Data.Engine", null)
                        .WithMany("Functions")
                        .HasForeignKey("EngineId");
                });

            modelBuilder.Entity("PiStellwerk.Data.Engine", b =>
                {
                    b.HasOne("PiStellwerk.Data.ECoSEngineData", "ECoSEngineData")
                        .WithMany()
                        .HasForeignKey("ECoSEngineDataId");

                    b.Navigation("ECoSEngineData");
                });

            modelBuilder.Entity("PiStellwerk.Data.EngineImage", b =>
                {
                    b.HasOne("PiStellwerk.Data.Engine", null)
                        .WithMany("Image")
                        .HasForeignKey("EngineId");
                });

            modelBuilder.Entity("PiStellwerk.Data.Engine", b =>
                {
                    b.Navigation("Functions");

                    b.Navigation("Image");
                });
#pragma warning restore 612, 618
        }
    }
}
