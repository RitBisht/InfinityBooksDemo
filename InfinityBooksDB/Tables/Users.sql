IF NOT EXISTS (
  SELECT 
    1 
  FROM 
    sysobjects 
  WHERE 
    name = 'Users'
) BEGIN CREATE TABLE infi.Users(
  id int NOT NULL IDENTITY(1, 1),
accountTypeId int null, 
  username nvarchar(300) null, 
  password nvarchar(max) NULL, 
  emailId nvarchar(100) NULL, 
  status int NULL, 
  createdAt datetime NULL, 
  updatedAt datetime NULL, 
  deletedAt datetime NULL, 
  CONSTRAINT PK_Users PRIMARY KEY CLUSTERED (id)
) END
 GO