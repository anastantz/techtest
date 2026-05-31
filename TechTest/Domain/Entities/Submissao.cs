// Domain/Entities/Submissao.cs
using System.Text.Json;
using TechTest.Domain.Enums;

namespace TechTest.Domain.Entities;

/// <summary>
/// Representa a realização completa de um teste por um candidato.
/// Encapsula o gabarito da Q2 e a lógica de pontuação automática —
/// são regras de negócio, não responsabilidade do banco ou da UI.
/// </summary>
public sealed class Submissao
{
    // Gabarito da Q2: definido no domínio porque é regra, não dado.
    // Mudar uma resposta correta é uma mudança de requisito de negócio.
    private static readonly IReadOnlyDictionary<int, bool> GabaritoQ2 =
        new Dictionary<int, bool>
        {
            { 1,  true  },  // if executa somente quando a condição é verdadeira
            { 2,  false },  // else NÃO é obrigatório
            { 3,  true  },  // switch aceita string em C# e Java
            { 4,  false },  // while NÃO garante ao menos uma execução
            { 5,  true  },  // do-while garante ao menos uma execução
            { 6,  true  },  // x++ retorna o valor antes de incrementar
            { 7,  false },  // array de 8 elementos: último índice é 7, não 8
            { 8,  true  },  // (false OR true) AND true = true
            { 9,  true  },  // for é mais adequado quando iterações são conhecidas
            { 10, true  },  // NOT false = true
            { 11, true  },  // AND retorna true somente quando ambos são true
            { 12, false },  // OR retorna true quando AO MENOS UM é true
            { 13, true  },  // while(true) sem break = loop infinito
            { 14, true  },  // if pode ser aninhado em outro if
            { 15, true  }   // arrays Java/C# têm tamanho fixo após declaração
        };

    public static int TotalAfirmacoesQ2 => GabaritoQ2.Count;

    // Nota de corte: 10 de 15 acertos (definida pelo time de TI)
    public static int NotaDeCorte => 10;

    private Submissao() { }

    public int              Id             { get; private set; }
    public int              CandidatoId    { get; private set; }
    public string?          RespostaQ1     { get; private set; }
    public string?          RespostasQ2Json { get; private set; }
    public string?          RespostaQ3     { get; private set; }
    public int              PontuacaoQ2    { get; private set; }
    public StatusSubmissao  Status         { get; private set; }
    public DateTime         IniciadoEm    { get; private set; }
    public DateTime?        EncerradoEm   { get; private set; }

    // Indica se o teste foi encerrado por esgotamento do tempo
    public bool EncerradoPorTimeout { get; private set; }

    // Navegação
    public Candidato? Candidato { get; private set; }

    /// <summary>
    /// Cria a submissão ao encerrar o teste, calculando a pontuação
    /// da Q2 automaticamente. Imutável após a criação.
    /// </summary>
    public static Submissao Criar(
        int      candidatoId,
        string?  respostaQ1,
        IReadOnlyList<(int Numero, bool Resposta)> respostasQ2,
        string?  respostaQ3,
        DateTime iniciadoEm,
        bool     encerradoPorTimeout = false)
    {
        int pontuacao = CalcularPontuacao(respostasQ2);

        return new Submissao
        {
            CandidatoId          = candidatoId,
            RespostaQ1           = respostaQ1,
            RespostasQ2Json      = SerializarRespostas(respostasQ2),
            RespostaQ3           = respostaQ3,
            PontuacaoQ2          = pontuacao,
            Status               = StatusSubmissao.EmAvaliacao,
            IniciadoEm           = iniciadoEm,
            EncerradoEm          = DateTime.UtcNow,
            EncerradoPorTimeout  = encerradoPorTimeout
        };
    }

    /// <summary>
    /// Atualiza o status da submissão.
    /// Só o perfil TI pode chamar este método — verificado na camada de aplicação.
    /// </summary>
    public void AtualizarStatus(StatusSubmissao novoStatus)
    {
        if (novoStatus == StatusSubmissao.EmAvaliacao)
            throw new InvalidOperationException(
                "Não é possível reverter o status para 'Em avaliação'.");

        Status = novoStatus;
    }

    /// <summary>
    /// Retorna as respostas da Q2 com o resultado de cada item (acerto/erro)
    /// para exibição no PDF e na tela de resultados.
    /// </summary>
    public IReadOnlyList<(int Numero, bool Resposta, bool Correto)> ObterRespostasComGabarito()
    {
        return DeserializarRespostas()
            .Select(r => (
                r.Numero,
                r.Resposta,
                Correto: GabaritoQ2.TryGetValue(r.Numero, out bool correto)
                         && r.Resposta == correto
            ))
            .ToList();
    }

    public static IReadOnlyDictionary<int, bool> ObterGabarito() => GabaritoQ2;

    // -------------------------------------------------------
    // Helpers privados
    // -------------------------------------------------------

    private static int CalcularPontuacao(
        IReadOnlyList<(int Numero, bool Resposta)> respostas)
        => respostas.Count(r =>
            GabaritoQ2.TryGetValue(r.Numero, out bool correto)
            && r.Resposta == correto);

    private static string SerializarRespostas(
        IReadOnlyList<(int Numero, bool Resposta)> respostas)
        => JsonSerializer.Serialize(
            respostas.Select(r => new { r.Numero, r.Resposta }));

    private IReadOnlyList<(int Numero, bool Resposta)> DeserializarRespostas()
    {
        if (string.IsNullOrEmpty(RespostasQ2Json))
            return Array.Empty<(int, bool)>();

        var itens = JsonSerializer
            .Deserialize<List<RespostaItem>>(RespostasQ2Json)
            ?? new List<RespostaItem>();

        return itens.Select(i => (i.Numero, i.Resposta)).ToList();
    }

    // Record auxiliar apenas para deserialização — sem exposição externa
    private sealed record RespostaItem(int Numero, bool Resposta);
}