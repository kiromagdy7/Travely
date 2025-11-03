USE [master]
GO
/****** Object:  Database [TravelyDB]    Script Date: 11/2/2025 3:24:26 AM ******/
CREATE DATABASE [TravelyDB]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'HotelBookingDB', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.DEPI1\MSSQL\DATA\HotelBookingDB.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'HotelBookingDB_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.DEPI1\MSSQL\DATA\HotelBookingDB_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [TravelyDB] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [TravelyDB].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [TravelyDB] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [TravelyDB] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [TravelyDB] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [TravelyDB] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [TravelyDB] SET ARITHABORT OFF 
GO
ALTER DATABASE [TravelyDB] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [TravelyDB] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [TravelyDB] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [TravelyDB] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [TravelyDB] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [TravelyDB] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [TravelyDB] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [TravelyDB] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [TravelyDB] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [TravelyDB] SET  ENABLE_BROKER 
GO
ALTER DATABASE [TravelyDB] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [TravelyDB] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [TravelyDB] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [TravelyDB] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [TravelyDB] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [TravelyDB] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [TravelyDB] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [TravelyDB] SET RECOVERY FULL 
GO
ALTER DATABASE [TravelyDB] SET  MULTI_USER 
GO
ALTER DATABASE [TravelyDB] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [TravelyDB] SET DB_CHAINING OFF 
GO
ALTER DATABASE [TravelyDB] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [TravelyDB] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [TravelyDB] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [TravelyDB] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'TravelyDB', N'ON'
GO
ALTER DATABASE [TravelyDB] SET QUERY_STORE = ON
GO
ALTER DATABASE [TravelyDB] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [TravelyDB]
GO
/****** Object:  Table [dbo].[lkpAmenities]    Script Date: 11/2/2025 3:24:26 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[lkpAmenities](
	[amenity_id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[amenity_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblBookings]    Script Date: 11/2/2025 3:24:26 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblBookings](
	[booking_id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NOT NULL,
	[room_id] [int] NOT NULL,
	[check_in] [date] NULL,
	[check_out] [date] NULL,
	[status] [nvarchar](50) NOT NULL,
	[total_price] [decimal](12, 2) NOT NULL,
	[booking_reference] [nvarchar](100) NOT NULL,
	[adults] [tinyint] NOT NULL,
	[children] [tinyint] NULL,
	[created_at] [datetime2](7) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[booking_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblHotelAmenities]    Script Date: 11/2/2025 3:24:26 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblHotelAmenities](
	[hotel_id] [int] NOT NULL,
	[amenity_id] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[hotel_id] ASC,
	[amenity_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblHotelImages]    Script Date: 11/2/2025 3:24:26 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblHotelImages](
	[image_id] [int] IDENTITY(1,1) NOT NULL,
	[hotel_id] [int] NOT NULL,
	[image_url] [nvarchar](500) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[image_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblHotels]    Script Date: 11/2/2025 3:24:26 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblHotels](
	[hotel_id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](250) NOT NULL,
	[stars] [tinyint] NULL,
	[contact_info] [nvarchar](500) NULL,
	[location] [nvarchar](250) NULL,
	[address] [nvarchar](500) NULL,
	[phone] [nvarchar](50) NULL,
	[check_in_time] [time](7) NULL,
	[check_out_time] [time](7) NULL,
	[cancellation_policy] [nvarchar](max) NULL,
	[fees] [nvarchar](500) NULL,
	[commission] [decimal](5, 2) NULL,
	[created_at] [datetime2](7) NOT NULL,
	[description] [nvarchar](max) NOT NULL,
	[Overview] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[hotel_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblNotifications]    Script Date: 11/2/2025 3:24:26 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblNotifications](
	[notification_id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NOT NULL,
	[title] [nvarchar](200) NOT NULL,
	[message] [nvarchar](max) NOT NULL,
	[notification_type] [nvarchar](50) NULL,
	[is_read] [bit] NULL,
	[created_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[notification_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblPayments]    Script Date: 11/2/2025 3:24:26 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblPayments](
	[payment_id] [int] IDENTITY(1,1) NOT NULL,
	[booking_id] [int] NOT NULL,
	[user_id] [int] NOT NULL,
	[payment_method] [nvarchar](50) NOT NULL,
	[payment_status] [nvarchar](50) NOT NULL,
	[amount] [decimal](12, 2) NOT NULL,
	[payment_date] [datetime2](7) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[payment_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblReviews]    Script Date: 11/2/2025 3:24:26 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblReviews](
	[review_id] [int] IDENTITY(1,1) NOT NULL,
	[booking_id] [int] NOT NULL,
	[rating] [tinyint] NULL,
	[comment] [nvarchar](max) NULL,
	[review_date] [datetime2](7) NOT NULL,
	[helpful_count] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[review_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblRoomImages]    Script Date: 11/2/2025 3:24:26 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblRoomImages](
	[image_id] [int] IDENTITY(1,1) NOT NULL,
	[room_id] [int] NOT NULL,
	[image_url] [nvarchar](500) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[image_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblRooms]    Script Date: 11/2/2025 3:24:26 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblRooms](
	[room_id] [int] IDENTITY(1,1) NOT NULL,
	[hotel_id] [int] NOT NULL,
	[room_number] [nvarchar](50) NULL,
	[room_type] [nvarchar](100) NULL,
	[beds_count] [tinyint] NULL,
	[price] [decimal](10, 2) NOT NULL,
	[max_guests] [tinyint] NULL,
	[description] [nvarchar](max) NULL,
	[breakfast_included] [bit] NOT NULL,
	[pets_allowed] [bit] NOT NULL,
	[created_at] [datetime2](7) NOT NULL,
	[available] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[room_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblUserHotelBooking]    Script Date: 11/2/2025 3:24:26 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblUserHotelBooking](
	[booking_id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NOT NULL,
	[hotel_id] [int] NOT NULL,
	[booking_date] [datetime2](7) NOT NULL,
	[status] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[booking_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblUsers]    Script Date: 11/2/2025 3:24:26 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblUsers](
	[user_id] [int] IDENTITY(1,1) NOT NULL,
	[fullname] [nvarchar](150) NOT NULL,
	[email] [nvarchar](255) NOT NULL,
	[password_hash] [nvarchar](255) NOT NULL,
	[phone] [nvarchar](50) NULL,
	[age] [tinyint] NULL,
	[role] [nvarchar](50) NOT NULL,
	[imagepath] [nvarchar](500) NULL,
	[country] [nvarchar](100) NULL,
	[status] [nvarchar](30) NOT NULL,
	[created_at] [datetime2](7) NOT NULL,
	[hotel_id] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[user_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblWishList]    Script Date: 11/2/2025 3:24:26 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblWishList](
	[wishlist_id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NOT NULL,
	[hotel_id] [int] NOT NULL,
	[added_date] [datetime2](7) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[wishlist_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[lkpAmenities] ON 
GO
INSERT [dbo].[lkpAmenities] ([amenity_id], [name]) VALUES (9, N'24/7 Reception')
GO
INSERT [dbo].[lkpAmenities] ([amenity_id], [name]) VALUES (4, N'Airport Shuttle')
GO
INSERT [dbo].[lkpAmenities] ([amenity_id], [name]) VALUES (2, N'Breakfast Included')
GO
INSERT [dbo].[lkpAmenities] ([amenity_id], [name]) VALUES (6, N'Fitness Center')
GO
INSERT [dbo].[lkpAmenities] ([amenity_id], [name]) VALUES (1, N'Free Wi-Fi')
GO
INSERT [dbo].[lkpAmenities] ([amenity_id], [name]) VALUES (7, N'Parking')
GO
INSERT [dbo].[lkpAmenities] ([amenity_id], [name]) VALUES (8, N'Pet Friendly')
GO
INSERT [dbo].[lkpAmenities] ([amenity_id], [name]) VALUES (10, N'Room Service')
GO
INSERT [dbo].[lkpAmenities] ([amenity_id], [name]) VALUES (5, N'Spa')
GO
INSERT [dbo].[lkpAmenities] ([amenity_id], [name]) VALUES (3, N'Swimming Pool')
GO
SET IDENTITY_INSERT [dbo].[lkpAmenities] OFF
GO
SET IDENTITY_INSERT [dbo].[tblBookings] ON 
GO
INSERT [dbo].[tblBookings] ([booking_id], [user_id], [room_id], [check_in], [check_out], [status], [total_price], [booking_reference], [adults], [children], [created_at]) VALUES (1, 1, 1, CAST(N'2025-10-25' AS Date), CAST(N'2025-10-27' AS Date), N'confirmed', CAST(1800.00 AS Decimal(12, 2)), N'REF-GP-0001', 1, 0, CAST(N'2025-10-23T17:02:34.7668179' AS DateTime2))
GO
INSERT [dbo].[tblBookings] ([booking_id], [user_id], [room_id], [check_in], [check_out], [status], [total_price], [booking_reference], [adults], [children], [created_at]) VALUES (2, 2, 4, CAST(N'2025-11-01' AS Date), CAST(N'2025-11-05' AS Date), N'pending', CAST(5600.00 AS Decimal(12, 2)), N'REF-BB-0002', 2, 1, CAST(N'2025-10-23T17:02:34.7668179' AS DateTime2))
GO
INSERT [dbo].[tblBookings] ([booking_id], [user_id], [room_id], [check_in], [check_out], [status], [total_price], [booking_reference], [adults], [children], [created_at]) VALUES (3, 1, 8, CAST(N'2025-12-10' AS Date), CAST(N'2025-12-13' AS Date), N'completed', CAST(6900.00 AS Decimal(12, 2)), N'REF-MV-0003', 2, 0, CAST(N'2025-10-23T17:02:34.7668179' AS DateTime2))
GO
INSERT [dbo].[tblBookings] ([booking_id], [user_id], [room_id], [check_in], [check_out], [status], [total_price], [booking_reference], [adults], [children], [created_at]) VALUES (4, 2, 10, CAST(N'2025-09-01' AS Date), CAST(N'2025-09-05' AS Date), N'canceled', CAST(8000.00 AS Decimal(12, 2)), N'REF-DS-0004', 2, 2, CAST(N'2025-10-23T17:02:34.7668179' AS DateTime2))
GO
INSERT [dbo].[tblBookings] ([booking_id], [user_id], [room_id], [check_in], [check_out], [status], [total_price], [booking_reference], [adults], [children], [created_at]) VALUES (5, 1, 12, CAST(N'2025-10-28' AS Date), CAST(N'2025-10-30' AS Date), N'confirmed', CAST(4800.00 AS Decimal(12, 2)), N'REF-CL-0005', 2, 0, CAST(N'2025-10-23T17:02:34.7668179' AS DateTime2))
GO
SET IDENTITY_INSERT [dbo].[tblBookings] OFF
GO
INSERT [dbo].[tblHotelAmenities] ([hotel_id], [amenity_id]) VALUES (1, 1)
GO
INSERT [dbo].[tblHotelAmenities] ([hotel_id], [amenity_id]) VALUES (1, 2)
GO
INSERT [dbo].[tblHotelAmenities] ([hotel_id], [amenity_id]) VALUES (1, 3)
GO
INSERT [dbo].[tblHotelAmenities] ([hotel_id], [amenity_id]) VALUES (1, 10)
GO
INSERT [dbo].[tblHotelAmenities] ([hotel_id], [amenity_id]) VALUES (2, 1)
GO
INSERT [dbo].[tblHotelAmenities] ([hotel_id], [amenity_id]) VALUES (2, 3)
GO
INSERT [dbo].[tblHotelAmenities] ([hotel_id], [amenity_id]) VALUES (2, 5)
GO
INSERT [dbo].[tblHotelAmenities] ([hotel_id], [amenity_id]) VALUES (2, 7)
GO
INSERT [dbo].[tblHotelAmenities] ([hotel_id], [amenity_id]) VALUES (3, 1)
GO
INSERT [dbo].[tblHotelAmenities] ([hotel_id], [amenity_id]) VALUES (3, 2)
GO
INSERT [dbo].[tblHotelAmenities] ([hotel_id], [amenity_id]) VALUES (3, 6)
GO
INSERT [dbo].[tblHotelAmenities] ([hotel_id], [amenity_id]) VALUES (4, 1)
GO
INSERT [dbo].[tblHotelAmenities] ([hotel_id], [amenity_id]) VALUES (4, 8)
GO
INSERT [dbo].[tblHotelAmenities] ([hotel_id], [amenity_id]) VALUES (5, 1)
GO
INSERT [dbo].[tblHotelAmenities] ([hotel_id], [amenity_id]) VALUES (5, 2)
GO
INSERT [dbo].[tblHotelAmenities] ([hotel_id], [amenity_id]) VALUES (5, 9)
GO
SET IDENTITY_INSERT [dbo].[tblHotelImages] ON 
GO
INSERT [dbo].[tblHotelImages] ([image_id], [hotel_id], [image_url]) VALUES (1, 1, N'/images/Hotels/1_20251101204731513.jpg')
GO
INSERT [dbo].[tblHotelImages] ([image_id], [hotel_id], [image_url]) VALUES (2, 2, N'/images/Hotels/2_20251101204740186.jpg')
GO
INSERT [dbo].[tblHotelImages] ([image_id], [hotel_id], [image_url]) VALUES (3, 3, N'/images/Hotels/3_20251101204745982.jpg')
GO
INSERT [dbo].[tblHotelImages] ([image_id], [hotel_id], [image_url]) VALUES (4, 4, N'/images/Hotels/4_20251101204801707.jpg')
GO
INSERT [dbo].[tblHotelImages] ([image_id], [hotel_id], [image_url]) VALUES (5, 5, N'/images/Hotels/5_20251101204807555.jpg')
GO
SET IDENTITY_INSERT [dbo].[tblHotelImages] OFF
GO
SET IDENTITY_INSERT [dbo].[tblHotels] ON 
GO
INSERT [dbo].[tblHotels] ([hotel_id], [name], [stars], [contact_info], [location], [address], [phone], [check_in_time], [check_out_time], [cancellation_policy], [fees], [commission], [created_at], [description], [Overview]) VALUES (1, N'Grand Palace Hotel', 5, N'contact@grandpalace.com', N'Cairo', N'123 Nile Street', N'+201000000001', CAST(N'14:00:00' AS Time), CAST(N'12:00:00' AS Time), N'Free cancellation up to 48 hours before arrival.', N'Service fee applies', CAST(10.00 AS Decimal(5, 2)), CAST(N'2025-10-23T17:02:34.7392596' AS DateTime2), N'Grand Palace Hotel is a luxurious 5-star property located in the heart of Cairo. The hotel offers elegant rooms with modern amenities, a rooftop pool overlooking the Nile, and fine dining restaurants serving both local and international cuisine. Perfect for travelers seeking comfort and sophistication.', N'This luxurious hotel offers a perfect blend of modern elegance and comfort. Guests can enjoy world-class facilities, including a rooftop infinity pool, fine dining restaurants, and a state-of-the-art fitness center. Located in the heart of the city, it provides easy access to major landmarks and shopping centers, making it ideal for both business and leisure travelers.')
GO
INSERT [dbo].[tblHotels] ([hotel_id], [name], [stars], [contact_info], [location], [address], [phone], [check_in_time], [check_out_time], [cancellation_policy], [fees], [commission], [created_at], [description], [Overview]) VALUES (2, N'Blue Bay Resort', 5, N'info@bluebayresort.com', N'Hurghada', N'Marina Road 7', N'+201000000002', CAST(N'15:00:00' AS Time), CAST(N'11:00:00' AS Time), N'Non-refundable for last-minute bookings.', N'Resort fee per night', CAST(12.50 AS Decimal(5, 2)), CAST(N'2025-10-23T17:02:34.7392596' AS DateTime2), N'Blue Bay Resort is a stunning beachfront destination in Hurghada, ideal for families and couples. The resort features spacious rooms with sea views, multiple swimming pools, a private sandy beach, and a variety of water sports and entertainment activities throughout the day.', N'Situated just a few steps from the coastline, this boutique-style hotel captures the charm of seaside living. The interior is designed with natural tones, wooden textures, and large windows that offer breathtaking views of the ocean. Guests can relax at the spa, savor fresh seafood, or simply enjoy the sunset from their private balcony.')
GO
INSERT [dbo].[tblHotels] ([hotel_id], [name], [stars], [contact_info], [location], [address], [phone], [check_in_time], [check_out_time], [cancellation_policy], [fees], [commission], [created_at], [description], [Overview]) VALUES (3, N'Mountain View Lodge', 4, N'stay@mountainview.com', N'Alexandria', N'Green Hill Road 3', N'+201000000003', CAST(N'13:00:00' AS Time), CAST(N'12:00:00' AS Time), N'Cancel 7 days before arrival for full refund.', NULL, CAST(8.00 AS Decimal(5, 2)), CAST(N'2025-10-23T17:02:34.7392596' AS DateTime2), N'Mountain View Hotel in Alexandria offers a peaceful retreat surrounded by greenery and fresh air. Guests can enjoy stylish rooms, an on-site spa, and panoramic views from the rooftop lounge. It is the perfect getaway for those who value relaxation and comfort.', N'Hidden among lush gardens and scenic landscapes, this tranquil hotel is designed for travelers seeking peace and privacy. It features spacious suites, open-air lounges, and personalized services. Whether you''re here for a romantic getaway or a family vacation, this resort ensures a memorable and refreshing stay.')
GO
INSERT [dbo].[tblHotels] ([hotel_id], [name], [stars], [contact_info], [location], [address], [phone], [check_in_time], [check_out_time], [cancellation_policy], [fees], [commission], [created_at], [description], [Overview]) VALUES (4, N'Desert Sun Inn', 4, N'hello@desertsun.com', N'Siwa', N'Oasis Road 45', N'+201000000004', CAST(N'14:00:00' AS Time), CAST(N'12:00:00' AS Time), N'No refund on non-peak season cancellations.', NULL, CAST(7.50 AS Decimal(5, 2)), CAST(N'2025-10-23T17:02:34.7392596' AS DateTime2), N'Desert Sun Inn is a charming eco-friendly lodge located in the heart of Siwa Oasis. Built with traditional materials, the inn provides guests with a genuine desert experience, complete with organic meals, camel rides, and breathtaking sunset views.', N'Located in the bustling city center, this modern property combines convenience and comfort. Business travelers will appreciate its conference halls and coworking spaces, while tourists can explore nearby attractions and nightlife. The hotel prides itself on offering exceptional service and a welcoming atmosphere.')
GO
INSERT [dbo].[tblHotels] ([hotel_id], [name], [stars], [contact_info], [location], [address], [phone], [check_in_time], [check_out_time], [cancellation_policy], [fees], [commission], [created_at], [description], [Overview]) VALUES (5, N'City Lights Hotel', 4, N'book@citylights.com', N'Giza', N'Pyramids Avenue 10', N'+201000000005', CAST(N'15:00:00' AS Time), CAST(N'11:00:00' AS Time), N'Free cancellation up to 24 hours.', N'Tourist tax may apply', CAST(9.00 AS Decimal(5, 2)), CAST(N'2025-10-23T17:02:34.7392596' AS DateTime2), N'City Lights Hotel is a modern 4-star hotel situated in Giza, just minutes away from the Pyramids. It offers comfortable rooms, a rooftop restaurant with panoramic city views, and excellent service designed to make every stay memorable for both tourists and business travelers.', N'An elegant beachfront resort offering luxury suites with panoramic sea views. The property includes multiple pools, an award-winning restaurant, and exclusive private cabanas. Perfect for honeymooners and vacationers who want to experience the best of coastal relaxation and hospitality.')
GO
SET IDENTITY_INSERT [dbo].[tblHotels] OFF
GO
SET IDENTITY_INSERT [dbo].[tblPayments] ON 
GO
INSERT [dbo].[tblPayments] ([payment_id], [booking_id], [user_id], [payment_method], [payment_status], [amount], [payment_date]) VALUES (1, 1, 1, N'Credit Card', N'paid', CAST(1800.00 AS Decimal(12, 2)), CAST(N'2025-10-23T09:30:00.0000000' AS DateTime2))
GO
INSERT [dbo].[tblPayments] ([payment_id], [booking_id], [user_id], [payment_method], [payment_status], [amount], [payment_date]) VALUES (2, 2, 2, N'PayPal', N'pending', CAST(5600.00 AS Decimal(12, 2)), CAST(N'2025-10-23T10:00:00.0000000' AS DateTime2))
GO
INSERT [dbo].[tblPayments] ([payment_id], [booking_id], [user_id], [payment_method], [payment_status], [amount], [payment_date]) VALUES (3, 3, 1, N'Credit Card', N'paid', CAST(6900.00 AS Decimal(12, 2)), CAST(N'2025-12-05T14:20:00.0000000' AS DateTime2))
GO
INSERT [dbo].[tblPayments] ([payment_id], [booking_id], [user_id], [payment_method], [payment_status], [amount], [payment_date]) VALUES (4, 4, 2, N'Cash', N'failed', CAST(8000.00 AS Decimal(12, 2)), CAST(N'2025-09-02T08:00:00.0000000' AS DateTime2))
GO
INSERT [dbo].[tblPayments] ([payment_id], [booking_id], [user_id], [payment_method], [payment_status], [amount], [payment_date]) VALUES (5, 5, 1, N'Debit Card', N'paid', CAST(4800.00 AS Decimal(12, 2)), CAST(N'2025-10-28T18:45:00.0000000' AS DateTime2))
GO
SET IDENTITY_INSERT [dbo].[tblPayments] OFF
GO
SET IDENTITY_INSERT [dbo].[tblReviews] ON 
GO
INSERT [dbo].[tblReviews] ([review_id], [booking_id], [rating], [comment], [review_date], [helpful_count]) VALUES (1, 1, 5, N'Excellent stay at Grand Palace. Staff were very professional.', CAST(N'2025-10-29T00:00:00.0000000' AS DateTime2), 12)
GO
INSERT [dbo].[tblReviews] ([review_id], [booking_id], [rating], [comment], [review_date], [helpful_count]) VALUES (2, 2, 4, N'Blue Bay was lovely; beach is beautiful but food needs improvement.', CAST(N'2025-11-10T00:00:00.0000000' AS DateTime2), 3)
GO
INSERT [dbo].[tblReviews] ([review_id], [booking_id], [rating], [comment], [review_date], [helpful_count]) VALUES (3, 3, 5, N'City Lights: perfect location and friendly staff.', CAST(N'2025-12-05T00:00:00.0000000' AS DateTime2), 7)
GO
SET IDENTITY_INSERT [dbo].[tblReviews] OFF
GO
SET IDENTITY_INSERT [dbo].[tblRoomImages] ON 
GO
INSERT [dbo].[tblRoomImages] ([image_id], [room_id], [image_url]) VALUES (1, 1, N'/images/rooms/room_1_1.jpg')
GO
INSERT [dbo].[tblRoomImages] ([image_id], [room_id], [image_url]) VALUES (2, 2, N'/images/rooms/room_1_2.jpg')
GO
INSERT [dbo].[tblRoomImages] ([image_id], [room_id], [image_url]) VALUES (3, 3, N'/images/rooms/room_1_3.jpg')
GO
INSERT [dbo].[tblRoomImages] ([image_id], [room_id], [image_url]) VALUES (4, 4, N'/images/rooms/room_2_1.jpg')
GO
INSERT [dbo].[tblRoomImages] ([image_id], [room_id], [image_url]) VALUES (5, 5, N'/images/rooms/room_2_2.jpg')
GO
INSERT [dbo].[tblRoomImages] ([image_id], [room_id], [image_url]) VALUES (6, 6, N'/images/rooms/room_2_3.jpg')
GO
INSERT [dbo].[tblRoomImages] ([image_id], [room_id], [image_url]) VALUES (7, 7, N'/images/rooms/room_3_1.jpg')
GO
INSERT [dbo].[tblRoomImages] ([image_id], [room_id], [image_url]) VALUES (8, 8, N'/images/rooms/room_3_2.jpg')
GO
INSERT [dbo].[tblRoomImages] ([image_id], [room_id], [image_url]) VALUES (9, 9, N'/images/rooms/room_4_1.jpg')
GO
INSERT [dbo].[tblRoomImages] ([image_id], [room_id], [image_url]) VALUES (10, 10, N'/images/rooms/room_4_2.jpg')
GO
INSERT [dbo].[tblRoomImages] ([image_id], [room_id], [image_url]) VALUES (11, 11, N'/images/rooms/room_5_1.jpg')
GO
INSERT [dbo].[tblRoomImages] ([image_id], [room_id], [image_url]) VALUES (12, 12, N'/images/rooms/room_5_2.jpg')
GO
INSERT [dbo].[tblRoomImages] ([image_id], [room_id], [image_url]) VALUES (13, 13, N'/images/rooms/room_5_3.jpg')
GO
SET IDENTITY_INSERT [dbo].[tblRoomImages] OFF
GO
SET IDENTITY_INSERT [dbo].[tblRooms] ON 
GO
INSERT [dbo].[tblRooms] ([room_id], [hotel_id], [room_number], [room_type], [beds_count], [price], [max_guests], [description], [breakfast_included], [pets_allowed], [created_at], [available]) VALUES (1, 1, N'101', N'Single', 1, CAST(900.00 AS Decimal(10, 2)), 1, N'Cozy single room with city view.', 0, 0, CAST(N'2025-10-23T17:02:34.7498112' AS DateTime2), 1)
GO
INSERT [dbo].[tblRooms] ([room_id], [hotel_id], [room_number], [room_type], [beds_count], [price], [max_guests], [description], [breakfast_included], [pets_allowed], [created_at], [available]) VALUES (2, 1, N'102', N'Double', 1, CAST(1300.00 AS Decimal(10, 2)), 2, N'Comfort double room with Nile view.', 1, 0, CAST(N'2025-10-23T17:02:34.7498112' AS DateTime2), 1)
GO
INSERT [dbo].[tblRooms] ([room_id], [hotel_id], [room_number], [room_type], [beds_count], [price], [max_guests], [description], [breakfast_included], [pets_allowed], [created_at], [available]) VALUES (3, 1, N'201', N'Suite', 2, CAST(2700.00 AS Decimal(10, 2)), 3, N'Executive suite with lounge area.', 1, 0, CAST(N'2025-10-23T17:02:34.7498112' AS DateTime2), 1)
GO
INSERT [dbo].[tblRooms] ([room_id], [hotel_id], [room_number], [room_type], [beds_count], [price], [max_guests], [description], [breakfast_included], [pets_allowed], [created_at], [available]) VALUES (4, 2, N'301', N'Double', 1, CAST(1400.00 AS Decimal(10, 2)), 2, N'Sea-facing double room.', 1, 0, CAST(N'2025-10-23T17:02:34.7498112' AS DateTime2), 1)
GO
INSERT [dbo].[tblRooms] ([room_id], [hotel_id], [room_number], [room_type], [beds_count], [price], [max_guests], [description], [breakfast_included], [pets_allowed], [created_at], [available]) VALUES (5, 2, N'302', N'Deluxe', 2, CAST(2100.00 AS Decimal(10, 2)), 2, N'Deluxe room with balcony.', 1, 0, CAST(N'2025-10-23T17:02:34.7498112' AS DateTime2), 1)
GO
INSERT [dbo].[tblRooms] ([room_id], [hotel_id], [room_number], [room_type], [beds_count], [price], [max_guests], [description], [breakfast_included], [pets_allowed], [created_at], [available]) VALUES (6, 2, N'303', N'Family', 3, CAST(2500.00 AS Decimal(10, 2)), 4, N'Spacious family room.', 1, 1, CAST(N'2025-10-23T17:02:34.7498112' AS DateTime2), 1)
GO
INSERT [dbo].[tblRooms] ([room_id], [hotel_id], [room_number], [room_type], [beds_count], [price], [max_guests], [description], [breakfast_included], [pets_allowed], [created_at], [available]) VALUES (7, 3, N'401', N'Single', 2, CAST(850.00 AS Decimal(10, 2)), 2, N'Small single room near trails.', 1, 1, CAST(N'2025-10-23T17:02:34.0000000' AS DateTime2), 1)
GO
INSERT [dbo].[tblRooms] ([room_id], [hotel_id], [room_number], [room_type], [beds_count], [price], [max_guests], [description], [breakfast_included], [pets_allowed], [created_at], [available]) VALUES (8, 3, N'402', N'Suite', 4, CAST(2300.00 AS Decimal(10, 2)), 4, N'Mountain view suite.', 1, 1, CAST(N'2025-10-23T17:02:34.0000000' AS DateTime2), 1)
GO
INSERT [dbo].[tblRooms] ([room_id], [hotel_id], [room_number], [room_type], [beds_count], [price], [max_guests], [description], [breakfast_included], [pets_allowed], [created_at], [available]) VALUES (9, 4, N'501', N'Double', 1, CAST(1200.00 AS Decimal(10, 2)), 2, N'Traditional desert-style double room.', 0, 0, CAST(N'2025-10-23T17:02:34.7498112' AS DateTime2), 1)
GO
INSERT [dbo].[tblRooms] ([room_id], [hotel_id], [room_number], [room_type], [beds_count], [price], [max_guests], [description], [breakfast_included], [pets_allowed], [created_at], [available]) VALUES (10, 4, N'502', N'Deluxe', 2, CAST(2000.00 AS Decimal(10, 2)), 2, N'Deluxe tent-style accommodation.', 1, 0, CAST(N'2025-10-23T17:02:34.7498112' AS DateTime2), 1)
GO
INSERT [dbo].[tblRooms] ([room_id], [hotel_id], [room_number], [room_type], [beds_count], [price], [max_guests], [description], [breakfast_included], [pets_allowed], [created_at], [available]) VALUES (11, 5, N'601', N'Single', 1, CAST(950.00 AS Decimal(10, 2)), 1, N'City single with modern design.', 0, 0, CAST(N'2025-10-23T17:02:34.7498112' AS DateTime2), 1)
GO
INSERT [dbo].[tblRooms] ([room_id], [hotel_id], [room_number], [room_type], [beds_count], [price], [max_guests], [description], [breakfast_included], [pets_allowed], [created_at], [available]) VALUES (12, 5, N'602', N'Suite', 2, CAST(2400.00 AS Decimal(10, 2)), 3, N'Penthouse suite with skyline view.', 1, 0, CAST(N'2025-10-23T17:02:34.7498112' AS DateTime2), 1)
GO
INSERT [dbo].[tblRooms] ([room_id], [hotel_id], [room_number], [room_type], [beds_count], [price], [max_guests], [description], [breakfast_included], [pets_allowed], [created_at], [available]) VALUES (13, 5, N'603', N'Family', 3, CAST(2600.00 AS Decimal(10, 2)), 4, N'Family room with extra beds.', 1, 1, CAST(N'2025-10-23T17:02:34.7498112' AS DateTime2), 1)
GO
INSERT [dbo].[tblRooms] ([room_id], [hotel_id], [room_number], [room_type], [beds_count], [price], [max_guests], [description], [breakfast_included], [pets_allowed], [created_at], [available]) VALUES (15, 1, N'311', N'Single', 11, CAST(1111.00 AS Decimal(10, 2)), 11, N'test room 2', 1, 1, CAST(N'2025-10-29T01:37:09.3258844' AS DateTime2), 1)
GO
SET IDENTITY_INSERT [dbo].[tblRooms] OFF
GO
SET IDENTITY_INSERT [dbo].[tblUserHotelBooking] ON 
GO
INSERT [dbo].[tblUserHotelBooking] ([booking_id], [user_id], [hotel_id], [booking_date], [status]) VALUES (1, 1, 1, CAST(N'2025-10-20T10:00:00.0000000' AS DateTime2), N'confirmed')
GO
INSERT [dbo].[tblUserHotelBooking] ([booking_id], [user_id], [hotel_id], [booking_date], [status]) VALUES (2, 1, 2, CAST(N'2025-11-05T12:30:00.0000000' AS DateTime2), N'pending')
GO
INSERT [dbo].[tblUserHotelBooking] ([booking_id], [user_id], [hotel_id], [booking_date], [status]) VALUES (3, 2, 5, CAST(N'2025-12-01T09:00:00.0000000' AS DateTime2), N'confirmed')
GO
INSERT [dbo].[tblUserHotelBooking] ([booking_id], [user_id], [hotel_id], [booking_date], [status]) VALUES (4, 3, 4, CAST(N'2025-10-15T14:00:00.0000000' AS DateTime2), N'active')
GO
SET IDENTITY_INSERT [dbo].[tblUserHotelBooking] OFF
GO
SET IDENTITY_INSERT [dbo].[tblUsers] ON 
GO
INSERT [dbo].[tblUsers] ([user_id], [fullname], [email], [password_hash], [phone], [age], [role], [imagepath], [country], [status], [created_at], [hotel_id]) VALUES (1, N'Samir Mohamed', N'samir.mohamed@example.com', N'hash_samir', N'01000000001', 30, N'customer', N'/images/users/john.jpg', N'Egypt', N'Deleted', CAST(N'2025-10-23T17:02:34.7340000' AS DateTime2), NULL)
GO
INSERT [dbo].[tblUsers] ([user_id], [fullname], [email], [password_hash], [phone], [age], [role], [imagepath], [country], [status], [created_at], [hotel_id]) VALUES (2, N'Menna Khaled', N'menna.khaled@example.com', N'hash_menna', N'01000000002', 28, N'customer', N'/images/users/sara.jpg', N'UK', N'Deleted', CAST(N'2025-10-23T17:02:34.7340000' AS DateTime2), NULL)
GO
INSERT [dbo].[tblUsers] ([user_id], [fullname], [email], [password_hash], [phone], [age], [role], [imagepath], [country], [status], [created_at], [hotel_id]) VALUES (3, N'Ahmed Ali', N'ahmed.ali@example.com', N'hash_ahmed', N'01000000003', 35, N'hotelowner', N'/images/users/ahmed.jpg', N'Egypt', N'Deleted', CAST(N'2025-10-23T17:02:34.7349893' AS DateTime2), NULL)
GO
INSERT [dbo].[tblUsers] ([user_id], [fullname], [email], [password_hash], [phone], [age], [role], [imagepath], [country], [status], [created_at], [hotel_id]) VALUES (4, N'Mona Hassan', N'mona.hassan@example.com', N'hash_mona', N'01000000004', 40, N'staff', N'/images/users/mona.jpg', N'Egypt', N'Deleted', CAST(N'2025-10-23T17:02:34.7349893' AS DateTime2), NULL)
GO
INSERT [dbo].[tblUsers] ([user_id], [fullname], [email], [password_hash], [phone], [age], [role], [imagepath], [country], [status], [created_at], [hotel_id]) VALUES (5, N'Site Admin', N'admin@example.com', N'hash_admin', N'01000000005', 45, N'admin', N'/images/users/admin.jpg', N'USA', N'Deleted', CAST(N'2025-10-23T17:02:34.7349893' AS DateTime2), NULL)
GO
INSERT [dbo].[tblUsers] ([user_id], [fullname], [email], [password_hash], [phone], [age], [role], [imagepath], [country], [status], [created_at], [hotel_id]) VALUES (6, N'kiro magdy', N'kiro@gmail.com', N'$2a$11$kHqvNuFWdCRHqkljrtgoUOjh.LNQ/XJuAtcclsmVX12OZ3R18TUbG', N'01200000000', 23, N'admin', NULL, N'Egypt', N'active', CAST(N'2025-10-25T22:26:10.4170091' AS DateTime2), NULL)
GO
INSERT [dbo].[tblUsers] ([user_id], [fullname], [email], [password_hash], [phone], [age], [role], [imagepath], [country], [status], [created_at], [hotel_id]) VALUES (7, N'Kirollos Magdy', N'kiromagdy@gmail.com', N'$2a$11$L0gRg0Z0IrWFMR8Q.naFceWOQKZsAACRlGNTmQb.KXRf/Akn96hpO', N'01270200486', 23, N'admin', NULL, N'Egypt', N'active', CAST(N'2025-10-26T19:07:25.5838420' AS DateTime2), NULL)
GO
INSERT [dbo].[tblUsers] ([user_id], [fullname], [email], [password_hash], [phone], [age], [role], [imagepath], [country], [status], [created_at], [hotel_id]) VALUES (8, N'kiro', N'kiroCustomer@gmail.com', N'$2a$11$eXlBQ66tM2gU1CGJoe/Gwek/7zhNG12clxYVZWWy9YI60ehdnags6', N'012222222222', 23, N'customer', N'/images/profiles/004dc7fc-9842-47c2-80e7-8b724a6caaff_.png', N'Egypt', N'active', CAST(N'2025-10-27T13:06:24.5780896' AS DateTime2), NULL)
GO
INSERT [dbo].[tblUsers] ([user_id], [fullname], [email], [password_hash], [phone], [age], [role], [imagepath], [country], [status], [created_at], [hotel_id]) VALUES (10, N'admin', N'admin@gmail.com', N'$2a$11$5UYVyGhClnjir8oKZVUNOORiyFS9bwbBVj25gPIeaoOE9z19Lu3Em', N'012222222222', 23, N'admin', N'/images/profiles/20251028_212732.png', N'Egypt', N'active', CAST(N'2025-10-28T02:40:36.5768938' AS DateTime2), NULL)
GO
INSERT [dbo].[tblUsers] ([user_id], [fullname], [email], [password_hash], [phone], [age], [role], [imagepath], [country], [status], [created_at], [hotel_id]) VALUES (12, N'Samir Mohamed', N'samir@gmail.com', N'$2a$11$YoMzJK26YCWv8U32biZN5Odw5V0sahGaUS4eZfmDue9/0S3TUfjiC', NULL, NULL, N'customer', N'/images/profiles/20251028_212542.png', NULL, N'active', CAST(N'2025-10-28T15:27:15.5203234' AS DateTime2), NULL)
GO
INSERT [dbo].[tblUsers] ([user_id], [fullname], [email], [password_hash], [phone], [age], [role], [imagepath], [country], [status], [created_at], [hotel_id]) VALUES (13, N'kareem', N'kareem@gmail.com', N'$2a$11$urt1M8caLM4zpL418ianY.6XviMRVjNOMe0HS38SXYN8GZiXBLBAi', N'01250500500', NULL, N'customer', NULL, NULL, N'active', CAST(N'2025-10-28T15:36:03.4996935' AS DateTime2), NULL)
GO
INSERT [dbo].[tblUsers] ([user_id], [fullname], [email], [password_hash], [phone], [age], [role], [imagepath], [country], [status], [created_at], [hotel_id]) VALUES (14, N'staffuser', N'staff@gmail.com', N'$2a$11$xSVS2jx1MlgCUyeidh9pfOlClXTGTATW25PydLt8Xy65pCbDPNhJO', N'01222222222', NULL, N'staff', NULL, NULL, N'active', CAST(N'2025-10-28T23:54:45.2801898' AS DateTime2), 3)
GO
SET IDENTITY_INSERT [dbo].[tblUsers] OFF
GO
SET IDENTITY_INSERT [dbo].[tblWishList] ON 
GO
INSERT [dbo].[tblWishList] ([wishlist_id], [user_id], [hotel_id], [added_date]) VALUES (1, 1, 2, CAST(N'2025-10-01T08:00:00.0000000' AS DateTime2))
GO
INSERT [dbo].[tblWishList] ([wishlist_id], [user_id], [hotel_id], [added_date]) VALUES (2, 1, 5, CAST(N'2025-10-02T09:00:00.0000000' AS DateTime2))
GO
INSERT [dbo].[tblWishList] ([wishlist_id], [user_id], [hotel_id], [added_date]) VALUES (3, 2, 1, CAST(N'2025-11-01T11:00:00.0000000' AS DateTime2))
GO
SET IDENTITY_INSERT [dbo].[tblWishList] OFF
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__lkpAmeni__72E12F1B6FB36A1B]    Script Date: 11/2/2025 3:24:26 AM ******/
ALTER TABLE [dbo].[lkpAmenities] ADD UNIQUE NONCLUSTERED 
(
	[name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__tblBooki__BADA455927423170]    Script Date: 11/2/2025 3:24:26 AM ******/
ALTER TABLE [dbo].[tblBookings] ADD UNIQUE NONCLUSTERED 
(
	[booking_reference] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__tblHotel__72E12F1BED7C40EC]    Script Date: 11/2/2025 3:24:26 AM ******/
ALTER TABLE [dbo].[tblHotels] ADD UNIQUE NONCLUSTERED 
(
	[name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [UQ__tblRevie__5DE3A5B0889ABF13]    Script Date: 11/2/2025 3:24:26 AM ******/
ALTER TABLE [dbo].[tblReviews] ADD UNIQUE NONCLUSTERED 
(
	[booking_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__tblUsers__AB6E616422F1317D]    Script Date: 11/2/2025 3:24:26 AM ******/
ALTER TABLE [dbo].[tblUsers] ADD UNIQUE NONCLUSTERED 
(
	[email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[tblBookings] ADD  DEFAULT ('pending') FOR [status]
GO
ALTER TABLE [dbo].[tblBookings] ADD  DEFAULT ((0)) FOR [total_price]
GO
ALTER TABLE [dbo].[tblBookings] ADD  DEFAULT ((1)) FOR [adults]
GO
ALTER TABLE [dbo].[tblBookings] ADD  DEFAULT ((0)) FOR [children]
GO
ALTER TABLE [dbo].[tblBookings] ADD  DEFAULT (sysutcdatetime()) FOR [created_at]
GO
ALTER TABLE [dbo].[tblHotels] ADD  DEFAULT (sysutcdatetime()) FOR [created_at]
GO
ALTER TABLE [dbo].[tblHotels] ADD  DEFAULT ('No description yet') FOR [description]
GO
ALTER TABLE [dbo].[tblHotels] ADD  CONSTRAINT [DF_tblHotels_Overview]  DEFAULT (N'No overview available yet.') FOR [Overview]
GO
ALTER TABLE [dbo].[tblNotifications] ADD  DEFAULT ((0)) FOR [is_read]
GO
ALTER TABLE [dbo].[tblNotifications] ADD  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[tblPayments] ADD  DEFAULT ('pending') FOR [payment_status]
GO
ALTER TABLE [dbo].[tblPayments] ADD  DEFAULT (sysutcdatetime()) FOR [payment_date]
GO
ALTER TABLE [dbo].[tblReviews] ADD  DEFAULT (sysutcdatetime()) FOR [review_date]
GO
ALTER TABLE [dbo].[tblReviews] ADD  DEFAULT ((0)) FOR [helpful_count]
GO
ALTER TABLE [dbo].[tblRooms] ADD  DEFAULT ((0)) FOR [price]
GO
ALTER TABLE [dbo].[tblRooms] ADD  DEFAULT ((0)) FOR [breakfast_included]
GO
ALTER TABLE [dbo].[tblRooms] ADD  DEFAULT ((0)) FOR [pets_allowed]
GO
ALTER TABLE [dbo].[tblRooms] ADD  DEFAULT (sysutcdatetime()) FOR [created_at]
GO
ALTER TABLE [dbo].[tblRooms] ADD  DEFAULT ((1)) FOR [available]
GO
ALTER TABLE [dbo].[tblUserHotelBooking] ADD  DEFAULT (sysutcdatetime()) FOR [booking_date]
GO
ALTER TABLE [dbo].[tblUserHotelBooking] ADD  DEFAULT ('active') FOR [status]
GO
ALTER TABLE [dbo].[tblUsers] ADD  DEFAULT ('customer') FOR [role]
GO
ALTER TABLE [dbo].[tblUsers] ADD  DEFAULT ('active') FOR [status]
GO
ALTER TABLE [dbo].[tblUsers] ADD  DEFAULT (sysutcdatetime()) FOR [created_at]
GO
ALTER TABLE [dbo].[tblWishList] ADD  DEFAULT (sysutcdatetime()) FOR [added_date]
GO
ALTER TABLE [dbo].[tblBookings]  WITH CHECK ADD  CONSTRAINT [FK_Booking_Room] FOREIGN KEY([room_id])
REFERENCES [dbo].[tblRooms] ([room_id])
GO
ALTER TABLE [dbo].[tblBookings] CHECK CONSTRAINT [FK_Booking_Room]
GO
ALTER TABLE [dbo].[tblBookings]  WITH CHECK ADD  CONSTRAINT [FK_Booking_User] FOREIGN KEY([user_id])
REFERENCES [dbo].[tblUsers] ([user_id])
GO
ALTER TABLE [dbo].[tblBookings] CHECK CONSTRAINT [FK_Booking_User]
GO
ALTER TABLE [dbo].[tblHotelAmenities]  WITH CHECK ADD  CONSTRAINT [FK_HotelAmenity_Amenity] FOREIGN KEY([amenity_id])
REFERENCES [dbo].[lkpAmenities] ([amenity_id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[tblHotelAmenities] CHECK CONSTRAINT [FK_HotelAmenity_Amenity]
GO
ALTER TABLE [dbo].[tblHotelAmenities]  WITH CHECK ADD  CONSTRAINT [FK_HotelAmenity_Hotel] FOREIGN KEY([hotel_id])
REFERENCES [dbo].[tblHotels] ([hotel_id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[tblHotelAmenities] CHECK CONSTRAINT [FK_HotelAmenity_Hotel]
GO
ALTER TABLE [dbo].[tblHotelImages]  WITH CHECK ADD  CONSTRAINT [FK_HotelImage_Hotel] FOREIGN KEY([hotel_id])
REFERENCES [dbo].[tblHotels] ([hotel_id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[tblHotelImages] CHECK CONSTRAINT [FK_HotelImage_Hotel]
GO
ALTER TABLE [dbo].[tblNotifications]  WITH CHECK ADD  CONSTRAINT [FK_Notifications_Users] FOREIGN KEY([user_id])
REFERENCES [dbo].[tblUsers] ([user_id])
GO
ALTER TABLE [dbo].[tblNotifications] CHECK CONSTRAINT [FK_Notifications_Users]
GO
ALTER TABLE [dbo].[tblPayments]  WITH CHECK ADD  CONSTRAINT [FK_Payment_Booking] FOREIGN KEY([booking_id])
REFERENCES [dbo].[tblBookings] ([booking_id])
GO
ALTER TABLE [dbo].[tblPayments] CHECK CONSTRAINT [FK_Payment_Booking]
GO
ALTER TABLE [dbo].[tblPayments]  WITH CHECK ADD  CONSTRAINT [FK_Payment_User] FOREIGN KEY([user_id])
REFERENCES [dbo].[tblUsers] ([user_id])
GO
ALTER TABLE [dbo].[tblPayments] CHECK CONSTRAINT [FK_Payment_User]
GO
ALTER TABLE [dbo].[tblReviews]  WITH CHECK ADD  CONSTRAINT [FK_Review_Booking] FOREIGN KEY([booking_id])
REFERENCES [dbo].[tblUserHotelBooking] ([booking_id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[tblReviews] CHECK CONSTRAINT [FK_Review_Booking]
GO
ALTER TABLE [dbo].[tblRoomImages]  WITH CHECK ADD  CONSTRAINT [FK_RoomImage_Room] FOREIGN KEY([room_id])
REFERENCES [dbo].[tblRooms] ([room_id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[tblRoomImages] CHECK CONSTRAINT [FK_RoomImage_Room]
GO
ALTER TABLE [dbo].[tblRooms]  WITH CHECK ADD  CONSTRAINT [FK_Room_Hotel] FOREIGN KEY([hotel_id])
REFERENCES [dbo].[tblHotels] ([hotel_id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[tblRooms] CHECK CONSTRAINT [FK_Room_Hotel]
GO
ALTER TABLE [dbo].[tblUserHotelBooking]  WITH CHECK ADD  CONSTRAINT [FK_UHB_Hotel] FOREIGN KEY([hotel_id])
REFERENCES [dbo].[tblHotels] ([hotel_id])
GO
ALTER TABLE [dbo].[tblUserHotelBooking] CHECK CONSTRAINT [FK_UHB_Hotel]
GO
ALTER TABLE [dbo].[tblUserHotelBooking]  WITH CHECK ADD  CONSTRAINT [FK_UHB_User] FOREIGN KEY([user_id])
REFERENCES [dbo].[tblUsers] ([user_id])
GO
ALTER TABLE [dbo].[tblUserHotelBooking] CHECK CONSTRAINT [FK_UHB_User]
GO
ALTER TABLE [dbo].[tblUsers]  WITH CHECK ADD  CONSTRAINT [FK_Users_Hotels] FOREIGN KEY([hotel_id])
REFERENCES [dbo].[tblHotels] ([hotel_id])
GO
ALTER TABLE [dbo].[tblUsers] CHECK CONSTRAINT [FK_Users_Hotels]
GO
ALTER TABLE [dbo].[tblWishList]  WITH CHECK ADD  CONSTRAINT [FK_WishList_User] FOREIGN KEY([user_id])
REFERENCES [dbo].[tblUsers] ([user_id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[tblWishList] CHECK CONSTRAINT [FK_WishList_User]
GO
ALTER TABLE [dbo].[tblReviews]  WITH CHECK ADD CHECK  (([rating]>=(1) AND [rating]<=(5)))
GO
USE [master]
GO
ALTER DATABASE [TravelyDB] SET  READ_WRITE 
GO
