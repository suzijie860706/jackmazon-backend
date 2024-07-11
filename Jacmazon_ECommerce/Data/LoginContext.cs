using System;
using System.Collections.Generic;
using Jacmazon_ECommerce.Models.LoginContext;
using Microsoft.EntityFrameworkCore;

namespace Jacmazon_ECommerce.Data;

public partial class LoginContext : DbContext
{
    public LoginContext()
    {
    }

    public LoginContext(DbContextOptions<LoginContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Table1> Table1s { get; set; }

    public virtual DbSet<Token> Tokens { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Table1>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Table_1");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Id).HasColumnName("ID");
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

        modelBuilder.Entity<Token>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Refresh_Tokens");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ExpiredDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_db_user");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Account)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(256)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
