IF NOT EXISTS (
  SELECT 
    1 
  FROM 
    sysobjects 
  WHERE 
    name = 'ProductImages'
) BEGIN CREATE TABLE infi.ProductImages(
  productImageId int NOT NULL IDENTITY(1, 1), 
  productId int NOT NULL, 
  blobimagePath nvarchar(MAX) null, 
  createdAt datetime NULL, 
  updatedAt datetime NULL, 
  deletedAt datetime NULL, 
  CONSTRAINT PK_ProductImages PRIMARY KEY CLUSTERED (productImageId)
) END
 GO
