using Estoque.Models;
using Npgsql;
using System.Data;

namespace Estoque.Repositories;

public class ItemEstoqueRepository
{
    private readonly NpgsqlConnection _connection;

    public ItemEstoqueRepository(IDbConnection connection)
    {
        _connection = (NpgsqlConnection)connection ?? throw new ArgumentNullException(nameof(connection));
    }

    public async Task<List<ItemEstoqueRecuperado>> ObterItens()
    {
        const string sql = @"
            SELECT
                item_estoque_id,
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

            var itens = new List<ItemEstoqueRecuperado>();

            while (await reader.ReadAsync())
            {
                itens.Add(new ItemEstoqueRecuperado
                {
                    ItemEstoqueId = reader.GetGuid("item_estoque_id"),
                    UnidadeOrganizacionalId = reader.GetGuid("unidade_organizacional_id"),
                    Espaco = reader.GetInt32("espaco"),
                    Descricao = reader.GetString("descricao"),
                    TipoUnidadeMedida = (Estoque.Models.TipoUnidadeMedida)reader.GetInt32("tipo_unidade_medida"),
                    Quantidade = reader.GetDecimal("quantidade")
                });
            }

            return itens;
        }
        catch
        {
            throw;
        }
    }

    public async Task<ItemEstoqueRecuperado?> ObterItem(Guid itemEstoqueId)
    {
        const string sql = @"
            SELECT
                item_estoque_id,
                unidade_organizacional_id,
                espaco,
                descricao,
                tipo_unidade_medida,
                quantidade
            FROM
                estoque.item_estoque
            WHERE
                item_estoque_id = @id
            LIMIT 1;
        ";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, _connection);
            cmd.Parameters.AddWithValue("id", itemEstoqueId);

            await using var reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync()) return null;

            return new ItemEstoqueRecuperado
            {
                ItemEstoqueId = reader.GetGuid("item_estoque_id"),
                UnidadeOrganizacionalId = reader.GetGuid("unidade_organizacional_id"),
                Espaco = reader.GetInt32("espaco"),
                Descricao = reader.GetString("descricao"),
                TipoUnidadeMedida = (Estoque.Models.TipoUnidadeMedida)reader.GetInt32("tipo_unidade_medida"),
                Quantidade = reader.GetDecimal("quantidade")
            };
        }
        catch
        {
            throw;
        }
    }

    public async Task<Guid> CadastrarItemEstoque(ItemEstoque item)
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
            RETURNING item_estoque_id;
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

            return (Guid)result!;
        }
        catch
        {
            throw;
        }
    }

    public async Task<int> AtualizarItemEstoque(ItemEstoque item, Guid itemEstoqueId)
    {
        const string sql = @"
            UPDATE
                estoque.item_estoque
            SET
                espaco = @espaco,
                descricao = @descricao,
                tipo_unidade_medida = @tipo_unidade_medida,
                quantidade = @quantidade
            WHERE
                item_estoque_id = @id;
        ";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, _connection);

            cmd.Parameters.AddWithValue("id", itemEstoqueId);
            cmd.Parameters.AddWithValue("espaco", item.Espaco);
            cmd.Parameters.AddWithValue("descricao", item.Descricao);
            cmd.Parameters.AddWithValue("tipo_unidade_medida", (int)item.TipoUnidadeMedida);
            cmd.Parameters.AddWithValue("quantidade", item.Quantidade);

            return await cmd.ExecuteNonQueryAsync();
        }
        catch
        {
            throw;
        }
    }

    public async Task<int> ExcluirItemEstoque(Guid itemEstoqueId)
    {
        const string sql = "DELETE FROM estoque.item_estoque WHERE item_estoque_id = @id";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, _connection);
            cmd.Parameters.AddWithValue("id", itemEstoqueId);

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