using Estoque.Models;
using Estoque.Server.Models;
using Npgsql;
using System.Data;

namespace Estoque.Repositories;

public class UsuarioRepository
{
    private readonly NpgsqlConnection _connection;

    public UsuarioRepository(IDbConnection connection)
    {
        _connection = (NpgsqlConnection)connection ?? throw new ArgumentNullException(nameof(connection));
    }

    public async Task<Guid> CadastrarUsuario(Usuario usuario)
    {
        const string sql = @"
            INSERT INTO estoque.usuario
            (
                username,
                senha,
                nome,
                telefone,
                perfil,
                unidade_organizacional_id,
                valido
            )
            VALUES
            (
                @username,
                @senha,
                @nome,
                @telefone,
                @perfil,
                @unidade_organizacional_id,
                @valido
            )
            RETURNING usuario_id;
        ";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, _connection);

            cmd.Parameters.AddWithValue("username", usuario.Username);
            cmd.Parameters.AddWithValue("senha", usuario.Senha);
            cmd.Parameters.AddWithValue("nome", usuario.Nome);
            cmd.Parameters.AddWithValue("telefone", usuario.Telefone);
            cmd.Parameters.AddWithValue("perfil", (int)usuario.Perfil);
            cmd.Parameters.AddWithValue("unidade_organizacional_id", usuario.UnidadeOrganizacionalId);
            cmd.Parameters.AddWithValue("valido", usuario.Valido);

            var result = await cmd.ExecuteScalarAsync();

            return (Guid)result!;
        }
        catch
        {
            throw;
        }
    }

    public async Task<int> AtualizarUsuario(Usuario usuario, Guid usuarioId)
    {
        const string sql = @"
            UPDATE
                estoque.usuario
            SET
                username = @username,
                senha = @senha,
                nome = @nome,
                telefone = @telefone             
            WHERE
                usuario_id = @usuario_id;
        ";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, _connection);

            cmd.Parameters.AddWithValue("usuario_id", usuarioId);
            cmd.Parameters.AddWithValue("username", usuario.Username);
            cmd.Parameters.AddWithValue("senha", usuario.Senha);
            cmd.Parameters.AddWithValue("nome", usuario.Nome);
            cmd.Parameters.AddWithValue("telefone", usuario.Telefone);

            return await cmd.ExecuteNonQueryAsync();
        }
        catch
        {
            throw;
        }
    }

    public async Task<UsuarioRecuperado?> ObterUsuario(Guid usuarioId)
    {
        const string sql = @"
            SELECT
                usuario_id,
                username,
                senha,
                nome,
                telefone,
                perfil,
                unidade_organizacional_id,
                valido
            FROM
                estoque.usuario
            WHERE
                usuario_id = @usuario_id
            LIMIT 1;
        ";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, _connection);
            cmd.Parameters.AddWithValue("usuario_id", usuarioId);

            await using var reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync()) return null;

            return new UsuarioRecuperado
            {
                UsuarioId = reader.GetGuid("usuario_id"),
                Username = reader.GetString("username"),
                Senha = reader.GetString("senha"),
                Nome = reader.GetString("nome"),
                Telefone = reader.GetString("telefone"),
                Perfil = (PerfilUsuario)reader.GetInt32("perfil"),
                UnidadeOrganizacionalId = reader.GetGuid("unidade_organizacional_id"),
                Valido = reader.GetBoolean("valido")
            };
        }
        catch
        {
            throw;
        }
    }

    public async Task<List<UsuarioRecuperado>> ObterUsuarios(string? username = null, Guid? unidadeOrganizacionalId = null)
    {
        string sql = @"
            SELECT
                usuario_id,
                username,
                senha,
                nome,
                telefone,
                perfil,
                unidade_organizacional_id,
                valido
            FROM
                estoque.usuario
            WHERE 1 = 1
        ";

        if (!string.IsNullOrWhiteSpace(username))
            sql += " AND username ILIKE @username";

        if (unidadeOrganizacionalId != null)
            sql += " AND unidade_organizacional_id = @unidade_id";

        sql += " ORDER BY nome;";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, _connection);

            if (!string.IsNullOrWhiteSpace(username))
                cmd.Parameters.AddWithValue("username", $"%{username}%");

            if (unidadeOrganizacionalId != null)
                cmd.Parameters.AddWithValue("unidade_id", unidadeOrganizacionalId.Value);

            await using var reader = await cmd.ExecuteReaderAsync();

            var usuarios = new List<UsuarioRecuperado>();

            while (await reader.ReadAsync())
            {
                usuarios.Add(new UsuarioRecuperado
                {
                    UsuarioId = reader.GetGuid("usuario_id"),
                    Username = reader.GetString("username"),
                    Senha = reader.GetString("senha"),
                    Nome = reader.GetString("nome"),
                    Telefone = reader.GetString("telefone"),
                    Perfil = (PerfilUsuario)reader.GetInt32("perfil"),
                    UnidadeOrganizacionalId = reader.GetGuid("unidade_organizacional_id"),
                    Valido = reader.GetBoolean("valido")
                });
            }

            return usuarios;
        }
        catch
        {
            throw;
        }
    }

    public async Task<bool> VerificarUsuarioExiste(string username, Guid unidadeOrganizacionalId, Guid ignoreId)
    {
        const string sql = @"
            SELECT
                1
            FROM
                estoque.usuario
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

            await using var cmd = new NpgsqlCommand(sql, _connection);
            cmd.Parameters.AddWithValue("username", username);
            cmd.Parameters.AddWithValue("unidade_organizacional_id", unidadeOrganizacionalId);
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
        const string sql = "UPDATE estoque.usuario SET valido = true WHERE usuario_id = @usuario_id";

        try
        {
            await EnsureOpenAsync();
            await using var cmd = new NpgsqlCommand(sql, _connection);

            cmd.Parameters.AddWithValue("usuario_id", usuarioId);

            return await cmd.ExecuteNonQueryAsync();
        }
        catch
        {
            throw;
        }
    }

    public async Task<int> ExcluirUsuario(Guid usuarioId)
    {
        const string sql = "DELETE FROM estoque.usuario WHERE usuario_id = @usuario_id";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, _connection);
            cmd.Parameters.AddWithValue("usuario_id", usuarioId);

            return await cmd.ExecuteNonQueryAsync();
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