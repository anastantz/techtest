// Domain/Entities/Usuario.cs
using TechTest.Domain.Enums;

namespace TechTest.Domain.Entities;

/// <summary>
/// Representa um usuário autenticável do sistema (Recrutador ou TI).
/// Candidatos não são usuários — não possuem login.
/// </summary>
public sealed class Usuario
{
    // Construtor privado: a única forma de criar um usuário válido
    // é pelo factory method Criar(), que garante invariantes.
    private Usuario() { }

    public int           Id        { get; private set; }
    public string        Nome      { get; private set; } = string.Empty;
    public string        Email     { get; private set; } = string.Empty;
    public string        SenhaHash { get; private set; } = string.Empty;
    public PerfilUsuario Perfil    { get; private set; }
    public bool          Ativo     { get; private set; }
    public DateTime      CriadoEm { get; private set; }

    public IReadOnlyCollection<Candidato> Candidatos { get; private set; }
        = new List<Candidato>();

    public static Usuario Criar(
        string        nome,
        string        email,
        string        senhaHash,
        PerfilUsuario perfil)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome é obrigatório.", nameof(nome));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("E-mail é obrigatório.", nameof(email));

        return new Usuario
        {
            Nome      = nome.Trim(),
            Email     = email.Trim().ToLowerInvariant(),
            SenhaHash = senhaHash,
            Perfil    = perfil,
            Ativo     = true,
            CriadoEm = DateTime.UtcNow
        };
    }
}