using Estoque.Server.Models;
using Npgsql;
using System.Data;

namespace Estoque.Server.Repositories;

public class UnidadeOrganizacionalRepository : BaseRepository
{
    public UnidadeOrganizacionalRepository(IDbConnection connection) : base(connection) { }

    public async Task<Guid> CriarUnidade(UnidadeOrganizacional unidade)
    {
        const string sql = @"
            INSERT INTO estoque_certo.unidade_organizacional 
            (
                id_matriz,
                Cnpj,
                razao_social,
                nome_fantasia,
                cep,
                numero,
                complemento,
                bairro,
                cidade,
                uf,
                pais,
                telefone,
                email
            )
            VALUES
            (
                @id_matriz,
                @Cnpj,
                @razao_social,
                @nome_fantasia,
                @cep,
                @numero,
                @complemento,
                @bairro,
                @cidade,
                @uf,
                @pais,
                @telefone,
                @email
            )
            RETURNING unidade_organizacional_id;
        ";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, Connection);

            cmd.Parameters.AddWithValue("id_matriz", unidade.IdMatriz.HasValue ? (object)unidade.IdMatriz.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("Cnpj", unidade.Cnpj);
            cmd.Parameters.AddWithValue("razao_social", unidade.RazaoSocial);
            cmd.Parameters.AddWithValue("nome_fantasia", unidade.NomeFantasia);
            cmd.Parameters.AddWithValue("cep", unidade.Cep);
            cmd.Parameters.AddWithValue("numero", unidade.Numero);
            cmd.Parameters.AddWithValue("complemento", unidade.Complemento);
            cmd.Parameters.AddWithValue("bairro", unidade.Bairro);
            cmd.Parameters.AddWithValue("cidade", unidade.Cidade);
            cmd.Parameters.AddWithValue("uf", unidade.Uf);
            cmd.Parameters.AddWithValue("pais", unidade.Pais);
            cmd.Parameters.AddWithValue("telefone", unidade.Telefone);
            cmd.Parameters.AddWithValue("email", unidade.Email);

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
            id_matriz = @id_matriz,
            Cnpj = @Cnpj,
            razao_social = @razao_social,
            nome_fantasia = @nome_fantasia,
            cep = @cep,
            numero = @numero,
            complemento = @complemento,
            bairro = @bairro,
            cidade = @cidade,
            uf = @uf,
            pais = @pais,
            telefone = @telefone,
            email = @email
        WHERE
            unidade_organizacional_id = @unidade_organizacional_id;
    ";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, Connection);

            cmd.Parameters.AddWithValue("unidade_organizacional_id", unidadeOrganizacionalId);

            cmd.Parameters.AddWithValue("id_matriz", unidade.IdMatriz.HasValue ? (object)unidade.IdMatriz.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("Cnpj", unidade.Cnpj);
            cmd.Parameters.AddWithValue("razao_social", unidade.RazaoSocial);
            cmd.Parameters.AddWithValue("nome_fantasia", unidade.NomeFantasia);
            cmd.Parameters.AddWithValue("cep", unidade.Cep);
            cmd.Parameters.AddWithValue("numero", unidade.Numero);
            cmd.Parameters.AddWithValue("complemento", unidade.Complemento);
            cmd.Parameters.AddWithValue("bairro", unidade.Bairro);
            cmd.Parameters.AddWithValue("cidade", unidade.Cidade);
            cmd.Parameters.AddWithValue("uf", unidade.Uf);
            cmd.Parameters.AddWithValue("pais", unidade.Pais);
            cmd.Parameters.AddWithValue("telefone", unidade.Telefone);
            cmd.Parameters.AddWithValue("email", unidade.Email);

            return await cmd.ExecuteNonQueryAsync();
        }
        catch
        {
            throw;
        }
    }

    public async Task<List<UnidadeOrganizacionalRecuperado>> ObterUnidades()
    {
        const string sql = @"
            SELECT
                unidade_organizacional_id,
                id_matriz,
                Cnpj,
                razao_social,
                nome_fantasia,
                cep,
                numero,
                complemento,
                bairro,
                cidade,
                uf,
                pais,
                telefone,
                email
            FROM
                estoque_certo.unidade_organizacional
            ORDER BY
                razao_social;
        ";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, Connection);
            await using var reader = await cmd.ExecuteReaderAsync();

            var unidades = new List<UnidadeOrganizacionalRecuperado>();

            while (await reader.ReadAsync())
            {
                unidades.Add(new UnidadeOrganizacionalRecuperado
                {
                    UnidadeOrganizacionalId = reader.GetGuid("unidade_organizacional_id"),
                    IdMatriz = reader.GetGuidNullable("id_matriz"),
                    Cnpj = reader.GetString("cnpj"),
                    RazaoSocial = reader.GetString("razao_social"),
                    NomeFantasia = reader.GetString("nome_fantasia"),
                    Cep = reader.GetString("cep"),
                    Numero = reader.GetString("numero"),
                    Complemento = reader.GetString("complemento"),
                    Bairro = reader.GetString("bairro"),
                    Cidade = reader.GetString("cidade"),
                    Uf = reader.GetString("uf"),
                    Pais = reader.GetString("pais"),
                    Telefone = reader.GetString("telefone"),
                    Email = reader.GetString("email")
                });
            }
            return unidades;
        }
        catch
        {
            throw;
        }
    }

    public async Task<UnidadeOrganizacionalRecuperado?> ObterUnidade(Guid unidadeOrganizacionald)
    {
        const string sql = @"
            SELECT
                unidade_organizacional_id,
                id_matriz,
                Cnpj,
                razao_social,
                nome_fantasia,
                cep,
                numero,
                complemento,
                bairro,
                cidade,
                uf,
                pais,
                telefone,
                email
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

            return new UnidadeOrganizacionalRecuperado
            {
                UnidadeOrganizacionalId = reader.GetGuid("unidade_organizacional_id"),
                IdMatriz = reader.GetGuidNullable("id_matriz"),
                Cnpj = reader.GetString("cnpj"),
                RazaoSocial = reader.GetString("razao_social"),
                NomeFantasia = reader.GetString("nome_fantasia"),
                Cep = reader.GetString("cep"),
                Numero = reader.GetString("numero"),
                Complemento = reader.GetString("complemento"),
                Bairro = reader.GetString("bairro"),
                Cidade = reader.GetString("cidade"),
                Uf = reader.GetString("uf"),
                Pais = reader.GetString("pais"),
                Telefone = reader.GetString("telefone"),
                Email = reader.GetString("email")
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

    public async Task<bool> VerificaExisteUnidade(string Cnpj, Guid ignoreId)
    {
        const string sql = @"
            SELECT
                1
            FROM
                estoque_certo.unidade_organizacional
            WHERE
                Cnpj = @Cnpj
            AND
                unidade_organizacional_id <> @ignoreunidade_organizacional_id
            LIMIT 1;
        ";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, Connection);
            cmd.Parameters.AddWithValue("Cnpj", Cnpj);
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