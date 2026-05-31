// Application/Testes/TesteService.cs
using TechTest.Application.ViewModels;
using TechTest.Domain.Interfaces.Services;

namespace TechTest.Application.Testes;

/// <summary>
/// Fornece a estrutura estática do teste técnico.
/// Os dados vivem em código — não em banco — porque são regra de negócio
/// imutável. Mudar uma questão é uma mudança de requisito, não de dado.
/// Singleton: sem estado mutável, carregado uma única vez.
/// </summary>
public sealed class TesteService : ITesteService
{
    private readonly TesteViewModel _teste = ConstruirTeste();

    public TesteViewModel ObterTeste() => _teste;

    private static TesteViewModel ConstruirTeste() => new(
        EnunciadoQ1:   EnunciadoQ1(),
        ContextoSqlQ1: ContextoSqlQ1(),
        AfirmacoesQ2:  AfirmacoesQ2(),
        EnunciadoQ3:   EnunciadoQ3(),
        CodigoBaseQ3:  CodigoBaseQ3()
    );

    private static string EnunciadoQ1() =>
        "Você trabalha em uma empresa de e-commerce. Com base nas tabelas " +
        "abaixo, escreva um script SQL que retorne: o nome do cliente, a data " +
        "do pedido, o nome do produto, a quantidade e o valor total do item " +
        "(quantidade × preço unitário). Considere apenas pedidos com status " +
        "'Aprovado', ordenados pela data do pedido de forma decrescente.";

    private static string ContextoSqlQ1() =>
        """
        -- Tabelas disponíveis:
        --
        -- clientes     (id, nome, email, cidade)
        -- pedidos      (id, cliente_id, data_pedido, status)
        --                   status: 'Pendente' | 'Aprovado' | 'Cancelado'
        -- itens_pedido (id, pedido_id, produto_id, quantidade, preco_unitario)
        -- produtos     (id, nome, categoria, preco)
        """;

    private static IReadOnlyList<AfirmacaoViewModel> AfirmacoesQ2() =>
    [
        new(1,  "O comando `if` executa o bloco de código somente quando a condição avaliada é verdadeira."),
        new(2,  "O bloco `else` é obrigatório em toda estrutura `if`."),
        new(3,  "Em C# e Java, a estrutura `switch` aceita expressões do tipo `string` como critério de comparação."),
        new(4,  "Um laço `while` sempre executa o bloco de código interno pelo menos uma vez, independentemente da condição."),
        new(5,  "Um laço `do-while` garante que o bloco interno seja executado pelo menos uma vez antes de verificar a condição."),
        new(6,  "O operador de pós-incremento `x++` retorna o valor atual de `x` antes de efetuar o incremento."),
        new(7,  "Em um array de 8 elementos com indexação a partir de 0, o índice do último elemento é 8."),
        new(8,  "A expressão lógica `(false OR true) AND true` resulta em verdadeiro."),
        new(9,  "O laço `for` é mais adequado que `while` quando o número de iterações é conhecido antecipadamente."),
        new(10, "`NOT false` é equivalente a `true` de acordo com a tabela verdade do operador de negação."),
        new(11, "O operador lógico `AND` retorna verdadeiro apenas quando ambos os operandos são verdadeiros."),
        new(12, "O operador lógico `OR` retorna verdadeiro apenas quando ambos os operandos são verdadeiros."),
        new(13, "Um laço `while(true)` sem instrução de quebra (`break`) resulta em loop infinito."),
        new(14, "É possível aninhar uma estrutura `if` dentro de outra estrutura `if`."),
        new(15, "Em Java e C#, um array tem seu tamanho definido na criação e não pode ser redimensionado.")
    ];

    private static string EnunciadoQ3() =>
        "QUESTÃO OPCIONAL — Orientação a Objetos em C#\n\n" +
        "Complete o código abaixo implementando a hierarquia de classes " +
        "para um sistema de biblioteca. Siga os comentários TODO.";

    private static string CodigoBaseQ3() =>
        """
        using System;

        public abstract class Publicacao
        {
            public string Titulo        { get; set; }
            public int    AnoPublicacao { get; set; }
            public string Autor         { get; set; }

            // TODO: Implemente o construtor recebendo titulo, anoPublicacao e autor.
            public Publicacao(string titulo, int anoPublicacao, string autor)
            {
                // sua implementação aqui
            }

            // TODO: Declare um método abstrato Descrever() que retorne string.
        }

        public class Livro : Publicacao
        {
            public string ISBN          { get; set; }
            public int    NumeroPaginas { get; set; }

            // TODO: Construtor recebendo todos os campos. Use base() para os herdados.

            // TODO: Override de Descrever() retornando:
            // "Livro: {Titulo} | Autor: {Autor} | Ano: {AnoPublicacao} | ISBN: {ISBN} | Páginas: {NumeroPaginas}"
            public override string Descrever() => throw new NotImplementedException();
        }

        public class Revista : Publicacao
        {
            // TODO: Propriedades Edicao (int) e Periodicidade (string).
            // TODO: Construtor e override de Descrever() retornando:
            // "Revista: {Titulo} | Autor/Editora: {Autor} | Ano: {AnoPublicacao} | Edição: {Edicao} | Periodicidade: {Periodicidade}"
        }

        // NÃO ALTERE — usado para validar sua solução:
        class Program
        {
            static void Main()
            {
                Publicacao[] pub =
                [
                    new Livro("Clean Code", 2008, "Robert C. Martin", "978-0132350884", 431),
                    new Revista("National Geographic", 2024, "Nat Geo Society", 312, "Mensal")
                ];
                foreach (var p in pub) Console.WriteLine(p.Descrever());
            }
        }
        """;
}