using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using BrasaoHamburgueria.Model;

namespace BrasaoHamburgueria.Web.Context
{
    public class BrasaoContext : DbContext
    {
        public BrasaoContext()
            : base("BrasaoContext")
        {
            //disable initializer
            Database.CreateIfNotExists();
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<BrasaoContext, BrasaoHamburgueria.Web.Migrations.Brasao.Configuration>());
            this.Configuration.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<System.Data.Entity.ModelConfiguration.Conventions.ManyToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<System.Data.Entity.ModelConfiguration.Conventions.OneToManyCascadeDeleteConvention>();
        }

        public DbSet<ClasseItemCardapio> Classes { get; set; }
        public DbSet<ItemCardapio> ItensCardapio { get; set; }
        public DbSet<ComplementoItemCardapio> ComplementosItens { get; set; }
        public DbSet<ObservacaoProducao> ObservacoesProducao { get; set; }
        public DbSet<ObservacaoProducaoPermitidaItemCardapio> ObservacoesPermitidas { get; set; }
        public DbSet<OpcaoExtra> Extras { get; set; }
        public DbSet<OpcaoExtraItemCardapio> ExtrasPermitidos { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<ItemPedido> ItensPedidos { get; set; }
        public DbSet<ObservacaoItemPedido> ObservacoesItensPedidos { get; set; }
        public DbSet<ExtraItemPedido> ExtrasItensPedidos { get; set; }
        public DbSet<ParametroSistema> ParametrosSistema { get; set; }
        public DbSet<FuncionamentoEstabelecimento> FuncionamentosEstabelecimento { get; set; }
        public DbSet<ImpressoraProducao> ImpressorasProducao { get; set; }
        public DbSet<ItemCardapioImpressora> ImpressorasItens { get; set; }
        public DbSet<ProgramaFidelidade> ProgramasFidelidade { get; set; }
        public DbSet<UsuarioParticipanteProgramaFidelidade> UsuariosParticipantesProgramaFidelidade { get; set; }
        public DbSet<TipoPontuacaoProgramaFidelidade> TiposPontuacaoProgramaFidelidade { get; set; }
        public DbSet<ExtratoUsuarioProgramaFidelidade> ExtratosUsuariosProgramasFidelidade { get; set; }
        public DbSet<SaldoUsuarioProgramaFidelidade> SaldosUsuariosProgramasFidelidade { get; set; }
        public DbSet<PontuacaoDinheiroProgramaFidelidade> PontuacoesDinheiroProgramaFidelidade { get; set; }
        public DbSet<PromocaoVenda> PromocoesVenda { get; set; }
        public DbSet<TipoAplicacaoDescontoPromocao> TiposDescontoPromocao { get; set; }
        public DbSet<ClasseItemCardapioPromocaoVenda> ClassesPromocaoVenda { get; set; }
        public DbSet<ItemCardapioPromocaoVenda> ItensPromocaoVenda { get; set; }
        public DbSet<DiaSemanaPromocaoVenda> DiasSemanaPromocaoVenda { get; set; }
        public DbSet<DiaSemanaCombo> DiasSemanaCombo { get; set; }
        public DbSet<Combo> Combos { get; set; }
        public DbSet<ComboItemCardapio> ItensCombo { get; set; }
        public DbSet<Entregador> Entregadores { get; set; }
    }
}