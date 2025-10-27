create database dbVirtus;
use dbVirtus;



CREATE TABLE produtos(
Id INT PRIMARY KEY AUTO_INCREMENT,
Nome VARCHAR(100),
Marca VARCHAR(50),
Categoria VARCHAR(70),
Tipo VARCHAR(70),
Descricao VARCHAR(500),
Preco DECIMAL(10,2),
ImageUrl VARCHAR(255),
Estoque INT,
DataCriada DATE DEFAULT (CURRENT_DATE)
);

CREATE TABLE usuarios(
Id INT PRIMARY KEY AUTO_INCREMENT,
Nome VARCHAR(100),
Sobrenome VARCHAR(100),
Email VARCHAR(150) UNIQUE,
Senha VARCHAR(100),
CPF VARCHAR(11) UNIQUE,
Telefone VARCHAR(11),
Tipo ENUM  ('admin', 'cliente') NOT NULL DEFAULT 'cliente'
);

CREATE TABLE enderecos (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    UsuarioId INT NOT NULL,
    NomeCompleto VARCHAR(100) NOT NULL,
    Logradouro VARCHAR(150) NOT NULL,
    Numero VARCHAR(10),
    Bairro VARCHAR(80),
    Cidade VARCHAR(100),
    Estado VARCHAR(50),
    CEP VARCHAR(15),
    Complemento VARCHAR(100),
    FOREIGN KEY (UsuarioId) REFERENCES usuarios(Id)
        ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE metodosPagamento (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Descricao VARCHAR(50) NOT NULL
);


CREATE TABLE cartoes (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    UsuarioId INT NOT NULL,
    Tipo varchar(20) NOT NULL,
    NomeTitular VARCHAR(100) NOT NULL,
    Numero VARCHAR(25) NOT NULL, 
    Bandeira VARCHAR(30) NOT NULL,
    Validade VARCHAR(10) NOT NULL,
    CVV VARCHAR(3) NOT NULL,
    FOREIGN KEY (UsuarioId) REFERENCES usuarios(Id)
        ON DELETE CASCADE ON UPDATE CASCADE
);


CREATE TABLE pedidos (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    UsuarioId INT NOT NULL,
    EnderecoId INT NOT NULL,
    MetodoPagamentoId INT NULL,
    CartaoId INT,
    TaxaEntrega DECIMAL(10,2) DEFAULT 0.00,
    StatusPedido VARCHAR(50) DEFAULT 'Pendente',
    CriadoEm DATETIME DEFAULT CURRENT_TIMESTAMP,
    DataPagamento DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (UsuarioId) REFERENCES usuarios(Id)
        ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (EnderecoId) REFERENCES enderecos(Id)
        ON DELETE RESTRICT ON UPDATE CASCADE,
    FOREIGN KEY (MetodoPagamentoId) REFERENCES metodosPagamento(Id)
        ON DELETE SET NULL ON UPDATE CASCADE,
    FOREIGN KEY (CartaoId) REFERENCES cartoes(Id)
        ON DELETE SET NULL ON UPDATE CASCADE
);


CREATE TABLE itensPedido (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    PedidoId INT NOT NULL,
    ProdutoId INT NOT NULL,
    Quantidade INT NOT NULL,
    PrecoUnitario DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (PedidoId) REFERENCES pedidos(Id)
        ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (ProdutoId) REFERENCES produtos(Id)
        ON DELETE RESTRICT ON UPDATE CASCADE
);

INSERT INTO metodosPagamento (Descricao) 
VALUES ('Cartão'),
('Pix');

SELECT * FROM pedidos;

insert into produtos (Nome, Marca, Categoria, Tipo, Descricao, Preco, ImageUrl, Estoque)
values ('Carrinhos', 'Hot Wheels', 'Brinquedo', 'Infantil', 'Pacote com 5 Carros', 67.90, '/img/01.jpg', 5),
('Guincho Tubarão',  'Hot Wheels', 'Brinquedo', 'Infantil', 'Conjunto de Guincho Tubarão', 172.90, '/img/02.jpg', 5),
('Pista de Percurso T-Rex X', 'Hot Wheels', 'Brinquedo', 'Infantil', 'Quartel de Bombeiros', 286.90, '/img/03.jpg', 10),
('Pista de Percurso e Mini Veículo',  'Hot Wheels', 'Infantil', 'Brinquedo', 'City - Lava-Rápido', 109.90, '/img/04.jpg', 15),
('City Nemesis - Gorila', 'Hot Wheels', 'Brinquedo', 'Infantil', 'Pista de Percurso e Mini Veículo', 161.90, '/img/05.jpg', 20),
('Reboque e Mini Veículo', 'Hot Wheels', 'Brinquedo', 'Infantil', 'City - Reboque de Dragão', 127.90, '/img/06.jpg', 25);

insert into usuarios(Nome, Sobrenome, Email, Senha, CPF, Telefone, Tipo)
values ('Admin', 'Admin', 'admin@gmail.com', 'admin123', '11111111111', '11999999999', 'admin');

INSERT INTO enderecos (
    UsuarioId,
    NomeCompleto,
    Rua,
    Numero,
    Bairro,
    Cidade,
    Estado,
    CEP,
    Complemento
) VALUES (
    1,
    'Pedro Henrique',
    'Rua das Flores',
    '123',
    'Jardim Primavera',
    'São Paulo',
    'SP',
    '01234-567',
    'Apartamento 45'
);

INSERT INTO enderecos (
    UsuarioId,
    NomeCompleto,
    Rua,
    Numero,
    Bairro,
    Cidade,
    Estado,
    CEP,
    Complemento
) VALUES (
    1,
    'Pedro Henrique',
    'Rua das Árvores',
    '123',
    'Osasco',
    'São Paulo',
    'SP',
    '01234-555',
    'Apartamento 46'
);
