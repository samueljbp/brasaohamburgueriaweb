using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace AngularForms.Model
{
    public class BrasaoContext : DbContext
    {
        public BrasaoContext()
            : base("BrasaoContext")
        {
            //disable initializer
            //Database.SetInitializer<BrasaoContext>(null);
            this.Configuration.LazyLoadingEnabled = false;
        }

        public DbSet<ClasseItemCardapio> Classes { get; set; }
        public DbSet<ItemCardapio> ItensCardapio { get; set; }
        public DbSet<ComplementoItemCardapio> ComplementosItens { get; set; }
        public DbSet<ObservacaoProducao> ObservacoesProducao { get; set; }
        public DbSet<ObservacaoProducaoPermitidaItemCardapio> ObservacoesPermitidas { get; set; }
        public DbSet<OpcaoExtra> Extras { get; set; }
        public DbSet<OpcaoExtraItemCardapio> ExtrasPermitidos { get; set; }
    }
}