﻿// <auto-generated />
using System;
using MeetupBot;
using MeetupBot.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace MeetupBot.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20241111153326_AddMeetupGroupsAndMeetupEvents")]
    partial class AddMeetupGroupsAndMeetupEvents
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.10");

            modelBuilder.Entity("MeetupBot.Models.MeetupEvent", b =>
                {
                    b.Property<string>("Url")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("MeetupGroupUrl")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Published")
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Url");

                    b.HasIndex("MeetupGroupUrl");

                    b.ToTable("MeetupEvents");
                });

            modelBuilder.Entity("MeetupBot.Models.MeetupGroup", b =>
                {
                    b.Property<string>("Url")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Published")
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Url");

                    b.ToTable("MeetupGroups");
                });

            modelBuilder.Entity("MeetupBot.Models.MeetupEvent", b =>
                {
                    b.HasOne("MeetupBot.Models.MeetupGroup", null)
                        .WithMany("Events")
                        .HasForeignKey("MeetupGroupUrl");
                });

            modelBuilder.Entity("MeetupBot.Models.MeetupGroup", b =>
                {
                    b.Navigation("Events");
                });
#pragma warning restore 612, 618
        }
    }
}
