namespace BrasaoSolution.Repository.Migrations.Brasao
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RetiraObrigatoriedadeFundos : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.EMPRESA", "IMAGEM_BACKGROUND_PUBLICA", c => c.String());
            AlterColumn("dbo.EMPRESA", "IMAGEM_BACKGROUND_AUTENTICADA", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.EMPRESA", "IMAGEM_BACKGROUND_AUTENTICADA", c => c.String(nullable: false));
            AlterColumn("dbo.EMPRESA", "IMAGEM_BACKGROUND_PUBLICA", c => c.String(nullable: false));
        }
    }
}
