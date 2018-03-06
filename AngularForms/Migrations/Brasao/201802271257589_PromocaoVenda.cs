namespace BrasaoHamburgueria.Web.Migrations.Brasao
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PromocaoVenda : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ITEM_CARDAPIO_PROMOCAO_VENDA",
                c => new
                    {
                        COD_PROMOCAO_VENDA = c.Int(nullable: false),
                        COD_ITEM_CARDAPIO = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.COD_PROMOCAO_VENDA, t.COD_ITEM_CARDAPIO })
                .ForeignKey("dbo.PROMOCAO_VENDA", t => t.COD_PROMOCAO_VENDA)
                .ForeignKey("dbo.ITEM_CARDAPIO", t => t.COD_ITEM_CARDAPIO)
                .Index(t => t.COD_PROMOCAO_VENDA)
                .Index(t => t.COD_ITEM_CARDAPIO);
            
            CreateTable(
                "dbo.PROMOCAO_VENDA",
                c => new
                    {
                        COD_PROMOCAO_VENDA = c.Int(nullable: false),
                        DESCRICAO_PROMOCAO = c.String(nullable: false),
                        DATA_INICIO = c.DateTime(nullable: false),
                        DATA_FIM = c.DateTime(nullable: false),
                        COD_TIPO_APLICACAO_DESCONTO = c.Int(nullable: false),
                        PERCENTUAL_DESCONTO = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PROMOCAO_ATIVA = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.COD_PROMOCAO_VENDA)
                .ForeignKey("dbo.TIPO_APLICACAO_DESCONTO_PROMOCAO", t => t.COD_TIPO_APLICACAO_DESCONTO)
                .Index(t => t.COD_TIPO_APLICACAO_DESCONTO);
            
            CreateTable(
                "dbo.CLASSE_ITEM_CARDAPIO_PROMOCAO_VENDA",
                c => new
                    {
                        COD_PROMOCAO_VENDA = c.Int(nullable: false),
                        COD_CLASSE = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.COD_PROMOCAO_VENDA, t.COD_CLASSE })
                .ForeignKey("dbo.PROMOCAO_VENDA", t => t.COD_PROMOCAO_VENDA)
                .ForeignKey("dbo.CLASSE_ITEM_CARDAPIO", t => t.COD_CLASSE)
                .Index(t => t.COD_PROMOCAO_VENDA)
                .Index(t => t.COD_CLASSE);
            
            CreateTable(
                "dbo.DIA_SEMANA_PROMOCAO_VENDA",
                c => new
                    {
                        COD_PROMOCAO_VENDA = c.Int(nullable: false),
                        DIA_SEMANA = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.COD_PROMOCAO_VENDA, t.DIA_SEMANA })
                .ForeignKey("dbo.PROMOCAO_VENDA", t => t.COD_PROMOCAO_VENDA)
                .Index(t => t.COD_PROMOCAO_VENDA);
            
            CreateTable(
                "dbo.TIPO_APLICACAO_DESCONTO_PROMOCAO",
                c => new
                    {
                        COD_TIPO_APLICACAO_DESCONTO = c.Int(nullable: false),
                        DESCRICAO = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.COD_TIPO_APLICACAO_DESCONTO);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CLASSE_ITEM_CARDAPIO_PROMOCAO_VENDA", "COD_CLASSE", "dbo.CLASSE_ITEM_CARDAPIO");
            DropForeignKey("dbo.ITEM_CARDAPIO_PROMOCAO_VENDA", "COD_ITEM_CARDAPIO", "dbo.ITEM_CARDAPIO");
            DropForeignKey("dbo.PROMOCAO_VENDA", "COD_TIPO_APLICACAO_DESCONTO", "dbo.TIPO_APLICACAO_DESCONTO_PROMOCAO");
            DropForeignKey("dbo.ITEM_CARDAPIO_PROMOCAO_VENDA", "COD_PROMOCAO_VENDA", "dbo.PROMOCAO_VENDA");
            DropForeignKey("dbo.DIA_SEMANA_PROMOCAO_VENDA", "COD_PROMOCAO_VENDA", "dbo.PROMOCAO_VENDA");
            DropForeignKey("dbo.CLASSE_ITEM_CARDAPIO_PROMOCAO_VENDA", "COD_PROMOCAO_VENDA", "dbo.PROMOCAO_VENDA");
            DropIndex("dbo.DIA_SEMANA_PROMOCAO_VENDA", new[] { "COD_PROMOCAO_VENDA" });
            DropIndex("dbo.CLASSE_ITEM_CARDAPIO_PROMOCAO_VENDA", new[] { "COD_CLASSE" });
            DropIndex("dbo.CLASSE_ITEM_CARDAPIO_PROMOCAO_VENDA", new[] { "COD_PROMOCAO_VENDA" });
            DropIndex("dbo.PROMOCAO_VENDA", new[] { "COD_TIPO_APLICACAO_DESCONTO" });
            DropIndex("dbo.ITEM_CARDAPIO_PROMOCAO_VENDA", new[] { "COD_ITEM_CARDAPIO" });
            DropIndex("dbo.ITEM_CARDAPIO_PROMOCAO_VENDA", new[] { "COD_PROMOCAO_VENDA" });
            DropTable("dbo.TIPO_APLICACAO_DESCONTO_PROMOCAO");
            DropTable("dbo.DIA_SEMANA_PROMOCAO_VENDA");
            DropTable("dbo.CLASSE_ITEM_CARDAPIO_PROMOCAO_VENDA");
            DropTable("dbo.PROMOCAO_VENDA");
            DropTable("dbo.ITEM_CARDAPIO_PROMOCAO_VENDA");
        }
    }
}
