using Estoque.Server.Models;
using Npgsql;
using NpgsqlTypes;
using System.Data;

namespace Estoque.Server.Repositories;

public class RelatorioRepository : BaseRepository
{
    public RelatorioRepository(IDbConnection connection) : base(connection) { }

    public async Task<List<PizzaDashboardItem>> ObterPizzaDashboard(
        Guid unidadeOrganizacionalId,
        Guid? espacoId,
        int? tipoUnidadeMedida // ou TipoUnidadeMedida? se preferir enum
    )
    {
        var itens = new List<PizzaDashboardItem>();

        // Sem espacoId -> agrupa por ESPAÇO (pizza por espaço)
        string sql = @"
        SELECT
            e.nome AS label,
            SUM(i.quantidade) AS value
        FROM estoque_certo.item_estoque i
        JOIN estoque_certo.espaco e ON e.espaco_id = i.espaco_id
        WHERE i.unidade_organizacional_id = @unidade_organizacional_id
        ";

        if (espacoId != null)
        {
            // Com espacoId -> agrupa por TIPO DE UNIDADE (pizza dentro do espaço)
            sql = @"
            SELECT
                i.tipo_unidade_medida::text AS label,
                SUM(i.quantidade) AS value
            FROM estoque_certo.item_estoque i
            WHERE i.unidade_organizacional_id = @unidade_organizacional_id
              AND i.espaco_id = @espaco_id
            ";
        }

        if (tipoUnidadeMedida != null)
        {
            sql += " AND i.tipo_unidade_medida = @tipo_unidade_medida";
        }

        if (espacoId == null)
        {
            sql += " GROUP BY e.nome ORDER BY e.nome;";
        }
        else
        {
            sql += " GROUP BY i.tipo_unidade_medida ORDER BY i.tipo_unidade_medida;";
        }

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, Connection);

            cmd.Parameters.Add("unidade_organizacional_id", NpgsqlDbType.Uuid)
                .Value = unidadeOrganizacionalId;

            if (espacoId != null)
            {
                cmd.Parameters.Add("espaco_id", NpgsqlDbType.Uuid).Value = espacoId.Value;
            }

            if (tipoUnidadeMedida != null)
            {
                cmd.Parameters.Add("tipo_unidade_medida", NpgsqlDbType.Integer).Value = tipoUnidadeMedida.Value;
            }

            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                itens.Add(new PizzaDashboardItem
                {
                    Label = reader.GetString("label"),
                    Value = reader.GetDecimal("value")
                });
            }

            return itens;
        }
        catch
        {
            throw;
        }
    }
}