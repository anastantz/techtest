// Domain/Entities/Candidato.cs
namespace TechTest.Domain.Entities;

/// <summary>
/// Representa o candidato cadastrado pelo recrutador antes do teste.
/// Candidato não tem login — não é um usuário do sistema.
/// </summary>
public sealed class Candidato
{
    private Candidato() { }

    public int      Id            { get; private set; }
    public string   NomeCompleto  { get; private set; } = string.Empty;
    public string?  WhatsApp      { get; private set; }
    public string?  LinkedIn      { get; private set; }
    public string?  GitHub        { get; private set; }
    public string   GrauInstrucao { get; private set; } = string.Empty;
    public string   Formacao      { get; private set; } = string.Empty;

    // Chave estrangeira: recrutador que cadastrou o candidato
    public int?     UsuarioId     { get; private set; }
    public DateTime CriadoEm     { get; private set; }

    // Navegação
    public Usuario?   Usuario   { get; private set; }
    public Submissao? Submissao { get; private set; }

    /// <summary>
    /// Regra de negócio central: um candidato só pode
    /// realizar o teste uma vez. Verificado antes de iniciar.
    /// </summary>
    public bool JaRealizouTeste => Submissao is not null;

    public static Candidato Criar(
        string  nomeCompleto,
        string  grauInstrucao,
        string  formacao,
        int?    usuarioId,
        string? whatsApp = null,
        string? linkedIn = null,
        string? gitHub   = null)
    {
        if (string.IsNullOrWhiteSpace(nomeCompleto))
            throw new ArgumentException("Nome completo é obrigatório.", nameof(nomeCompleto));

        if (string.IsNullOrWhiteSpace(grauInstrucao))
            throw new ArgumentException("Grau de instrução é obrigatório.", nameof(grauInstrucao));

        if (string.IsNullOrWhiteSpace(formacao))
            throw new ArgumentException("Formação é obrigatória.", nameof(formacao));

        return new Candidato
        {
            NomeCompleto  = nomeCompleto.Trim(),
            GrauInstrucao = grauInstrucao.Trim(),
            Formacao      = formacao.Trim(),
            UsuarioId     = usuarioId,
            WhatsApp      = whatsApp?.Trim(),
            LinkedIn      = linkedIn?.Trim(),
            GitHub        = gitHub?.Trim(),
            CriadoEm     = DateTime.UtcNow
        };
    }
}