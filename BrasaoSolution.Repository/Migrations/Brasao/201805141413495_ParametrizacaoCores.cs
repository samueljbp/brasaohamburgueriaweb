namespace BrasaoSolution.Repository.Migrations.Brasao
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ParametrizacaoCores : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EMPRESA", "COD_PRINCIPAL", c => c.String(nullable: false));
            AddColumn("dbo.EMPRESA", "COR_SECUNDARIA", c => c.String(nullable: false));
            AddColumn("dbo.EMPRESA", "COD_PRINCIPAL_CONTRASTE", c => c.String(nullable: false));
            AddColumn("dbo.EMPRESA", "COD_DESTAQUE", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.EMPRESA", "COD_DESTAQUE");
            DropColumn("dbo.EMPRESA", "COD_PRINCIPAL_CONTRASTE");
            DropColumn("dbo.EMPRESA", "COR_SECUNDARIA");
            DropColumn("dbo.EMPRESA", "COD_PRINCIPAL");
        }
    }
}
