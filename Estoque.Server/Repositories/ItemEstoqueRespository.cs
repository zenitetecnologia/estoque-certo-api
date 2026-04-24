using Estoque.Server.Models;
using Npgsql;
using System.Data;

namespace Estoque.Server.Repositories;

public class ItemEstoqueRepository : BaseRepository
{
    public ItemEstoqueRepository(IDbConnection connection) : base(connection) { }

    public async Task<Guid> CadastrarItemEstoque(ItemEstoque item)
    {
        const string sql = @"
            INSERT INTO estoque_certo.item_estoque
            (
                unidade_organizacional_id,
                espaco_id,
                descricao,
                tipo_unidade_medida,
                quantidade
            )
            VALUES
            (
                @unidade_organizacional_id,
                @espaco_id,
                @descricao,
                @tipo_unidade_medida,
                @quantidade
            )
            RETURNING item_estoque_id;
        ";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, Connection);

            cmd.Parameters.AddWithValue("unidade_organizacional_id", item.UnidadeOrganizacionalId);
            cmd.Parameters.AddWithValue("espaco_id", item.EspacoId);
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
                estoque_certo.item_estoque
            SET
                espaco_id = @espaco_id,
                descricao = @descricao,
                tipo_unidade_medida = @tipo_unidade_medida,
                quantidade = @quantidade
            WHERE
                item_estoque_id = @id;
        ";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, Connection);

            cmd.Parameters.AddWithValue("id", itemEstoqueId);
            cmd.Parameters.AddWithValue("espaco_id", item.EspacoId);
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

    public async Task<List<ItemEstoqueRecuperado>> ObterItens(int skip, int top, string? descricao, Guid? unidadeOrganizacionalId, Guid? espacoId)
    {
        var itens = new List<ItemEstoqueRecuperado>();

        string sql = @"
            SELECT
                item_estoque_id,
                unidade_organizacional_id,
                espaco_id,
                descricao,
                tipo_unidade_medida,
                quantidade
            FROM
                estoque_certo.item_estoque
            WHERE 1 = 1
        ";

        if (!string.IsNullOrEmpty(descricao)) sql += " AND descricao ILIKE @descricao";

        if (unidadeOrganizacionalId != null) sql += " AND unidade_organizacional_id = @unidade_organizacional_id";

        if (espacoId != null) sql += " AND espaco_id = @espaco_id";

        sql += " ORDER BY descricao LIMIT @top OFFSET @skip";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, Connection);

            cmd.Parameters.AddWithValue("descricao", $"%{descricao}%");
            cmd.Parameters.AddWithValue("unidade_organizacional_id", unidadeOrganizacionalId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("espaco_id", espacoId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("top", top);
            cmd.Parameters.AddWithValue("skip", skip);

            await using var reader = await cmd.ExecuteReaderAsync();


            while (await reader.ReadAsync())
            {
                var itemEstoqueRecuperado = new ItemEstoqueRecuperado();

                itemEstoqueRecuperado.ItemEstoqueId = reader.GetGuid("item_estoque_id");
                itemEstoqueRecuperado.UnidadeOrganizacionalId = reader.GetGuid("unidade_organizacional_id");
                itemEstoqueRecuperado.EspacoId = reader.GetGuid("espaco_id");
                itemEstoqueRecuperado.Descricao = reader.GetString("descricao");
                itemEstoqueRecuperado.TipoUnidadeMedida = (TipoUnidadeMedida)reader.GetInt32("tipo_unidade_medida");
                itemEstoqueRecuperado.Quantidade = reader.GetDecimal("quantidade");

                itens.Add(itemEstoqueRecuperado);
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
                espaco_id,
                descricao,
                tipo_unidade_medida,
                quantidade
            FROM
                estoque_certo.item_estoque
            WHERE
                item_estoque_id = @id
            LIMIT 1;
        ";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, Connection);
            cmd.Parameters.AddWithValue("id", itemEstoqueId);

            await using var reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync()) return null;

            return new ItemEstoqueRecuperado
            {
                ItemEstoqueId = reader.GetGuid("item_estoque_id"),
                UnidadeOrganizacionalId = reader.GetGuid("unidade_organizacional_id"),
                EspacoId = reader.GetGuid("espaco_id"),
                Descricao = reader.GetString("descricao"),
                TipoUnidadeMedida = (TipoUnidadeMedida)reader.GetInt32("tipo_unidade_medida"),
                Quantidade = reader.GetDecimal("quantidade")
            };
        }
        catch
        {
            throw;
        }
    }

    public async Task<int> ExcluirItemEstoque(Guid itemEstoqueId)
    {
        const string sql = "DELETE FROM estoque_certo.item_estoque WHERE item_estoque_id = @id";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, Connection);
            cmd.Parameters.AddWithValue("id", itemEstoqueId);

            return await cmd.ExecuteNonQueryAsync();
        }
        catch
        {
            throw;
        }
    }

    public async Task<bool> MovimentarEstoque(Guid itemEstoqueId, decimal quantidadeMovimento, TipoMovimentacao tipoMovimentacao, Guid? usuarioId)
    {
        string operacao = tipoMovimentacao == TipoMovimentacao.Acrescimo ? "+" : "-";

        string sql = $@"
            WITH updated_item AS (
                UPDATE 
                    estoque_certo.item_estoque
                SET 
                    quantidade = quantidade {operacao} @quantidade_movimento
                WHERE 
                    item_estoque_id = @item_estoque_id
                    AND (quantidade {operacao} @quantidade_movimento) >= 0
                RETURNING 
                    item_estoque_id, 
                    
                    quantidade {(operacao == "+" ? "-" : "+")} @quantidade_movimento AS qtd_anterior, 
                    quantidade AS qtd_resultante
            )
            INSERT INTO estoque_certo.historico
            (
                item_estoque_id,
                tipo_movimentacao,
                usuario_id,
                quantidade_anterior,
                quantidade_resultante
            )
            SELECT
                item_estoque_id,
                @tipo_movimentacao,
                @usuario_id,
                qtd_anterior,
                qtd_resultante
            FROM
                updated_item
            RETURNING
                item_estoque_id;
        ";

        try
        {
            await EnsureOpenAsync();
            await using var cmd = new NpgsqlCommand(sql, Connection);

            cmd.Parameters.AddWithValue("item_estoque_id", itemEstoqueId);
            cmd.Parameters.AddWithValue("quantidade_movimento", quantidadeMovimento);
            cmd.Parameters.AddWithValue("tipo_movimentacao", (int)tipoMovimentacao);
            cmd.Parameters.AddWithValue("usuario_id", usuarioId.HasValue ? (object)usuarioId.Value : DBNull.Value);

            var result = await cmd.ExecuteScalarAsync();
            return result != null;
        }
        catch
        {
            throw;
        }
    }
}