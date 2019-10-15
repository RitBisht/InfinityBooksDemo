IF NOT EXISTS (
  SELECT 
    1 
  FROM 
    sysobjects 
  WHERE 
    name = 'UserProfiles'
) BEGIN CREATE TABLE infi.UserProfiles(
  id int NOT NULL IDENTITY(1, 1), 
  userId int NOT NULL, 
  firstName nvarchar(300) null, 
  lastName nvarchar(300) NULL, 
  emailId nvarchar(100) NULL, 
  contactNumber nvarchar(22) NULL, 
  gender char(10) NULL, 
  createdAt datetime NULL, 
  updatedAt datetime NULL, 
  deletedAt datetime NULL, 
  CONSTRAINT PK_UserProfiles PRIMARY KEY CLUSTERED (id)
) END
 GO
