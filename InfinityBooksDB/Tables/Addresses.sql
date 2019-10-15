IF NOT EXISTS (
  SELECT 
    1 
  FROM 
    sysobjects 
  WHERE 
    name = 'Addresses'
) BEGIN CREATE TABLE infi.Addresses(
  addressesId int NOT NULL IDENTITY(1, 1), 
  userId int NOT NULL, 
  addressTypeId int NOT NULL, 
  addressLine1 nvarchar(MAX) NULL, 
  addressLine2 nvarchar(MAX) NULL, 
  city nvarchar(200) NULL, 
  state nvarchar(200) NULL, 
  country nvarchar(200) NULL, 
  pincode char(10) NULL, 
  status int NULL, 
  createdAt datetime NULL, 
  updatedAt datetime NULL, 
  deletedAt datetime NULL, 
  CONSTRAINT PK_Addresses PRIMARY KEY CLUSTERED (addressesId)
) END
 GO
