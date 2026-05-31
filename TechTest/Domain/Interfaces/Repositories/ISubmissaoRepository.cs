// Domain/Interfaces/Repositories/ISubmissaoRepository.cs
using TechTest.Domain.Entities;
using TechTest.Domain.Enums;

namespace TechTest.Domain.Interfaces.Repositories;

public interface ISubmissaoRepository
{
    /// <summary>
    /// Busca a submissão com candidato e usuário carregados.
    /// Necessário para geração do PDF em uma única consulta.
    /// </summary>
    Task<Submissao?> ObterCompletaPorCandidatoAsync(int candidatoId, CancellationToken ct = default);

    /// <summary>
    /// Lista todas as submissões ordenadas da mais recente à mais antiga.
    /// Usado na tela "Testes Realizados".
    /// </summary>
    Task<List<Submissao>> ListarOrdenadosAsync(
        DateTime? dataInicio   = null,
        DateTime? dataFim      = null,
        string?   termoBusca   = null,
        StatusSubmissao? status = null,
        int pagina             = 1,
        int itensPorPagina     = 20,
        CancellationToken ct   = default);

    Task<int> TotalAsync(
        DateTime? dataInicio    = null,
        DateTime? dataFim       = null,
        string?   termoBusca    = null,
        StatusSubmissao? status = null,
        CancellationToken ct    = default);

    Task SalvarAsync(Submissao submissao, CancellationToken ct = default);
    Task AtualizarAsync(Submissao submissao, CancellationToken ct = default);
}