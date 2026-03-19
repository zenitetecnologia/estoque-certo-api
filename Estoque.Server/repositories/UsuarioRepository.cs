using Npgsql;
using System.Data;
using Estoque.models;

namespace Estoque.Repositories;

public class UsuarioRepository
{
    private readonly NpgsqlConnection _connection;

    public UsuarioRepository(IDbConnection connection)
    {
        _connection = (NpgsqlConnection)connection ?? throw new ArgumentNullException(nameof(connection));
    }

    public async Task<List<Usuario>> ObterUsuarios()
    {
        const string sql = @"
            SELECT
                id,
                username,
                senha,
                nome,
                telefone,
                validado,
                perfil,
                id_unidades_organizacionais
            FROM
                estoque.usuarios
            ORDER BY
                nome;
        ";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, _connection);
            await using var reader = await cmd.ExecuteReaderAsync();

            var usuarios = new List<Usuario>();

            while (await reader.ReadAsync())
            {
                usuarios.Add(new Usuario
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Username = reader.GetString(reader.GetOrdinal("username")),
                    Senha = reader.GetString(reader.GetOrdinal("senha")),
                    Nome = reader.GetString(reader.GetOrdinal("nome")),
                    Telefone = reader.GetString(reader.GetOrdinal("telefone")),
                    Validado = reader.GetBoolean(reader.GetOrdinal("validado")),
                    Perfil = (PerfilUsuario)reader.GetInt32(reader.GetOrdinal("perfil")),
                    IdUnidadesOrganizacionais = reader.IsDBNull(reader.GetOrdinal("id_unidades_organizacionais"))
                        ? new List<int>()
                        : reader.GetFieldValue<List<int>>(reader.GetOrdinal("id_unidades_organizacionais"))
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

    public async Task<Usuario?> ObterUsuarioPorId(int id)
    {
        const string sql = @"
            SELECT
                id,
                username,
                senha,
                nome,
                telefone,
                validado,
                perfil,
                id_unidades_organizacionais
            FROM
                estoque.usuarios
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

            return new Usuario
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Username = reader.GetString(reader.GetOrdinal("username")),
                Senha = reader.GetString(reader.GetOrdinal("senha")),
                Nome = reader.GetString(reader.GetOrdinal("nome")),
                Telefone = reader.GetString(reader.GetOrdinal("telefone")),
                Validado = reader.GetBoolean(reader.GetOrdinal("validado")),
                Perfil = (PerfilUsuario)reader.GetInt32(reader.GetOrdinal("perfil")),
                IdUnidadesOrganizacionais = reader.IsDBNull(reader.GetOrdinal("id_unidades_organizacionais"))
                    ? new List<int>()
                    : reader.GetFieldValue<List<int>>(reader.GetOrdinal("id_unidades_organizacionais"))
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    public async Task<int> CadastrarUsuario(Usuario usuario)
    {
        const string sql = @"
            INSERT INTO estoque.usuarios
            (
                username,
                senha,
                nome,
                telefone,
                validado,
                perfil,
                id_unidades_organizacionais
            )
            VALUES
            (
                @username,
                @senha,
                @nome,
                @telefone,
                @validado,
                @perfil,
                @id_unidades_organizacionais
            )
            RETURNING id;
        ";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, _connection);

            cmd.Parameters.AddWithValue("username", usuario.Username);
            cmd.Parameters.AddWithValue("senha", usuario.Senha);
            cmd.Parameters.AddWithValue("nome", usuario.Nome);
            cmd.Parameters.AddWithValue("telefone", usuario.Telefone);
            cmd.Parameters.AddWithValue("validado", usuario.Validado);
            cmd.Parameters.AddWithValue("perfil", (int)usuario.Perfil);
            cmd.Parameters.AddWithValue("id_unidades_organizacionais", usuario.IdUnidadesOrganizacionais.ToArray());

            var result = await cmd.ExecuteScalarAsync();

            return (int)result!;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    public async Task<bool> AtualizarUsuario(Usuario usuario)
    {
        const string sql = @"
            UPDATE
                estoque.usuarios
            SET
                username = @username,
                senha = @senha,
                nome = @nome,
                telefone = @telefone,
                validado = @validado,
                perfil = @perfil,
                id_unidades_organizacionais = @id_unidades_organizacionais
            WHERE
                id = @id;
        ";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, _connection);

            cmd.Parameters.AddWithValue("id", usuario.Id);
            cmd.Parameters.AddWithValue("username", usuario.Username);
            cmd.Parameters.AddWithValue("senha", usuario.Senha);
            cmd.Parameters.AddWithValue("nome", usuario.Nome);
            cmd.Parameters.AddWithValue("telefone", usuario.Telefone);
            cmd.Parameters.AddWithValue("validado", usuario.Validado);
            cmd.Parameters.AddWithValue("perfil", (int)usuario.Perfil);
            cmd.Parameters.AddWithValue("id_unidades_organizacionais", usuario.IdUnidadesOrganizacionais.ToArray());

            var rowsAffected = await cmd.ExecuteNonQueryAsync();
            return rowsAffected > 0;
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
                estoque.usuarios
            WHERE
                id = @id";

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

    public async Task<bool> VerificaExisteUsuario(string username, int ignoreId)
    {
        const string sql = @"
            SELECT
                1
            FROM
                estoque.usuarios
            WHERE
                username = @username
            AND
                id <> @ignoreId
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