// Domain/Interfaces/Services/ISubmissaoService.cs
using TechTest.Application.Submissoes;
using TechTest.Domain.Enums;

namespace TechTest.Domain.Interfaces.Services;

public interface ISubmissaoService
{
    /// <summary>
    /// Registra as respostas do candidato ao encerrar o teste.
    /// Lança InvalidOperationException se o candidato já realizou o teste.
    /// </summary>
    Task RegistrarAsync(
        RegistrarSubmissaoCommand command,
        CancellationToken ct = default);

    /// <summary>
    /// Atualiza o status de uma submissão.
    /// Apenas o perfil TI tem permissão — verificado na Page.
    /// </summary>
    Task AtualizarStatusAsync(
        int candidatoId,
        StatusSubmissao novoStatus,
        CancellationToken ct = default);
}