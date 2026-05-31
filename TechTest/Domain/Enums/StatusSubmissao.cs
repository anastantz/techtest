// Domain/Enums/StatusSubmissao.cs
namespace TechTest.Domain.Enums;

/// <summary>
/// Ciclo de vida de uma submissão após o encerramento do teste.
/// A transição de status é responsabilidade exclusiva do perfil TI.
/// </summary>
public enum StatusSubmissao
{
    EmAvaliacao = 1,  // estado inicial ao encerrar o teste
    Aprovado    = 2,  // definido pelo time de TI após correção
    Reprovado   = 3   // definido pelo time de TI após correção
}