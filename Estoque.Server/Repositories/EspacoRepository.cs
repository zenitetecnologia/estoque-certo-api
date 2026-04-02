using Estoque.Models;
using Npgsql;
using System.Data;

namespace Estoque.Repositories;

public class EspacoRepository
{
    private readonly NpgsqlConnection _connection;

    public EspacoRepository(IDbConnection connection)
    {
        _connection = (NpgsqlConnection)connection ?? throw new ArgumentNullException(nameof(connection));
    }

    public async Task<List<EspacoRecuperado>> ObterEspacos()
    {
        const string sql = @"
            SELECT
                espaco_id,
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

            var espacos = new List<EspacoRecuperado>();

            while (await reader.ReadAsync())
            {
                espacos.Add(new EspacoRecuperado
                {
                    EspacoId = reader.GetGuid(reader.GetOrdinal("espaco_id")),
                    UnidadeOrganizacionalId = reader.GetGuid("unidade_organizacional_id"),
                    Nome = reader.GetString(reader.GetOrdinal("nome")),
                    Descricao = reader.GetSafeString("descricao")
                });
            }

            return espacos;
        }
        catch
        {
            throw;
        }
    }

    public async Task<EspacoRecuperado?> ObterEspaco(Guid espacoId)
    {
        const string sql = @"
            SELECT
                espaco_id,
                unidade_organizacional_id,
                nome,
                descricao
            FROM
                estoque.espaco
            WHERE
                espaco_id = @id
            LIMIT 1;
        ";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, _connection);
            cmd.Parameters.AddWithValue("id", espacoId);

            await using var reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync()) return null;

            return new EspacoRecuperado
            {
                EspacoId = reader.GetGuid(reader.GetOrdinal("espaco_id")),
                UnidadeOrganizacionalId = reader.GetGuid("unidade_organizacional_id"),
                Nome = reader.GetString(reader.GetOrdinal("nome")),
                Descricao = reader.GetSafeString("descricao")
            };
        }
        catch
        {
            throw;
        }
    }

    public async Task<Guid> CadastrarEspaco(Espaco espaco)
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
            RETURNING espaco_id;
        ";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, _connection);

            cmd.Parameters.AddWithValue("unidade_organizacional_id", espaco.UnidadeOrganizacionalId);
            cmd.Parameters.AddWithValue("nome", espaco.Nome);
            cmd.Parameters.AddWithValue("descricao", string.IsNullOrWhiteSpace(espaco.Descricao) ? DBNull.Value : espaco.Descricao);

            var result = await cmd.ExecuteScalarAsync();

            return (Guid)result!;
        }
        catch
        {
            throw;
        }
    }

    public async Task<int> AtualizarEspaco(Espaco espaco, Guid espacoId)
    {
        const string sql = @"
            UPDATE
                estoque.espaco
            SET
                nome = @nome,
                descricao = @descricao
            WHERE
                espaco_id = @id;
        ";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, _connection);

            cmd.Parameters.AddWithValue("id", espacoId);
            cmd.Parameters.AddWithValue("nome", espaco.Nome);
            cmd.Parameters.AddWithValue("descricao", string.IsNullOrWhiteSpace(espaco.Descricao) ? DBNull.Value : espaco.Descricao);

            return await cmd.ExecuteNonQueryAsync();
        }
        catch
        {
            throw;
        }
    }

    public async Task<int> ExcluirEspaco(Guid espacoId)
    {
        const string sql = "DELETE FROM estoque.espaco WHERE espaco_id = @id";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, _connection);
            cmd.Parameters.AddWithValue("id", espacoId);

            return await cmd.ExecuteNonQueryAsync();
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