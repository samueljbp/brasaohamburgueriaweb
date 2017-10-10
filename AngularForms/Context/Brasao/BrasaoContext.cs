using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using BrasaoHamburgueria.Model;

namespace BrasaoHamburgueriaWeb.Context
{
    public class BrasaoContext : DbContext
    {
        public BrasaoContext()
            : base("BrasaoContext")
        {
            //disable initializer
            Database.CreateIfNotExists();
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<BrasaoContext, BrasaoHamburgueriaWeb.Migrations.Brasao.Configuration>());
            this.Configuration.LazyLoadingEnabled = false;
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
        public DbSet<ParametrosSistema> ParametrosSistema { get; set; }
        public DbSet<FuncionamentoEstabelecimento> FuncionamentosEstabelecimento { get; set; }
        public DbSet<ImpressoraProducao> ImpressorasProducao { get; set; }
    }
}