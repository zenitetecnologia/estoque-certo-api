using Npgsql;
using System.Data;
using Estoque.models;

namespace Estoque.Repositories;

public class EspacosRepository
{
    private readonly NpgsqlConnection _connection;

    public EspacosRepository(IDbConnection connection)
    {
        _connection = (NpgsqlConnection)connection ?? throw new ArgumentNullException(nameof(connection));
    }

    public async Task<List<Espacos>> ObterEspacos()
    {
        const string sql = @"
            SELECT
                id,
                id_unidade_organizacional,
                nome,
                descricao
            FROM
                estoque.espacos
            ORDER BY
                nome;
        ";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, _connection);
            await using var reader = await cmd.ExecuteReaderAsync();

            var espacos = new List<Espacos>();

            while (await reader.ReadAsync())
            {
                espacos.Add(new Espacos
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    IdUnidadeOrganizacional = reader.GetInt32(reader.GetOrdinal("id_unidade_organizacional")),
                    Nome = reader.GetString(reader.GetOrdinal("nome")),
                    Descricao = reader.IsDBNull(reader.GetOrdinal("descricao"))
                        ? string.Empty
                        : reader.GetString(reader.GetOrdinal("descricao"))
                });
            }

            return espacos;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    public async Task<Espacos?> ObterEspacoPorId(int id)
    {
        const string sql = @"
            SELECT
                id,
                id_unidade_organizacional,
                nome,
                descricao
            FROM
                estoque.espacos
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

            return new Espacos
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                IdUnidadeOrganizacional = reader.GetInt32(reader.GetOrdinal("id_unidade_organizacional")),
                Nome = reader.GetString(reader.GetOrdinal("nome")),
                Descricao = reader.IsDBNull(reader.GetOrdinal("descricao"))
                    ? string.Empty
                    : reader.GetString(reader.GetOrdinal("descricao"))
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    public async Task<int> CadastrarEspaco(Espacos espaco)
    {
        const string sql = @"
            INSERT INTO estoque.espacos
            (
                id_unidade_organizacional,
                nome,
                descricao
            )
            VALUES
            (
                @id_unidade_organizacional,
                @nome,
                @descricao
            )
            RETURNING id;
        ";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, _connection);

            cmd.Parameters.AddWithValue("id_unidade_organizacional", espaco.IdUnidadeOrganizacional);
            cmd.Parameters.AddWithValue("nome", espaco.Nome);
            cmd.Parameters.AddWithValue("descricao", (object?)espaco.Descricao ?? DBNull.Value);

            var result = await cmd.ExecuteScalarAsync();

            return (int)result!;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    public async Task<bool> AtualizarEspaco(Espacos espaco)
    {
        const string sql = @"
            UPDATE
                estoque.espacos
            SET
                id_unidade_organizacional = @id_unidade_organizacional,
                nome = @nome,
                descricao = @descricao
            WHERE
                id = @id;
        ";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, _connection);

            cmd.Parameters.AddWithValue("id", espaco.Id);
            cmd.Parameters.AddWithValue("id_unidade_organizacional", espaco.IdUnidadeOrganizacional);
            cmd.Parameters.AddWithValue("nome", espaco.Nome);
            cmd.Parameters.AddWithValue("descricao", (object?)espaco.Descricao ?? DBNull.Value);

            var rowsAffected = await cmd.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    public async Task<bool> ExcluirEspaco(int id)
    {
        const string sql = @"
            DELETE FROM
                estoque.espacos
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

    private async Task EnsureOpenAsync()
    {
        if (_connection.State != ConnectionState.Open) await _connection.OpenAsync();
    }
}