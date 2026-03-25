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
                perfil
            )
            VALUES
            (
                @username,
                @senha,
                @nome,
                @telefone,
                @perfil
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

            var result = await cmd.ExecuteScalarAsync();
            int idGerado = (int)result!;

            foreach (var unidade in usuario.UnidadesOrganizacionais)
            {
                await VincularUnidadeOrganizacional(idGerado, unidade.UnidadeOrganizacionalId);
            }

            return idGerado;
        }
        catch
        {
            throw;
        }
    }

    public async Task<bool> AtualizarUsuario(Usuario usuario, int usuarioId)
    {
        const string sql = @"
            UPDATE
                estoque.usuario
            SET
                username = @username,
                senha = @senha,
                nome = @nome,
                telefone = @telefone,
                perfil = @perfil
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
            cmd.Parameters.AddWithValue("perfil", (int)usuario.Perfil);

            var rowsAffected = await cmd.ExecuteNonQueryAsync();

            if (rowsAffected > 0)
            {
                await RemoverUnidadesOrganizacionais(usuarioId);

                if (usuario.UnidadesOrganizacionais != null)
                {
                    foreach (var unidade in usuario.UnidadesOrganizacionais)
                    {
                        await VincularUnidadeOrganizacional(usuarioId, unidade.UnidadeOrganizacionalId);
                    }
                }
            }

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

























    public async Task VincularUnidadeOrganizacional(int idUsuario, int idUnidadeOrganizacional)
    {
        const string sql = @"
            INSERT INTO estoque.usuario_unidade_organizacional
            (
                usuario_id,
                unidade_organizacional_id
            )
            VALUES
            (
                @usuario_id,
                @unidade_organizacional_id
            );
        ";

        try
        {
            await EnsureOpenAsync();
            await using var cmd = new NpgsqlCommand(sql, _connection);

            cmd.Parameters.AddWithValue("usuario_id", idUsuario);
            cmd.Parameters.AddWithValue("unidade_organizacional_id", idUnidadeOrganizacional);

            await cmd.ExecuteNonQueryAsync();
        }
        catch
        {
            throw;
        }
    }

    public async Task<bool> VerificaExisteUsuario(string username, int ignoreId)
    {
        const string sql = @"
            SELECT
                1
            FROM
                estoque.usuario
            WHERE
                username = @username
            AND
                usuario_id <> @ignoreId
            LIMIT 1;
        ";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, _connection);
            cmd.Parameters.AddWithValue("username", username);
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
                perfil
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

    public async Task<UsuarioRecuperado?> ObterUsuario(int id)
    {
        const string sql = @"
            SELECT
                usuario_id,
                username,
                senha,
                nome,
                telefone,
                perfil
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
            cmd.Parameters.AddWithValue("usuario_id", id);

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
            };
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

    private async Task EnsureOpenAsync()
    {
        if (_connection.State != ConnectionState.Open) await _connection.OpenAsync();
    }

    public async Task RemoverUnidadesOrganizacionais(int idUsuario)
    {
        const string sql = @"
            DELETE FROM estoque.usuario_unidade_organizacional WHERE usuario_id = @usuario_id;
        ";
        try
        {
            await EnsureOpenAsync();
            await using var cmd = new NpgsqlCommand(sql, _connection);
            cmd.Parameters.AddWithValue("usuario_id", idUsuario);
            await cmd.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw new Exception($"Erro ao remover unidades organizacionais: {ex.Message}");
        }
    }
}