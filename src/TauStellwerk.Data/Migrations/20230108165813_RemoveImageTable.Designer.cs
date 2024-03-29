// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TauStellwerk.Data;
using TauStellwerk.Server.Database;

#nullable disable

namespace TauStellwerk.Server.Database.Migrations
{
    [DbContext(typeof(StwDbContext))]
    [Migration("20230108165813_RemoveImageTable")]
    partial class RemoveImageTable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.1");

            modelBuilder.Entity("TauStellwerk.Server.Database.Model.DccFunction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Duration")
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

            modelBuilder.Entity("TauStellwerk.Server.Database.Model.ECoSEngineData", b =>
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

            modelBuilder.Entity("TauStellwerk.Server.Database.Model.Engine", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<ushort>("Address")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Created")
                        .HasColumnType("TEXT");

                    b.Property<int?>("ECoSEngineDataId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ImageSizes")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsHidden")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("LastImageUpdate")
                        .HasColumnType("TEXT");

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

                    b.HasIndex("Created", "IsHidden");

                    b.HasIndex("LastUsed", "IsHidden");

                    b.HasIndex("Name", "IsHidden");

                    b.ToTable("Engines");
                });

            modelBuilder.Entity("TauStellwerk.Server.Database.Model.Turnout", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Address")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsInverted")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Kind")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Turnouts");
                });

            modelBuilder.Entity("TauStellwerk.Server.Database.Model.DccFunction", b =>
                {
                    b.HasOne("TauStellwerk.Server.Database.Model.Engine", null)
                        .WithMany("Functions")
                        .HasForeignKey("EngineId");
                });

            modelBuilder.Entity("TauStellwerk.Server.Database.Model.Engine", b =>
                {
                    b.HasOne("TauStellwerk.Server.Database.Model.ECoSEngineData", "ECoSEngineData")
                        .WithMany()
                        .HasForeignKey("ECoSEngineDataId");

                    b.Navigation("ECoSEngineData");
                });

            modelBuilder.Entity("TauStellwerk.Server.Database.Model.Engine", b =>
                {
                    b.Navigation("Functions");
                });
#pragma warning restore 612, 618
        }
    }
}
