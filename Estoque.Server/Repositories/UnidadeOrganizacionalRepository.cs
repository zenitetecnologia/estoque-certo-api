using Estoque.Server.Models;
using Npgsql;
using NpgsqlTypes;
using System.Data;

namespace Estoque.Server.Repositories;

public class UnidadeOrganizacionalRepository : BaseRepository
{
    public UnidadeOrganizacionalRepository(IDbConnection connection) : base(connection) { }

    public async Task<Guid> Cadastrar(UnidadeOrganizacional unidadeOrganizacional)
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

            cmd.Parameters.Add("matriz_id", NpgsqlDbType.Uuid).Value = unidadeOrganizacional.MatrizId.ToDbValue();
            cmd.Parameters.Add("cnpj", NpgsqlDbType.Varchar).Value = unidadeOrganizacional.Cnpj;
            cmd.Parameters.Add("razao_social", NpgsqlDbType.Varchar).Value = unidadeOrganizacional.RazaoSocial;
            cmd.Parameters.Add("nome_fantasia", NpgsqlDbType.Varchar).Value = unidadeOrganizacional.NomeFantasia.ToDbValue();
            cmd.Parameters.Add("cep", NpgsqlDbType.Varchar).Value = unidadeOrganizacional.Cep.ToDbValue();
            cmd.Parameters.Add("endereco", NpgsqlDbType.Varchar).Value = unidadeOrganizacional.Endereco.ToDbValue();
            cmd.Parameters.Add("numero", NpgsqlDbType.Varchar).Value = unidadeOrganizacional.Numero.ToDbValue();
            cmd.Parameters.Add("complemento", NpgsqlDbType.Varchar).Value = unidadeOrganizacional.Complemento.ToDbValue();
            cmd.Parameters.Add("bairro", NpgsqlDbType.Varchar).Value = unidadeOrganizacional.Bairro.ToDbValue();
            cmd.Parameters.Add("cidade", NpgsqlDbType.Varchar).Value = unidadeOrganizacional.Cidade.ToDbValue();
            cmd.Parameters.Add("uf", NpgsqlDbType.Char).Value = unidadeOrganizacional.Uf.ToDbValue();
            cmd.Parameters.Add("pais", NpgsqlDbType.Varchar).Value = unidadeOrganizacional.Pais.ToDbValue();
            cmd.Parameters.Add("email", NpgsqlDbType.Varchar).Value = unidadeOrganizacional.Email.ToDbValue();
            cmd.Parameters.Add("telefone", NpgsqlDbType.Varchar).Value = unidadeOrganizacional.Telefone.ToDbValue();

            var result = await cmd.ExecuteScalarAsync();

            return (Guid)result!;
        }
        catch
        {
            throw;
        }
    }

    public async Task Atualizar(UnidadeOrganizacional unidadeOrganizacional, Guid unidadeOrganizacionalId)
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
                endereco = @endereco,
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

            cmd.Parameters.Add("unidade_organizacional_id", NpgsqlDbType.Uuid).Value = unidadeOrganizacionalId;

            cmd.Parameters.Add("matriz_id", NpgsqlDbType.Uuid).Value = unidadeOrganizacional.MatrizId.ToDbValue();
            cmd.Parameters.Add("cnpj", NpgsqlDbType.Varchar).Value = unidadeOrganizacional.Cnpj;
            cmd.Parameters.Add("razao_social", NpgsqlDbType.Varchar).Value = unidadeOrganizacional.RazaoSocial;
            cmd.Parameters.Add("nome_fantasia", NpgsqlDbType.Varchar).Value = unidadeOrganizacional.NomeFantasia.ToDbValue();
            cmd.Parameters.Add("cep", NpgsqlDbType.Varchar).Value = unidadeOrganizacional.Cep.ToDbValue();
            cmd.Parameters.Add("endereco", NpgsqlDbType.Varchar).Value = unidadeOrganizacional.Endereco.ToDbValue();
            cmd.Parameters.Add("numero", NpgsqlDbType.Varchar).Value = unidadeOrganizacional.Numero.ToDbValue();
            cmd.Parameters.Add("complemento", NpgsqlDbType.Varchar).Value = unidadeOrganizacional.Complemento.ToDbValue();
            cmd.Parameters.Add("bairro", NpgsqlDbType.Varchar).Value = unidadeOrganizacional.Bairro.ToDbValue();
            cmd.Parameters.Add("cidade", NpgsqlDbType.Varchar).Value = unidadeOrganizacional.Cidade.ToDbValue();
            cmd.Parameters.Add("uf", NpgsqlDbType.Char).Value = unidadeOrganizacional.Uf.ToDbValue();
            cmd.Parameters.Add("pais", NpgsqlDbType.Varchar).Value = unidadeOrganizacional.Pais.ToDbValue();
            cmd.Parameters.Add("email", NpgsqlDbType.Varchar).Value = unidadeOrganizacional.Email.ToDbValue();
            cmd.Parameters.Add("telefone", NpgsqlDbType.Varchar).Value = unidadeOrganizacional.Telefone.ToDbValue();

            await cmd.ExecuteNonQueryAsync();
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

    public async Task<bool> VerificarDuplicidade(string Cnpj, Guid ignoreId)
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