namespace BrasaoSolution.Repository.Migrations.Brasao
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AjusteColunasCores : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.EMPRESA", name: "COD_PRINCIPAL", newName: "COR_PRINCIPAL");
            RenameColumn(table: "dbo.EMPRESA", name: "COD_PRINCIPAL_CONTRASTE", newName: "COR_PRINCIPAL_CONTRASTE");
            RenameColumn(table: "dbo.EMPRESA", name: "COD_DESTAQUE", newName: "COR_DESTAQUE");
        }
        
        public override void Down()
        {
            RenameColumn(table: "dbo.EMPRESA", name: "COR_DESTAQUE", newName: "COD_DESTAQUE");
            RenameColumn(table: "dbo.EMPRESA", name: "COR_PRINCIPAL_CONTRASTE", newName: "COD_PRINCIPAL_CONTRASTE");
            RenameColumn(table: "dbo.EMPRESA", name: "COR_PRINCIPAL", newName: "COD_PRINCIPAL");
        }
    }
}
