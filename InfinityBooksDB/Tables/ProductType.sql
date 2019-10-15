IF NOT EXISTS (
  SELECT 
    1 
  FROM 
    sysobjects 
  WHERE 
    name = 'ProductType'
) BEGIN CREATE TABLE infi.ProductType(
  productTypeId int NOT NULL IDENTITY(1, 1), 
  Name nvarchar(MAX) null, 
  description nvarchar(MAX) null, 
  status int null, 
  createdAt datetime NULL, 
  updatedAt datetime NULL, 
  deletedAt datetime NULL, 
  CONSTRAINT PK_ProductType PRIMARY KEY CLUSTERED (productTypeId)
) END 
GO
