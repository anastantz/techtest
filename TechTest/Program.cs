// Program.cs
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using TechTest.Application.Auth;
using TechTest.Application.Candidatos;
using TechTest.Application.Submissoes;
using TechTest.Application.Testes;
using TechTest.Domain.Interfaces.Repositories;
using TechTest.Domain.Interfaces.Services;
using TechTest.Infrastructure.Data;
using TechTest.Infrastructure.Data.Repositories;
using TechTest.Infrastructure.Pdf;

var builder = WebApplication.CreateBuilder(args);

// ── Banco de dados ──────────────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsql => npgsql.EnableRetryOnFailure(3)
    )
);

// ── Autenticação por cookie ─────────────────────────────────
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(opt =>
    {
        opt.LoginPath         = "/Auth/Login";
        opt.LogoutPath        = "/Auth/Logout";
        opt.AccessDeniedPath  = "/Auth/Login";
        opt.ExpireTimeSpan    = TimeSpan.FromHours(8);
        opt.SlidingExpiration = true;
        opt.Cookie.HttpOnly   = true;
        opt.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        opt.Cookie.SameSite     = SameSiteMode.Strict;
        opt.Cookie.Name         = "TechTest.Auth";
    });

// ── Autorização ─────────────────────────────────────────────
builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("PerfilRecrutador",     p => p.RequireClaim("Perfil", "Recrutador"));
    opt.AddPolicy("PerfilTI",             p => p.RequireClaim("Perfil", "TI"));
    opt.AddPolicy("PerfilRecrutadorOuTI", p => p.RequireClaim("Perfil", "Recrutador", "TI"));
});

builder.Services.AddRazorPages(opt =>
{
    opt.Conventions.AuthorizeFolder("/");
    opt.Conventions.AllowAnonymousToPage("/Auth/Login");
});

// ── Injeção de dependência ───────────────────────────────────
// Repositórios (Scoped: uma instância por request)
builder.Services.AddScoped<IUsuarioRepository,   UsuarioRepository>();
builder.Services.AddScoped<ICandidatoRepository, CandidatoRepository>();
builder.Services.AddScoped<ISubmissaoRepository, SubmissaoRepository>();

// Serviços (Scoped)
builder.Services.AddScoped<IAuthService,      AuthService>();
builder.Services.AddScoped<ICandidatoService, CandidatoService>();
builder.Services.AddScoped<ISubmissaoService, SubmissaoService>();
builder.Services.AddScoped<IPdfService,       QuestPdfService>();

// TesteService: Singleton — dados estáticos, sem estado mutável
builder.Services.AddSingleton<ITesteService, TesteService>();

// ── Pipeline ────────────────────────────────────────────────
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Erro");
    app.UseHsts();
}
else
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();

app.Run();