// Application/Candidatos/CadastrarCandidatoCommand.cs
namespace TechTest.Application.Candidatos;

/// <summary>
/// Dados necessários para cadastrar um candidato.
/// Record imutável: criado uma vez, não alterado.
/// </summary>
public sealed record CadastrarCandidatoCommand(
    string  NomeCompleto,
    string  GrauInstrucao,
    string  Formacao,
    int?    UsuarioId,
    string? WhatsApp = null,
    string? LinkedIn = null,
    string? GitHub   = null
);