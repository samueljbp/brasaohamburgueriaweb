using BrasaoSolution.Casa.Model;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BrasaoSolution.Web.Casa
{
    public class BrasaoSolutionContext : DbContext
    {
        public BrasaoSolutionContext(DbContextOptions<BrasaoSolutionContext> options) : base(options)
        {
            base.ChangeTracker.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            var cascadeFKs = modelBuilder.Model.GetEntityTypes()
        .SelectMany(t => t.GetForeignKeys())
        .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

            foreach (var fk in cascadeFKs)
                fk.DeleteBehavior = DeleteBehavior.Restrict;

            modelBuilder.Entity<ClasseItemCardapioPromocaoVenda>()
            .HasKey(c => new { c.CodPromocaoVenda, c.CodClasse });

            modelBuilder.Entity<ComboItemCardapio>()
            .HasKey(c => new { c.CodCombo, c.CodItemCardapio });

            modelBuilder.Entity<DiaSemanaCombo>()
            .HasKey(c => new { c.CodCombo, c.DiaSemana });

            modelBuilder.Entity<DiaSemanaPromocaoVenda>()
            .HasKey(c => new { c.CodPromocaoVenda, c.DiaSemana });

            modelBuilder.Entity<ExtraItemPedido>()
            .HasKey(c => new { c.CodPedido, c.SeqItem, c.CodOpcaoExtra });

            modelBuilder.Entity<ExtratoUsuarioProgramaFidelidade>()
            .HasKey(c => new { c.LoginUsuario, c.CodProgramaFidelidade, c.DataHoraLancamento });

            modelBuilder.Entity<FuncionamentoEstabelecimento>()
            .HasKey(c => new { c.DiaSemana, c.Abertura, c.CodEmpresa });

            modelBuilder.Entity<HistoricoPedido>()
            .HasKey(c => new { c.CodPedido, c.DataHora });

            modelBuilder.Entity<ItemCardapioImpressora>()
            .HasKey(c => new { c.CodItemCardapio, c.CodImpressora });

            modelBuilder.Entity<ItemCardapioPromocaoVenda>()
            .HasKey(c => new { c.CodPromocaoVenda, c.CodItemCardapio });

            modelBuilder.Entity<ItemPedido>()
            .HasKey(c => new { c.CodPedido, c.SeqItem });

            modelBuilder.Entity<ObservacaoItemPedido>()
            .HasKey(c => new { c.CodPedido, c.SeqItem, c.CodObservacao });

            modelBuilder.Entity<ObservacaoProducaoPermitidaItemCardapio>()
            .HasKey(c => new { c.CodItemCardapio, c.CodObservacao });

            modelBuilder.Entity<OpcaoExtraItemCardapio>()
            .HasKey(c => new { c.CodItemCardapio, c.CodOpcaoExtra });

            modelBuilder.Entity<ParametroSistema>()
            .HasKey(c => new { c.CodParametro, c.CodEmpresa });

            modelBuilder.Entity<SaldoUsuarioProgramaFidelidade>()
            .HasKey(c => new { c.LoginUsuario, c.CodProgramaFidelidade });

            modelBuilder.Entity<UsuarioParticipanteProgramaFidelidade>()
            .HasKey(c => new { c.LoginUsuario, c.CodProgramaFidelidade });

            base.OnModelCreating(modelBuilder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                return base.SaveChangesAsync(cancellationToken);
            }
            catch (System.Exception ex)
            {
                string mensagem = "";
                var allExceptions = BrasaoSolution.Helper.BrasaoUtil.GetInnerExceptions(ex);
                if (allExceptions != null)
                {
                    foreach (var exc in allExceptions)
                    {
                        mensagem += "\nErro: " + exc.Message;
                    }
                }
            }

            throw new System.Exception("Erro desconhecido.");
        }

        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (System.Exception ex)
            {
                string mensagem = "";
                var allExceptions = BrasaoSolution.Helper.BrasaoUtil.GetInnerExceptions(ex);
                if (allExceptions != null)
                {
                    foreach (var exc in allExceptions)
                    {
                        mensagem += "\nErro: " + exc.Message;
                    }
                }
            }

            throw new System.Exception("Erro desconhecido.");
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
        public DbSet<FormaPagamento> FormasPagamento { get; set; }
        public DbSet<BandeiraCartao> BandeirasCartao { get; set; }
        public DbSet<HistoricoPedido> HistoricosPedido { get; set; }
        public DbSet<Empresa> Empresas { get; set; }
        public DbSet<Bairro> Bairros { get; set; }
        public DbSet<Cidade> Cidades { get; set; }
    }
}