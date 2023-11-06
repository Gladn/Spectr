create database Spectr
use Spectr

create table EmployerPosition
(PositionID int not null primary key identity,
PositionName varchar(100) not null);

create table Employer
(EmployerID int not null primary key identity,
EmFirstName varchar(100) not null,
EmSecondName varchar(100) not null,
PhoneNumber char(11) not null,
Salary dec(8,2) not null,
PositionID int not null,
Foreign key (PositionID) references EmployerPosition);

create table Customer
(CustomerID int not null primary key identity,
CustomerType bit not null,
DocNumber char(19) UNIQUE,
CustomerFirstName varchar(100) not null,
CustomerSecontName varchar(100) not null,
CustomerPatronymic varchar(100),
PhoneNumber char(11) not null UNIQUE,
EmailAdress varchar(50));

create table Device
(DeviceID int not null primary key identity,
SerialNumber varchar(30) not null UNIQUE,
Type varchar(50) not null,
Company varchar(50) not null,
Model varchar(50) not null,
ManufactureYear int);

create table RepairCategory
(CategoryID int not null primary key identity,
Category varchar(100) not null);

create table Repair
(OrderID int not null primary key identity,
DateStart date not null,
CustomerID int not null,
DeviceID int not null,
EmployerID int not null,
PlainDateEnd date not null,
DateEnd date,
Status bit not null,
Discount dec(5,2),
TotalCost dec(8,2) not null,
Comment varchar(300),
Foreign key (CustomerID) references Customer,
Foreign key (EmployerID) references Employer,
Foreign key (DeviceID) references Device);

create table RepairCategoryJunction
(JunctionID int not null primary key identity,
OrderID int not null,
CategoryID int not null,
Foreign key (OrderID) references Repair,
Foreign key (CategoryID) references RepairCategory);

INSERT INTO EmployerPosition
VALUES ('Руководитель'),('Мастер профи'),('Мастер новичок');

INSERT INTO RepairCategory
VALUES ('Ремонт стекла'),('Смена батареи'),('Ремонт корпуса'),('Смена прошивки');

INSERT INTO Employer
VALUES ('Макаров','Михаил','89928941139',100000,1),
('Столяров','Игорь','89912349710',70000,2),
('Парфенов','Даниил','89968150062',40000,3),
('Гордеев','Александр','89960931185',30000,3);

INSERT INTO Customer
VALUES (0,'4512 566936','Виноградов', 'Марк', 'Данилович','89926481590',''),
(0, '4872 991328','Попов','Вячесла', 'Артёмович','89921635690','popov54@mail.ru'),
(0, '4557 400900','Суворова', 'Сабина', 'Данииловна','89996032759','Davilov1999@mail.ru'),
(0, '4911 291923','Григорьева', 'Ксения', 'Егоровна','89991579952','GrigorievafKS@list.ru'),
(0, '4133 577607','Баранова', 'Маргарита', 'Николаевна','89998791233',''),
(0, '4924 216200','Кузнецов', 'Дмитрий', '','89996638907','');

select * from Customer

INSERT INTO Device
VALUES ('GT8645014494579','Смартфон', 'Apple', 'Iphone 13',2020),
('T5391241395270','Смартфон', 'Xiaomi', 'Spark C10',2017),
('BV9163546949352','Смартфон', 'Sony', 'A51',2019),
('NB0162859386563','Смартфон', 'Vivo', 'Y02',2021),
('EF5255248854161','Смартфон', 'Itel', 'Inoi 2 lite',2022),
('F3596708543253','Смартфон', 'Apple', 'Iphone 13 SE',2022),
('EF0148188928790','Кнопочный', 'Nokia', 'C01 plus',2019),
('YN4442375132325','Смартфон', 'Poco', 'B10',2018);


INSERT INTO Repair
VALUES ('13.12.2021', 1, 1, 2, '14.12.2021', '14.12.2021', 1, 0, 10000, ''),
('17.12.2021', 2, 2, 4, '19.12.2021', '20.12.2021', 1, 0, 2000, ''),
('02.01.2022', 3, 3, 3, '04.01.2022', '05.01.2022', 1, 0, 3000, ''),
('05.01.2022', 3, 4, 4, '06.01.2022', '06.01.2022', 1, 0, 8000, ''),
('11.01.2022', 4, 5, 3, '12.01.2022', '13.01.2022', 1, 0, 4000, ''),
('14.01.2022', 1, 6, 2, '16.01.2022', '16.01.2022', 1, 0, 10000, ''),
('19.01.2022', 5, 7, 3, '20.01.2022', '20.01.2022', 1, 0, 4000, ''),
('26.01.2022', 6, 8, 1, '27.01.2022', '28.01.2022', 1, 10, 1800, '');

INSERT INTO RepairCategoryJunction
VALUES (1, 1),
(1, 2),
(2, 1),
(3, 1),
(4, 1),
(4, 3),
(5, 3),
(6, 1),
(6, 2),
(7, 3),
(8, 4);

select * from Repair
join Employer on Employer.EmployerID = Repair.EmployerID
join EmployerPosition on EmployerPosition.PositionID = Employer.PositionID
join Customer on Customer.CustomerID = Repair.CustomerID
join Device on Device.DeviceID = Repair.DeviceID
join RepairCategoryJunction on RepairCategoryJunction.OrderID = Repair.OrderID
join RepairCategory on RepairCategory.CategoryID = RepairCategoryJunction.CategoryID
--where Customer.CustomerID like 3
Order by Repair.OrderID

SELECT CustomerID AS PersonId, CustomerFirstName AS FirstName, CustomerSecontName AS LastName, DocNumber AS Passport FROM Customer