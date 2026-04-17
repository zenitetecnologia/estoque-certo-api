-- docker run --name pg-estoque -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=admin -e POSTGRES_DB=estoque_certo -p 5432:5432 -d postgres:latest


-- 1. Criar o Esquema Principal
CREATE SCHEMA IF NOT EXISTS estoque_certo;

-- 2. Tabela de Unidades Organizacionais (Matriz e Filiais)
CREATE TABLE IF NOT EXISTS estoque_certo.unidade_organizacional (
    unidade_organizacional_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    id_matriz UUID,
    cnpj VARCHAR(20),
    razao_social TEXT,
    nome_fantasia TEXT,
    cep VARCHAR(20),
    numero VARCHAR(50),
    complemento TEXT,
    bairro TEXT,
    cidade TEXT,
    uf VARCHAR(2),
    pais VARCHAR(50),
    telefone VARCHAR(50),
    email VARCHAR(150)
);

-- 3. Tabela de Usuários (Relação 1:N direta com Unidade Organizacional)
CREATE TABLE IF NOT EXISTS estoque_certo.usuario (
    usuario_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    username VARCHAR(100) NOT NULL,
    senha TEXT NOT NULL,
    nome VARCHAR(150) NOT NULL,
    telefone VARCHAR(50),
    perfil INTEGER NOT NULL,
    unidade_organizacional_id UUID NOT NULL,
    valido BOOLEAN NOT NULL DEFAULT FALSE,
    
    -- Restrição que permite o mesmo username apenas se for em unidades diferentes
    CONSTRAINT uk_usuario_username_unidade UNIQUE (username, unidade_organizacional_id),
    
    -- Chave estrangeira para garantir a integridade referencial
    CONSTRAINT fk_usuario_unidade FOREIGN KEY (unidade_organizacional_id) 
        REFERENCES estoque_certo.unidade_organizacional (unidade_organizacional_id) ON DELETE CASCADE
);

-- 4. Tabela de Espaços (Locais físicos de armazenamento)
CREATE TABLE IF NOT EXISTS estoque_certo.espaco (
    espaco_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    unidade_organizacional_id UUID NOT NULL,
    nome VARCHAR(150) NOT NULL,
    descricao TEXT,
    
    CONSTRAINT fk_espaco_unidade FOREIGN KEY (unidade_organizacional_id) 
        REFERENCES estoque_certo.unidade_organizacional (unidade_organizacional_id) ON DELETE CASCADE
);

-- 5. Tabela de Itens de Estoque (Produtos)
CREATE TABLE IF NOT EXISTS estoque_certo.item_estoque (
    item_estoque_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    unidade_organizacional_id UUID NOT NULL,
    espaco_id UUID NOT NULL, -- CORREÇÃO: Alterado de 'espaco' para 'espaco_id'
    descricao TEXT NOT NULL,
    tipo_unidade_medida INTEGER NOT NULL, -- Enum mantido como INTEGER
    quantidade NUMERIC(18,4) NOT NULL DEFAULT 0,
    
    CONSTRAINT fk_item_unidade FOREIGN KEY (unidade_organizacional_id) 
        REFERENCES estoque_certo.unidade_organizacional (unidade_organizacional_id) ON DELETE CASCADE,
        
    -- Nova chave estrangeira atualizada para usar espaco_id
    CONSTRAINT fk_item_espaco FOREIGN KEY (espaco_id) 
        REFERENCES estoque_certo.espaco (espaco_id) ON DELETE RESTRICT
);

-- 6. Tabela de Histórico (Auditoria de Movimentações)
CREATE TABLE IF NOT EXISTS estoque_certo.historico (
    historico_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    item_estoque_id UUID NOT NULL,
    tipo_movimentacao INTEGER NOT NULL, -- Enum mantido como INTEGER
    usuario_id UUID,
    data_hora TIMESTAMP NOT NULL DEFAULT NOW(),
    quantidade_anterior NUMERIC(18,4) NOT NULL,
    quantidade_resultante NUMERIC(18,4) NOT NULL,
    
    CONSTRAINT fk_historico_item FOREIGN KEY (item_estoque_id) 
        REFERENCES estoque_certo.item_estoque (item_estoque_id) ON DELETE CASCADE,
        
    CONSTRAINT fk_historico_usuario FOREIGN KEY (usuario_id) 
        REFERENCES estoque_certo.usuario (usuario_id) ON DELETE SET NULL
);

INSERT INTO estoque_certo.unidade_organizacional
(
unidade_organizacional_id,
razao_social,
nome_fantasia,
cnpj
)
VALUES
(
'e286f99e-5075-4b73-96d9-af462c36a5a6',
'Zênite Tecnologia LTDA',
'Zênite Tecnologia',
'60935686000134'
);

INSERT INTO estoque_certo.espaco
(
espaco_id,
unidade_organizacional_id,
nome,
descricao
)
VALUES
(
'6ab71fef-cf4e-4dbf-af76-f2058224721a',
'e286f99e-5075-4b73-96d9-af462c36a5a6',
'Sala 307',
'Sede Moema'
);

INSERT INTO estoque_certo.usuario
(
usuario_id,
username,
senha,
nome,
telefone,
perfil,
unidade_organizacional_id,
valido
)
VALUES
(
'bcfd8e5d-ccdb-46cf-bfe2-da19e5514981',
'teste_de_username_duplicado',
'insomnia',
'teste',
'99966mole60',
'1',
'e286f99e-5075-4b73-96d9-af462c36a5a6',
'true'
);

insert into estoque_certo.item_estoque 
(
	unidade_organizacional_id,
	espaco_id,
	descricao,
	tipo_unidade_medida,
	quantidade
)
values 
(
	'e286f99e-5075-4b73-96d9-af462c36a5a6',
	'6ab71fef-cf4e-4dbf-af76-f2058224721a',
	'bala de caramelo',
	'1',
	'10'
);