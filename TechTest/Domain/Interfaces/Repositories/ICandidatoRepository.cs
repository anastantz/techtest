// Domain/Interfaces/Repositories/ICandidatoRepository.cs
using TechTest.Domain.Entities;

namespace TechTest.Domain.Interfaces.Repositories;

public interface ICandidatoRepository
{
    /// <summary>
    /// Busca o candidato com a submissão carregada.
    /// Necessário para verificar JaRealizouTeste antes de iniciar.
    /// </summary>
    Task<Candidato?> ObterPorIdComSubmissaoAsync(int id, CancellationToken ct = default);

    Task<int> SalvarAsync(Candidato candidato, CancellationToken ct = default);
}