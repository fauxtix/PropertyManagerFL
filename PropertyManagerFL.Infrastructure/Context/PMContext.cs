using Microsoft.EntityFrameworkCore;
using PropertyManagerFL.Core.Entities;

namespace PropertyManagerFL.Infrastructure.Context;

public class PMContext : DbContext
{
    public DbSet<Arrendamento>? Arrendamentos { get; set; }
    public DbSet<CategoriaDespesa>? CategoriasDespesas { get; set; }
    public DbSet<Contact>? Contactos  { get; set; }
    public DbSet<Contrato>? Contratos { get; set; }
    public DbSet<DadosOutorgante>? Outorgantes { get; set; }
    public DbSet<Despesa>? Despesas { get; set; }
    //public DbSet<Utilizador> Utilizadores { get; set; }
    public DbSet<Funcao>? Funcoes { get; set; }
    public DbSet<EstadoCivil>? EstadosCivis { get; set; }
    public DbSet<EstadoConservacao>? Conservacoes { get; set; }
    public DbSet<Fiador>? Fiadores { get; set; }
    public DbSet<FormaPagamento>? FormasPagamento { get; set; }
    public DbSet<Fracao>? Fraccoes { get; set; }
    public DbSet<HelpIndex>? HelpIndexes { get; set; }
    public DbSet<HelpIndex_Parent>? HelpIndexes_Parent { get; set; }
    public DbSet<Imovel>? Imoveis { get; set; }
    public DbSet<Inquilino>? Inquilinos { get; set; }
    public DbSet<Mediador>? Mediadores { get; set; }
    public DbSet<Proprietario>? Proprietarios { get; set; }
    public DbSet<Recebimento>? Recebimentos { get; set; }
    public DbSet<RoleDetails>? RoleDetails { get; set; }
    //public DbSet<Roles> Roles { get; set; }
    //public DbSet<RolesDetail> RolesDetails { get; set; }
    public DbSet<SituacaoFracao>? SituacaoFraccoes { get; set; }
    public DbSet<TipoContacto>? TipoContactos { get; set; }
    public DbSet<TipoDespesa>? TipoDespesas { get; set; }
    public DbSet<TipologiaFracao>? TipologiaFraccoes { get; set; }
    public DbSet<TipoPropriedade>? TipoPropriedades { get; set; }
    public DbSet<TipoRecebimento>? TipoRecebimentos { get; set; }
    public DbSet<User_Info>? User_Infos { get; set; }



    public PMContext(DbContextOptions options) : base(options)
    {
    }

    //protected override void OnModelCreating(ModelBuilder modelBuilder)
    //{
    //    base.OnModelCreating(modelBuilder);

    //    modelBuilder.ApplyConfiguration(new ClienteConfiguration());
    //    modelBuilder.ApplyConfiguration(new EnderecoConfiguration());
    //    modelBuilder.ApplyConfiguration(new TelefoneConfiguration());
    //    modelBuilder.ApplyConfiguration(new UsuarioConfiguration());
    //}
}