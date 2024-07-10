using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Jacmazon_ECommerce.Models;

public partial class LoginContext : DbContext
{
    public LoginContext()
    {
    }

    public LoginContext(DbContextOptions<LoginContext> options)
        : base(options)
    {
    }

    public virtual DbSet<DbToken> DbTokens { get; set; }

    public virtual DbSet<DbUser> DbUsers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DbToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Refresh_Tokens");

            entity.ToTable("db_Token");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ExpiredDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<DbUser>(entity =>
        {
            entity.ToTable("db_user");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.UserAccount)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UserEmail).HasMaxLength(50);
            entity.Property(e => e.UserName)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UserPassword)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.UserPhone).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
