// Domain/Interfaces/Services/IAuthService.cs
namespace TechTest.Domain.Interfaces.Services;

/// <summary>
/// Contrato de autenticação de usuários.
/// Retorna null se as credenciais forem inválidas.
/// </summary>
public interface IAuthService
{
    Task<AuthResult?> AutenticarAsync(
        string email,
        string senha,
        CancellationToken ct = default);
}

/// <summary>
/// Dados retornados após autenticação bem-sucedida.
/// Usados para montar os claims do cookie de sessão.
/// </summary>
public sealed record AuthResult(
    int    UsuarioId,
    string Nome,
    string Email,
    string Perfil
);