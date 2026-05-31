// Infrastructure/Data/AppDbContext.cs
using Microsoft.EntityFrameworkCore;
using TechTest.Domain.Entities;
using TechTest.Domain.Enums;

namespace TechTest.Infrastructure.Data;

/// <summary>
/// Contexto do Entity Framework Core.
///
/// Toda a configuração de mapeamento usa Fluent API exclusivamente.
/// As entidades de domínio ficam livres de atributos de ORM,
/// preservando a independência da camada de domínio.
///
/// Fluent API aqui, nada nos arquivos de Domain/Entities/.
/// </summary>
public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Usuario>   Usuarios   => Set<Usuario>();
    public DbSet<Candidato> Candidatos => Set<Candidato>();
    public DbSet<Submissao> Submissoes => Set<Submissao>();

    protected override void OnModelCreating(ModelBuilder model)
    {
        ConfigurarUsuario(model);
        ConfigurarCandidato(model);
        ConfigurarSubmissao(model);
        SeedUsuarios(model);
    }

    // -------------------------------------------------------
    // Configuração: Usuario
    // -------------------------------------------------------
    private static void ConfigurarUsuario(ModelBuilder model)
    {
        model.Entity<Usuario>(e =>
        {
            e.ToTable("usuarios");

            e.HasKey(u => u.Id);
            e.Property(u => u.Id).HasColumnName("id");

            e.Property(u => u.Nome)
                .HasColumnName("nome")
                .HasMaxLength(255)
                .IsRequired();

            e.Property(u => u.Email)
                .HasColumnName("email")
                .HasMaxLength(255)
                .IsRequired();

            e.Property(u => u.SenhaHash)
                .HasColumnName("senha_hash")
                .HasMaxLength(255)
                .IsRequired();

            // Enum armazenado como inteiro no banco
            e.Property(u => u.Perfil)
                .HasColumnName("perfil")
                .HasConversion<int>();

            e.Property(u => u.Ativo)
                .HasColumnName("ativo")
                .HasDefaultValue(true);

            e.Property(u => u.CriadoEm)
                .HasColumnName("criado_em");

            // E-mail único: dois usuários não podem ter o mesmo e-mail
            e.HasIndex(u => u.Email)
                .IsUnique()
                .HasDatabaseName("ix_usuarios_email");

            // EF Core não precisa da coleção de navegação mapeada
            // porque acessamos candidatos pelo lado do Candidato
            e.Ignore(u => u.Candidatos);
        });
    }

    // -------------------------------------------------------
    // Configuração: Candidato
    // -------------------------------------------------------
    private static void ConfigurarCandidato(ModelBuilder model)
    {
        model.Entity<Candidato>(e =>
        {
            e.ToTable("candidatos");

            e.HasKey(c => c.Id);
            e.Property(c => c.Id).HasColumnName("id");

            e.Property(c => c.NomeCompleto)
                .HasColumnName("nome_completo")
                .HasMaxLength(255)
                .IsRequired();

            e.Property(c => c.WhatsApp)
                .HasColumnName("whatsapp")
                .HasMaxLength(30);

            e.Property(c => c.LinkedIn)
                .HasColumnName("linkedin")
                .HasMaxLength(255);

            e.Property(c => c.GitHub)
                .HasColumnName("github")
                .HasMaxLength(255);

            e.Property(c => c.GrauInstrucao)
                .HasColumnName("grau_instrucao")
                .HasMaxLength(100)
                .IsRequired();

            e.Property(c => c.Formacao)
                .HasColumnName("formacao")
                .HasMaxLength(255)
                .IsRequired();

            e.Property(c => c.UsuarioId)
                .HasColumnName("usuario_id");

            e.Property(c => c.CriadoEm)
                .HasColumnName("criado_em");

            // Índice para filtrar candidatos por recrutador com performance
            e.HasIndex(c => c.UsuarioId)
                .HasDatabaseName("ix_candidatos_usuario_id");

            // Relacionamento: candidato pertence a um usuário (recrutador)
            e.HasOne(c => c.Usuario)
                .WithMany()
                .HasForeignKey(c => c.UsuarioId)
                .OnDelete(DeleteBehavior.SetNull);

            // Relacionamento 1:1 com submissão
            e.HasOne(c => c.Submissao)
                .WithOne(s => s.Candidato)
                .HasForeignKey<Submissao>(s => s.CandidatoId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    // -------------------------------------------------------
    // Configuração: Submissao
    // -------------------------------------------------------
    private static void ConfigurarSubmissao(ModelBuilder model)
    {
        model.Entity<Submissao>(e =>
        {
            e.ToTable("submissoes");

            e.HasKey(s => s.Id);
            e.Property(s => s.Id).HasColumnName("id");

            e.Property(s => s.CandidatoId)
                .HasColumnName("candidato_id");

            e.Property(s => s.RespostaQ1)
                .HasColumnName("resposta_q1");

            // JSONB: tipo nativo do PostgreSQL para JSON com indexação
            e.Property(s => s.RespostasQ2Json)
                .HasColumnName("respostas_q2")
                .HasColumnType("jsonb");

            e.Property(s => s.RespostaQ3)
                .HasColumnName("resposta_q3");

            e.Property(s => s.PontuacaoQ2)
                .HasColumnName("pontuacao_q2");

            e.Property(s => s.Status)
                .HasColumnName("status")
                .HasConversion<int>();

            e.Property(s => s.IniciadoEm)
                .HasColumnName("iniciado_em");

            e.Property(s => s.EncerradoEm)
                .HasColumnName("encerrado_em");

            e.Property(s => s.EncerradoPorTimeout)
                .HasColumnName("encerrado_por_timeout")
                .HasDefaultValue(false);

            // Índices para as queries mais comuns da tela "Testes Realizados"
            e.HasIndex(s => s.Status)
                .HasDatabaseName("ix_submissoes_status");

            e.HasIndex(s => s.EncerradoEm)
                .HasDatabaseName("ix_submissoes_encerrado_em");
        });
    }

    // -------------------------------------------------------
    // Seed: usuários iniciais
    //
    // Os hashes abaixo correspondem às senhas:
    //   Recrutador: Recrutador@2025
    //   TI:         TI@2025
    //
    // IMPORTANTE: altere as senhas no primeiro acesso em produção.
    // Os hashes foram gerados com BCrypt custo 12.
    // -------------------------------------------------------
    private static void SeedUsuarios(ModelBuilder model)
    {
        // Para gerar novos hashes em C#:
        // BCrypt.Net.BCrypt.HashPassword("SuaSenha", workFactor: 12)
        const string hashRecrutador =
            "$2a$12$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi";

        const string hashTI =
            "$2a$12$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi";

        // HasData requer que o Id seja explícito (não gerado pelo banco)
        // e que as propriedades private set sejam acessíveis via reflection.
        // Por isso usamos o construtor sem parâmetros com valores diretos.
        model.Entity<Usuario>().HasData(
            new
            {
                Id        = 1,
                Nome      = "Recrutador",
                Email     = "recrutador@empresa.com.br",
                SenhaHash = hashRecrutador,
                Perfil    = PerfilUsuario.Recrutador,
                Ativo     = true,
                CriadoEm = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new
            {
                Id        = 2,
                Nome      = "Time de TI",
                Email     = "ti@empresa.com.br",
                SenhaHash = hashTI,
                Perfil    = PerfilUsuario.TI,
                Ativo     = true,
                CriadoEm = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}