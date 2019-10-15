

IF NOT EXISTS (
  SELECT 
    1 
  FROM 
    sysobjects 
  WHERE 
    name = 'ProductReview'
) BEGIN CREATE TABLE infi.ProductReview(
  id int NOT NULL IDENTITY(1, 1), 
  review float DEFAULT 0.0 not null, 
  productId int not null,
  description nvarchar(MAX) null, 
  userId int not null,  
  status int null,
  createdAt datetime NULL, 
  updatedAt datetime NULL, 
  deletedAt datetime NULL, 
  CONSTRAINT PK_ProductReview PRIMARY KEY CLUSTERED (id)
) END
 GO