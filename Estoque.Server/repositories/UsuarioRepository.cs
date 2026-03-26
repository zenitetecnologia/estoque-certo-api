using Estoque.models;
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

    public async Task<int> CadastrarUsuario(Usuario usuario)
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

            return (int)result!;
        }
        catch
        {
            throw;
        }
    }

    public async Task<int> AtualizarUsuario(Usuario usuario, int usuarioId)
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

    public async Task<UsuarioRecuperado?> ObterUsuario(int usuarioId)
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
                UsuarioId = reader.GetInt32(reader.GetOrdinal("usuario_id")),
                Username = reader.GetString(reader.GetOrdinal("username")),
                Senha = reader.GetString(reader.GetOrdinal("senha")),
                Nome = reader.GetString(reader.GetOrdinal("nome")),
                Telefone = reader.GetString(reader.GetOrdinal("telefone")),
                Perfil = (PerfilUsuario)reader.GetInt32(reader.GetOrdinal("perfil")),
                UnidadeOrganizacionalId = reader.GetInt32(reader.GetOrdinal("unidade_organizacional_id")),
                Valido = reader.GetBoolean(reader.GetOrdinal("valido"))
            };
        }
        catch
        {
            throw;
        }
    }

    public async Task<bool> VerificaExisteUsuario(string username, int unidadeOrganizacionalId, int ignoreId)
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


























    public async Task<List<UsuarioRecuperado>> ObterUsuarios()
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
            ORDER BY
                nome;
        ";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, _connection);
            await using var reader = await cmd.ExecuteReaderAsync();

            var usuarios = new List<UsuarioRecuperado>();

            while (await reader.ReadAsync())
            {
                usuarios.Add(new UsuarioRecuperado
                {
                    UsuarioId = reader.GetInt32(reader.GetOrdinal("usuario_id")),
                    Username = reader.GetString(reader.GetOrdinal("username")),
                    Senha = reader.GetString(reader.GetOrdinal("senha")),
                    Nome = reader.GetString(reader.GetOrdinal("nome")),
                    Telefone = reader.GetString(reader.GetOrdinal("telefone")),
                    Perfil = (PerfilUsuario)reader.GetInt32(reader.GetOrdinal("perfil")),
                    UnidadeOrganizacionalId = reader.GetInt32(reader.GetOrdinal("unidade_organizacional_id")),
                    Valido = reader.GetBoolean(reader.GetOrdinal("valido"))
                });
            }

            return usuarios;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }



    public async Task<bool> ExcluirUsuario(int id)
    {
        const string sql = @"
            DELETE FROM
                estoque.usuario
            WHERE
                usuario_id = @usuario_id";
        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, _connection);
            cmd.Parameters.AddWithValue("usuario_id", id);

            var affected = await cmd.ExecuteNonQueryAsync();
            return affected > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }























    public async Task<bool> ValidarAcesso(int usuarioId)
    {
        const string sql = "UPDATE estoque.usuario SET valido = true WHERE usuario_id = @usuario_id";

        try
        {
            await EnsureOpenAsync();
            await using var cmd = new NpgsqlCommand(sql, _connection);

            cmd.Parameters.AddWithValue("usuario_id", usuarioId);

            var rowsAffected = await cmd.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw new Exception($"Erro ao habilitar usuário no banco de dados: {ex.Message}");
        }
    }

    private async Task EnsureOpenAsync()
    {
        if (_connection.State != ConnectionState.Open) await _connection.OpenAsync();
    }
}