namespace BrasaoSolution.Repository.Migrations.Brasao
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CodBairroPedidoNull : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.PEDIDO", new[] { "COD_BAIRRO" });
            AlterColumn("dbo.PEDIDO", "COD_BAIRRO", c => c.Int());
            CreateIndex("dbo.PEDIDO", "COD_BAIRRO");
        }
        
        public override void Down()
        {
            DropIndex("dbo.PEDIDO", new[] { "COD_BAIRRO" });
            AlterColumn("dbo.PEDIDO", "COD_BAIRRO", c => c.Int(nullable: false));
            CreateIndex("dbo.PEDIDO", "COD_BAIRRO");
        }
    }
}
