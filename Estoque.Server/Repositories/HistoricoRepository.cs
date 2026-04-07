using Estoque.Server.Models;
using Npgsql;
using System.Data;

namespace Estoque.Server.Repositories;

public class HistoricoRepository
{
    private readonly NpgsqlConnection _connection;

    public HistoricoRepository(IDbConnection connection)
    {
        _connection = (NpgsqlConnection)connection ?? throw new ArgumentNullException(nameof(connection));
    }

    public async Task<List<HistoricoRecuperado>> ObterHistoricoPorItem(Guid itemEstoqueId)
    {
        const string sql = @"
            SELECT
                historico_id,
                item_estoque_id,
                tipo_movimentacao,
                usuario_id,
                data_hora,
                quantidade_anterior,
                quantidade_resultante
            FROM
                estoque.historico
            WHERE
                item_estoque_id = @item_estoque_id
            ORDER BY
                data_hora DESC;
        ";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, _connection);
            cmd.Parameters.AddWithValue("item_estoque_id", itemEstoqueId);

            await using var reader = await cmd.ExecuteReaderAsync();

            var historicos = new List<HistoricoRecuperado>();

            while (await reader.ReadAsync())
            {
                historicos.Add(new HistoricoRecuperado
                {
                    HistoricoId = reader.GetGuid("historico_id"),
                    ItemEstoqueId = reader.GetGuid("item_estoque_id"),
                    TipoMovimentacao = (TipoMovimentacao)reader.GetInt32("tipo_movimentacao"),
                    UsuarioId = reader.GetGuidNullable("usuario_id"),
                    DataHora = reader.GetDateTime(reader.GetOrdinal("data_hora")),
                    QuantidadeAnterior = reader.GetDecimal("quantidade_anterior"),
                    QuantidadeResultante = reader.GetDecimal("quantidade_resultante")
                });
            }

            return historicos;
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