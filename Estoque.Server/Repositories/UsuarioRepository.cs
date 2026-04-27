using Estoque.Server.Models;
using Npgsql;
using NpgsqlTypes;
using System.Data;

namespace Estoque.Server.Repositories;

public class UsuarioRepository : BaseRepository
{
    public UsuarioRepository(IDbConnection connection) : base(connection) { }

    public async Task<Guid> Cadastrar(Usuario usuario)
    {
        const string sql = @"
            INSERT INTO estoque_certo.usuario
            (
                username,
                senha,
                nome,
                perfil,
                unidade_organizacional_id,
                valido
            )
            VALUES
            (
                @username,
                @senha,
                @nome,
                @perfil,
                @unidade_organizacional_id,
                @valido
            )
            RETURNING usuario_id;
        ";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, Connection);

            cmd.Parameters.Add("username", NpgsqlDbType.Varchar).Value = usuario.Username;
            cmd.Parameters.Add("senha", NpgsqlDbType.Varchar).Value = usuario.Senha;
            cmd.Parameters.Add("nome", NpgsqlDbType.Varchar).Value = usuario.Nome;
            cmd.Parameters.Add("perfil", NpgsqlDbType.Integer).Value = (int)usuario.Perfil;
            cmd.Parameters.Add("unidade_organizacional_id", NpgsqlDbType.Uuid).Value = usuario.UnidadeOrganizacionalId!;
            cmd.Parameters.Add("valido", NpgsqlDbType.Boolean).Value = usuario.Valido;

            var result = await cmd.ExecuteScalarAsync();

            return (Guid)result!;
        }
        catch
        {
            throw;
        }
    }

    public async Task<int> Atualizar(Usuario usuario, Guid usuarioId)
    {
        const string sql = @"
            UPDATE
                estoque_certo.usuario
            SET
                username = @username,
                senha = @senha,
                nome = @nome
            WHERE
                usuario_id = @usuario_id
            AND
                unidade_organizacional_id = @unidade_organizacional_id
        ";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, Connection);

            cmd.Parameters.Add("usuario_id", NpgsqlDbType.Uuid).Value = usuarioId;
            cmd.Parameters.Add("unidade_organizacional_id", NpgsqlDbType.Uuid).Value = usuario.UnidadeOrganizacionalId!;
            cmd.Parameters.Add("username", NpgsqlDbType.Varchar).Value = usuario.Username;
            cmd.Parameters.Add("senha", NpgsqlDbType.Varchar).Value = usuario.Senha;
            cmd.Parameters.Add("nome", NpgsqlDbType.Varchar).Value = usuario.Nome;
            return await cmd.ExecuteNonQueryAsync();
        }
        catch
        {
            throw;
        }
    }

    public async Task<UsuarioGetResponse?> Obter(Guid usuarioId)
    {
        const string sql = @"
            SELECT
                usuario_id,
                username,
                senha,
                nome,
                perfil,
                unidade_organizacional_id,
                valido
            FROM
                estoque_certo.usuario
            WHERE
                usuario_id = @usuario_id
            LIMIT 1;
        ";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, Connection);

            cmd.Parameters.Add("usuario_id", NpgsqlDbType.Uuid).Value = usuarioId;

            await using var reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync()) return null;

            var usuarioGetResponse = new UsuarioGetResponse();

            usuarioGetResponse.UsuarioId = reader.GetGuid("usuario_id");
            usuarioGetResponse.Username = reader.GetString("username");
            usuarioGetResponse.Senha = reader.GetString("senha");
            usuarioGetResponse.Nome = reader.GetString("nome");
            usuarioGetResponse.Perfil = (PerfilUsuario)reader.GetInt32("perfil");
            usuarioGetResponse.UnidadeOrganizacionalId = reader.GetGuidNullable("unidade_organizacional_id");
            usuarioGetResponse.Valido = reader.GetBoolean("valido");

            return usuarioGetResponse;
        }
        catch
        {
            throw;
        }
    }

    public async Task<List<UsuarioGetResponse>> Obter(int skip, int top, string? username, Guid? unidadeOrganizacionalId)
    {
        var usuarios = new List<UsuarioGetResponse>();

        string sql = @"
            SELECT
                usuario_id,
                username,
                senha,
                nome,
                perfil,
                unidade_organizacional_id,
                valido
            FROM
                estoque_certo.usuario
            WHERE 1 = 1
        ";

        if (!string.IsNullOrWhiteSpace(username)) sql += " AND username ILIKE @username";

        if (unidadeOrganizacionalId != null) sql += " AND unidade_organizacional_id = @unidade_organizacional_id";

        sql += " ORDER BY nome LIMIT @top OFFSET @skip";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, Connection);

            cmd.Parameters.Add("username", NpgsqlDbType.Varchar).Value = $"%{username}%";
            cmd.Parameters.Add("unidade_organizacional_id", NpgsqlDbType.Uuid).Value = unidadeOrganizacionalId ?? (object)DBNull.Value;
            cmd.Parameters.Add("top", NpgsqlDbType.Integer).Value = top;
            cmd.Parameters.Add("skip", NpgsqlDbType.Integer).Value = skip;

            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var usuarioGetResponse = new UsuarioGetResponse();

                usuarioGetResponse.UsuarioId = reader.GetGuid("usuario_id");
                usuarioGetResponse.Username = reader.GetString("username");
                usuarioGetResponse.Senha = reader.GetString("senha");
                usuarioGetResponse.Nome = reader.GetString("nome");
                usuarioGetResponse.Perfil = (PerfilUsuario)reader.GetInt32("perfil");
                usuarioGetResponse.UnidadeOrganizacionalId = reader.GetGuidNullable("unidade_organizacional_id");
                usuarioGetResponse.Valido = reader.GetBoolean("valido");

                usuarios.Add(usuarioGetResponse);
            }

            return usuarios;
        }
        catch
        {
            throw;
        }
    }

    public async Task<bool> VerificarUsuarioExiste(string username, Guid? unidadeOrganizacionalId, Guid ignoreId)
    {
        const string sql = @"
            SELECT
                1
            FROM
                estoque_certo.usuario
            WHERE
                username = @username
            AND
                unidade_organizacional_id = @unidade_organizacional_id
            AND
                usuario_id <> @ignoreId
            LIMIT 1;
        ";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, Connection);

            cmd.Parameters.AddWithValue("username", username);
            cmd.Parameters.AddWithValue("unidade_organizacional_id", (object?)unidadeOrganizacionalId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("ignoreId", ignoreId);

            var result = await cmd.ExecuteScalarAsync();

            return result != null;
        }
        catch
        {
            throw;
        }
    }

    public async Task<int> ValidarAcesso(Guid usuarioId)
    {
        const string sql = "UPDATE estoque_certo.usuario SET valido = true WHERE usuario_id = @usuario_id";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, Connection);

            cmd.Parameters.Add("usuario_id", NpgsqlDbType.Uuid).Value = usuarioId;

            return await cmd.ExecuteNonQueryAsync();
        }
        catch
        {
            throw;
        }
    }

    public async Task<int> Excluir(Guid usuarioId)
    {
        const string sql = "DELETE FROM estoque_certo.usuario WHERE usuario_id = @usuario_id";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, Connection);

            cmd.Parameters.Add("usuario_id", NpgsqlDbType.Uuid).Value = usuarioId;

            return await cmd.ExecuteNonQueryAsync();
        }
        catch
        {
            throw;
        }
    }
}