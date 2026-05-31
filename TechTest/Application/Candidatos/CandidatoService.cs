// Application/Candidatos/CandidatoService.cs
using TechTest.Domain.Entities;
using TechTest.Domain.Interfaces.Repositories;
using TechTest.Domain.Interfaces.Services;

namespace TechTest.Application.Candidatos;

/// <summary>
/// Orquestra o cadastro de candidatos.
/// Delega a criação ao factory da entidade e a persistência ao repositório.
/// </summary>
public sealed class CandidatoService : ICandidatoService
{
    private readonly ICandidatoRepository _candidatos;

    public CandidatoService(ICandidatoRepository candidatos)
        => _candidatos = candidatos;

    public async Task<int> CadastrarAsync(
        CadastrarCandidatoCommand command,
        CancellationToken ct = default)
    {
        var candidato = Candidato.Criar(
            nomeCompleto:  command.NomeCompleto,
            grauInstrucao: command.GrauInstrucao,
            formacao:      command.Formacao,
            usuarioId:     command.UsuarioId,
            whatsApp:      command.WhatsApp,
            linkedIn:      command.LinkedIn,
            gitHub:        command.GitHub
        );

        return await _candidatos.SalvarAsync(candidato, ct);
    }
}