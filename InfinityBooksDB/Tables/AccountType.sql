IF NOT EXISTS (
  SELECT 
    1 
  FROM 
    sysobjects 
  WHERE 
    name = 'AccountType'
) BEGIN CREATE TABLE infi.AccountType(
  id int NOT NULL IDENTITY(1, 1), 
  name nvarchar(300) null, 
  description nvarchar(max) NULL, 
  status int NULL, 
  createdAt datetime NULL, 
  updatedAt datetime NULL, 
  deletedAt datetime NULL, 
  CONSTRAINT PK_AccountType PRIMARY KEY CLUSTERED (id)
) END
 GO