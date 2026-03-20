using Npgsql;
using System.Data;
using Estoque.models;

namespace Estoque.Repositories;

public class UnidadeOrganizacionalRepository
{
    private readonly NpgsqlConnection _connection;

    public UnidadeOrganizacionalRepository(IDbConnection connection)
    {
        _connection = (NpgsqlConnection)connection ?? throw new ArgumentNullException(nameof(connection));
    }

    public async Task<List<UnidadeOrganizacional>> ObterUnidades()
    {
        const string sql = @"
            SELECT
                id,
                id_ou_matriz,
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
                unidades.Add(new UnidadeOrganizacional
                {
                    UnidadeOrganizacionalId = reader.GetInt32(reader.GetOrdinal("id")),
                    IdMatriz = reader.GetInt32(reader.GetOrdinal("id_ou_matriz")),
                    Cnpj = reader.IsDBNull(reader.GetOrdinal("Cnpj")) ? string.Empty : reader.GetString(reader.GetOrdinal("Cnpj")),
                    RazaoSocial = reader.IsDBNull(reader.GetOrdinal("razao_social")) ? string.Empty : reader.GetString(reader.GetOrdinal("razao_social")),
                    NomeFantasia = reader.IsDBNull(reader.GetOrdinal("nome_fantasia")) ? string.Empty : reader.GetString(reader.GetOrdinal("nome_fantasia")),
                    Cep = reader.IsDBNull(reader.GetOrdinal("cep")) ? string.Empty : reader.GetString(reader.GetOrdinal("cep")),
                    Numero = reader.IsDBNull(reader.GetOrdinal("numero")) ? string.Empty : reader.GetString(reader.GetOrdinal("numero")),
                    Complemento = reader.IsDBNull(reader.GetOrdinal("complemento")) ? string.Empty : reader.GetString(reader.GetOrdinal("complemento")),
                    Bairro = reader.IsDBNull(reader.GetOrdinal("bairro")) ? string.Empty : reader.GetString(reader.GetOrdinal("bairro")),
                    Cidade = reader.IsDBNull(reader.GetOrdinal("cidade")) ? string.Empty : reader.GetString(reader.GetOrdinal("cidade")),
                    Uf = reader.IsDBNull(reader.GetOrdinal("uf")) ? string.Empty : reader.GetString(reader.GetOrdinal("uf")),
                    Pais = reader.IsDBNull(reader.GetOrdinal("pais")) ? string.Empty : reader.GetString(reader.GetOrdinal("pais")),
                    Telefone = reader.IsDBNull(reader.GetOrdinal("telefone")) ? string.Empty : reader.GetString(reader.GetOrdinal("telefone")),
                    Email = reader.IsDBNull(reader.GetOrdinal("email")) ? string.Empty : reader.GetString(reader.GetOrdinal("email"))
                });
            }

            return unidades;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    public async Task<UnidadeOrganizacional?> ObterUnidadePorId(int id)
    {
        const string sql = @"
            SELECT
                id,
                id_ou_matriz,
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
                id = @id
            LIMIT 1;
        ";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, _connection);
            cmd.Parameters.AddWithValue("id", id);

            await using var reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync()) return null;

            return new UnidadeOrganizacional
            {
                UnidadeOrganizacionalId = reader.GetInt32(reader.GetOrdinal("id")),
                IdMatriz = reader.GetInt32(reader.GetOrdinal("id_ou_matriz")),
                Cnpj = reader.IsDBNull(reader.GetOrdinal("Cnpj")) ? string.Empty : reader.GetString(reader.GetOrdinal("Cnpj")),
                RazaoSocial = reader.IsDBNull(reader.GetOrdinal("razao_social")) ? string.Empty : reader.GetString(reader.GetOrdinal("razao_social")),
                NomeFantasia = reader.IsDBNull(reader.GetOrdinal("nome_fantasia")) ? string.Empty : reader.GetString(reader.GetOrdinal("nome_fantasia")),
                Cep = reader.IsDBNull(reader.GetOrdinal("cep")) ? string.Empty : reader.GetString(reader.GetOrdinal("cep")),
                Numero = reader.IsDBNull(reader.GetOrdinal("numero")) ? string.Empty : reader.GetString(reader.GetOrdinal("numero")),
                Complemento = reader.IsDBNull(reader.GetOrdinal("complemento")) ? string.Empty : reader.GetString(reader.GetOrdinal("complemento")),
                Bairro = reader.IsDBNull(reader.GetOrdinal("bairro")) ? string.Empty : reader.GetString(reader.GetOrdinal("bairro")),
                Cidade = reader.IsDBNull(reader.GetOrdinal("cidade")) ? string.Empty : reader.GetString(reader.GetOrdinal("cidade")),
                Uf = reader.IsDBNull(reader.GetOrdinal("uf")) ? string.Empty : reader.GetString(reader.GetOrdinal("uf")),
                Pais = reader.IsDBNull(reader.GetOrdinal("pais")) ? string.Empty : reader.GetString(reader.GetOrdinal("pais")),
                Telefone = reader.IsDBNull(reader.GetOrdinal("telefone")) ? string.Empty : reader.GetString(reader.GetOrdinal("telefone")),
                Email = reader.IsDBNull(reader.GetOrdinal("email")) ? string.Empty : reader.GetString(reader.GetOrdinal("email"))
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    public async Task<int> CadastrarUnidade(UnidadeOrganizacional unidade)
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
                @id_ou_matriz,
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
            RETURNING id;
        ";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, _connection);

            cmd.Parameters.AddWithValue("id_ou_matriz", unidade.IdMatriz);
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
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    public async Task<bool> AtualizarUnidade(UnidadeOrganizacional unidade)
    {
        const string sql = @"
            UPDATE
                estoque.unidade_organizacional
            SET
                id_ou_matriz = @id_ou_matriz,
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
                id = @id;
        ";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, _connection);
            cmd.Parameters.AddWithValue("id", unidade.UnidadeOrganizacionalId);
            cmd.Parameters.AddWithValue("id_ou_matriz", unidade.IdMatriz);
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
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    public async Task<bool> ExcluirUnidade(int id)
    {
        const string sql = @"
            DELETE FROM
                estoque.unidade_organizacional
            WHERE
                id = @id;
        ";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, _connection);
            cmd.Parameters.AddWithValue("id", id);

            var affected = await cmd.ExecuteNonQueryAsync();
            return affected > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
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
                id <> @ignoreId
            LIMIT 1;
        ";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, _connection);
            cmd.Parameters.AddWithValue("Cnpj", Cnpj);
            cmd.Parameters.AddWithValue("ignoreId", ignoreId);

            var result = await cmd.ExecuteScalarAsync();
            return result != null;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    private async Task EnsureOpenAsync()
    {
        if (_connection.State != ConnectionState.Open) await _connection.OpenAsync();
    }
}