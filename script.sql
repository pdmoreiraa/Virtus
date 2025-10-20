create database dbVirtus;
use dbVirtus;



create table produtos(
 Id int primary key auto_increment,
 Nome varchar(100),
 Marca varchar(50),
 Categoria varchar(70),
 Tipo varchar(70),
Descricao varchar(500),
Preco decimal(10,2),
ImageUrl varchar(255),
Estoque int,
DataCriada date default (current_date)
);

INSERT INTO produtos (Nome, Marca, Categoria, Tipo, Descricao, Preco, ImageUrl, Estoque)
VALUES ('Carrinhos', 'Hot Wheels', 'Brinquedo', 'Infantil', 'Pacote com 5 Carros', 67.90, '/img/01.jpg', 5),
('Guincho Tubarão',  'Hot Wheels', 'Brinquedo', 'Infantil', 'Conjunto de Guincho Tubarão', 172.90, '/img/02.jpg', 5),
('Pista de Percurso T-Rex X', 'Hot Wheels', 'Brinquedo', 'Infantil', 'Quartel de Bombeiros', 286.90, '/img/03.jpg', 10),
('Pista de Percurso e Mini Veículo',  'Hot Wheels', 'Infantil', 'Brinquedo', 'City - Lava-Rápido', 109.90, '/img/04.jpg', 15),
('City Nemesis - Gorila', 'Hot Wheels', 'Brinquedo', 'Infantil', 'Pista de Percurso e Mini Veículo', 161.90, '/img/05.jpg', 20),
('Reboque e Mini Veículo', 'Hot Wheels', 'Brinquedo', 'Infantil', 'City - Reboque de Dragão', 127.90, '/img/06.jpg', 25);