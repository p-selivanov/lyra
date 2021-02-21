#Lyra Test Project

##Scripts
drop table [Customers]

create table [Customers] (
	[Id] int not null primary key identity(1,1),
	[Balance] int not null,
	[Version] int not null,
)

insert into [Customers] ([Balance], [Version]) values (0, 0)
go 10

select * from [Customers]

update [Customers] set [Balance] = 0, [Version] = 0

##Snippets
endpointConfig.SetQueueArgument("x-single-active-consumer", true);
endpointConfig.UseConcurrencyLimit(1);