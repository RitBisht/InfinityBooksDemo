

IF NOT EXISTS (
  SELECT 
    1 
  FROM 
    sysobjects 
  WHERE 
    name = 'StatusType'
) BEGIN CREATE TABLE infi.StatusType(
  id int NOT NULL IDENTITY(1, 1), 
  name nvarchar(300) not null, 
  description nvarchar(MAX) null,
  status int null,
  createdAt datetime NULL, 
  updatedAt datetime NULL, 
  deletedAt datetime NULL, 
  CONSTRAINT PK_StatusType PRIMARY KEY CLUSTERED (id)
) END
 GO