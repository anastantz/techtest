// Domain/Interfaces/Services/IPdfService.cs
using TechTest.Domain.Entities;

namespace TechTest.Domain.Interfaces.Services;

/// <summary>
/// Gera o PDF do resultado do teste.
/// Fica na infraestrutura (QuestPDF); o domínio conhece apenas esta interface.
/// </summary>
public interface IPdfService
{
    byte[] Gerar(Submissao submissao);
}