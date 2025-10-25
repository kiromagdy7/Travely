USE [master]
GO
/****** Object:  Database [TravelyDB]    Script Date: 10/22/2025 10:19:02 PM ******/
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
/****** Object:  Table [dbo].[lkpAmenities]    Script Date: 10/22/2025 10:19:02 PM ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblBookings]    Script Date: 10/22/2025 10:19:02 PM ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[booking_reference] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblHotelAmenities]    Script Date: 10/22/2025 10:19:02 PM ******/
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
/****** Object:  Table [dbo].[tblHotelImages]    Script Date: 10/22/2025 10:19:02 PM ******/
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
/****** Object:  Table [dbo].[tblHotels]    Script Date: 10/22/2025 10:19:02 PM ******/
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
PRIMARY KEY CLUSTERED 
(
	[hotel_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblPayments]    Script Date: 10/22/2025 10:19:02 PM ******/
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
/****** Object:  Table [dbo].[tblReviews]    Script Date: 10/22/2025 10:19:02 PM ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[booking_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblRoomImages]    Script Date: 10/22/2025 10:19:02 PM ******/
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
/****** Object:  Table [dbo].[tblRooms]    Script Date: 10/22/2025 10:19:02 PM ******/
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
PRIMARY KEY CLUSTERED 
(
	[room_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblUserHotelBooking]    Script Date: 10/22/2025 10:19:02 PM ******/
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
/****** Object:  Table [dbo].[tblUsers]    Script Date: 10/22/2025 10:19:02 PM ******/
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
PRIMARY KEY CLUSTERED 
(
	[user_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblWishList]    Script Date: 10/22/2025 10:19:02 PM ******/
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
