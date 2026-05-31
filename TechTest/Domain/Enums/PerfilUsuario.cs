// Domain/Enums/PerfilUsuario.cs
namespace TechTest.Domain.Enums;

/// <summary>
/// Define os perfis de acesso do sistema.
/// Candidato não tem perfil — não realiza login.
/// </summary>
public enum PerfilUsuario
{
    Recrutador = 1,
    TI         = 2
}