// Infrastructure/Data/Repositories/UsuarioRepository.cs
using Microsoft.EntityFrameworkCore;
using TechTest.Domain.Entities;
using TechTest.Domain.Interfaces.Repositories;

namespace TechTest.Infrastructure.Data.Repositories;

public sealed class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbContext _db;

    public UsuarioRepository(AppDbContext db) => _db = db;

    public Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken ct)
        => _db.Usuarios
              .AsNoTracking()
              .FirstOrDefaultAsync(u => u.Email == email && u.Ativo, ct);

    public Task<Usuario?> ObterPorIdAsync(int id, CancellationToken ct)
        => _db.Usuarios
              .AsNoTracking()
              .FirstOrDefaultAsync(u => u.Id == id, ct);
}