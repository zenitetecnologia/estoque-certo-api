using Estoque.Server.Models;
using Npgsql;
using NpgsqlTypes;
using System.Data;

namespace Estoque.Server.Repositories;

public class HistoricoRepository : BaseRepository
{
    public HistoricoRepository(IDbConnection connection) : base(connection) { }

    public async Task<List<HistoricoRecuperado>> ObterHistoricoPorItem(Guid itemEstoqueId)
    {
        var historicos = new List<HistoricoRecuperado>();

        const string sql = @"
            SELECT
                historico_id,
                item_estoque_id,
                tipo_movimentacao,
                estoque_certo.historico.usuario_id,
                nome,
                data_hora,
                quantidade_anterior,
                quantidade_resultante
            FROM
                estoque_certo.historico
            LEFT JOIN
                estoque_certo.usuario ON estoque_certo.historico.usuario_id = estoque_certo.usuario.usuario_id
            WHERE
                item_estoque_id = @item_estoque_id
            ORDER BY
                data_hora DESC;
        ";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, Connection);
            cmd.Parameters.Add("item_estoque_id", NpgsqlDbType.Uuid).Value = itemEstoqueId;

            await using var reader = await cmd.ExecuteReaderAsync();


            while (await reader.ReadAsync())
            {
                var historico = new HistoricoRecuperado();

                historico.HistoricoId = reader.GetGuid("historico_id");
                historico.ItemEstoqueId = reader.GetGuid("item_estoque_id");
                historico.TipoMovimentacao = (TipoMovimentacao)reader.GetInt32("tipo_movimentacao");
                historico.UsuarioId = reader.GetGuidNullable("usuario_id");
                historico.Nome = reader.GetStringNullable("nome");
                historico.DataHora = reader.GetDateTime(reader.GetOrdinal("data_hora"));
                historico.QuantidadeAnterior = reader.GetDecimal("quantidade_anterior");
                historico.QuantidadeResultante = reader.GetDecimal("quantidade_resultante");

                historicos.Add(historico);
            }

            return historicos;
        }
        catch
        {
            throw;
        }
    }
}