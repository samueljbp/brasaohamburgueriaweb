namespace BrasaoHamburgueria.Web.Migrations.Brasao
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FormaPagamentoCadastrada : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PEDIDO", "COD_ENTREGADOR", "dbo.ENTREGADOR");
            DropPrimaryKey("dbo.ENTREGADOR");
            CreateTable(
                "dbo.BANDEIRA_CARTAO",
                c => new
                    {
                        COD_BANDEIRA_CARTAO = c.Int(nullable: false),
                        DESCRICAO = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.COD_BANDEIRA_CARTAO);
            
            CreateTable(
                "dbo.FORMA_PAGAMENTO",
                c => new
                    {
                        COD_FORMA_PAGAMENTO = c.String(nullable: false, maxLength: 128),
                        DESCRICAO = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.COD_FORMA_PAGAMENTO);
            
            AddColumn("dbo.PEDIDO", "COD_FORMA_PAGAMENTO", c => c.String(maxLength: 128));
            AddColumn("dbo.PEDIDO", "COD_BANDEIRA_CARTAO", c => c.Int());
            AlterColumn("dbo.ENTREGADOR", "COD_ENTREGADOR", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.ENTREGADOR", "COD_ENTREGADOR");
            CreateIndex("dbo.PEDIDO", "COD_FORMA_PAGAMENTO");
            CreateIndex("dbo.PEDIDO", "COD_BANDEIRA_CARTAO");
            AddForeignKey("dbo.PEDIDO", "COD_BANDEIRA_CARTAO", "dbo.BANDEIRA_CARTAO", "COD_BANDEIRA_CARTAO");
            AddForeignKey("dbo.PEDIDO", "COD_FORMA_PAGAMENTO", "dbo.FORMA_PAGAMENTO", "COD_FORMA_PAGAMENTO");
            AddForeignKey("dbo.PEDIDO", "COD_ENTREGADOR", "dbo.ENTREGADOR", "COD_ENTREGADOR");
            DropColumn("dbo.PEDIDO", "FORMA_PAGAMENTO");
            DropColumn("dbo.PEDIDO", "BANDEIRA_CARTAO");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PEDIDO", "BANDEIRA_CARTAO", c => c.String());
            AddColumn("dbo.PEDIDO", "FORMA_PAGAMENTO", c => c.String(nullable: false));
            DropForeignKey("dbo.PEDIDO", "COD_ENTREGADOR", "dbo.ENTREGADOR");
            DropForeignKey("dbo.PEDIDO", "COD_FORMA_PAGAMENTO", "dbo.FORMA_PAGAMENTO");
            DropForeignKey("dbo.PEDIDO", "COD_BANDEIRA_CARTAO", "dbo.BANDEIRA_CARTAO");
            DropIndex("dbo.PEDIDO", new[] { "COD_BANDEIRA_CARTAO" });
            DropIndex("dbo.PEDIDO", new[] { "COD_FORMA_PAGAMENTO" });
            DropPrimaryKey("dbo.ENTREGADOR");
            AlterColumn("dbo.ENTREGADOR", "COD_ENTREGADOR", c => c.Int(nullable: false, identity: true));
            DropColumn("dbo.PEDIDO", "COD_BANDEIRA_CARTAO");
            DropColumn("dbo.PEDIDO", "COD_FORMA_PAGAMENTO");
            DropTable("dbo.FORMA_PAGAMENTO");
            DropTable("dbo.BANDEIRA_CARTAO");
            AddPrimaryKey("dbo.ENTREGADOR", "COD_ENTREGADOR");
            AddForeignKey("dbo.PEDIDO", "COD_ENTREGADOR", "dbo.ENTREGADOR", "COD_ENTREGADOR");
        }
    }
}
