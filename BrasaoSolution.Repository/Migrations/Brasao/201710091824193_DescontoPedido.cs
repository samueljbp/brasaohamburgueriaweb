namespace BrasaoSolution.Repository.Migrations.Brasao
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DescontoPedido : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PEDIDO", "PERCENTUAL_DESCONTO", c => c.Double());
            AddColumn("dbo.PEDIDO", "VALOR_DESCONTO", c => c.Double());
            AddColumn("dbo.PEDIDO", "MOTIVO_DESCONTO", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PEDIDO", "MOTIVO_DESCONTO");
            DropColumn("dbo.PEDIDO", "VALOR_DESCONTO");
            DropColumn("dbo.PEDIDO", "PERCENTUAL_DESCONTO");
        }
    }
}
