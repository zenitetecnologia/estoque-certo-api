using Npgsql;
using System.Data;
using Estoque.models;

namespace Estoque.Repositories;

public class EspacoRepository
{
    private readonly NpgsqlConnection _connection;

    public EspacoRepository(IDbConnection connection)
    {
        _connection = (NpgsqlConnection)connection ?? throw new ArgumentNullException(nameof(connection));
    }

    public async Task<List<Espacos>> ObterEspacos()
    {
        const string sql = @"
            SELECT
                id,
                unidade_organizacional_id,
                nome,
                descricao
            FROM
                estoque.espaco
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
                    EspacoId = reader.GetInt32(reader.GetOrdinal("id")),
                    UnidadeOrganizacionalId = reader.GetInt32(reader.GetOrdinal("unidade_organizacional_id")),
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
                unidade_organizacional_id,
                nome,
                descricao
            FROM
                estoque.espaco
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
                EspacoId = reader.GetInt32(reader.GetOrdinal("id")),
                UnidadeOrganizacionalId = reader.GetInt32(reader.GetOrdinal("unidade_organizacional_id")),
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
            INSERT INTO estoque.espaco
            (
                unidade_organizacional_id,
                nome,
                descricao
            )
            VALUES
            (
                @unidade_organizacional_id,
                @nome,
                @descricao
            )
            RETURNING id;
        ";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, _connection);

            cmd.Parameters.AddWithValue("unidade_organizacional_id", espaco.UnidadeOrganizacionalId);
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
                estoque.espaco
            SET
                unidade_organizacional_id = @unidade_organizacional_id,
                nome = @nome,
                descricao = @descricao
            WHERE
                id = @id;
        ";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, _connection);

            cmd.Parameters.AddWithValue("id", espaco.EspacoId);
            cmd.Parameters.AddWithValue("unidade_organizacional_id", espaco.UnidadeOrganizacionalId);
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
                estoque.espaco
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