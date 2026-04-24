using Estoque.Server.Models;
using Npgsql;
using System.Data;

namespace Estoque.Server.Repositories;

public class AuthRepository : BaseRepository
{
    public AuthRepository(IDbConnection connection) : base(connection) { }

    public async Task<Usuario?> ObterUsername(string username)
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
            FROM estoque_certo.usuario
            WHERE username = @username
            LIMIT 1;
        ";

        try
        {
            await EnsureOpenAsync();

            await using var cmd = new NpgsqlCommand(sql, Connection);

            cmd.Parameters.AddWithValue("username", username);

            await using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Usuario
                {
                    UsuarioId = reader.GetGuid("usuario_id"),
                    Username = reader.GetString("username"),
                    Senha = reader.GetString("senha"),
                    Nome = reader.GetString("nome"),
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

    public async Task<CodigoAcesso?> ObterCodigoPorSms(string codigoSms)
    {
        const string sql = @"
            SELECT 
                usuario_id,
                codigo,
                data_solicitacao,
                data_validacao,
                utilizado,
                codigo_acesso_id,
                data_acesso_id,
                reset_efetuado
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
                    Utilizado = reader.GetBoolean(4),
                    CodigoAcessoId = reader.IsDBNull(5) ? null : reader.GetString(5),
                    DataAcessoId = reader.IsDBNull(6) ? null : reader.GetDateTime(6),
                    ResetEfetuado = reader.GetBoolean(7)
                };
            }
            return null;
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
                utilizado = true, 
                data_validacao = @data_validacao,
                codigo_acesso_id = @codigo_acesso_id,
                data_acesso_id = @data_acesso_id -- Grava a hora exata da geração do token gigante
            WHERE 
                usuario_id = @usuario_id 
                AND codigo = @codigo;
        ";

        try
        {
            await EnsureOpenAsync();
            await using var cmd = new NpgsqlCommand(sql, Connection);

            var agora = DateTime.UtcNow;
            cmd.Parameters.AddWithValue("data_validacao", agora);
            cmd.Parameters.AddWithValue("codigo_acesso_id", codigoAcessoId);
            cmd.Parameters.AddWithValue("data_acesso_id", agora);
            cmd.Parameters.AddWithValue("usuario_id", usuarioId);
            cmd.Parameters.AddWithValue("codigo", codigoSms);

            await cmd.ExecuteNonQueryAsync();
        }
        catch
        {
            throw;
        }
    }

    public async Task<bool> BuscarUltimoCodigo(Guid usuarioId, DateTime dataSolicitacao)
    {
        const string sql = @"
            SELECT EXISTS (
                SELECT 1 
                FROM estoque_certo.codigo_acesso 
                WHERE usuario_id = @usuario_id 
                  AND data_solicitacao > @data_solicitacao
            );
        ";

        try
        {
            await EnsureOpenAsync();
            await using var cmd = new NpgsqlCommand(sql, Connection);

            cmd.Parameters.AddWithValue("usuario_id", usuarioId);
            cmd.Parameters.AddWithValue("data_solicitacao", dataSolicitacao);

            return (bool)(await cmd.ExecuteScalarAsync())!;
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
                utilizado,
                codigo_acesso_id,
                data_acesso_id,
                reset_efetuado
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
                    Utilizado = reader.GetBoolean(4),
                    CodigoAcessoId = reader.IsDBNull(5) ? null : reader.GetString(5),
                    DataAcessoId = reader.IsDBNull(6) ? null : reader.GetDateTime(6),
                    ResetEfetuado = reader.GetBoolean(7)
                };
            }
            return null;
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
                utilizado
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

    public async Task MarcarResetEfetuado(string codigoAcessoId)
    {
        const string sql = @"
            UPDATE 
                estoque_certo.codigo_acesso 
            SET 
                reset_efetuado = true 
            WHERE 
                codigo_acesso_id = @codigo_acesso_id;
        ";

        try
        {
            await EnsureOpenAsync();
            await using var cmd = new NpgsqlCommand(sql, Connection);
            cmd.Parameters.AddWithValue("codigo_acesso_id", codigoAcessoId);

            await cmd.ExecuteNonQueryAsync();
        }
        catch
        {
            throw;
        }
    }
}