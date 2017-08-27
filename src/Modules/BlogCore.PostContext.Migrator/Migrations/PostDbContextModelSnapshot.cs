﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using BlogCore.PostContext.Infrastructure;

namespace BlogCore.PostContext.Migrator.Migrations
{
    [DbContext(typeof(PostDbContext))]
    partial class PostDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("BlogCore.PostContext.Domain.AuthorId", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.HasKey("Id");

                    b.ToTable("AuthorIds","post");
                });

            modelBuilder.Entity("BlogCore.PostContext.Domain.BlogId", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.HasKey("Id");

                    b.ToTable("BlogIds","post");
                });

            modelBuilder.Entity("BlogCore.PostContext.Domain.Comment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("AuthorIdId");

                    b.Property<string>("Body")
                        .IsRequired();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<Guid?>("PostId");

                    b.Property<DateTime>("UpdatedAt");

                    b.HasKey("Id");

                    b.HasIndex("AuthorIdId");

                    b.HasIndex("PostId");

                    b.ToTable("Comments","post");
                });

            modelBuilder.Entity("BlogCore.PostContext.Domain.Post", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("AuthorId");

                    b.Property<Guid>("BlogId");

                    b.Property<string>("Body")
                        .IsRequired();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("Excerpt")
                        .IsRequired();

                    b.Property<string>("Slug")
                        .IsRequired();

                    b.Property<string>("Title")
                        .IsRequired();

                    b.Property<DateTime>("UpdatedAt");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("BlogId");

                    b.ToTable("Posts","post");
                });

            modelBuilder.Entity("BlogCore.PostContext.Domain.Tag", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Frequency");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<Guid?>("PostId");

                    b.HasKey("Id");

                    b.HasIndex("PostId");

                    b.ToTable("Tags","post");
                });

            modelBuilder.Entity("BlogCore.PostContext.Domain.Comment", b =>
                {
                    b.HasOne("BlogCore.PostContext.Domain.AuthorId", "AuthorId")
                        .WithMany()
                        .HasForeignKey("AuthorIdId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("BlogCore.PostContext.Domain.Post")
                        .WithMany("Comments")
                        .HasForeignKey("PostId");
                });

            modelBuilder.Entity("BlogCore.PostContext.Domain.Post", b =>
                {
                    b.HasOne("BlogCore.PostContext.Domain.AuthorId", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("BlogCore.PostContext.Domain.BlogId", "Blog")
                        .WithMany()
                        .HasForeignKey("BlogId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("BlogCore.PostContext.Domain.Tag", b =>
                {
                    b.HasOne("BlogCore.PostContext.Domain.Post")
                        .WithMany("Tags")
                        .HasForeignKey("PostId");
                });
        }
    }
}
