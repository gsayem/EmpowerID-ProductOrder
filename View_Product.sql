Alter VIEW dbo.vw_Products  
AS  
SELECT        p.product_id, p.product_name, c.category_name, p.price, p.description, p.date_added  
FROM            dbo.Categories AS c INNER JOIN  
                         dbo.Products AS p ON p.category_id = c.category_id  

						 Select *from vw_Products