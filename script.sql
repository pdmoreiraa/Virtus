create database dbVirtus;
use dbVirtus;

drop database dbvirtus;

CREATE TABLE tbProduto(
PrdId INT PRIMARY KEY AUTO_INCREMENT,
PrdNome VARCHAR(100),
PrdMarca VARCHAR(50),
PrdCategoria VARCHAR(70),
PrdTipo VARCHAR(70),
PrdEsporte VARCHAR(70),
PrdDescricao VARCHAR(500),
PrdPreco DECIMAL(10,2),
PrdCor VARCHAR(50),
PrdData DATE DEFAULT (CURRENT_DATE)
);

CREATE TABLE tbPrdImagem (
    PimgId INT AUTO_INCREMENT PRIMARY KEY,
    ProdutoId INT NOT NULL,
    PimgUrl VARCHAR(255) NOT NULL,
    PimgOrdemImagem INT DEFAULT 1,
    FOREIGN KEY (ProdutoId) REFERENCES tbProduto(PrdId)
        ON DELETE CASCADE
        ON UPDATE CASCADE
);

CREATE TABLE tbEstoque (
    EstId INT PRIMARY KEY AUTO_INCREMENT,
    ProdutoId INT NOT NULL,
    EstTamanho VARCHAR(10),
    EstQuantidade INT NOT NULL DEFAULT 0,
    FOREIGN KEY (ProdutoId) REFERENCES tbProduto(PrdId)
        ON DELETE CASCADE ON UPDATE CASCADE
);


CREATE TABLE tbUsuario(
UsuId INT PRIMARY KEY AUTO_INCREMENT,
UsuNome VARCHAR(100),
UsuSobrenome VARCHAR(100),
UsuEmail VARCHAR(150) UNIQUE,
UsuSenha VARCHAR(100),
UsuCPF VARCHAR(15) UNIQUE,
UsuTelefone VARCHAR(15),
UsuTipo ENUM  ('admin', 'cliente') NOT NULL DEFAULT 'cliente'
);

CREATE TABLE tbEndereco (
    EndId INT AUTO_INCREMENT PRIMARY KEY,
    UsuarioId INT NOT NULL,
    EndNomeCompleto VARCHAR(100) NOT NULL,
    EndLogradouro VARCHAR(150) NOT NULL,
    EndNumero VARCHAR(10),
    EndBairro VARCHAR(80),
    EndCidade VARCHAR(100),
    EndEstado VARCHAR(50),
    EndCEP VARCHAR(15),
    EndComplemento VARCHAR(100),
    FOREIGN KEY (UsuarioId) REFERENCES tbUsuario(UsuId)
        ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE tbMetPagamento (
    MpId INT AUTO_INCREMENT PRIMARY KEY,
    MpDescricao VARCHAR(50) NOT NULL
);


CREATE TABLE tbCartao (
    CarId INT AUTO_INCREMENT PRIMARY KEY,
    UsuarioId INT NOT NULL,
    CarTipo varchar(20) NOT NULL,
    CarNomeTitular VARCHAR(100) NOT NULL,
    CarNumero VARCHAR(25) NOT NULL, 
    CarBandeira VARCHAR(30) NOT NULL,
    CarValidade VARCHAR(10) NOT NULL,
    CarCVV VARCHAR(3) NOT NULL,
    FOREIGN KEY (UsuarioId) REFERENCES tbUsuario(UsuId)
        ON DELETE CASCADE ON UPDATE CASCADE
);


CREATE TABLE tbPedido (
    PdId INT AUTO_INCREMENT PRIMARY KEY,
    UsuarioId INT NOT NULL,
    EnderecoId INT NOT NULL,
    MetodoPagamentoId INT NULL,
    CartaoId INT,
    PdTaxaEntrega DECIMAL(10,2) DEFAULT 0.00,
	PdStatusPagamento VARCHAR(50) DEFAULT 'Aguardando Pagamento',
	PdStatusPedido VARCHAR(50) DEFAULT 'Criado',
    PdCriadoEm DATETIME DEFAULT CURRENT_TIMESTAMP,
    PdDataPagamento DATETIME DEFAULT CURRENT_TIMESTAMP,
    PdValorTotal DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (UsuarioId) REFERENCES tbUsuario(UsuId)
        ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (EnderecoId) REFERENCES tbEndereco(EndId)
        ON DELETE RESTRICT ON UPDATE CASCADE,
    FOREIGN KEY (MetodoPagamentoId) REFERENCES tbMetPagamento(MpId)
        ON DELETE SET NULL ON UPDATE CASCADE,
    FOREIGN KEY (CartaoId) REFERENCES tbCartao(CarId)
        ON DELETE SET NULL ON UPDATE CASCADE
);


CREATE TABLE tbItemPedido (
    IpId INT AUTO_INCREMENT PRIMARY KEY,
    PedidoId INT NOT NULL,
    ProdutoId INT NOT NULL,
    IpQuantidade INT NOT NULL,
    IpPrecoUnitario DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (PedidoId) REFERENCES tbPedido(PdId)
        ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (ProdutoId) REFERENCES tbProduto(PrdId)
        ON DELETE RESTRICT ON UPDATE CASCADE
);

INSERT INTO tbMetPagamento (MpDescricao) 
VALUES ('Cartão'),
('Pix');

SELECT * FROM tbEstoque;

INSERT INTO tbProduto (PrdNome, PrdMarca, PrdCategoria, PrdTipo, PrdEsporte, PrdDescricao, PrdPreco, PrdCor)
VALUES ('Top Adidas Suporte Leve Colorblock ', 'Adidas', 'Feminino', 'Top', 'Corrida', 'Top Adidas Suporte Leve Colorblock Feminino - Roxo', 161.49, 'Roxo'),
('Top Nike Dri-FIT Swoosh Bra Média Sustentação',  'Nike', 'Feminino', 'Top', 'Corrida', 'Top Nike Dri-FIT Swoosh Bra Média Sustentação - Preto+Branco', 147.36, 'Preto+Branco'),
('Calça Legging Adidas Essentials 3 Listras ', 'Adidas', 'Masculina', 'Calça', 'Corrida', 'Calça Legging Adidas Essentials 3 Listras Feminina - Preto+Branco', 179.90, 'Preto+Branco'),
('Camisa Palmeiras III 25/26 s/n Torcedor', 'Adidas', 'Masculina', 'Camisa', 'Futebol', 'Camisa Palmeiras III 25/26 s/n Torcedor Puma Masculina - Amarelo', 399.99, 'Amarelo'),
('Camisa Flamengo III 25/26 Torcedor', 'Adidas', 'Masculina', 'Camisa', 'Futebol', 'Camisa Flamengo III 25/26 Torcedor Adidas Masculina - Off White', 399.99, 'Off White'),
('Chuteira Futsal Adidas X Crazyfast 3', 'Puma', 'Unissex', 'Calçado', 'Futebol', 'Chuteira Futsal Adidas X Crazyfast 3 Unissex - Amarelo+Preto', 258.00, 'Amarelo+Preto'),
('Chuteira Nike Zoom Vapor 15 Academy Futsal', 'Nike', 'Masculina', 'Calçado', 'Futebol', 'Chuteira Nike Zoom Vapor 15 Academy Futsal - Rosa', 249.99, 'Rosa'),
('Chuteira Society Adidas Deportivo III', 'Adidas', 'Unissex', 'Calçado', 'Futebol', 'Chuteira Society Adidas Deportivo III Unissex - Roxo', 249.99, 'Roxo'),
('TÊNIS FEMININO ASICS GEL REBOUND AZUL', 'Asics', 'Feminino', 'Calçado', 'Corrida', 'TÊNIS FEMININO ASICS GEL REBOUND AZUL', 284.90, 'Azul'),
('Tênis Olympikus Qu4dra BR1', 'Olympikus', 'Masculino', 'Calçado', 'Basquete',  'Tênis Olympikus Qu4dra BR1 - Azul', 296.99, 'Azul');

INSERT INTO tbPrdImagem (ProdutoId, PimgUrl) VALUES
(10, '001.jpeg'),(9, '002.jpeg'),(9, '003.jpeg'),(8, '005.jpeg'),(8, '004.jpeg'),(7, '006.jpeg'),
(7, '007.jpeg'),(6, '008.jpeg'),(5, '009.jpeg'),(5, '010.jpeg'),(4, '011.jpeg'),(4, '012.jpeg'),
(3, '014.jpeg'),(3, '013.jpeg'),(2, '016.jpeg'),(2, '015.jpeg'),(1, '018.jpeg'),(1, '017.jpeg');

INSERT INTO tbEstoque(ProdutoId, EstTamanho, EstQuantidade) VALUES
(1, 'P',10), (1, 'M',10), (1, 'G',10), (2, 'P',10), (2, 'M',10), (2, 'G',10),
(3, 'P',10), (3, 'M',10), (3, 'G',10),(4, 'P',10), (4, 'M',10), (4, 'G',10),
(5, 'P',10), (5, 'M',10), (5, 'G',10), (6, '39',10), (6, '40',10), (6, '41',10), (6, '42',10),
(7, '39',10), (7, '40',10), (7, '41',10), (7, '42',10),
(8, '39',10), (8, '40',10), (8, '41',10), (8, '42',10),
(9, '39',10), (9, '40',10), (9, '41',10), (9, '42',10),
(10, '39',10), (10, '40',10), (10, '41',10), (10, '42',10);