// Domain/Entities/QuestaoResposta.cs
namespace TechTest.Domain.Entities;

/// <summary>
/// Representa a resposta a uma afirmação individual da Questão 2.
/// Usado como objeto de transferência entre a View e o serviço de submissão.
/// Não é persistido diretamente — as respostas são serializadas em JSON
/// dentro de Submissao.RespostasQ2Json.
/// </summary>
public sealed class QuestaoResposta
{
    /// <summary>Número da afirmação (1 a 15).</summary>
    public int  Numero   { get; init; }

    /// <summary>Resposta do candidato: true = Verdadeiro, false = Falso.</summary>
    public bool Resposta { get; init; }

    public QuestaoResposta(int numero, bool resposta)
    {
        if (numero < 1 || numero > 15)
            throw new ArgumentOutOfRangeException(
                nameof(numero), "Número da afirmação deve estar entre 1 e 15.");

        Numero   = numero;
        Resposta = resposta;
    }
}