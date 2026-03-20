using Npgsql;
using System.Data;
using Estoque.models;

namespace Estoque.Repositories;

public class ItemEstoqueRepository
{
    private readonly NpgsqlConnection _connection;

    public ItemEstoqueRepository(IDbConnection connection)
    {
        _connection = (NpgsqlConnection)connection ?? throw new ArgumentNullException(nameof(connection));
    }

    public async Task<List<ItemEstoque>> ObterItensEstoque()
    {
        const string sql = @"
            SELECT
                id,
                unidade_organizacional_id,
                espaco,
                descricao,
                tipo_unidade_medida,
                quantidade
            FROM
                estoque.item_estoque
            ORDER BY
                descricao;
        ";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, _connection);
            await using var reader = await cmd.ExecuteReaderAsync();

            var itens = new List<ItemEstoque>();

            while (await reader.ReadAsync())
            {
                itens.Add(new ItemEstoque
                {
                    ItemEstoqueId = reader.GetInt32(reader.GetOrdinal("id")),
                    UnidadeOrganizacionalId = reader.GetInt32(reader.GetOrdinal("unidade_organizacional_id")),
                    Espaco = reader.GetInt32(reader.GetOrdinal("espaco")),
                    Descricao = reader.GetString(reader.GetOrdinal("descricao")),
                    TipoUnidadeMedida = (TipoUnidadeMedida)reader.GetInt32(reader.GetOrdinal("tipo_unidade_medida")),
                    Quantidade = reader.GetDecimal(reader.GetOrdinal("quantidade"))
                });
            }

            return itens;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    public async Task<ItemEstoque?> ObterItemEstoquePorId(int id)
    {
        const string sql = @"
            SELECT
                id,
                unidade_organizacional_id,
                espaco,
                descricao,
                tipo_unidade_medida,
                quantidade
            FROM
                estoque.item_estoque
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

            return new ItemEstoque
            {
                ItemEstoqueId = reader.GetInt32(reader.GetOrdinal("id")),
                UnidadeOrganizacionalId = reader.GetInt32(reader.GetOrdinal("unidade_organizacional_id")),
                Espaco = reader.GetInt32(reader.GetOrdinal("espaco")),
                Descricao = reader.GetString(reader.GetOrdinal("descricao")),
                TipoUnidadeMedida = (TipoUnidadeMedida)reader.GetInt32(reader.GetOrdinal("tipo_unidade_medida")),
                Quantidade = reader.GetDecimal(reader.GetOrdinal("quantidade"))
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    public async Task<int> CadastrarItemEstoque(ItemEstoque item)
    {
        const string sql = @"
            INSERT INTO estoque.item_estoque
            (
                unidade_organizacional_id,
                espaco,
                descricao,
                tipo_unidade_medida,
                quantidade
            )
            VALUES
            (
                @unidade_organizacional_id,
                @espaco,
                @descricao,
                @tipo_unidade_medida,
                @quantidade
            )
            RETURNING id;
        ";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, _connection);

            cmd.Parameters.AddWithValue("unidade_organizacional_id", item.UnidadeOrganizacionalId);
            cmd.Parameters.AddWithValue("espaco", item.Espaco);
            cmd.Parameters.AddWithValue("descricao", item.Descricao);
            cmd.Parameters.AddWithValue("tipo_unidade_medida", (int)item.TipoUnidadeMedida);
            cmd.Parameters.AddWithValue("quantidade", item.Quantidade);

            var result = await cmd.ExecuteScalarAsync();

            return (int)result!;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    public async Task<bool> AtualizarItemEstoque(ItemEstoque item)
    {
        const string sql = @"
            UPDATE
                estoque.item_estoque
            SET
                unidade_organizacional_id = @unidade_organizacional_id,
                espaco = @espaco,
                descricao = @descricao,
                tipo_unidade_medida = @tipo_unidade_medida,
                quantidade = @quantidade
            WHERE
                id = @id;
        ";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, _connection);

            cmd.Parameters.AddWithValue("id", item.ItemEstoqueId);
            cmd.Parameters.AddWithValue("unidade_organizacional_id", item.UnidadeOrganizacionalId);
            cmd.Parameters.AddWithValue("espaco", item.Espaco);
            cmd.Parameters.AddWithValue("descricao", item.Descricao);
            cmd.Parameters.AddWithValue("tipo_unidade_medida", (int)item.TipoUnidadeMedida);
            cmd.Parameters.AddWithValue("quantidade", item.Quantidade);

            var rowsAffected = await cmd.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    public async Task<bool> ExcluirItemEstoque(int id)
    {
        const string sql = @"
            DELETE FROM
                estoque.item_estoque
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