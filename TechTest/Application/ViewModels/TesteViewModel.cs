// Application/ViewModels/TesteViewModel.cs
namespace TechTest.Application.ViewModels;

/// <summary>
/// Estrutura do teste exibida na tela de aplicação.
/// Dados estáticos — não vêm do banco.
/// </summary>
public sealed record TesteViewModel(
    string EnunciadoQ1,
    string ContextoSqlQ1,
    IReadOnlyList<AfirmacaoViewModel> AfirmacoesQ2,
    string EnunciadoQ3,
    string CodigoBaseQ3
);

/// <summary>Uma afirmação da Q2 com número e texto.</summary>
public sealed record AfirmacaoViewModel(int Numero, string Texto);