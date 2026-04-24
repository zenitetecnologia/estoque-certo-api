using Estoque.Server.Models;
using Npgsql;
using System.Data;

namespace Estoque.Server.Repositories;

public class UsuarioRepository : BaseRepository
{
    public UsuarioRepository(IDbConnection connection) : base(connection) { }

    public async Task<Guid> CadastrarUsuario(Usuario usuario)
    {
        const string sql = @"
            INSERT INTO estoque_certo.usuario
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

            await using var cmd = new NpgsqlCommand(sql, Connection);

            cmd.Parameters.AddWithValue("username", usuario.Username);
            cmd.Parameters.AddWithValue("senha", usuario.Senha);
            cmd.Parameters.AddWithValue("nome", usuario.Nome);
            cmd.Parameters.AddWithValue("telefone", usuario.Telefone);
            cmd.Parameters.AddWithValue("perfil", (int)usuario.Perfil);
            cmd.Parameters.AddWithValue("unidade_organizacional_id", usuario.UnidadeOrganizacionalId!);
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
                estoque_certo.usuario
            SET
                username = @username,
                senha = @senha,
                nome = @nome,
                telefone = @telefone
            WHERE
                usuario_id = @usuario_id
            AND
                unidade_organizacional_id = @unidade_organizacional_id
        ";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, Connection);

            cmd.Parameters.AddWithValue("unidade_organizacional_id", usuario.UnidadeOrganizacionalId!);
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

    public async Task<UsuarioGetResponse?> ObterUsuario(Guid usuarioId)
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
                estoque_certo.usuario
            WHERE
                usuario_id = @usuario_id
            LIMIT 1;
        ";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, Connection);
            cmd.Parameters.AddWithValue("usuario_id", usuarioId);

            await using var reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync()) return null;

            return new UsuarioGetResponse
            {
                UsuarioId = reader.GetGuid("usuario_id"),
                Username = reader.GetString("username"),
                Senha = reader.GetString("senha"),
                Nome = reader.GetString("nome"),
                Telefone = reader.GetString("telefone"),
                Perfil = (PerfilUsuario)reader.GetInt32("perfil"),
                UnidadeOrganizacionalId = reader.GetGuidNullable("unidade_organizacional_id"),
                Valido = reader.GetBoolean("valido")
            };
        }
        catch
        {
            throw;
        }
    }

    public async Task<List<UsuarioGetResponse>> ObterUsuarios(int skip, int top, string? username, Guid? unidadeOrganizacionalId)
    {
        var usuarios = new List<UsuarioGetResponse>();

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

            cmd.Parameters.AddWithValue("username", $"%{username}%");
            cmd.Parameters.AddWithValue("unidade_organizacional_id", unidadeOrganizacionalId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("top", top);
            cmd.Parameters.AddWithValue("skip", skip);

            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var usuarioGetResponse = new UsuarioGetResponse();

                usuarioGetResponse.UsuarioId = reader.GetGuid("usuario_id");
                usuarioGetResponse.Username = reader.GetString("username");
                usuarioGetResponse.Senha = reader.GetString("senha");
                usuarioGetResponse.Nome = reader.GetString("nome");
                usuarioGetResponse.Telefone = reader.GetString("telefone");
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
            cmd.Parameters.AddWithValue("unidade_organizacional_id", (object)unidadeOrganizacionalId! ?? DBNull.Value);
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
        const string sql = "DELETE FROM estoque_certo.usuario WHERE usuario_id = @usuario_id";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, Connection);

            cmd.Parameters.AddWithValue("usuario_id", usuarioId);

            return await cmd.ExecuteNonQueryAsync();
        }
        catch
        {
            throw;
        }
    }

    public async Task<Usuario?> ObterIdentificador(string identificador)
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
            FROM estoque_certo.usuario 
            WHERE username = @identificador OR telefone = @identificador
            LIMIT 1;
        ";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, Connection);

            cmd.Parameters.AddWithValue("identificador", identificador);

            await using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Usuario
                {
                    UsuarioId = reader.GetGuid("usuario_id"),
                    Username = reader.GetString("username"),
                    Senha = reader.GetString("senha"),
                    Nome = reader.GetString("nome"),
                    Telefone = reader.IsDBNull("telefone") ? string.Empty : reader.GetString("telefone"),
                    Perfil = (PerfilUsuario)reader.GetInt32("perfil"),
                    UnidadeOrganizacionalId = reader.GetGuid("unidade_organizacional_id"),
                    Valido = reader.GetBoolean("valido")
                };
            }

            return null;
        }
        catch
        {
            throw;
        }
    }

    public async Task<int> RedefinirSenha(Guid usuarioId, string novaSenha)
    {
        const string sql = "UPDATE estoque_certo.usuario SET senha = @senha WHERE usuario_id = @usuario_id";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, Connection);

            cmd.Parameters.AddWithValue("usuario_id", usuarioId);
            cmd.Parameters.AddWithValue("senha", novaSenha);

            return await cmd.ExecuteNonQueryAsync();
        }
        catch
        {
            throw;
        }
    }

    public async Task InserirCodigoAcesso(Guid usuarioId, string codigo)
    {
        const string sql = @"
            INSERT INTO estoque_certo.codigo_acesso 
            (
                usuario_id,
                codigo,
                data_solicitacao,
                validado
                -- O codigo_acesso_id não entra aqui pois só é gerado no verify
            ) 
            VALUES 
            (
                @usuario_id,
                @codigo,
                @data_solicitacao,
                false
            );
        ";

        try
        {
            await EnsureOpenAsync();
            await using var cmd = new NpgsqlCommand(sql, Connection);

            cmd.Parameters.AddWithValue("usuario_id", usuarioId);
            cmd.Parameters.AddWithValue("codigo", codigo);
            cmd.Parameters.AddWithValue("data_solicitacao", DateTime.UtcNow);

            await cmd.ExecuteNonQueryAsync();
        }
        catch
        {
            throw;
        }
    }


    public async Task AtualizarCodigoValidado(Guid usuarioId, string codigoSms, string codigoAcessoId)
    {
        const string sql = @"
            UPDATE 
                estoque_certo.codigo_acesso 
            SET 
                validado = true, 
                data_validacao = @data_validacao,
                codigo_acesso_id = @codigo_acesso_id
            WHERE 
                usuario_id = @usuario_id 
                AND codigo = @codigo;
        ";

        try
        {
            await EnsureOpenAsync();
            await using var cmd = new NpgsqlCommand(sql, Connection);

            cmd.Parameters.AddWithValue("data_validacao", DateTime.UtcNow);
            cmd.Parameters.AddWithValue("codigo_acesso_id", codigoAcessoId);
            cmd.Parameters.AddWithValue("usuario_id", usuarioId);
            cmd.Parameters.AddWithValue("codigo", codigoSms);

            await cmd.ExecuteNonQueryAsync();
        }
        catch
        {
            throw;
        }
    }

    public async Task<CodigoAcesso?> ObterCodigoPorSms(string codigoSms)
    {
        const string sql = @"
            SELECT 
                usuario_id,
                codigo,
                data_solicitacao,
                data_validacao,
                validado,
                codigo_acesso_id
            FROM 
                estoque_certo.codigo_acesso
            WHERE 
                codigo = @codigo
            ORDER BY 
                data_solicitacao DESC
            LIMIT 1;
        ";

        try
        {
            await EnsureOpenAsync();
            await using var cmd = new NpgsqlCommand(sql, Connection);
            cmd.Parameters.AddWithValue("codigo", codigoSms);
            await using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new CodigoAcesso
                {
                    UsuarioId = reader.GetGuid(0),
                    Codigo = reader.GetString(1),
                    DataSolicitacao = reader.GetDateTime(2),
                    DataValidacao = reader.IsDBNull(3) ? null : reader.GetDateTime(3),
                    Validado = reader.GetBoolean(4),
                    CodigoAcessoId = reader.IsDBNull(5) ? null : reader.GetString(5)
                };
            }
            return null;
        }
        catch
        {
            throw;
        }
    }

    public async Task<CodigoAcesso?> ObterCodigoPorId(string codigoAcessoId)
    {
        const string sql = @"
            SELECT 
                usuario_id,
                codigo,
                data_solicitacao,
                data_validacao,
                validado,
                codigo_acesso_id
            FROM 
                estoque_certo.codigo_acesso
            WHERE 
                codigo_acesso_id = @codigo_acesso_id;
        ";

        try
        {
            await EnsureOpenAsync();
            await using var cmd = new NpgsqlCommand(sql, Connection);
            cmd.Parameters.AddWithValue("codigo_acesso_id", codigoAcessoId);
            await using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new CodigoAcesso
                {
                    UsuarioId = reader.GetGuid(0),
                    Codigo = reader.GetString(1),
                    DataSolicitacao = reader.GetDateTime(2),
                    DataValidacao = reader.IsDBNull(3) ? null : reader.GetDateTime(3),
                    Validado = reader.GetBoolean(4),
                    CodigoAcessoId = reader.IsDBNull(5) ? null : reader.GetString(5)
                };
            }
            return null;
        }
        catch
        {
            throw;
        }
    }
}