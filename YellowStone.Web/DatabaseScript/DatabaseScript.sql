

ALTER TABLE dbo.Roles ADD
	ApprovedBy varchar(50) NULL,
	ApprovedDate datetime NULL
GO


ALTER TABLE dbo.Departments ADD
	ApprovedBy varchar(50) NULL,
	ApprovedDate datetime NULL
GO



ALTER TABLE dbo.Users ADD
	ApprovedBy varchar(50) NULL,
	ApprovedDate datetime NULL,
	Notes varchar(5000) null,
	LoggedIn bit

GO