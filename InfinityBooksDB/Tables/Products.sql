IF NOT EXISTS (
  SELECT 
    1 
  FROM 
    sysobjects 
  WHERE 
    name = 'Products'
) BEGIN CREATE TABLE infi.Products(
  productId int NOT NULL IDENTITY(1, 1), 
  productCode nvarchar(100) NULL, 
  productTypeId int null,
  productCategoryId int null,
  name nvarchar(300) null,
  description nvarchar(MAX) null,
  author nvarchar(300) null,
  publisher nvarchar(500) null,
  publishDate datetime null,
  quantity int null DEFAULT 0,
  price float null,
  saleprice float null,
  avgRating float null,
  status int not null,
  createdAt datetime NULL, 
  updatedAt datetime NULL, 
  deletedAt datetime NULL, 
  CONSTRAINT PK_Products PRIMARY KEY CLUSTERED (productId)
) END
 GO
