namespace BrasaoHamburgueria.Web.Migrations.Brasao
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RetirarPedidoNaCasa : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PEDIDO", "RETIRAR_NA_CASA", c => c.Boolean(nullable: false));
            AlterColumn("dbo.PEDIDO", "UF_ENTREGA", c => c.String());
            AlterColumn("dbo.PEDIDO", "CIDADE_ENTREGA", c => c.String());
            AlterColumn("dbo.PEDIDO", "LOGRADOURO_ENTREGA", c => c.String());
            AlterColumn("dbo.PEDIDO", "NUMERO_ENTREGA", c => c.String());
            AlterColumn("dbo.PEDIDO", "COMPLEMENTO_ENTREGA", c => c.String());
            AlterColumn("dbo.PEDIDO", "BAIRRO_ENTREGA", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PEDIDO", "BAIRRO_ENTREGA", c => c.String(nullable: false));
            AlterColumn("dbo.PEDIDO", "COMPLEMENTO_ENTREGA", c => c.String(nullable: false));
            AlterColumn("dbo.PEDIDO", "NUMERO_ENTREGA", c => c.String(nullable: false));
            AlterColumn("dbo.PEDIDO", "LOGRADOURO_ENTREGA", c => c.String(nullable: false));
            AlterColumn("dbo.PEDIDO", "CIDADE_ENTREGA", c => c.String(nullable: false));
            AlterColumn("dbo.PEDIDO", "UF_ENTREGA", c => c.String(nullable: false));
            DropColumn("dbo.PEDIDO", "RETIRAR_NA_CASA");
        }
    }
}
