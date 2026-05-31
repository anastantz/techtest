// Domain/Interfaces/Repositories/IUsuarioRepository.cs
using TechTest.Domain.Entities;

namespace TechTest.Domain.Interfaces.Repositories;

/// <summary>
/// Contrato de acesso a dados para Usuario.
/// O domínio define o que precisa; a infraestrutura define como faz.
/// </summary>
public interface IUsuarioRepository
{
    Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken ct = default);
    Task<Usuario?> ObterPorIdAsync(int id, CancellationToken ct = default);
}