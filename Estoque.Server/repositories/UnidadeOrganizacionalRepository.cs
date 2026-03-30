using Estoque.models;
using Npgsql;
using System.Data;

namespace Estoque.Repositories;

public class UnidadeOrganizacionalRepository
{
    private readonly NpgsqlConnection _connection;

    public UnidadeOrganizacionalRepository(IDbConnection connection)
    {
        _connection = (NpgsqlConnection)connection ?? throw new ArgumentNullException(nameof(connection));
    }

    public async Task<int> CriarUnidade(UnidadeOrganizacional unidade)
    {
        const string sql = @"
            INSERT INTO estoque.unidade_organizacional 
            (
                id_matriz,
                Cnpj,
                razao_social,
                nome_fantasia,
                cep,
                numero,
                complemento,
                bairro,
                cidade,
                uf,
                pais,
                telefone,
                email
            )
            VALUES
            (
                @id_matriz,
                @Cnpj,
                @razao_social,
                @nome_fantasia,
                @cep,
                @numero,
                @complemento,
                @bairro,
                @cidade,
                @uf,
                @pais,
                @telefone,
                @email
            )
            RETURNING unidade_organizacional_id;
        ";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, _connection);

            cmd.Parameters.AddWithValue("id_matriz", unidade.IdMatriz);
            cmd.Parameters.AddWithValue("Cnpj", unidade.Cnpj);
            cmd.Parameters.AddWithValue("razao_social", unidade.RazaoSocial);
            cmd.Parameters.AddWithValue("nome_fantasia", unidade.NomeFantasia);
            cmd.Parameters.AddWithValue("cep", unidade.Cep);
            cmd.Parameters.AddWithValue("numero", unidade.Numero);
            cmd.Parameters.AddWithValue("complemento", unidade.Complemento);
            cmd.Parameters.AddWithValue("bairro", unidade.Bairro);
            cmd.Parameters.AddWithValue("cidade", unidade.Cidade);
            cmd.Parameters.AddWithValue("uf", unidade.Uf);
            cmd.Parameters.AddWithValue("pais", unidade.Pais);
            cmd.Parameters.AddWithValue("telefone", unidade.Telefone);
            cmd.Parameters.AddWithValue("email", unidade.Email);

            var result = await cmd.ExecuteScalarAsync();

            return (int)result!;
        }
        catch
        {
            throw;
        }
    }

    public async Task<bool> AtualizarUnidade(UnidadeOrganizacional unidade, int unidadeOrganizacionalId)
    {
        const string sql = @"
        UPDATE
            estoque.unidade_organizacional
        SET
            id_matriz = @id_matriz,
            Cnpj = @Cnpj,
            razao_social = @razao_social,
            nome_fantasia = @nome_fantasia,
            cep = @cep,
            numero = @numero,
            complemento = @complemento,
            bairro = @bairro,
            cidade = @cidade,
            uf = @uf,
            pais = @pais,
            telefone = @telefone,
            email = @email
        WHERE
            unidade_organizacional_id = @unidade_organizacional_id;
    ";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, _connection);

            cmd.Parameters.AddWithValue("unidade_organizacional_id", unidadeOrganizacionalId);

            cmd.Parameters.AddWithValue("id_matriz", unidade.IdMatriz);
            cmd.Parameters.AddWithValue("Cnpj", unidade.Cnpj);
            cmd.Parameters.AddWithValue("razao_social", unidade.RazaoSocial);
            cmd.Parameters.AddWithValue("nome_fantasia", unidade.NomeFantasia);
            cmd.Parameters.AddWithValue("cep", unidade.Cep);
            cmd.Parameters.AddWithValue("numero", unidade.Numero);
            cmd.Parameters.AddWithValue("complemento", unidade.Complemento);
            cmd.Parameters.AddWithValue("bairro", unidade.Bairro);
            cmd.Parameters.AddWithValue("cidade", unidade.Cidade);
            cmd.Parameters.AddWithValue("uf", unidade.Uf);
            cmd.Parameters.AddWithValue("pais", unidade.Pais);
            cmd.Parameters.AddWithValue("telefone", unidade.Telefone);
            cmd.Parameters.AddWithValue("email", unidade.Email);

            var rowsAffected = await cmd.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
        catch
        {
            throw;
        }
    }

    public async Task<List<UnidadeOrganizacional>> ObterUnidades()
    {
        const string sql = @"
            SELECT
                unidade_organizacional_id,
                id_matriz,
                Cnpj,
                razao_social,
                nome_fantasia,
                cep,
                numero,
                complemento,
                bairro,
                cidade,
                uf,
                pais,
                telefone,
                email
            FROM
                estoque.unidade_organizacional
            ORDER BY
                razao_social;
        ";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, _connection);
            await using var reader = await cmd.ExecuteReaderAsync();

            var unidades = new List<UnidadeOrganizacional>();

            while (await reader.ReadAsync())
            {
                unidades.Add(new UnidadeOrganizacionalRecuperado
                {
                    UnidadeOrganizacionalId = (int)reader["unidade_organizacional_id"],
                    IdMatriz = (int)reader["id_matriz"],
                    Cnpj = (string)reader["cnpj"],
                    RazaoSocial = (string)reader["razao_social"],
                    NomeFantasia = (string)reader["nome_fantasia"],
                    Cep = (string)reader["cep"],
                    Numero = (string)reader["numero"],
                    Complemento = (string)reader["complemento"],
                    Bairro = (string)reader["bairro"],
                    Cidade = (string)reader["cidade"],
                    Uf = (string)reader["uf"],
                    Pais = (string)reader["pais"],
                    Telefone = (string)reader["telefone"],
                    Email = (string)reader["email"]
                });
            }
            return unidades;
        }
        catch
        {
            throw;
        }
    }

    public async Task<UnidadeOrganizacional?> ObterUnidade(int unidadeOrganizacionald)
    {
        const string sql = @"
            SELECT
                unidade_organizacional_id,
                id_matriz,
                Cnpj,
                razao_social,
                nome_fantasia,
                cep,
                numero,
                complemento,
                bairro,
                cidade,
                uf,
                pais,
                telefone,
                email
            FROM
                estoque.unidade_organizacional
            WHERE
                unidade_organizacional_id = @unidade_organizacional_id
            LIMIT 1;
        ";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, _connection);
            cmd.Parameters.AddWithValue("unidade_organizacional_id", unidadeOrganizacionald);

            await using var reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync()) return null;

            return new UnidadeOrganizacionalRecuperado
            {
                UnidadeOrganizacionalId = (int)reader["unidade_organizacional_id"],
                IdMatriz = (int)reader["id_matriz"],
                Cnpj = (string)reader["cnpj"],
                RazaoSocial = (string)reader["razao_social"],
                NomeFantasia = (string)reader["nome_fantasia"],
                Cep = (string)reader["cep"],
                Numero = (string)reader["numero"],
                Complemento = (string)reader["complemento"],
                Bairro = (string)reader["bairro"],
                Cidade = (string)reader["cidade"],
                Uf = (string)reader["uf"],
                Pais = (string)reader["pais"],
                Telefone = (string)reader["telefone"],
                Email = (string)reader["email"]
            };
        }
        catch
        {
            throw;
        }
    }

    public async Task<bool> ExcluirUnidade(int id)
    {
        const string sql = @"
            DELETE FROM
                estoque.unidade_organizacional
            WHERE
                unidade_organizacional_id = @unidade_organizacional_id;
        ";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, _connection);
            cmd.Parameters.AddWithValue("unidade_organizacional_id", id);

            var affected = await cmd.ExecuteNonQueryAsync();
            return affected > 0;
        }
        catch
        {
            throw;
        }
    }

    public async Task<bool> VerificaExisteUnidade(string Cnpj, int ignoreId)
    {
        const string sql = @"
            SELECT
                1
            FROM
                estoque.unidade_organizacional
            WHERE
                Cnpj = @Cnpj
            AND
                unidade_organizacional_id <> @ignoreunidade_organizacional_id
            LIMIT 1;
        ";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, _connection);
            cmd.Parameters.AddWithValue("Cnpj", Cnpj);
            cmd.Parameters.AddWithValue("ignoreunidade_organizacional_id", ignoreId);

            var result = await cmd.ExecuteScalarAsync();
            return result != null;
        }
        catch
        {
            throw;
        }
    }

    private async Task EnsureOpenAsync()
    {
        if (_connection.State != ConnectionState.Open) await _connection.OpenAsync();
    }
}