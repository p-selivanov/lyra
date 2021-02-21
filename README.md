#Lyra Test Project

##Scripts
drop table [Customers]

create table [Customers] (
	[Id] int not null primary key identity(1,1),
	[Balance] int not null,
	[Timestamp] datetime not null,
)

insert into [Customers] ([Balance], [Timestamp]) values (0, '2021-02-21')
go 10

select * from [Customers]

update [Customers] set [Balance] = 0, [Timestamp] = '2021-02-21'
