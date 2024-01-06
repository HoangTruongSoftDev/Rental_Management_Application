using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace RentalManagementFinalProject.Models;

public partial class RentalManagementDbContext : DbContext
{
    public RentalManagementDbContext()
    {
    }

    public RentalManagementDbContext(DbContextOptions<RentalManagementDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Apartment> Apartments { get; set; }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<Building> Buildings { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserType> UserTypes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Apartment>(entity =>
        {
            entity.HasKey(e => e.ApartmentNumber);

            entity.Property(e => e.ApartmentNumber).ValueGeneratedNever();
            entity.Property(e => e.Type).HasMaxLength(50);

            entity.HasOne(d => d.Building).WithMany(p => p.Apartments)
                .HasForeignKey(d => d.BuildingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Apartments_Buildings");

            entity.HasOne(d => d.Status).WithMany(p => p.Apartments)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Apartments_Statuses");
        });

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.Property(e => e.AppointmentId).ValueGeneratedNever();
            entity.Property(e => e.MeetingDateTime).HasColumnType("datetime");

            entity.HasOne(d => d.Apartment).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.ApartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Appointments_Apartments");

            entity.HasOne(d => d.PropertyManager).WithMany(p => p.AppointmentPropertyManagers)
                .HasForeignKey(d => d.PropertyManagerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Appointments_Users");

            entity.HasOne(d => d.Tenant).WithMany(p => p.AppointmentTenants)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Appointments_Users1");
        });

        modelBuilder.Entity<Building>(entity =>
        {
            entity.Property(e => e.BuildingId).ValueGeneratedNever();
            entity.Property(e => e.BuildingName).HasMaxLength(50);
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.Property(e => e.MessageId).ValueGeneratedNever();
            entity.Property(e => e.TimeCreated).HasColumnType("datetime");

            entity.HasOne(d => d.Recipient).WithMany(p => p.MessageRecipients)
                .HasForeignKey(d => d.RecipientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Messages_Users");

            entity.HasOne(d => d.Sender).WithMany(p => p.MessageSenders)
                .HasForeignKey(d => d.SenderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Messages_Users1");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.Property(e => e.StatusId).ValueGeneratedNever();
            entity.Property(e => e.Description).HasMaxLength(100);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.UserId).ValueGeneratedNever();
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(50);
            entity.Property(e => e.UserName).HasMaxLength(50);

            entity.HasOne(d => d.Status).WithMany(p => p.Users)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Statuses");

            entity.HasOne(d => d.UserType).WithMany(p => p.Users)
                .HasForeignKey(d => d.UserTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_UserTypes");
        });

        modelBuilder.Entity<UserType>(entity =>
        {
            entity.Property(e => e.UserTypeId).ValueGeneratedNever();
            entity.Property(e => e.Description).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
