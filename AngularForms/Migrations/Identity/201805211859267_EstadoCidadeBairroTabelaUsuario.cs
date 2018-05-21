namespace BrasaoHamburgueria.Web.Migrations.Identity
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EstadoCidadeBairroTabelaUsuario : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Usuarios", "CodCidade", c => c.Int(nullable: false));
            AddColumn("dbo.Usuarios", "CodBairro", c => c.Int(nullable: false));
            DropColumn("dbo.Usuarios", "Cidade");
            DropColumn("dbo.Usuarios", "Bairro");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Usuarios", "Bairro", c => c.String());
            AddColumn("dbo.Usuarios", "Cidade", c => c.String());
            DropColumn("dbo.Usuarios", "CodBairro");
            DropColumn("dbo.Usuarios", "CodCidade");
        }
    }
}
