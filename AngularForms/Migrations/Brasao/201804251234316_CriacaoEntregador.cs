namespace BrasaoHamburgueria.Web.Migrations.Brasao
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CriacaoEntregador : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ENTREGADOR",
                c => new
                    {
                        COD_ENTREGADOR = c.Int(nullable: false, identity: true),
                        NOME = c.String(nullable: false),
                        SEXO = c.String(nullable: false),
                        TELEFONE_FIXO = c.String(),
                        TELEFONE_CELULAR = c.String(),
                        EMAIL = c.String(),
                        ENDERECO_COMPLETO = c.String(),
                        CPF = c.String(),
                        OBSERVACAO = c.String(),
                        ORDEM_ACIONAMENTO = c.Int(nullable: false),
                        VALOR_POR_ENTREGA = c.Decimal(precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.COD_ENTREGADOR);
            
            AddColumn("dbo.PEDIDO", "COD_ENTREGADOR", c => c.Int());
            CreateIndex("dbo.PEDIDO", "COD_ENTREGADOR");
            AddForeignKey("dbo.PEDIDO", "COD_ENTREGADOR", "dbo.ENTREGADOR", "COD_ENTREGADOR");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PEDIDO", "COD_ENTREGADOR", "dbo.ENTREGADOR");
            DropIndex("dbo.PEDIDO", new[] { "COD_ENTREGADOR" });
            DropColumn("dbo.PEDIDO", "COD_ENTREGADOR");
            DropTable("dbo.ENTREGADOR");
        }
    }
}
