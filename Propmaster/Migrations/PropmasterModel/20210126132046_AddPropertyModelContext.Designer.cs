﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Propmaster.Data;

namespace Propmaster.Migrations.PropmasterModel
{
    [DbContext(typeof(PropmasterModelContext))]
    [Migration("20210126132046_AddPropertyModelContext")]
    partial class AddPropertyModelContext
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Propmaster.Models.Property", b =>
                {
                    b.Property<int>("PropertyId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Bathroom")
                        .HasColumnType("int");

                    b.Property<int>("Bedroom")
                        .HasColumnType("int");

                    b.Property<int>("Carpark")
                        .HasColumnType("int");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Furnished")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PicUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(10,2)");

                    b.Property<string>("PropertyLocation")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PropertySize")
                        .HasColumnType("int");

                    b.Property<string>("PropertyStatus")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PropertyType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("PropertyId");

                    b.ToTable("Property");
                });
#pragma warning restore 612, 618
        }
    }
}