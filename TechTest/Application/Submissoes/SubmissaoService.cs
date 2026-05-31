// Application/Submissoes/SubmissaoService.cs
using TechTest.Domain.Entities;
using TechTest.Domain.Enums;
using TechTest.Domain.Interfaces.Repositories;
using TechTest.Domain.Interfaces.Services;

namespace TechTest.Application.Submissoes;

/// <summary>
/// Registra respostas e gerencia o ciclo de vida das submissões.
/// </summary>
public sealed class SubmissaoService : ISubmissaoService
{
    private readonly ICandidatoRepository _candidatos;
    private readonly ISubmissaoRepository _submissoes;

    public SubmissaoService(
        ICandidatoRepository candidatos,
        ISubmissaoRepository submissoes)
    {
        _candidatos = candidatos;
        _submissoes = submissoes;
    }

    public async Task RegistrarAsync(
        RegistrarSubmissaoCommand command,
        CancellationToken ct = default)
    {
        var candidato = await _candidatos
            .ObterPorIdComSubmissaoAsync(command.CandidatoId, ct)
            ?? throw new KeyNotFoundException(
                $"Candidato {command.CandidatoId} não encontrado.");

        // Regra de negócio: uma submissão por candidato
        if (candidato.JaRealizouTeste)
            throw new InvalidOperationException(
                $"O candidato '{candidato.NomeCompleto}' já realizou o teste.");

        var submissao = Submissao.Criar(
            candidatoId:         command.CandidatoId,
            respostaQ1:          command.RespostaQ1,
            respostasQ2:         command.RespostasQ2,
            respostaQ3:          command.RespostaQ3,
            iniciadoEm:          command.IniciadoEm,
            encerradoPorTimeout: command.EncerradoPorTimeout
        );

        await _submissoes.SalvarAsync(submissao, ct);
    }

    public async Task AtualizarStatusAsync(
        int candidatoId,
        StatusSubmissao novoStatus,
        CancellationToken ct = default)
    {
        var submissao = await _submissoes
            .ObterCompletaPorCandidatoAsync(candidatoId, ct)
            ?? throw new KeyNotFoundException(
                $"Nenhuma submissão encontrada para o candidato {candidatoId}.");

        submissao.AtualizarStatus(novoStatus);
        await _submissoes.AtualizarAsync(submissao, ct);
    }
}