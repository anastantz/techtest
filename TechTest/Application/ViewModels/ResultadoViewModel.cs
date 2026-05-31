// Application/ViewModels/ResultadoViewModel.cs
using TechTest.Domain.Enums;

namespace TechTest.Application.ViewModels;

/// <summary>
/// Dados exibidos na tela de resultado após o encerramento do teste.
/// Visível apenas para Recrutador e TI — nunca para o candidato.
/// </summary>
public sealed record ResultadoViewModel(
    int             SubmissaoId,
    int             CandidatoId,
    string          NomeCandidato,
    int             PontuacaoQ2,
    int             TotalQ2,
    int             NotaDeCorte,
    StatusSubmissao Status,
    DateTime        EncerradoEm,
    bool            EncerradoPorTimeout
);