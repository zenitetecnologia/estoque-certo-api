#  Configuração da Base de Dados (PostgreSQL + Docker)

O back-end deste projeto utiliza **PostgreSQL** com a biblioteca nativa **Npgsql** (ADO.NET). Para facilitar o ambiente de desenvolvimento local e evitar a instalação direta da base de dados na sua máquina, utilizamos o **Docker**.

Siga os passos abaixo para preparar a base de dados:

## Passo 1: Subir o contentor do PostgreSQL

Certifique-se de que tem o Docker Desktop instalado e a correr. Abra o seu terminal e execute o seguinte comando para baixar a imagem do Postgres e iniciar a base de dados `estoque` na porta `5432`:

> docker run --name pg-estoque -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=admin -e POSTGRES_DB=estoque -p 5432:5432 -d postgres:latest

---

## Passo 2: Conectar à Base de Dados

Utilize um gestor de base de dados da sua preferência (recomendamos o DBeaver ou o pgAdmin). Crie uma nova ligação PostgreSQL com as seguintes credenciais:

* **Host:** `localhost`
* **Porta:** `5432`
* **Database:** `estoque`
* **Username:** `postgres`
* **Password:** `admin`

---

## Passo 3: Executar o Script de Criação das Tabelas

Abra um novo Editor SQL na sua ligação e execute o script abaixo. O script irá criar o esquema e todas as tabelas necessárias:

```sql
-- 1. Criar o Esquema Principal
CREATE SCHEMA IF NOT EXISTS estoque;

-- 2. Tabela de Unidades Organizacionais (Matriz e Filiais)
CREATE TABLE IF NOT EXISTS estoque.unidades_organizacionais (
    id SERIAL PRIMARY KEY,
    id_ou_matriz INTEGER NOT NULL,
    cpnj VARCHAR(20),
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

-- 3. Tabela de Usuários (Utiliza Array nativo para as unidades)
CREATE TABLE IF NOT EXISTS estoque.usuarios (
    id SERIAL PRIMARY KEY,
    username VARCHAR(100) UNIQUE NOT NULL,
    senha TEXT NOT NULL,
    nome VARCHAR(150) NOT NULL,
    telefone VARCHAR(50),
    validado BOOLEAN DEFAULT FALSE,
    perfil INTEGER NOT NULL,
    id_unidades_organizacionais INTEGER[] 
);

-- 4. Tabela de Espaços (Locais físicos de armazenamento)
CREATE TABLE IF NOT EXISTS estoque.espacos (
    id SERIAL PRIMARY KEY,
    id_unidade_organizacional INTEGER NOT NULL,
    nome VARCHAR(150) NOT NULL,
    descricao TEXT
);

-- 5. Tabela de Itens de Estoque (Produtos)
CREATE TABLE IF NOT EXISTS estoque.itens_estoque (
    id SERIAL PRIMARY KEY,
    id_unidade_organizacional INTEGER NOT NULL,
    espaco INTEGER NOT NULL,
    descricao TEXT NOT NULL,
    tipo_unidade_medida INTEGER NOT NULL,
    quantidade NUMERIC(18,4) NOT NULL DEFAULT 0
);

-- 6. Tabela de Histórico (Auditoria de Movimentações)
CREATE TABLE IF NOT EXISTS estoque.historico (
    id SERIAL PRIMARY KEY,
    id_item_estoque INTEGER NOT NULL,
    tipo_movimentacao INTEGER NOT NULL,
    id_usuario INTEGER NOT NULL,
    data_hora TIMESTAMP NOT NULL DEFAULT NOW(),
    quantidade_anterior NUMERIC(18,4) NOT NULL,
    quantidade_resultante NUMERIC(18,4) NOT NULL
);
