IF NOT EXISTS (
  SELECT 
    1 
  FROM 
    sysobjects 
  WHERE 
    name = '
	'
) BEGIN CREATE TABLE infi.ProductCategories(
  productCategoriesId int NOT NULL IDENTITY(1, 1), 
  name nvarchar(MAX) null, 
  description nvarchar(MAX) null, 
  parentCategory int DEFAULT 0 not null,
  status int null,
  createdAt datetime NULL, 
  updatedAt datetime NULL, 
  deletedAt datetime NULL, 
  CONSTRAINT PK_ProductCategories PRIMARY KEY CLUSTERED (productCategoriesId)
) END
 GO