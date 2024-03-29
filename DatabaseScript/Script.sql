/****** Object:  Table [dbo].[Categories]    Script Date: 3/29/2024 11:56:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Categories](
	[category_id] [nvarchar](26) NOT NULL,
	[category_name] [nvarchar](200) NOT NULL,
 CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED 
(
	[category_id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Products]    Script Date: 3/29/2024 11:56:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Products](
	[product_id] [nvarchar](26) NOT NULL,
	[product_name] [nvarchar](200) NOT NULL,
	[category_id] [nvarchar](26) NOT NULL,
	[price] [decimal](18, 4) NOT NULL,
	[description] [nvarchar](max) NULL,
	[image_url] [nvarchar](400) NULL,
	[date_added] [datetime] NOT NULL,
 CONSTRAINT [PK_Products] PRIMARY KEY CLUSTERED 
(
	[product_id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  View [dbo].[vw_Products]    Script Date: 3/29/2024 11:56:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[vw_Products]  
AS  
SELECT        p.product_id, p.product_name, c.category_name, p.price, p.description, p.date_added  
FROM            dbo.Categories AS c INNER JOIN  
                         dbo.Products AS p ON p.category_id = c.category_id  
GO
/****** Object:  Table [dbo].[Orders]    Script Date: 3/29/2024 11:56:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Orders](
	[order_id] [nvarchar](26) NOT NULL,
	[order_date] [datetime] NOT NULL,
	[customer_name] [nvarchar](200) NULL,
 CONSTRAINT [PK_Orders] PRIMARY KEY CLUSTERED 
(
	[order_id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_PADDING ON
GO
/****** Object:  Index [CategoryName_Unique_NonClustered_Index]    Script Date: 3/29/2024 11:56:08 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [CategoryName_Unique_NonClustered_Index] ON [dbo].[Categories]
(
	[category_name] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [OrderDate_NonClustered_Index]    Script Date: 3/29/2024 11:56:08 PM ******/
CREATE NONCLUSTERED INDEX [OrderDate_NonClustered_Index] ON [dbo].[Orders]
(
	[order_date] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO

SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Products_category_id]    Script Date: 3/29/2024 11:56:08 PM ******/
CREATE NONCLUSTERED INDEX [IX_Products_category_id] ON [dbo].[Products]
(
	[category_id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [ProductDateAddded_Non_Clustered_Index]    Script Date: 3/29/2024 11:56:08 PM ******/
CREATE NONCLUSTERED INDEX [ProductDateAddded_Non_Clustered_Index] ON [dbo].[Products]
(
	[date_added] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [ProductName_Unique_NonClustered_Index]    Script Date: 3/29/2024 11:56:08 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [ProductName_Unique_NonClustered_Index] ON [dbo].[Products]
(
	[product_name] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [ProductPrice_Non_Clustered_Index]    Script Date: 3/29/2024 11:56:08 PM ******/
CREATE NONCLUSTERED INDEX [ProductPrice_Non_Clustered_Index] ON [dbo].[Products]
(
	[price] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Products]  WITH CHECK ADD  CONSTRAINT [FK_Products_Categories_category_id] FOREIGN KEY([category_id])
REFERENCES [dbo].[Categories] ([category_id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [FK_Products_Categories_category_id]
GO
/****** Object:  StoredProcedure [dbo].[sp_CDC_Categories_Table_Cleanup]    Script Date: 3/29/2024 11:56:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create   proc [dbo].[sp_CDC_Categories_Table_Cleanup]
as 
begin
Declare @from_lsn binary(10), @to_lsn binary(10), @cleanup_failed_bit int,  @retcode int
SELECT @cleanup_failed_bit = 0;

Set @from_lsn  =  sys.fn_cdc_get_min_lsn('dbo_Categories'); 
Set @to_lsn =  sys.fn_cdc_get_max_lsn(); 

-- Execute cleanup and obtain output bit
EXEC @retcode = sys.sp_cdc_cleanup_change_table
    @capture_instance = 'dbo_Categories',
    @low_water_mark = @to_lsn, --== LSN to be used for new low watermark for capture instance
    @threshold = 1,
    @fCleanupFailed = @cleanup_failed_bit OUTPUT;

--Return Category table schema to confirm in the application label.
SELECT IIF(@cleanup_failed_bit > 0, 'CLEANUP FAILURE', 'CLEANUP SUCCESS') as category_id, '' category_name, 'Dirty' as DataStatus
end
GO
/****** Object:  StoredProcedure [dbo].[sp_CDC_Orders_Table_Cleanup]    Script Date: 3/29/2024 11:56:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

Create   proc [dbo].[sp_CDC_Orders_Table_Cleanup]
as 
begin
Declare @from_lsn binary(10), @to_lsn binary(10), @cleanup_failed_bit int,  @retcode int
SELECT @cleanup_failed_bit = 0;

Set @from_lsn  =  sys.fn_cdc_get_min_lsn('dbo_Orders'); 
Set @to_lsn =  sys.fn_cdc_get_max_lsn(); 

-- Execute cleanup and obtain output bit
EXEC @retcode = sys.sp_cdc_cleanup_change_table
    @capture_instance = 'dbo_Orders',
    @low_water_mark = @to_lsn, --== LSN to be used for new low watermark for capture instance
    @threshold = 1,
    @fCleanupFailed = @cleanup_failed_bit OUTPUT;

--Return Order table schema to confirm in the application label.
SELECT IIF(@cleanup_failed_bit > 0, 'CLEANUP FAILURE', 'CLEANUP SUCCESS') as order_id, getDate() order_date, '' customer_name, 'Dirty' as DataStatus
end
GO
/****** Object:  StoredProcedure [dbo].[sp_CDC_Products_Table_Cleanup]    Script Date: 3/29/2024 11:56:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

Create   proc [dbo].[sp_CDC_Products_Table_Cleanup]
as 
begin
Declare @from_lsn binary(10), @to_lsn binary(10), @cleanup_failed_bit int,  @retcode int
SELECT @cleanup_failed_bit = 0;

Set @from_lsn  =  sys.fn_cdc_get_min_lsn('dbo_Products'); 
Set @to_lsn =  sys.fn_cdc_get_max_lsn(); 

-- Execute cleanup and obtain output bit
EXEC @retcode = sys.sp_cdc_cleanup_change_table
    @capture_instance = 'dbo_Products',
    @low_water_mark = @to_lsn, --== LSN to be used for new low watermark for capture instance
    @threshold = 1,
    @fCleanupFailed = @cleanup_failed_bit OUTPUT;

--Return Product table schema to confirm in the application label.
SELECT IIF(@cleanup_failed_bit > 0, 'CLEANUP FAILURE', 'CLEANUP SUCCESS') as product_id, '' product_name, 0.0 as price, '' category_id, 'Dirty' as DataStatus, GetDate() date_added, '' description, '' image_url;
end
GO
/****** Object:  StoredProcedure [dbo].[sp_get_CDC_Data_For_Categories]    Script Date: 3/29/2024 11:56:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create   proc [dbo].[sp_get_CDC_Data_For_Categories]
as
begin
Declare @from_lsn binary(10), @to_lsn binary(10)
Set @from_lsn  =  sys.fn_cdc_get_min_lsn('dbo_Categories'); 
Set @to_lsn =  sys.fn_cdc_get_max_lsn(); 
Select *,
CASE 
When __$operation = 1 then 'Deleted'
when __$operation = 2 then 'Inserted'
when __$operation = 3 then 'Dirty'
when __$operation = 4 then 'Updated'
end as DataStatus from cdc.[fn_cdc_get_all_changes_dbo_Categories](@from_lsn, @to_lsn, 'all')
order By __$seqval
end
GO
/****** Object:  StoredProcedure [dbo].[sp_get_CDC_Data_For_Orders]    Script Date: 3/29/2024 11:56:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create   proc [dbo].[sp_get_CDC_Data_For_Orders]
as
begin
Declare @from_lsn binary(10), @to_lsn binary(10)
Set @from_lsn  =  sys.fn_cdc_get_min_lsn('dbo_Orders'); 
Set @to_lsn =  sys.fn_cdc_get_max_lsn(); 
Select *,
CASE 
When __$operation = 1 then 'Deleted'
when __$operation = 2 then 'Inserted'
when __$operation = 3 then 'Dirty'
when __$operation = 4 then 'Updated'
end as DataStatus
from cdc.[fn_cdc_get_all_changes_dbo_Orders](@from_lsn, @to_lsn, 'all')
order By __$seqval
end
GO
/****** Object:  StoredProcedure [dbo].[sp_get_CDC_Data_For_Products]    Script Date: 3/29/2024 11:56:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create   proc [dbo].[sp_get_CDC_Data_For_Products]
as
begin
Declare @from_lsn binary(10), @to_lsn binary(10)
Set @from_lsn  =  sys.fn_cdc_get_min_lsn('dbo_Products'); 
Set @to_lsn =  sys.fn_cdc_get_max_lsn(); 
Select *,
CASE 
When __$operation = 1 then 'Deleted'
when __$operation = 2 then 'Inserted'
when __$operation = 3 then 'Dirty'
when __$operation = 4 then 'Updated'
end as DataStatus
from cdc.[fn_cdc_get_all_changes_dbo_Products](@from_lsn, @to_lsn, 'all')
order By __$seqval
end
GO



/*NOT in the Requirements but we can add below table If want to track product orders*/
/****** Object:  Table [dbo].[ProductOrders]    Script Date: 3/29/2024 11:56:08 PM ******/
--SET ANSI_NULLS ON
--GO
--SET QUOTED_IDENTIFIER ON
--GO
--CREATE TABLE [dbo].[ProductOrders](
--	[product_order_id] [nvarchar](26) NOT NULL,
--	[product_id] [nvarchar](26) NOT NULL,
--	[order_id] [nvarchar](26) NOT NULL,
-- CONSTRAINT [PK_ProductOrders] PRIMARY KEY CLUSTERED 
--(
--	[product_order_id] ASC
--)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
--) ON [PRIMARY]
--GO

--/****** Object:  Index [IX_ProductOrders_order_id]    Script Date: 3/29/2024 11:56:08 PM ******/
--CREATE NONCLUSTERED INDEX [IX_ProductOrders_order_id] ON [dbo].[ProductOrders]
--(
--	[order_id] ASC
--)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
--GO
--SET ANSI_PADDING ON
--GO
--/****** Object:  Index [IX_ProductOrders_product_id]    Script Date: 3/29/2024 11:56:08 PM ******/
--CREATE NONCLUSTERED INDEX [IX_ProductOrders_product_id] ON [dbo].[ProductOrders]
--(
--	[product_id] ASC
--)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
--GO

--ALTER TABLE [dbo].[ProductOrders]  WITH CHECK ADD  CONSTRAINT [FK_ProductOrders_Orders_order_id] FOREIGN KEY([order_id])
--REFERENCES [dbo].[Orders] ([order_id])
--ON DELETE CASCADE
--GO
--ALTER TABLE [dbo].[ProductOrders] CHECK CONSTRAINT [FK_ProductOrders_Orders_order_id]
--GO
--ALTER TABLE [dbo].[ProductOrders]  WITH CHECK ADD  CONSTRAINT [FK_ProductOrders_Products_product_id] FOREIGN KEY([product_id])
--REFERENCES [dbo].[Products] ([product_id])
--ON DELETE CASCADE
--GO
--ALTER TABLE [dbo].[ProductOrders] CHECK CONSTRAINT [FK_ProductOrders_Products_product_id]
--GO