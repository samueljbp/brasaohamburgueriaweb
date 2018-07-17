using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using BrasaoSolution.ViewModel;
using System.Data.Entity.Validation;

namespace BrasaoSolution.Repository.Context
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public virtual Usuario DadosUsuario { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public const string CLAIM_EMPRESA = "EMPRESA";

        public System.Data.Entity.DbSet<Usuario> DadosUsuarios { get; set; }

        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            Database.CreateIfNotExists();
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext, BrasaoSolution.Web.Migrations.Identity.Configuration>());
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
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
    }
}