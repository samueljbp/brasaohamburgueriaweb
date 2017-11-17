namespace BrasaoHamburgueria.Web.Migrations.Brasao
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CriaTabelasProgramaFidelidade : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EXTRATO_USUARIO_PROGRAMA_FIDELIDADE",
                c => new
                    {
                        LOGIN_USUARIO = c.String(nullable: false, maxLength: 128),
                        COD_PROGRAMA_FIDELIDADE = c.Int(nullable: false),
                        DATA_HORA_LANCAMENTO = c.DateTime(nullable: false),
                        DESCRICAO_LANCAMENTO = c.String(nullable: false),
                        VALOR_LANCAMENTO = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SALDO_POS_LANCAMENTO = c.Decimal(nullable: false, precision: 18, scale: 2),
                        COD_PEDIDO = c.Int(),
                    })
                .PrimaryKey(t => new { t.LOGIN_USUARIO, t.COD_PROGRAMA_FIDELIDADE, t.DATA_HORA_LANCAMENTO })
                .ForeignKey("dbo.PROGRAMA_FIDELIDADE", t => t.COD_PROGRAMA_FIDELIDADE)
                .ForeignKey("dbo.PEDIDO", t => t.COD_PEDIDO)
                .Index(t => t.COD_PROGRAMA_FIDELIDADE)
                .Index(t => t.COD_PEDIDO);
            
            CreateTable(
                "dbo.PROGRAMA_FIDELIDADE",
                c => new
                    {
                        COD_PROGRAMA_FIDELIDADE = c.Int(nullable: false),
                        COD_TIPO_PONTUACAO_PROGRAMA_FIDELIDADE = c.Int(nullable: false),
                        DESCRICAO_PROGRAMA_FIDELIDADE = c.String(nullable: false),
                        INICIO_VIGENCIA = c.DateTime(nullable: false),
                        TERMINO_VIGENCIA = c.DateTime(),
                        TERMOS_ACEITE = c.String(nullable: false),
                        PROGRAMA_ATIVO = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.COD_PROGRAMA_FIDELIDADE)
                .ForeignKey("dbo.TIPO_PONTUACAO_PROGRAMA_FIDELIDADE", t => t.COD_TIPO_PONTUACAO_PROGRAMA_FIDELIDADE)
                .Index(t => t.COD_TIPO_PONTUACAO_PROGRAMA_FIDELIDADE);
            
            CreateTable(
                "dbo.PONTUACAO_DINHEIRO_PROGRAMA_FIDELIDADE",
                c => new
                    {
                        COD_PROGRAMA_FIDELIDADE = c.Int(nullable: false),
                        PONTOS_GANHOS_POR_UNIDADE_MONETARIA_GASTA = c.Decimal(nullable: false, precision: 18, scale: 2),
                        VALOR_DINHEIRO_POR_PONTO_PARA_RESGATE = c.Decimal(nullable: false, precision: 18, scale: 2),
                        QUANTIDADE_MINIMA_PONTOS_PARA_RESGATE = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.COD_PROGRAMA_FIDELIDADE)
                .ForeignKey("dbo.PROGRAMA_FIDELIDADE", t => t.COD_PROGRAMA_FIDELIDADE)
                .Index(t => t.COD_PROGRAMA_FIDELIDADE);
            
            CreateTable(
                "dbo.SALDO_USUARIO_PROGRAMA_FIDELIDADE",
                c => new
                    {
                        LOGIN_USUARIO = c.String(nullable: false, maxLength: 128),
                        COD_PROGRAMA_FIDELIDADE = c.Int(nullable: false),
                        SALDO = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => new { t.LOGIN_USUARIO, t.COD_PROGRAMA_FIDELIDADE })
                .ForeignKey("dbo.PROGRAMA_FIDELIDADE", t => t.COD_PROGRAMA_FIDELIDADE)
                .Index(t => t.COD_PROGRAMA_FIDELIDADE);
            
            CreateTable(
                "dbo.TIPO_PONTUACAO_PROGRAMA_FIDELIDADE",
                c => new
                    {
                        COD_TIPO_PONTUACAO_PROGRAMA_FIDELIDADE = c.Int(nullable: false),
                        DESCRICAO_TIPO_PONTUACAO_PROGRAMA_FIDELIDADE = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.COD_TIPO_PONTUACAO_PROGRAMA_FIDELIDADE);
            
            CreateTable(
                "dbo.USUARIO_PARTICIPANTE_PROGRAMA_FIDELIDADE",
                c => new
                    {
                        LOGIN_USUARIO = c.String(nullable: false, maxLength: 128),
                        COD_PROGRAMA_FIDELIDADE = c.Int(nullable: false),
                        TERMOS_ACEITOS = c.Boolean(nullable: false),
                        DATA_HORA_ACEITE = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.LOGIN_USUARIO, t.COD_PROGRAMA_FIDELIDADE })
                .ForeignKey("dbo.PROGRAMA_FIDELIDADE", t => t.COD_PROGRAMA_FIDELIDADE)
                .Index(t => t.COD_PROGRAMA_FIDELIDADE);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EXTRATO_USUARIO_PROGRAMA_FIDELIDADE", "COD_PEDIDO", "dbo.PEDIDO");
            DropForeignKey("dbo.USUARIO_PARTICIPANTE_PROGRAMA_FIDELIDADE", "COD_PROGRAMA_FIDELIDADE", "dbo.PROGRAMA_FIDELIDADE");
            DropForeignKey("dbo.PROGRAMA_FIDELIDADE", "COD_TIPO_PONTUACAO_PROGRAMA_FIDELIDADE", "dbo.TIPO_PONTUACAO_PROGRAMA_FIDELIDADE");
            DropForeignKey("dbo.SALDO_USUARIO_PROGRAMA_FIDELIDADE", "COD_PROGRAMA_FIDELIDADE", "dbo.PROGRAMA_FIDELIDADE");
            DropForeignKey("dbo.PONTUACAO_DINHEIRO_PROGRAMA_FIDELIDADE", "COD_PROGRAMA_FIDELIDADE", "dbo.PROGRAMA_FIDELIDADE");
            DropForeignKey("dbo.EXTRATO_USUARIO_PROGRAMA_FIDELIDADE", "COD_PROGRAMA_FIDELIDADE", "dbo.PROGRAMA_FIDELIDADE");
            DropIndex("dbo.USUARIO_PARTICIPANTE_PROGRAMA_FIDELIDADE", new[] { "COD_PROGRAMA_FIDELIDADE" });
            DropIndex("dbo.SALDO_USUARIO_PROGRAMA_FIDELIDADE", new[] { "COD_PROGRAMA_FIDELIDADE" });
            DropIndex("dbo.PONTUACAO_DINHEIRO_PROGRAMA_FIDELIDADE", new[] { "COD_PROGRAMA_FIDELIDADE" });
            DropIndex("dbo.PROGRAMA_FIDELIDADE", new[] { "COD_TIPO_PONTUACAO_PROGRAMA_FIDELIDADE" });
            DropIndex("dbo.EXTRATO_USUARIO_PROGRAMA_FIDELIDADE", new[] { "COD_PEDIDO" });
            DropIndex("dbo.EXTRATO_USUARIO_PROGRAMA_FIDELIDADE", new[] { "COD_PROGRAMA_FIDELIDADE" });
            DropTable("dbo.USUARIO_PARTICIPANTE_PROGRAMA_FIDELIDADE");
            DropTable("dbo.TIPO_PONTUACAO_PROGRAMA_FIDELIDADE");
            DropTable("dbo.SALDO_USUARIO_PROGRAMA_FIDELIDADE");
            DropTable("dbo.PONTUACAO_DINHEIRO_PROGRAMA_FIDELIDADE");
            DropTable("dbo.PROGRAMA_FIDELIDADE");
            DropTable("dbo.EXTRATO_USUARIO_PROGRAMA_FIDELIDADE");
        }
    }
}
