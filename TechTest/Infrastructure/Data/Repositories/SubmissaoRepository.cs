// Infrastructure/Data/Repositories/SubmissaoRepository.cs
using Microsoft.EntityFrameworkCore;
using TechTest.Domain.Entities;
using TechTest.Domain.Enums;
using TechTest.Domain.Interfaces.Repositories;

namespace TechTest.Infrastructure.Data.Repositories;

public sealed class SubmissaoRepository : ISubmissaoRepository
{
    private readonly AppDbContext _db;

    public SubmissaoRepository(AppDbContext db) => _db = db;

    public Task<Submissao?> ObterCompletaPorCandidatoAsync(int candidatoId, CancellationToken ct)
        => _db.Submissoes
              .Include(s => s.Candidato)
                  .ThenInclude(c => c!.Usuario)
              .FirstOrDefaultAsync(s => s.CandidatoId == candidatoId, ct);

    public async Task<List<Submissao>> ListarOrdenadosAsync(
        DateTime? dataInicio    = null,
        DateTime? dataFim       = null,
        string?   termoBusca    = null,
        StatusSubmissao? status = null,
        int pagina              = 1,
        int itensPorPagina      = 20,
        CancellationToken ct    = default)
    {
        var query = _db.Submissoes
            .Include(s => s.Candidato)
            .AsNoTracking()
            .AsQueryable();

        // Filtros opcionais — só aplicados quando informados
        if (dataInicio.HasValue)
            query = query.Where(s => s.EncerradoEm >= dataInicio.Value);

        if (dataFim.HasValue)
            query = query.Where(s => s.EncerradoEm <= dataFim.Value.AddDays(1));

        if (status.HasValue)
            query = query.Where(s => s.Status == status.Value);

        if (!string.IsNullOrWhiteSpace(termoBusca))
        {
            var termo = termoBusca.Trim().ToLower();
            query = query.Where(s =>
                s.Candidato!.NomeCompleto.ToLower().Contains(termo) ||
                s.Id.ToString().Contains(termo) ||
                s.CandidatoId.ToString().Contains(termo));
        }

        return await query
            .OrderByDescending(s => s.EncerradoEm)
            .Skip((pagina - 1) * itensPorPagina)
            .Take(itensPorPagina)
            .ToListAsync(ct);
    }

    public async Task<int> TotalAsync(
        DateTime? dataInicio    = null,
        DateTime? dataFim       = null,
        string?   termoBusca    = null,
        StatusSubmissao? status = null,
        CancellationToken ct    = default)
    {
        var query = _db.Submissoes.AsNoTracking().AsQueryable();

        if (dataInicio.HasValue)
            query = query.Where(s => s.EncerradoEm >= dataInicio.Value);

        if (dataFim.HasValue)
            query = query.Where(s => s.EncerradoEm <= dataFim.Value.AddDays(1));

        if (status.HasValue)
            query = query.Where(s => s.Status == status.Value);

        if (!string.IsNullOrWhiteSpace(termoBusca))
        {
            var termo = termoBusca.Trim().ToLower();
            query = query.Where(s =>
                s.Candidato!.NomeCompleto.ToLower().Contains(termo) ||
                s.Id.ToString().Contains(termo));
        }

        return await query.CountAsync(ct);
    }

    public async Task SalvarAsync(Submissao submissao, CancellationToken ct)
    {
        _db.Submissoes.Add(submissao);
        await _db.SaveChangesAsync(ct);
    }

    public async Task AtualizarAsync(Submissao submissao, CancellationToken ct)
    {
        _db.Submissoes.Update(submissao);
        await _db.SaveChangesAsync(ct);
    }
}