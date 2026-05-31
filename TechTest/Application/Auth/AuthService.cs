// Application/Auth/AuthService.cs
using TechTest.Domain.Interfaces.Repositories;
using TechTest.Domain.Interfaces.Services;

namespace TechTest.Application.Auth;

/// <summary>
/// Verifica credenciais e retorna os dados para montar o cookie de sessão.
/// Não conhece HTTP, cookies nem claims — isso é responsabilidade da Page.
/// </summary>
public sealed class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarios;

    public AuthService(IUsuarioRepository usuarios) => _usuarios = usuarios;

    public async Task<AuthResult?> AutenticarAsync(
        string email,
        string senha,
        CancellationToken ct = default)
    {
        var emailNormalizado = email.Trim().ToLowerInvariant();
        var usuario = await _usuarios.ObterPorEmailAsync(emailNormalizado, ct);

        // Retorno null unificado: não diferencia e-mail inexistente de senha errada.
        // Evita enumeração de usuários via mensagem de erro diferente (segurança).
        if (usuario is null || !usuario.Ativo)
            return null;

        bool senhaCorreta = BCrypt.Net.BCrypt.Verify(senha, usuario.SenhaHash);
        if (!senhaCorreta)
            return null;

        return new AuthResult(
            UsuarioId: usuario.Id,
            Nome:      usuario.Nome,
            Email:     usuario.Email,
            Perfil:    usuario.Perfil.ToString()
        );
    }
}