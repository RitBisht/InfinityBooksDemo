create table [dbo].[Logs](
	[LogId] [int] IDENTITY(1,1) not null,
	[Level] [varchar](max) not null,
	[CallSite] [varchar](max) not null,
	[Type] [varchar](max) not null,
	[Message] [varchar](max) not null,
	[StackTrace] [varchar](max) not null,
	[InnerException] [varchar](max) not null,
	[AdditionalInfo] [varchar](max) not null,
	[LoggedOnDate] [datetime] not null constraint [df_logs_loggedondate]  default (getutcdate()),

	constraint [pk_logs] primary key clustered 
	(
		[LogId]
	)
)
