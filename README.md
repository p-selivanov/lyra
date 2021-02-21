#Lyra Test Project

##Scripts
drop table [Customers]

create table [Customers] (
	[Id] int not null primary key,
	[Balance] int not null
)

insert into [Customers] ([Id], [Balance]) values ((select max([Id]) + 1 from [Customers]), 0)
go 10

select * from [Customers]

update [Customers] set [Balance] = 0

##Snippets
endpointConfig.SetQueueArgument("x-single-active-consumer", true);
endpointConfig.UseConcurrencyLimit(1);