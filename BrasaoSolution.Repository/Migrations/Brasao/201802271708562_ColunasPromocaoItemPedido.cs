namespace BrasaoSolution.Repository.Migrations.Brasao
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ColunasPromocaoItemPedido : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ITEM_PEDIDO", "COD_PROMOCAO_VENDA", c => c.Double());
            AddColumn("dbo.ITEM_PEDIDO", "PERCENTUAL_DESCONTO", c => c.Double(nullable: false));
            AddColumn("dbo.ITEM_PEDIDO", "VALOR_DESCONTO", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ITEM_PEDIDO", "VALOR_DESCONTO");
            DropColumn("dbo.ITEM_PEDIDO", "PERCENTUAL_DESCONTO");
            DropColumn("dbo.ITEM_PEDIDO", "COD_PROMOCAO_VENDA");
        }
    }
}
