using Estoque.Models;
using Npgsql;
using System.Data;

namespace Estoque.Repositories;

public class HistoricoRepository
{
    private readonly NpgsqlConnection _connection;

    public HistoricoRepository(IDbConnection connection)
    {
        _connection = (NpgsqlConnection)connection ?? throw new ArgumentNullException(nameof(connection));
    }

    public async Task<List<Historico>> ObterHistoricoPorItem(int idItemEstoque)
    {
        const string sql = @"
            SELECT
                id,
                item_estoque_id
                tipo_movimentacao,
                usuario_id,
                data_hora,
                quantidade_anterior,
                quantidade_resultante
            FROM
                estoque.historico
            WHERE
                id_item_estoque = @id_item_estoque
            ORDER BY
                data_hora DESC;
        ";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, _connection);
            cmd.Parameters.AddWithValue("id_item_estoque", idItemEstoque);

            await using var reader = await cmd.ExecuteReaderAsync();

            var historico = new List<Historico>();

            while (await reader.ReadAsync())
            {
                historico.Add(new Historico
                {
                    HistoricoId = (int)reader["historico_id"],
                    ItemEstoqueId = (int)reader["item_estoque_id"],
                    TipoMovimentacao = (TipoMovimentacao)reader["tipo_movimentacao"],
                    UsuarioId = (int)reader["usuario_id"],
                    DataHora = (DateTime)reader["data_hora"],
                    QuantidadeAnterior = (decimal)reader["quantidade_anterior"],
                    QuantidadeResultante = (decimal)reader["quantidade_resultante"]
                });
            }

            return historico;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    public async Task<int> RegistrarHistorico(Historico historico)
    {
        const string sql = @"
            INSERT INTO estoque.historico
            (
                item_estoque_id
                tipo_movimentacao,
                usuario_id,
                data_hora,
                quantidade_anterior,
                quantidade_resultante
            )
            VALUES
            (
                @item_estoque_id
                @tipo_movimentacao,
                @usuario_id,
                @data_hora,
                @quantidade_anterior,
                @quantidade_resultante
            )
            RETURNING id;
        ";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, _connection);

            cmd.Parameters.AddWithValue("id_item_estoque", historico.ItemEstoqueId);
            cmd.Parameters.AddWithValue("tipo_movimentacao", (int)historico.TipoMovimentacao);
            cmd.Parameters.AddWithValue("usuario_id", historico.UsuarioId);
            cmd.Parameters.AddWithValue("data_hora", historico.DataHora);
            cmd.Parameters.AddWithValue("quantidade_anterior", historico.QuantidadeAnterior);
            cmd.Parameters.AddWithValue("quantidade_resultante", historico.QuantidadeResultante);

            var result = await cmd.ExecuteScalarAsync();

            return (int)result!;
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