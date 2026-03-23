--ÁREA DOS PEIXES:

-- TABELA COMPORTAMENTOS
CREATE TABLE comportamentos (
    id SERIAL PRIMARY KEY,
    nome VARCHAR (100) UNIQUE NOT NULL,
    descricao VARCHAR(250) UNIQUE NOT NULL
);
--  TABELA GUILDAS TRÓFICAS
CREATE TABLE  guildas_troficas (
    id SERIAL PRIMARY KEY,
    nome VARCHAR (255) UNIQUE NOT NULL
);
--  TABELA ESTADOS DE SAUDE
CREATE TABLE estados_saude (
    id SERIAL PRIMARY KEY,
    descricao VARCHAR(250) UNIQUE NOT NULL
);

-- TABELA ESTADOS DE DESENVOLVIMENTO
CREATE TABLE estados_desenvolvimento (
    id SERIAL PRIMARY KEY,
    descricao VARCHAR(250) NOT NULL
);

-- TABELA ESPECIES
CREATE TABLE especies (
    id SERIAL PRIMARY KEY,
    nome_vulgar VARCHAR(250) UNIQUE,
--Identificador para humanos:
    taxon VARCHAR(250) UNIQUE NOT NULL,
--Campos para precisão, opcionais:
    subespecie VARCHAR(250),
    linhagem VARCHAR(250),
    imagem_url VARCHAR(255) UNIQUE,
--separador
    comentario TEXT,
    -- FK seguindo o padrão "tabela_singular + _id"
    comportamento_id INTEGER,
    guilda_trofica_id INTEGER,
    CONSTRAINT fk_comportamento FOREIGN KEY (comportamento_id) REFERENCES comportamentos (id),
    CONSTRAINT fk_guilda_trofica FOREIGN KEY (guilda_trofica_id) REFERENCES  guildas_troficas (id)
);

--  TABELA LOTES
CREATE TABLE lotes (
    id SERIAL PRIMARY KEY,
    status INTEGER DEFAULT 0,
    descricao VARCHAR(255) NOT NULL, 
    quantidade_peixes INTEGER DEFAULT 0, 
    preco_lote DECIMAL DEFAULT 0
);

--  TABELA PRECOS
CREATE TABLE precos (
    id SERIAL PRIMARY KEY,
    valor DECIMAL(6, 2), -- Mudei de 'preco' para 'valor' para não ficar precos.preco
    especie_id INTEGER,
    estado_saude_id INTEGER,
    estado_desenvolvimento_id INTEGER,
    
    UNIQUE (especie_id, estado_saude_id, estado_desenvolvimento_id),
    
    CONSTRAINT fk_especie_preco FOREIGN KEY (especie_id) REFERENCES especies (id),
    CONSTRAINT fk_estado_saude_preco FOREIGN KEY (estado_saude_id) REFERENCES estados_saude (id),
    CONSTRAINT fk_estado_desenv_preco FOREIGN KEY (estado_desenvolvimento_id) REFERENCES estados_desenvolvimento (id)
);

--  TABELA Peixes
CREATE TABLE Peixes (
    id SERIAL PRIMARY KEY,
    sexo VARCHAR(10) CHECK (sexo IN ('Macho', 'Femea', 'Indefinido')),
    data_nascimento DATE,
    
    -- Chaves Estrangeiras obrigatórias
    especie_id INTEGER NOT NULL,
    estado_saude_id INTEGER NOT NULL,
    estado_desenvolvimento_id INTEGER NOT NULL,

    -- Chaves Estrangeiras possíveis/opcionais
    lote_id INTEGER,

    CONSTRAINT fk_especie_peixe FOREIGN KEY (especie_id) REFERENCES especies (id),
    CONSTRAINT fk_lote_peixe FOREIGN KEY (lote_id) REFERENCES lotes (id),
    CONSTRAINT fk_estado_saude_peixe FOREIGN KEY (estado_saude_id) REFERENCES estados_saude (id),
    CONSTRAINT fk_estado_desenv_peixe FOREIGN KEY (estado_desenvolvimento_id) REFERENCES estados_desenvolvimento (id)
);

--ÁREA DOS USUÁRIOS:

CREATE TABLE usuarios (
    id SERIAL PRIMARY KEY,
    username VARCHAR(100) UNIQUE NOT NULL,
    password_hash TEXT NOT NULL,
    cargo VARCHAR(50) NOT NULL
);

CREATE TABLE carrinhos (
    id SERIAL PRIMARY KEY,
    usuario_id INTEGER UNIQUE NOT NULL, -- UNIQUE garante que cada cliente só tem 1 carrinho ativo
    data_criacao TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_usuario_carrinho FOREIGN KEY (usuario_id) REFERENCES usuarios (id)
);

CREATE TABLE itens_carrinho (
    id SERIAL PRIMARY KEY,
    carrinho_id INTEGER NOT NULL,
    peixe_id INTEGER UNIQUE NOT NULL, -- Garante que o mesmo peixe não pode estar em dois carrinhos ao mesmo tempo
    
    CONSTRAINT fk_carrinho_item FOREIGN KEY (carrinho_id) REFERENCES carrinhos (id),
    CONSTRAINT fk_peixe_item FOREIGN KEY (peixe_id) REFERENCES peixes (id)
);

-- O "equivalente" desses dois últimos para o evento de finalização de compra:
CREATE TABLE pedidos (
    id SERIAL PRIMARY KEY,
    usuario_id INTEGER NOT NULL,
    data_pedido TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    valor_total DECIMAL(10,2) NOT NULL,
    status VARCHAR(20) CHECK (status IN ('AguardandoPagamento', 'Aprovado', 'EmTransporte', 'Cancelado')) DEFAULT 'Aprovado', -- Como é uma emulação, já nasce aprovado
    
    CONSTRAINT fk_usuario_pedido FOREIGN KEY (usuario_id) REFERENCES usuarios (id)
);


CREATE TABLE itens_pedido (
    id SERIAL PRIMARY KEY,
    pedido_id INTEGER NOT NULL,
    peixe_id INTEGER UNIQUE NOT NULL, -- O mesmo peixe não pode ser vendido duas vezes
    preco_no_momento DECIMAL(8,2) NOT NULL,
    
    CONSTRAINT fk_pedido_item FOREIGN KEY (pedido_id) REFERENCES pedidos (id),
    CONSTRAINT fk_peixe_pedido FOREIGN KEY (peixe_id) REFERENCES peixes (id)
);

--little nuker just in case--

-- DROP TABLE IF EXISTS peixes CASCADE;
-- DROP TABLE IF EXISTS precos CASCADE;
-- DROP TABLE IF EXISTS lotes CASCADE;
-- DROP TABLE IF EXISTS especies CASCADE;
-- DROP TABLE IF EXISTS estados_desenvolvimento CASCADE;
-- DROP TABLE IF EXISTS estados_saude CASCADE;
-- DROP TABLE IF EXISTS comportamentos CASCADE;
-- DROP TABLE IF EXISTS usuarios CASCADE;
-- DROP TABLE IF EXISTS carrinhos CASCADE;
-- DROP TABLE IF EXISTS guildas_troficas CASCADE;
-- DROP TABLE IF EXISTS itens_carrinho CASCADE;
-- DROP TABLE IF EXISTS pedidos CASCADE;
-- DROP TABLE IF EXISTS itens_pedido CASCADE;