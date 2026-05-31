// Application/Submissoes/RegistrarSubmissaoCommand.cs
namespace TechTest.Application.Submissoes;

/// <summary>
/// Dados enviados ao encerrar o teste.
/// IniciadoEm vem da sessão — registrado quando o recrutador clicou "Iniciar teste".
/// </summary>
public sealed record RegistrarSubmissaoCommand(
    int      CandidatoId,
    string?  RespostaQ1,
    IReadOnlyList<(int Numero, bool Resposta)> RespostasQ2,
    string?  RespostaQ3,
    DateTime IniciadoEm,
    bool     EncerradoPorTimeout = false
);