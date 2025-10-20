create database dbVirtus;
use dbVirtus;


create table produtos(
 Id int primary key auto_increment,
 Nome varchar(100),
 Marca varchar(50),
 Categoria varchar(70),
 Tipo varchar(70),
Descricao varchar(200),
Preco decimal(10,2),
ImageUrl varchar(255),
Estoque int,
DataCriada date default (current_date)
);