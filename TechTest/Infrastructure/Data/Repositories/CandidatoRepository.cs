// Infrastructure/Data/Repositories/CandidatoRepository.cs
using Microsoft.EntityFrameworkCore;
using TechTest.Domain.Entities;
using TechTest.Domain.Interfaces.Repositories;

namespace TechTest.Infrastructure.Data.Repositories;

public sealed class CandidatoRepository : ICandidatoRepository
{
    private readonly AppDbContext _db;

    public CandidatoRepository(AppDbContext db) => _db = db;

    public Task<Candidato?> ObterPorIdComSubmissaoAsync(int id, CancellationToken ct)
        => _db.Candidatos
              .Include(c => c.Submissao)
              .FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<int> SalvarAsync(Candidato candidato, CancellationToken ct)
    {
        _db.Candidatos.Add(candidato);
        await _db.SaveChangesAsync(ct);
        return candidato.Id;
    }
}