// Domain/Interfaces/Services/ICandidatoService.cs
using TechTest.Application.Candidatos;

namespace TechTest.Domain.Interfaces.Services;

public interface ICandidatoService
{
    /// <summary>
    /// Cadastra um candidato e retorna o ID gerado.
    /// </summary>
    Task<int> CadastrarAsync(
        CadastrarCandidatoCommand command,
        CancellationToken ct = default);
}