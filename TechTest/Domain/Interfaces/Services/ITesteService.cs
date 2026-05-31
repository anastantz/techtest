// Domain/Interfaces/Services/ITesteService.cs
using TechTest.Application.ViewModels;

namespace TechTest.Domain.Interfaces.Services;

/// <summary>
/// Fornece a estrutura estática do teste.
/// Singleton: os dados são imutáveis e carregados uma única vez.
/// </summary>
public interface ITesteService
{
    TesteViewModel ObterTeste();
}