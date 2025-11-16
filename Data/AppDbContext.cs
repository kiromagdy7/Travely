using Elfie.Serialization;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using Travely.Models;

namespace Travely.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<LkpAmenity> LkpAmenities { get; set; }
    public virtual DbSet<TblBooking> TblBookings { get; set; }
    public virtual DbSet<TblHotel> TblHotels { get; set; }
    public virtual DbSet<TblHotelImage> TblHotelImages { get; set; }
    public virtual DbSet<TblPayment> TblPayments { get; set; }
    public virtual DbSet<TblReview> TblReviews { get; set; }
    public virtual DbSet<TblRoom> TblRooms { get; set; }
    public virtual DbSet<TblRoomImage> TblRoomImages { get; set; }
    public virtual DbSet<TblUser> TblUsers { get; set; }
    public virtual DbSet<TblUserHotelBooking> TblUserHotelBookings { get; set; }
    public virtual DbSet<TblWishList> TblWishLists { get; set; }
    public DbSet<TblNotification> TblNotifications { get; set; }

    // Commented out OnConfiguring as connection string should be in appsettings.json
    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // { ... }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<LkpAmenity>(entity =>
        {
            entity.HasKey(e => e.AmenityId).HasName("PK__lkpAmeni__E908452D58205242");
            entity.ToTable("lkpAmenities");
            entity.HasIndex(e => e.Name, "UQ__lkpAmeni__72E12F1B6FB36A1B").IsUnique();
            entity.Property(e => e.AmenityId).HasColumnName("amenity_id");
            entity.Property(e => e.Name).HasMaxLength(100).HasColumnName("name");
        });

        modelBuilder.Entity<TblBooking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__tblBooki__5DE3A5B18C8ACDC1");
            entity.ToTable("tblBookings");
            entity.HasIndex(e => e.BookingReference, "UQ__tblBooki__BADA455927423170").IsUnique();
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.Adults).HasDefaultValue((int)1).HasColumnName("adults");
            entity.Property(e => e.BookingReference).HasMaxLength(100).HasColumnName("booking_reference");
            entity.Property(e => e.CheckIn).HasColumnName("check_in");
            entity.Property(e => e.CheckOut).HasColumnName("check_out");
            entity.Property(e => e.Children).HasDefaultValue((int)0).HasColumnName("children");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())").HasColumnName("created_at");
            entity.Property(e => e.RoomId).HasColumnName("room_id");
            entity.Property(e => e.Status).HasMaxLength(50).HasDefaultValue("pending").HasColumnName("status");
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(12, 2)").HasColumnName("total_price");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Room).WithMany(p => p.TblBookings)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Booking_Room");

            entity.HasOne(d => d.User).WithMany(p => p.TblBookings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Booking_User");
        });

        modelBuilder.Entity<TblHotel>(entity =>
        {
            entity.HasKey(e => e.HotelId).HasName("PK__tblHotel__45FE7E2639928AE5");
            entity.ToTable("tblHotels");
            entity.HasIndex(e => e.Name, "UQ__tblHotel__72E12F1BED7C40EC").IsUnique();
            entity.Property(e => e.HotelId).HasColumnName("hotel_id");
            entity.Property(e => e.Address).HasMaxLength(500).HasColumnName("address");
            entity.Property(e => e.CancellationPolicy).HasColumnName("cancellation_policy");
            entity.Property(e => e.CheckInTime).HasColumnName("check_in_time");
            entity.Property(e => e.CheckOutTime).HasColumnName("check_out_time");
            entity.Property(e => e.Commission).HasColumnType("decimal(5, 2)").HasColumnName("commission");
            entity.Property(e => e.ContactInfo).HasMaxLength(500).HasColumnName("contact_info");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())").HasColumnName("created_at");
            entity.Property(e => e.Fees).HasMaxLength(500).HasColumnName("fees");
            entity.Property(e => e.Location).HasMaxLength(250).HasColumnName("location");
            entity.Property(e => e.Name).HasMaxLength(250).HasColumnName("name");
            entity.Property(e => e.Phone).HasMaxLength(50).HasColumnName("phone");
            entity.Property(e => e.Stars).HasColumnName("stars");

            entity.HasMany(d => d.Amenities).WithMany(p => p.Hotels)
                .UsingEntity<Dictionary<string, object>>( /* ... Amenity mapping ... */ );
        });

        modelBuilder.Entity<TblHotelImage>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PK__tblHotel__DC9AC95586FE2E59");
            entity.ToTable("tblHotelImages");
            entity.Property(e => e.ImageId).HasColumnName("image_id");
            entity.Property(e => e.HotelId).HasColumnName("hotel_id");
            entity.Property(e => e.ImageUrl).HasMaxLength(500).HasColumnName("image_url");

            entity.HasOne(d => d.Hotel).WithMany(p => p.TblHotelImages)
                .HasForeignKey(d => d.HotelId)
                .HasConstraintName("FK_HotelImage_Hotel");
        });

        modelBuilder.Entity<TblPayment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__tblPayme__ED1FC9EA50A456C9");
            entity.ToTable("tblPayments");
            // ... Payment properties and relationships ...
            entity.Property(e => e.PaymentId).HasColumnName("payment_id");
            entity.Property(e => e.Amount).HasColumnType("decimal(12, 2)").HasColumnName("amount");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.PaymentDate).HasDefaultValueSql("(sysutcdatetime())").HasColumnName("payment_date");
            entity.Property(e => e.PaymentMethod).HasMaxLength(50).HasColumnName("payment_method");
            entity.Property(e => e.PaymentStatus).HasMaxLength(50).HasDefaultValue("pending").HasColumnName("payment_status");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Booking).WithMany(p => p.TblPayments)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Payment_Booking");

            entity.HasOne(d => d.User).WithMany(p => p.TblPayments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Payment_User");
        });

        modelBuilder.Entity<TblReview>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PK__tblRevie__60883D90A84AA0C2");
            entity.ToTable("tblReviews");
            // ... Review properties and relationships ...
            entity.HasIndex(e => e.BookingId, "UQ__tblRevie__5DE3A5B0889ABF13").IsUnique();
            entity.Property(e => e.ReviewId).HasColumnName("review_id");
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.HelpfulCount).HasColumnName("helpful_count");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.ReviewDate).HasDefaultValueSql("(sysutcdatetime())").HasColumnName("review_date");

            entity.HasOne(d => d.Booking).WithOne(p => p.TblReview)
                .HasForeignKey<TblReview>(d => d.BookingId)
                .HasConstraintName("FK_Review_Booking");
        });

        modelBuilder.Entity<TblRoom>(entity =>
        {
            entity.HasKey(e => e.RoomId).HasName("PK__tblRooms__19675A8A9EDAE80E");
            entity.ToTable("tblRooms");
            // ... Room properties ...
            entity.Property(e => e.RoomId).HasColumnName("room_id");
            entity.Property(e => e.BedsCount).HasColumnName("beds_count");
            entity.Property(e => e.BreakfastIncluded).HasColumnName("breakfast_included");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())").HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.HotelId).HasColumnName("hotel_id");
            entity.Property(e => e.MaxGuests).HasColumnName("max_guests");
            entity.Property(e => e.PetsAllowed).HasColumnName("pets_allowed");
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)").HasColumnName("price");
            entity.Property(e => e.RoomNumber).HasMaxLength(50).HasColumnName("room_number");
            entity.Property(e => e.RoomType).HasMaxLength(100).HasColumnName("room_type");
            entity.HasOne(d => d.Hotel).WithMany(p => p.TblRooms)
                .HasForeignKey(d => d.HotelId)
                .HasConstraintName("FK_Room_Hotel");
        });

        modelBuilder.Entity<TblRoomImage>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PK__tblRoomI__DC9AC955108AD689");
            entity.ToTable("tblRoomImages");
            // ... RoomImage properties and relationships ...
            entity.Property(e => e.ImageId).HasColumnName("image_id");
            entity.Property(e => e.ImageUrl).HasMaxLength(500).HasColumnName("image_url");
            entity.Property(e => e.RoomId).HasColumnName("room_id");

            entity.HasOne(d => d.Room).WithMany(p => p.TblRoomImages)
                .HasForeignKey(d => d.RoomId)
                .HasConstraintName("FK_RoomImage_Room");
        });

        modelBuilder.Entity<TblUser>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__tblUsers__B9BE370F4D5024FC");
            entity.ToTable("tblUsers");
            // ... User properties ...
            entity.HasIndex(e => e.Email, "UQ__tblUsers__AB6E616422F1317D").IsUnique();
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Age).HasColumnName("age");
            entity.Property(e => e.Country).HasMaxLength(100).HasColumnName("country");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())").HasColumnName("created_at");
            entity.Property(e => e.Email).HasMaxLength(255).HasColumnName("email");
            entity.Property(e => e.Fullname).HasMaxLength(150).HasColumnName("fullname");
            entity.Property(e => e.Imagepath).HasMaxLength(500).HasColumnName("imagepath");
            entity.Property(e => e.PasswordHash).HasMaxLength(255).HasColumnName("password_hash");
            entity.Property(e => e.Phone).HasMaxLength(50).HasColumnName("phone");
            entity.Property(e => e.Role).HasMaxLength(50).HasDefaultValue("customer").HasColumnName("role");
            entity.Property(e => e.Status).HasMaxLength(30).HasDefaultValue("active").HasColumnName("status");
            entity.Property(e => e.HotelId).HasColumnName("hotel_id");
            

        });

        modelBuilder.Entity<TblUserHotelBooking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__tblUserH__5DE3A5B18D70EFC5");
            entity.ToTable("tblUserHotelBooking");
            // ... UserHotelBooking properties and relationships ...
            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.BookingDate).HasDefaultValueSql("(sysutcdatetime())").HasColumnName("booking_date");
            entity.Property(e => e.HotelId).HasColumnName("hotel_id");
            entity.Property(e => e.Status).HasMaxLength(50).HasDefaultValue("active").HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Hotel).WithMany(p => p.TblUserHotelBookings)
                .HasForeignKey(d => d.HotelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UHB_Hotel");

            entity.HasOne(d => d.User).WithMany(p => p.TblUserHotelBookings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UHB_User");
        });

        // === START: Added/Updated WishList Configuration ===
        modelBuilder.Entity<TblWishList>(entity =>
        {
            entity.HasKey(e => e.WishlistId).HasName("PK__tblWishL__6151514E8CC42492");
            entity.ToTable("tblWishList");

            entity.Property(e => e.WishlistId).HasColumnName("wishlist_id");
            entity.Property(e => e.AddedDate)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("added_date");
            entity.Property(e => e.HotelId).HasColumnName("hotel_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            // Relationship with User
            entity.HasOne(d => d.User)
                .WithMany(p => p.TblWishLists)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_WishList_User");

            // Relationship with Hotel
            entity.HasOne(d => d.Hotel)
                .WithMany()
                .HasForeignKey(d => d.HotelId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_WishList_Hotel");
        });
        // === END: Added/Updated WishList Configuration ===

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
