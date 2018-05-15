namespace BrasaoHamburgueria.Web.Migrations.Brasao
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MaisColunasEmpresa : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EMPRESA", "EMAIL", c => c.String());
            AddColumn("dbo.EMPRESA", "FACEBOOK", c => c.String());
            AddColumn("dbo.EMPRESA", "IMAGEM_BACKGROUND_PUBLICA", c => c.String(nullable: false));
            AddColumn("dbo.EMPRESA", "IMAGEM_BACKGROUND_AUTENTICADA", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.EMPRESA", "IMAGEM_BACKGROUND_AUTENTICADA");
            DropColumn("dbo.EMPRESA", "IMAGEM_BACKGROUND_PUBLICA");
            DropColumn("dbo.EMPRESA", "FACEBOOK");
            DropColumn("dbo.EMPRESA", "EMAIL");
        }
    }
}
