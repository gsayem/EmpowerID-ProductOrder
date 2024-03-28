exec sys.sp_cdc_enable_db

sp_helpText sp_cdc_enable_table 

exec sp_cdc_enable_table  @source_schema = 'dbo',  @source_name = 'Categories',  @role_name =null

exec sp_cdc_enable_table  @source_schema = 'dbo',  @source_name = 'Products',  @role_name =null


exec sp_cdc_enable_table  @source_schema = 'dbo',  @source_name = 'ProductOrders',  @role_name =null


exec sp_cdc_enable_table  @source_schema = 'dbo',  @source_name = 'Orders',  @role_name =null

Select *from Categories

Update Categories set category_name = 'Acanthaceae - CDC - TEST' where category_name = 'Acanthaceae'

Update Categories set category_name = 'Acarosporaceae - CDC - TEST' where category_name = 'Acarosporaceae'

Select * from cdc.dbo_Categories_CT

exec sp_get_CDC_Data_For_Categories

create or alter proc sp_get_CDC_Data_For_Categories
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
end as Operation from cdc.[fn_cdc_get_all_changes_dbo_Categories](@from_lsn, @to_lsn, 'all')
order By __$seqval
end


delete Products where product_id = '01HT32Q67240WZ8R9PPWF2D95F'

Delete Products where 

Update Products set product_name = 'Practical Leather Computer - CDC - TEST' where product_name = 'Practical Leather Computer'

Update Products set product_name = 'Synergistic Granite Chair - CDC - TEST' where product_name = 'Synergistic Granite Chair'

Select __$operation from cdc.dbo_Products_CT

sp_helpText cdc.dbo_Products_CT


exec sp_get_CDC_Data_For_Products

create or alter proc sp_get_CDC_Data_For_Products
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
end as Operation
from cdc.[fn_cdc_get_all_changes_dbo_Products](@from_lsn, @to_lsn, 'all')
order By __$seqval
end
