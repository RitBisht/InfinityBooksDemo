

IF NOT EXISTS (
  SELECT 
    1 
  FROM 
    sysobjects 
  WHERE 
    name = 'OrderHistory'
) BEGIN CREATE TABLE infi.OrderHistory(
  orderid int NOT NULL IDENTITY(1, 1), 
  productId int not null, 
  userId int not null,
  deliveryAddressId int null,
  shippingAddressId int null,
  amount float null,
  orderStatusId int null,  
  createdAt datetime NULL, 
  updatedAt datetime NULL, 
  deletedAt datetime NULL, 
  CONSTRAINT PK_OrderHistory PRIMARY KEY CLUSTERED (orderid)
) END
 GO