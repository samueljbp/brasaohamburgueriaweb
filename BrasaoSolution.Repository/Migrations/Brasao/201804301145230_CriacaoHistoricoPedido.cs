namespace BrasaoSolution.Repository.Migrations.Brasao
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CriacaoHistoricoPedido : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.HISTORICO_PEDIDO",
                c => new
                    {
                        COD_PEDIDO = c.Int(nullable: false),
                        DATA_HORA = c.DateTime(nullable: false),
                        COD_SITUACAO = c.Int(),
                        USUARIO = c.String(nullable: false),
                        DESCRICAO = c.String(nullable: false),
                    })
                .PrimaryKey(t => new { t.COD_PEDIDO, t.DATA_HORA })
                .ForeignKey("dbo.SITUACAO_PEDIDO", t => t.COD_SITUACAO)
                .ForeignKey("dbo.PEDIDO", t => t.COD_PEDIDO)
                .Index(t => t.COD_PEDIDO)
                .Index(t => t.COD_SITUACAO);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.HISTORICO_PEDIDO", "COD_PEDIDO", "dbo.PEDIDO");
            DropForeignKey("dbo.HISTORICO_PEDIDO", "COD_SITUACAO", "dbo.SITUACAO_PEDIDO");
            DropIndex("dbo.HISTORICO_PEDIDO", new[] { "COD_SITUACAO" });
            DropIndex("dbo.HISTORICO_PEDIDO", new[] { "COD_PEDIDO" });
            DropTable("dbo.HISTORICO_PEDIDO");
        }
    }
}
