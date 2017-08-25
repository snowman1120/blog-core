﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using BlogCore.BlogContext.Infrastructure;
using BlogCore.BlogContext.Domain;

namespace BlogCore.BlogContext.Migrator.Migrations
{
    [DbContext(typeof(BlogDbContext))]
    partial class BlogDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("BlogCore.BlogContext.Domain.Blog", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("BlogSettingId");

                    b.Property<string>("Description");

                    b.Property<string>("ImageFilePath")
                        .IsRequired();

                    b.Property<string>("OwnerEmail")
                        .IsRequired();

                    b.Property<int>("Status");

                    b.Property<int>("Theme");

                    b.Property<string>("Title")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("BlogSettingId");

                    b.ToTable("Blogs","blog");
                });

            modelBuilder.Entity("BlogCore.BlogContext.Domain.BlogSetting", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("DaysToComment");

                    b.Property<bool>("ModerateComments");

                    b.Property<int>("PostsPerPage");

                    b.HasKey("Id");

                    b.ToTable("BlogSettings","blog");
                });

            modelBuilder.Entity("BlogCore.BlogContext.Domain.Blog", b =>
                {
                    b.HasOne("BlogCore.BlogContext.Domain.BlogSetting", "BlogSetting")
                        .WithMany()
                        .HasForeignKey("BlogSettingId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
