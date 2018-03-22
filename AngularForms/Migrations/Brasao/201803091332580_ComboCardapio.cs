namespace BrasaoHamburgueria.Web.Migrations.Brasao
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ComboCardapio : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.COMBO_ITEM_CARDAPIO",
                c => new
                    {
                        COB_COMBO = c.Int(nullable: false),
                        COD_ITEM_CARDAPIO = c.Int(nullable: false),
                        QUANTIDADE = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.COB_COMBO, t.COD_ITEM_CARDAPIO })
                .ForeignKey("dbo.COMBO", t => t.COB_COMBO)
                .ForeignKey("dbo.ITEM_CARDAPIO", t => t.COD_ITEM_CARDAPIO)
                .Index(t => t.COB_COMBO)
                .Index(t => t.COD_ITEM_CARDAPIO);
            
            CreateTable(
                "dbo.COMBO",
                c => new
                    {
                        COD_COMBO = c.Int(nullable: false),
                        NOME_COMBO = c.String(nullable: false),
                        DESCRICAO_COMBO = c.String(nullable: false),
                        PRECO_COMBO = c.Double(nullable: false),
                        ATIVO = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.COD_COMBO);
            
            AddColumn("dbo.ITEM_PEDIDO", "COD_COMBO", c => c.Int());
            AddColumn("dbo.ITEM_PEDIDO", "PRECO_COMBO", c => c.Double(nullable: false));
            AlterColumn("dbo.ITEM_PEDIDO", "COD_PROMOCAO_VENDA", c => c.Int());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.COMBO_ITEM_CARDAPIO", "COD_ITEM_CARDAPIO", "dbo.ITEM_CARDAPIO");
            DropForeignKey("dbo.COMBO_ITEM_CARDAPIO", "COB_COMBO", "dbo.COMBO");
            DropIndex("dbo.COMBO_ITEM_CARDAPIO", new[] { "COD_ITEM_CARDAPIO" });
            DropIndex("dbo.COMBO_ITEM_CARDAPIO", new[] { "COB_COMBO" });
            AlterColumn("dbo.ITEM_PEDIDO", "COD_PROMOCAO_VENDA", c => c.Double());
            DropColumn("dbo.ITEM_PEDIDO", "PRECO_COMBO");
            DropColumn("dbo.ITEM_PEDIDO", "COD_COMBO");
            DropTable("dbo.COMBO");
            DropTable("dbo.COMBO_ITEM_CARDAPIO");
        }
    }
}
