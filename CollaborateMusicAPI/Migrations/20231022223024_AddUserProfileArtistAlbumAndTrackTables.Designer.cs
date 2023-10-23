﻿// <auto-generated />
using System;
using CollaborateMusicAPI.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CollaborateMusicAPI.Migrations
{
    [DbContext(typeof(ApplicationDBContext))]
    [Migration("20231022223024_AddUserProfileArtistAlbumAndTrackTables")]
    partial class AddUserProfileArtistAlbumAndTrackTables
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.12")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CollaborateMusicAPI.Models.Entities.Album", b =>
                {
                    b.Property<int>("AlbumID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AlbumID"));

                    b.Property<string>("AlbumName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ArtistID")
                        .HasColumnType("int");

                    b.Property<string>("CoverImagePath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Genre")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ReleaseDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("TotalTracks")
                        .HasColumnType("int");

                    b.HasKey("AlbumID");

                    b.HasIndex("ArtistID");

                    b.ToTable("Albums");
                });

            modelBuilder.Entity("CollaborateMusicAPI.Models.Entities.Artist", b =>
                {
                    b.Property<int>("ArtistID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ArtistID"));

                    b.Property<string>("ArtistName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ArtistPicturePath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Genre")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserProfileID")
                        .HasColumnType("int");

                    b.HasKey("ArtistID");

                    b.HasIndex("UserProfileID");

                    b.ToTable("Artists");
                });

            modelBuilder.Entity("CollaborateMusicAPI.Models.Entities.Track", b =>
                {
                    b.Property<int>("TrackID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TrackID"));

                    b.Property<int>("AlbumID")
                        .HasColumnType("int");

                    b.Property<TimeSpan>("Duration")
                        .HasColumnType("time");

                    b.Property<string>("FilePath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Lyrics")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TrackName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TrackNumber")
                        .HasColumnType("int");

                    b.HasKey("TrackID");

                    b.HasIndex("AlbumID");

                    b.ToTable("Tracks");
                });

            modelBuilder.Entity("CollaborateMusicAPI.Models.Entities.UserProfile", b =>
                {
                    b.Property<int>("UserProfileID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserProfileID"));

                    b.Property<string>("Bio")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Location")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProfilePicturePath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.Property<string>("WebsiteURL")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserProfileID");

                    b.HasIndex("UserID")
                        .IsUnique();

                    b.ToTable("UserProfiles");
                });

            modelBuilder.Entity("CollaborateMusicAPI.Models.Entities.Users", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("OAuthId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OAuthProvider")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("CollaborateMusicAPI.Models.Entities.Album", b =>
                {
                    b.HasOne("CollaborateMusicAPI.Models.Entities.Artist", "Artist")
                        .WithMany()
                        .HasForeignKey("ArtistID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Artist");
                });

            modelBuilder.Entity("CollaborateMusicAPI.Models.Entities.Artist", b =>
                {
                    b.HasOne("CollaborateMusicAPI.Models.Entities.UserProfile", "UserProfile")
                        .WithMany("Artists")
                        .HasForeignKey("UserProfileID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("UserProfile");
                });

            modelBuilder.Entity("CollaborateMusicAPI.Models.Entities.Track", b =>
                {
                    b.HasOne("CollaborateMusicAPI.Models.Entities.Album", "Album")
                        .WithMany()
                        .HasForeignKey("AlbumID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Album");
                });

            modelBuilder.Entity("CollaborateMusicAPI.Models.Entities.UserProfile", b =>
                {
                    b.HasOne("CollaborateMusicAPI.Models.Entities.Users", "User")
                        .WithOne("UserProfile")
                        .HasForeignKey("CollaborateMusicAPI.Models.Entities.UserProfile", "UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("CollaborateMusicAPI.Models.Entities.UserProfile", b =>
                {
                    b.Navigation("Artists");
                });

            modelBuilder.Entity("CollaborateMusicAPI.Models.Entities.Users", b =>
                {
                    b.Navigation("UserProfile")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
