namespace BrasaoHamburgueria.Web.Migrations.Brasao
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CamposLocalPedido : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PEDIDO", "COD_BAIRRO", c => c.Int(nullable: false, defaultValue: 1));
            CreateIndex("dbo.PEDIDO", "COD_BAIRRO");
            AddForeignKey("dbo.PEDIDO", "COD_BAIRRO", "dbo.BAIRRO", "COD_BAIRRO");
            DropColumn("dbo.PEDIDO", "UF_ENTREGA");
            DropColumn("dbo.PEDIDO", "CIDADE_ENTREGA");
            DropColumn("dbo.PEDIDO", "BAIRRO_ENTREGA");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PEDIDO", "BAIRRO_ENTREGA", c => c.String());
            AddColumn("dbo.PEDIDO", "CIDADE_ENTREGA", c => c.String());
            AddColumn("dbo.PEDIDO", "UF_ENTREGA", c => c.String());
            DropForeignKey("dbo.PEDIDO", "COD_BAIRRO", "dbo.BAIRRO");
            DropIndex("dbo.PEDIDO", new[] { "COD_BAIRRO" });
            DropColumn("dbo.PEDIDO", "COD_BAIRRO");
        }
    }
}
