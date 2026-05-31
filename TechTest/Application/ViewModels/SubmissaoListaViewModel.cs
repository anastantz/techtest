// Application/ViewModels/SubmissaoListaViewModel.cs
using TechTest.Domain.Enums;

namespace TechTest.Application.ViewModels;

/// <summary>
/// Item da lista na tela "Testes Realizados".
/// Projeção leve — sem carregar as respostas completas.
/// </summary>
public sealed record SubmissaoListaViewModel(
    int             Id,
    int             CandidatoId,
    string          NomeCandidato,
    DateTime        EncerradoEm,
    StatusSubmissao Status,
    int             PontuacaoQ2
);