IF NOT EXISTS (
  SELECT 
    1 
  FROM 
    sysobjects 
  WHERE 
    name = 'AddressType'
) BEGIN CREATE TABLE infi.AddressType(
  addressTypeId int NOT NULL IDENTITY(1, 1), 
  name nvarchar(200) NOT NULL, 
  CONSTRAINT PK_AddressType PRIMARY KEY CLUSTERED (addressTypeId)
) END
 GO
