using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using BrasaoSolution.Model;
using System.Data.Entity.Validation;

namespace BrasaoSolution.Repository.Context
{
    public class CodeConfig : DbConfiguration
    {
        public CodeConfig()
        {
            SetProviderServices("System.Data.SqlClient",
                System.Data.Entity.SqlServer.SqlProviderServices.Instance);
        }
    }

    [DbConfigurationType(typeof(CodeConfig))]
    public class BrasaoContext : DbContext
    {
        public BrasaoContext()
            : base("BrasaoContext")
        {
            //disable initializer
            Database.CreateIfNotExists();
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<BrasaoContext, BrasaoSolution.Repository.Migrations.Brasao.Configuration>());
            this.Configuration.LazyLoadingEnabled = false;
        }

        public BrasaoContext(string connectionString)
            : base(connectionString)
        {
            //disable initializer
            Database.CreateIfNotExists();
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<BrasaoContext, BrasaoSolution.Repository.Migrations.Brasao.Configuration>());
            this.Configuration.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<System.Data.Entity.ModelConfiguration.Conventions.ManyToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<System.Data.Entity.ModelConfiguration.Conventions.OneToManyCascadeDeleteConvention>();
        }

        public override Task<int> SaveChangesAsync()
        {
            try
            {
                return base.SaveChangesAsync();
            }
            catch (DbEntityValidationException e)
            {
                string mensagem = "";
                foreach (var eve in e.EntityValidationErrors)
                {
                    mensagem += "A entidade do tipo " + eve.Entry.Entity.GetType().Name + " possui os seguintes erros de validação:";
                    foreach (var ve in eve.ValidationErrors)
                    {
                        mensagem += "\nPropriedade: " + ve.PropertyName + ", Erro: " + ve.ErrorMessage;
                    }
                }

                throw new System.Exception(mensagem);
            }
            catch (System.Exception ex)
            {
                string mensagem = "";
                var allExceptions = BrasaoSolution.Helper.BrasaoUtil.GetInnerExceptions(ex);
                if (allExceptions != null)
                {
                    foreach(var exc in allExceptions)
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
            catch (DbEntityValidationException e)
            {
                string mensagem = "";
                foreach (var eve in e.EntityValidationErrors)
                {
                    mensagem += "A entidade do tipo " + eve.Entry.Entity.GetType().Name + " possui os seguintes erros de validação:";
                    foreach (var ve in eve.ValidationErrors)
                    {
                        mensagem += "\nPropriedade: " + ve.PropertyName + ", Erro: " + ve.ErrorMessage;
                    }
                }

                throw new System.Exception(mensagem);
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