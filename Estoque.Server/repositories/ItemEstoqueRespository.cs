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
                id_unidade_organizacional,
                espaco,
                descricao,
                tipo_unidade_medida,
                quantidade
            FROM
                estoque.itens_estoque
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
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    IdUnidadeOrganizacional = reader.GetInt32(reader.GetOrdinal("id_unidade_organizacional")),
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
                id_unidade_organizacional,
                espaco,
                descricao,
                tipo_unidade_medida,
                quantidade
            FROM
                estoque.itens_estoque
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
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                IdUnidadeOrganizacional = reader.GetInt32(reader.GetOrdinal("id_unidade_organizacional")),
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
            INSERT INTO estoque.itens_estoque
            (
                id_unidade_organizacional,
                espaco,
                descricao,
                tipo_unidade_medida,
                quantidade
            )
            VALUES
            (
                @id_unidade_organizacional,
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

            cmd.Parameters.AddWithValue("id_unidade_organizacional", item.IdUnidadeOrganizacional);
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
                estoque.itens_estoque
            SET
                id_unidade_organizacional = @id_unidade_organizacional,
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

            cmd.Parameters.AddWithValue("id", item.Id);
            cmd.Parameters.AddWithValue("id_unidade_organizacional", item.IdUnidadeOrganizacional);
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
                estoque.itens_estoque
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