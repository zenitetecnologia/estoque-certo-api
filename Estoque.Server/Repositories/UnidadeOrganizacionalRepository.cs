using Estoque.Server.Models;
using Npgsql;
using System.Data;

namespace Estoque.Server.Repositories;

public class UnidadeOrganizacionalRepository : BaseRepository
{
    public UnidadeOrganizacionalRepository(IDbConnection connection) : base(connection) { }

    public async Task<Guid> CadastrarUnidade(UnidadeOrganizacional unidade)
    {
        const string sql = @"
            INSERT INTO estoque_certo.unidade_organizacional 
            (
                matriz_id,
                cnpj,
                razao_social,
                nome_fantasia,
                cep,
                endereco,
                numero,
                complemento,
                bairro,
                cidade,
                uf,
                pais,
                email,
                telefone
            )
            VALUES
            (
                @matriz_id,
                @cnpj,
                @razao_social,
                @nome_fantasia,
                @cep,
                @endereco,
                @numero,
                @complemento,
                @bairro,
                @cidade,
                @uf,
                @pais,
                @email,
                @telefone
            )
            RETURNING unidade_organizacional_id;
        ";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, Connection);

            cmd.Parameters.AddWithValue("matriz_id", unidade.MatrizId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("cnpj", unidade.Cnpj);
            cmd.Parameters.AddWithValue("razao_social", unidade.RazaoSocial);
            cmd.Parameters.AddWithValue("nome_fantasia", unidade.NomeFantasia ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("cep", unidade.Cep ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("endereco", unidade.Endereco ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("numero", unidade.Numero ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("complemento", unidade.Complemento ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("bairro", unidade.Bairro ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("cidade", unidade.Cidade ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("uf", unidade.Uf ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("pais", unidade.Pais ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("email", unidade.Email ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("telefone", unidade.Telefone ?? (object)DBNull.Value);

            var result = await cmd.ExecuteScalarAsync();

            return (Guid)result!;
        }
        catch
        {
            throw;
        }
    }

    public async Task<int> AtualizarUnidade(UnidadeOrganizacional unidade, Guid unidadeOrganizacionalId)
    {
        const string sql = @"
        UPDATE
            estoque_certo.unidade_organizacional
        SET
            matriz_id = @matriz_id,
            cnpj = @cnpj,
            razao_social = @razao_social,
            nome_fantasia = @nome_fantasia,
            cep = @cep,
            numero = @numero,
            complemento = @complemento,
            bairro = @bairro,
            cidade = @cidade,
            uf = @uf,
            pais = @pais,
            email = @email,
            telefone = @telefone
        WHERE
            unidade_organizacional_id = @unidade_organizacional_id;
    ";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, Connection);

            cmd.Parameters.AddWithValue("matriz_id", unidade.MatrizId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("cnpj", unidade.Cnpj);
            cmd.Parameters.AddWithValue("razao_social", unidade.RazaoSocial);
            cmd.Parameters.AddWithValue("nome_fantasia", unidade.NomeFantasia ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("cep", unidade.Cep ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("endereco", unidade.Endereco ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("numero", unidade.Numero ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("complemento", unidade.Complemento ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("bairro", unidade.Bairro ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("cidade", unidade.Cidade ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("uf", unidade.Uf ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("pais", unidade.Pais ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("email", unidade.Email ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("telefone", unidade.Telefone ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("unidade_organizacional_id", unidadeOrganizacionalId);

            return await cmd.ExecuteNonQueryAsync();
        }
        catch
        {
            throw;
        }
    }

    public async Task<List<UnidadeOrganizacionalGetResponse>> ObterUnidades(int skip, int top, string? razaoSocial, string? Cnpj)
    {
        var unidades = new List<UnidadeOrganizacionalGetResponse>();

        string sql = @"
            SELECT
                unidade_organizacional_id,
                matriz_id,
                cnpj,
                razao_social,
                nome_fantasia,
                cep,
                numero,
                complemento,
                bairro,
                cidade,
                uf,
                pais,
                email,
                telefone
            FROM
                estoque_certo.unidade_organizacional
            WHERE 1 = 1 
        ";

        if (!string.IsNullOrEmpty(razaoSocial)) sql += " AND razao_social ILIKE @razao_social ";

        if (!string.IsNullOrEmpty(Cnpj)) sql += " AND cnpj ILIKE @cnpj ";

        sql += " ORDER BY razao_social LIMIT @top OFFSET @skip";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, Connection);

            cmd.Parameters.AddWithValue("razao_social", $"%{razaoSocial}%");
            cmd.Parameters.AddWithValue("cnpj", $"%{Cnpj}%");
            cmd.Parameters.AddWithValue("skip", skip);
            cmd.Parameters.AddWithValue("top", top);

            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var unidadeOrganizacionalRecuperada = new UnidadeOrganizacionalGetResponse();

                unidadeOrganizacionalRecuperada.UnidadeOrganizacionalId = reader.GetGuid("unidade_organizacional_id");
                unidadeOrganizacionalRecuperada.MatrizId = reader.GetGuidNullable("matriz_id");
                unidadeOrganizacionalRecuperada.Cnpj = reader.GetString("cnpj");
                unidadeOrganizacionalRecuperada.RazaoSocial = reader.GetString("razao_social");
                unidadeOrganizacionalRecuperada.NomeFantasia = reader.GetStringSafe("nome_fantasia");
                unidadeOrganizacionalRecuperada.Cep = reader.GetStringSafe("cep");
                unidadeOrganizacionalRecuperada.Numero = reader.GetStringSafe("numero");
                unidadeOrganizacionalRecuperada.Complemento = reader.GetStringSafe("complemento");
                unidadeOrganizacionalRecuperada.Bairro = reader.GetStringSafe("bairro");
                unidadeOrganizacionalRecuperada.Cidade = reader.GetStringSafe("cidade");
                unidadeOrganizacionalRecuperada.Uf = reader.GetStringSafe("uf");
                unidadeOrganizacionalRecuperada.Pais = reader.GetStringSafe("pais");
                unidadeOrganizacionalRecuperada.Email = reader.GetStringSafe("email");
                unidades.Add(unidadeOrganizacionalRecuperada);
            }

            return unidades;
        }
        catch
        {
            throw;
        }
    }

    public async Task<UnidadeOrganizacionalGetResponse?> ObterUnidade(Guid unidadeOrganizacionald)
    {
        const string sql = @"
            SELECT
                unidade_organizacional_id,
                matriz_id,
                cnpj,
                razao_social,
                nome_fantasia,
                cep,
                numero,
                complemento,
                bairro,
                cidade,
                uf,
                pais,
                email,
                telefone
            FROM
                estoque_certo.unidade_organizacional
            WHERE
                unidade_organizacional_id = @unidade_organizacional_id
            LIMIT 1;
        ";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, Connection);
            cmd.Parameters.AddWithValue("unidade_organizacional_id", unidadeOrganizacionald);

            await using var reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync()) return null;

            return new UnidadeOrganizacionalGetResponse
            {
                UnidadeOrganizacionalId = reader.GetGuid("unidade_organizacional_id"),
                MatrizId = reader.GetGuidNullable("matriz_id"),
                Cnpj = reader.GetString("cnpj"),
                RazaoSocial = reader.GetString("razao_social"),
                NomeFantasia = reader.GetStringSafe("nome_fantasia"),
                Cep = reader.GetStringSafe("cep"),
                Numero = reader.GetStringSafe("numero"),
                Complemento = reader.GetStringSafe("complemento"),
                Bairro = reader.GetStringSafe("bairro"),
                Cidade = reader.GetStringSafe("cidade"),
                Uf = reader.GetStringSafe("uf"),
                Pais = reader.GetStringSafe("pais"),
                Email = reader.GetStringSafe("email"),
                Telefone = reader.GetStringSafe("telefone")
            };
        }
        catch
        {
            throw;
        }
    }

    public async Task<int> ExcluirUnidade(Guid unidadeOrganizacionalId)
    {
        const string sql = "DELETE FROM estoque_certo.unidade_organizacional WHERE unidade_organizacional_id = @unidade_organizacional_id";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, Connection);
            cmd.Parameters.AddWithValue("unidade_organizacional_id", unidadeOrganizacionalId);

            return await cmd.ExecuteNonQueryAsync();
        }
        catch
        {
            throw;
        }
    }

    public async Task<bool> VerificaUnidadeExiste(string Cnpj, Guid ignoreId)
    {
        const string sql = @"
            SELECT
                1
            FROM
                estoque_certo.unidade_organizacional
            WHERE
                cnpj = @cnpj
            AND
                unidade_organizacional_id <> @ignoreunidade_organizacional_id
            LIMIT 1;
        ";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, Connection);
            cmd.Parameters.AddWithValue("cnpj", Cnpj);
            cmd.Parameters.AddWithValue("ignoreunidade_organizacional_id", ignoreId);

            var result = await cmd.ExecuteScalarAsync();
            return result != null;
        }
        catch
        {
            throw;
        }
    }
}