

IF NOT EXISTS (
  SELECT 
    1 
  FROM 
    sysobjects 
  WHERE 
    name = 'Cart'
) BEGIN CREATE TABLE infi.Cart(
  id int NOT NULL IDENTITY(1, 1), 
  productId int not null, 
  userId int not null,
  statusTypeId int null,
  quantity int DEFAULT 0;
  description nvarchar(MAX) null,
  status int null,
  createdAt datetime NULL, 
  updatedAt datetime NULL, 
  deletedAt datetime NULL, 
  CONSTRAINT PK_Cart PRIMARY KEY CLUSTERED (id)
) END
 GO