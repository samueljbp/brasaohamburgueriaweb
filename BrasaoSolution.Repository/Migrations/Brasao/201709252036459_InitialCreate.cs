namespace BrasaoSolution.Repository.Migrations.Brasao
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CLASSE_ITEM_CARDAPIO",
                c => new
                    {
                        COD_CLASSE = c.Int(nullable: false),
                        DESCRICAO_CLASSE = c.String(nullable: false),
                        IMAGEM = c.String(),
                        ORDEM_EXIBICAO = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.COD_CLASSE);
            
            CreateTable(
                "dbo.ITEM_CARDAPIO",
                c => new
                    {
                        COD_ITEM_CARDAPIO = c.Int(nullable: false),
                        COD_CLASSE = c.Int(nullable: false),
                        NOME = c.String(nullable: false),
                        PRECO = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.COD_ITEM_CARDAPIO)
                .ForeignKey("dbo.CLASSE_ITEM_CARDAPIO", t => t.COD_CLASSE, cascadeDelete: true)
                .Index(t => t.COD_CLASSE);
            
            CreateTable(
                "dbo.COMPLEMENTO_ITEM_CARDAPIO",
                c => new
                    {
                        COD_ITEM_CARDAPIO = c.Int(nullable: false),
                        DESCRICAO = c.String(nullable: false),
                        IMAGEM = c.String(),
                        ORDEM_EXIBICAO = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.COD_ITEM_CARDAPIO)
                .ForeignKey("dbo.ITEM_CARDAPIO", t => t.COD_ITEM_CARDAPIO)
                .Index(t => t.COD_ITEM_CARDAPIO);
            
            CreateTable(
                "dbo.OPCAO_EXTRA_ITEM_CARDAPIO",
                c => new
                    {
                        COD_ITEM_CARDAPIO = c.Int(nullable: false),
                        COD_OPCAO_EXTRA = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.COD_ITEM_CARDAPIO, t.COD_OPCAO_EXTRA })
                .ForeignKey("dbo.OPCAO_EXTRA", t => t.COD_OPCAO_EXTRA, cascadeDelete: true)
                .ForeignKey("dbo.ITEM_CARDAPIO", t => t.COD_ITEM_CARDAPIO, cascadeDelete: true)
                .Index(t => t.COD_ITEM_CARDAPIO)
                .Index(t => t.COD_OPCAO_EXTRA);
            
            CreateTable(
                "dbo.OPCAO_EXTRA",
                c => new
                    {
                        COD_OPCAO_EXTRA = c.Int(nullable: false),
                        DESCRICAO_OPCAO_EXTRA = c.String(nullable: false),
                        PRECO = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.COD_OPCAO_EXTRA);
            
            CreateTable(
                "dbo.OPCAO_EXTRA_ITEM_PEDIDO",
                c => new
                    {
                        COD_PEDIDO = c.Int(nullable: false),
                        SEQ_ITEM = c.Int(nullable: false),
                        COD_OPCAO_EXTRA = c.Int(nullable: false),
                        PRECO = c.Double(nullable: false),
                    })
                .PrimaryKey(t => new { t.COD_PEDIDO, t.SEQ_ITEM, t.COD_OPCAO_EXTRA })
                .ForeignKey("dbo.ITEM_PEDIDO", t => new { t.COD_PEDIDO, t.SEQ_ITEM }, cascadeDelete: true)
                .ForeignKey("dbo.OPCAO_EXTRA", t => t.COD_OPCAO_EXTRA, cascadeDelete: true)
                .Index(t => new { t.COD_PEDIDO, t.SEQ_ITEM })
                .Index(t => t.COD_OPCAO_EXTRA);
            
            CreateTable(
                "dbo.ITEM_PEDIDO",
                c => new
                    {
                        COD_PEDIDO = c.Int(nullable: false),
                        SEQ_ITEM = c.Int(nullable: false),
                        COD_ITEM_CARDAPIO = c.Int(nullable: false),
                        OBSERVACAO_LIVRE = c.String(),
                        QUANTIDADE = c.Int(nullable: false),
                        PRECO_UNITARIO = c.Double(nullable: false),
                        VALOR_EXTRAS = c.Double(nullable: false),
                        VALOR_TOTAL = c.Double(nullable: false),
                        CANCELADO = c.Boolean(nullable: false),
                        MOTIVO_CANCELAMENTO = c.String(),
                    })
                .PrimaryKey(t => new { t.COD_PEDIDO, t.SEQ_ITEM })
                .ForeignKey("dbo.PEDIDO", t => t.COD_PEDIDO, cascadeDelete: true)
                .ForeignKey("dbo.ITEM_CARDAPIO", t => t.COD_ITEM_CARDAPIO, cascadeDelete: true)
                .Index(t => t.COD_PEDIDO)
                .Index(t => t.COD_ITEM_CARDAPIO);
            
            CreateTable(
                "dbo.OBSERVACAO_PRODUCAO_ITEM_PEDIDO",
                c => new
                    {
                        COD_PEDIDO = c.Int(nullable: false),
                        SEQ_ITEM = c.Int(nullable: false),
                        COD_OBSERVACAO = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.COD_PEDIDO, t.SEQ_ITEM, t.COD_OBSERVACAO })
                .ForeignKey("dbo.OBSERVACAO_PRODUCAO", t => t.COD_OBSERVACAO, cascadeDelete: true)
                .ForeignKey("dbo.ITEM_PEDIDO", t => new { t.COD_PEDIDO, t.SEQ_ITEM }, cascadeDelete: true)
                .Index(t => new { t.COD_PEDIDO, t.SEQ_ITEM })
                .Index(t => t.COD_OBSERVACAO);
            
            CreateTable(
                "dbo.OBSERVACAO_PRODUCAO",
                c => new
                    {
                        COD_OBSERVACAO = c.Int(nullable: false),
                        DESCRICAO_OBSERVACAO = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.COD_OBSERVACAO);
            
            CreateTable(
                "dbo.OBSERVACAO_PRODUCAO_PERMITIDA_ITEM_CARDAPIO",
                c => new
                    {
                        COD_ITEM_CARDAPIO = c.Int(nullable: false),
                        COD_OBSERVACAO = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.COD_ITEM_CARDAPIO, t.COD_OBSERVACAO })
                .ForeignKey("dbo.OBSERVACAO_PRODUCAO", t => t.COD_OBSERVACAO, cascadeDelete: true)
                .ForeignKey("dbo.ITEM_CARDAPIO", t => t.COD_ITEM_CARDAPIO, cascadeDelete: true)
                .Index(t => t.COD_ITEM_CARDAPIO)
                .Index(t => t.COD_OBSERVACAO);
            
            CreateTable(
                "dbo.PEDIDO",
                c => new
                    {
                        COD_PEDIDO = c.Int(nullable: false, identity: true),
                        COD_SITUACAO = c.Int(nullable: false),
                        USUARIO = c.String(nullable: false),
                        PEDIDO_EXTERNO = c.Boolean(nullable: false),
                        DATA_HORA = c.DateTime(nullable: false),
                        TAXA_ENTREGA = c.Double(nullable: false),
                        FORMA_PAGAMENTO = c.String(nullable: false),
                        TROCO_PARA = c.Double(),
                        TROCO = c.Double(),
                        BANDEIRA_CARTAO = c.String(),
                        VALOR_TOTAL = c.Double(nullable: false),
                        NOME_CLIENTE = c.String(nullable: false),
                        TELEFONE_CLIENTE = c.String(nullable: false),
                        UF_ENTREGA = c.String(nullable: false),
                        CIDADE_ENTREGA = c.String(nullable: false),
                        LOGRADOURO_ENTREGA = c.String(nullable: false),
                        NUMERO_ENTREGA = c.String(nullable: false),
                        COMPLEMENTO_ENTREGA = c.String(nullable: false),
                        BAIRRO_ENTREGA = c.String(nullable: false),
                        REFERENCIA_ENTREGA = c.String(),
                        MOTIVO_CANCELAMENTO = c.String(),
                        FEEDBACK_CLIENTE = c.String(),
                    })
                .PrimaryKey(t => t.COD_PEDIDO)
                .ForeignKey("dbo.SITUACAO_PEDIDO", t => t.COD_SITUACAO, cascadeDelete: true)
                .Index(t => t.COD_SITUACAO);
            
            CreateTable(
                "dbo.SITUACAO_PEDIDO",
                c => new
                    {
                        COD_SITUACAO = c.Int(nullable: false),
                        DESCRICAO = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.COD_SITUACAO);
            
            CreateTable(
                "dbo.ITEM_CARDAPIO_IMPRESSORA_PRODUCAO",
                c => new
                    {
                        COD_ITEM_CARDAPIO = c.Int(nullable: false),
                        COD_IMPRESSORA = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.COD_ITEM_CARDAPIO, t.COD_IMPRESSORA })
                .ForeignKey("dbo.IMPRESSORA_PRODUCAO", t => t.COD_IMPRESSORA, cascadeDelete: true)
                .ForeignKey("dbo.ITEM_CARDAPIO", t => t.COD_ITEM_CARDAPIO, cascadeDelete: true)
                .Index(t => t.COD_ITEM_CARDAPIO)
                .Index(t => t.COD_IMPRESSORA);
            
            CreateTable(
                "dbo.IMPRESSORA_PRODUCAO",
                c => new
                    {
                        COD_IMPRESSORA = c.Int(nullable: false, identity: true),
                        DESCRICAO = c.String(nullable: false),
                        PORTA = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.COD_IMPRESSORA);
            
            CreateTable(
                "dbo.FUNCIONAMENTO_ESTABELECIMENTO",
                c => new
                    {
                        DIA_SEMANA = c.Int(nullable: false),
                        ABERTURA = c.String(nullable: false, maxLength: 128),
                        FECHAMENTO = c.String(nullable: false),
                        TEM_DELIVERY = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.DIA_SEMANA, t.ABERTURA });
            
            CreateTable(
                "dbo.PARAMETRO_SISTEMA",
                c => new
                    {
                        COD_PARAMETRO = c.Int(nullable: false),
                        DESCRICAO_PARAMETRO = c.String(nullable: false),
                        VALOR_PARAMETRO = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.COD_PARAMETRO);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ITEM_CARDAPIO", "COD_CLASSE", "dbo.CLASSE_ITEM_CARDAPIO");
            DropForeignKey("dbo.OBSERVACAO_PRODUCAO_PERMITIDA_ITEM_CARDAPIO", "COD_ITEM_CARDAPIO", "dbo.ITEM_CARDAPIO");
            DropForeignKey("dbo.ITEM_PEDIDO", "COD_ITEM_CARDAPIO", "dbo.ITEM_CARDAPIO");
            DropForeignKey("dbo.ITEM_CARDAPIO_IMPRESSORA_PRODUCAO", "COD_ITEM_CARDAPIO", "dbo.ITEM_CARDAPIO");
            DropForeignKey("dbo.ITEM_CARDAPIO_IMPRESSORA_PRODUCAO", "COD_IMPRESSORA", "dbo.IMPRESSORA_PRODUCAO");
            DropForeignKey("dbo.OPCAO_EXTRA_ITEM_CARDAPIO", "COD_ITEM_CARDAPIO", "dbo.ITEM_CARDAPIO");
            DropForeignKey("dbo.OPCAO_EXTRA_ITEM_CARDAPIO", "COD_OPCAO_EXTRA", "dbo.OPCAO_EXTRA");
            DropForeignKey("dbo.OPCAO_EXTRA_ITEM_PEDIDO", "COD_OPCAO_EXTRA", "dbo.OPCAO_EXTRA");
            DropForeignKey("dbo.PEDIDO", "COD_SITUACAO", "dbo.SITUACAO_PEDIDO");
            DropForeignKey("dbo.ITEM_PEDIDO", "COD_PEDIDO", "dbo.PEDIDO");
            DropForeignKey("dbo.OBSERVACAO_PRODUCAO_ITEM_PEDIDO", new[] { "COD_PEDIDO", "SEQ_ITEM" }, "dbo.ITEM_PEDIDO");
            DropForeignKey("dbo.OBSERVACAO_PRODUCAO_ITEM_PEDIDO", "COD_OBSERVACAO", "dbo.OBSERVACAO_PRODUCAO");
            DropForeignKey("dbo.OBSERVACAO_PRODUCAO_PERMITIDA_ITEM_CARDAPIO", "COD_OBSERVACAO", "dbo.OBSERVACAO_PRODUCAO");
            DropForeignKey("dbo.OPCAO_EXTRA_ITEM_PEDIDO", new[] { "COD_PEDIDO", "SEQ_ITEM" }, "dbo.ITEM_PEDIDO");
            DropForeignKey("dbo.COMPLEMENTO_ITEM_CARDAPIO", "COD_ITEM_CARDAPIO", "dbo.ITEM_CARDAPIO");
            DropIndex("dbo.ITEM_CARDAPIO_IMPRESSORA_PRODUCAO", new[] { "COD_IMPRESSORA" });
            DropIndex("dbo.ITEM_CARDAPIO_IMPRESSORA_PRODUCAO", new[] { "COD_ITEM_CARDAPIO" });
            DropIndex("dbo.PEDIDO", new[] { "COD_SITUACAO" });
            DropIndex("dbo.OBSERVACAO_PRODUCAO_PERMITIDA_ITEM_CARDAPIO", new[] { "COD_OBSERVACAO" });
            DropIndex("dbo.OBSERVACAO_PRODUCAO_PERMITIDA_ITEM_CARDAPIO", new[] { "COD_ITEM_CARDAPIO" });
            DropIndex("dbo.OBSERVACAO_PRODUCAO_ITEM_PEDIDO", new[] { "COD_OBSERVACAO" });
            DropIndex("dbo.OBSERVACAO_PRODUCAO_ITEM_PEDIDO", new[] { "COD_PEDIDO", "SEQ_ITEM" });
            DropIndex("dbo.ITEM_PEDIDO", new[] { "COD_ITEM_CARDAPIO" });
            DropIndex("dbo.ITEM_PEDIDO", new[] { "COD_PEDIDO" });
            DropIndex("dbo.OPCAO_EXTRA_ITEM_PEDIDO", new[] { "COD_OPCAO_EXTRA" });
            DropIndex("dbo.OPCAO_EXTRA_ITEM_PEDIDO", new[] { "COD_PEDIDO", "SEQ_ITEM" });
            DropIndex("dbo.OPCAO_EXTRA_ITEM_CARDAPIO", new[] { "COD_OPCAO_EXTRA" });
            DropIndex("dbo.OPCAO_EXTRA_ITEM_CARDAPIO", new[] { "COD_ITEM_CARDAPIO" });
            DropIndex("dbo.COMPLEMENTO_ITEM_CARDAPIO", new[] { "COD_ITEM_CARDAPIO" });
            DropIndex("dbo.ITEM_CARDAPIO", new[] { "COD_CLASSE" });
            DropTable("dbo.PARAMETRO_SISTEMA");
            DropTable("dbo.FUNCIONAMENTO_ESTABELECIMENTO");
            DropTable("dbo.IMPRESSORA_PRODUCAO");
            DropTable("dbo.ITEM_CARDAPIO_IMPRESSORA_PRODUCAO");
            DropTable("dbo.SITUACAO_PEDIDO");
            DropTable("dbo.PEDIDO");
            DropTable("dbo.OBSERVACAO_PRODUCAO_PERMITIDA_ITEM_CARDAPIO");
            DropTable("dbo.OBSERVACAO_PRODUCAO");
            DropTable("dbo.OBSERVACAO_PRODUCAO_ITEM_PEDIDO");
            DropTable("dbo.ITEM_PEDIDO");
            DropTable("dbo.OPCAO_EXTRA_ITEM_PEDIDO");
            DropTable("dbo.OPCAO_EXTRA");
            DropTable("dbo.OPCAO_EXTRA_ITEM_CARDAPIO");
            DropTable("dbo.COMPLEMENTO_ITEM_CARDAPIO");
            DropTable("dbo.ITEM_CARDAPIO");
            DropTable("dbo.CLASSE_ITEM_CARDAPIO");
        }
    }
}
