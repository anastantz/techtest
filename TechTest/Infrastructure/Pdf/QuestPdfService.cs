// Infrastructure/Pdf/QuestPdfService.cs
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TechTest.Application.Testes;
using TechTest.Domain.Entities;
using TechTest.Domain.Interfaces.Services;

namespace TechTest.Infrastructure.Pdf;

/// <summary>
/// Gera o PDF do resultado usando QuestPDF.
/// Fica na infraestrutura porque QuestPDF é dependência externa.
/// O domínio conhece apenas IPdfService.
/// </summary>
public sealed class QuestPdfService : IPdfService
{
    private readonly ITesteService _testeService;

    public QuestPdfService(ITesteService testeService)
    {
        _testeService = testeService;
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] Gerar(Submissao submissao)
    {
        var candidato = submissao.Candidato
            ?? throw new InvalidOperationException(
                "Submissão sem candidato carregado. Use ObterCompletaPorCandidatoAsync.");

        var respostasQ2 = submissao.ObterRespostasComGabarito();
        var teste       = _testeService.ObterTeste();

        return Document.Create(doc =>
        {
            doc.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(t => t.FontSize(10).FontFamily("Arial"));

                page.Header().Column(col =>
                {
                    col.Item().Text("TESTE TÉCNICO — ATRAÇÃO DE TALENTOS")
                        .Bold().FontSize(14).FontColor(Colors.Blue.Darken3);
                    col.Item().Text("Vaga: Estágio / Desenvolvedor Júnior em Sistemas")
                        .FontSize(9).FontColor(Colors.Grey.Darken1);
                });

                page.Content().Column(col =>
                {
                    col.Spacing(14);

                    // ── Dados do candidato ──
                    col.Item().Column(dados =>
                    {
                        dados.Item().Text("DADOS DO CANDIDATO")
                            .Bold().FontSize(11).FontColor(Colors.Blue.Darken3);

                        dados.Item().PaddingTop(6).Table(t =>
                        {
                            t.ColumnsDefinition(c =>
                            {
                                c.RelativeColumn(1); c.RelativeColumn(2);
                                c.RelativeColumn(1); c.RelativeColumn(2);
                            });

                            void Label(string txt) => t.Cell().PaddingVertical(2)
                                .Text(txt).Bold().FontSize(9).FontColor(Colors.Grey.Darken2);
                            void Valor(string txt) => t.Cell().PaddingVertical(2)
                                .Text(txt).FontSize(9);

                            Label("Nome:");          Valor(candidato.NomeCompleto);
                            Label("Data/Hora:");     Valor(submissao.EncerradoEm?.ToString("dd/MM/yyyy HH:mm") ?? "—");
                            Label("WhatsApp:");      Valor(candidato.WhatsApp ?? "—");
                            Label("Grau:");          Valor(candidato.GrauInstrucao);
                            Label("LinkedIn:");      Valor(candidato.LinkedIn ?? "—");
                            Label("Formação:");      Valor(candidato.Formacao);
                            Label("GitHub:");        Valor(candidato.GitHub ?? "—");
                            Label("Recrutador(a):"); Valor(candidato.Usuario?.Nome ?? "—");
                        });
                    });

                    col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                    // ── Q1 ──
                    col.Item().Column(q1 =>
                    {
                        q1.Item().Text("QUESTÃO 1 — SQL")
                            .Bold().FontSize(11).FontColor(Colors.Blue.Darken3);
                        q1.Item().PaddingTop(4)
                            .Background(Colors.Grey.Lighten4)
                            .Border(1).BorderColor(Colors.Grey.Lighten2)
                            .Padding(6)
                            .Text(string.IsNullOrWhiteSpace(submissao.RespostaQ1)
                                ? "(sem resposta)"
                                : submissao.RespostaQ1)
                            .FontFamily("Courier New").FontSize(8);
                    });

                    col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                    // ── Q2 ──
                    col.Item().Column(q2 =>
                    {
                        q2.Item().Row(row =>
                        {
                            row.RelativeItem()
                                .Text("QUESTÃO 2 — LÓGICA (V/F)")
                                .Bold().FontSize(11).FontColor(Colors.Blue.Darken3);
                            row.ConstantItem(130).AlignRight()
                                .Text($"Pontuação: {submissao.PontuacaoQ2}/{Submissao.TotalAfirmacoesQ2}")
                                .Bold().FontSize(11)
                                .FontColor(submissao.PontuacaoQ2 >= Submissao.NotaDeCorte
                                    ? Colors.Green.Darken2
                                    : Colors.Red.Darken2);
                        });

                        q2.Item().PaddingTop(6).Table(t =>
                        {
                            t.ColumnsDefinition(c =>
                            {
                                c.ConstantColumn(20);
                                c.RelativeColumn();
                                c.ConstantColumn(70);
                                c.ConstantColumn(70);
                                c.ConstantColumn(25);
                            });

                            void Cab(string txt) => t.Cell()
                                .Background(Colors.Blue.Darken3).Padding(3)
                                .Text(txt).Bold().FontSize(8).FontColor(Colors.White);

                            Cab("Nº"); Cab("Afirmação"); Cab("Candidato"); Cab("Gabarito"); Cab("✓");

                            var gabarito    = Submissao.ObterGabarito();
                            var afirmacoes  = teste.AfirmacoesQ2;

                            foreach (var (numero, resposta, correto) in respostasQ2)
                            {
                                var afirmacao = afirmacoes.FirstOrDefault(a => a.Numero == numero);
                                bool isImpar  = numero % 2 != 0;
                                var  bg       = isImpar ? Colors.Grey.Lighten5 : Colors.White;
                                var  gabCorr  = gabarito.GetValueOrDefault(numero);

                                void Cel(string txt, string? cor = null, bool bold = false)
                                {
                                    var cell = t.Cell().Background(bg)
                                        .PaddingHorizontal(3).PaddingVertical(2);
                                    var text = cell.Text(txt).FontSize(8);
                                    if (bold) text.Bold();
                                    if (cor is not null) text.FontColor(cor);
                                }

                                Cel(numero.ToString());
                                Cel(afirmacao?.Texto ?? "—");
                                Cel(resposta ? "Verdadeiro" : "Falso");
                                Cel(gabCorr  ? "Verdadeiro" : "Falso");
                                Cel(correto  ? "✓" : "✗",
                                    correto  ? Colors.Green.Darken2 : Colors.Red.Darken2,
                                    bold: true);
                            }
                        });
                    });

                    col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                    // ── Q3 ──
                    col.Item().Column(q3 =>
                    {
                        q3.Item().Row(row =>
                        {
                            row.RelativeItem()
                                .Text("QUESTÃO 3 — POO EM C# (OPCIONAL)")
                                .Bold().FontSize(11).FontColor(Colors.Blue.Darken3);
                            row.ConstantItem(80).AlignRight()
                                .Text(string.IsNullOrWhiteSpace(submissao.RespostaQ3)
                                    ? "Não respondida" : "Respondida")
                                .FontSize(8).Italic().FontColor(Colors.Grey.Darken1);
                        });
                        q3.Item().PaddingTop(4)
                            .Background(Colors.Grey.Lighten4)
                            .Border(1).BorderColor(Colors.Grey.Lighten2)
                            .Padding(6)
                            .Text(string.IsNullOrWhiteSpace(submissao.RespostaQ3)
                                ? "(sem resposta)" : submissao.RespostaQ3)
                            .FontFamily("Courier New").FontSize(8);
                    });
                });

                page.Footer().AlignCenter().Text(t =>
                {
                    t.Span($"TechTest  |  {submissao.EncerradoEm:dd/MM/yyyy HH:mm}  |  Página ")
                        .FontSize(8).FontColor(Colors.Grey.Medium);
                    t.CurrentPageNumber().FontSize(8).FontColor(Colors.Grey.Medium);
                    t.Span(" de ").FontSize(8).FontColor(Colors.Grey.Medium);
                    t.TotalPages().FontSize(8).FontColor(Colors.Grey.Medium);
                });
            });
        }).GeneratePdf();
    }
}