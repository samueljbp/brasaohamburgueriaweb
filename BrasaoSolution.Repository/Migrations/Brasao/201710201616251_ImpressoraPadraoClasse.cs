namespace BrasaoSolution.Repository.Migrations.Brasao
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ImpressoraPadraoClasse : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ITEM_CARDAPIO", "COD_CLASSE", "dbo.CLASSE_ITEM_CARDAPIO");
            DropForeignKey("dbo.OPCAO_EXTRA_ITEM_CARDAPIO", "COD_ITEM_CARDAPIO", "dbo.ITEM_CARDAPIO");
            DropForeignKey("dbo.ITEM_CARDAPIO_IMPRESSORA_PRODUCAO", "COD_ITEM_CARDAPIO", "dbo.ITEM_CARDAPIO");
            DropForeignKey("dbo.ITEM_PEDIDO", "COD_ITEM_CARDAPIO", "dbo.ITEM_CARDAPIO");
            DropForeignKey("dbo.OBSERVACAO_PRODUCAO_PERMITIDA_ITEM_CARDAPIO", "COD_ITEM_CARDAPIO", "dbo.ITEM_CARDAPIO");
            DropForeignKey("dbo.OPCAO_EXTRA_ITEM_CARDAPIO", "COD_OPCAO_EXTRA", "dbo.OPCAO_EXTRA");
            DropForeignKey("dbo.OPCAO_EXTRA_ITEM_PEDIDO", "COD_OPCAO_EXTRA", "dbo.OPCAO_EXTRA");
            DropForeignKey("dbo.OPCAO_EXTRA_ITEM_PEDIDO", new[] { "COD_PEDIDO", "SEQ_ITEM" }, "dbo.ITEM_PEDIDO");
            DropForeignKey("dbo.OBSERVACAO_PRODUCAO_ITEM_PEDIDO", new[] { "COD_PEDIDO", "SEQ_ITEM" }, "dbo.ITEM_PEDIDO");
            DropForeignKey("dbo.ITEM_PEDIDO", "COD_PEDIDO", "dbo.PEDIDO");
            DropForeignKey("dbo.OBSERVACAO_PRODUCAO_ITEM_PEDIDO", "COD_OBSERVACAO", "dbo.OBSERVACAO_PRODUCAO");
            DropForeignKey("dbo.OBSERVACAO_PRODUCAO_PERMITIDA_ITEM_CARDAPIO", "COD_OBSERVACAO", "dbo.OBSERVACAO_PRODUCAO");
            DropForeignKey("dbo.PEDIDO", "COD_SITUACAO", "dbo.SITUACAO_PEDIDO");
            DropForeignKey("dbo.ITEM_CARDAPIO_IMPRESSORA_PRODUCAO", "COD_IMPRESSORA", "dbo.IMPRESSORA_PRODUCAO");
            AddColumn("dbo.CLASSE_ITEM_CARDAPIO", "COD_IMPRESSORA_PADRAO", c => c.Int());
            CreateIndex("dbo.CLASSE_ITEM_CARDAPIO", "COD_IMPRESSORA_PADRAO");
            AddForeignKey("dbo.CLASSE_ITEM_CARDAPIO", "COD_IMPRESSORA_PADRAO", "dbo.IMPRESSORA_PRODUCAO", "COD_IMPRESSORA");
            AddForeignKey("dbo.ITEM_CARDAPIO", "COD_CLASSE", "dbo.CLASSE_ITEM_CARDAPIO", "COD_CLASSE");
            AddForeignKey("dbo.OPCAO_EXTRA_ITEM_CARDAPIO", "COD_ITEM_CARDAPIO", "dbo.ITEM_CARDAPIO", "COD_ITEM_CARDAPIO");
            AddForeignKey("dbo.ITEM_CARDAPIO_IMPRESSORA_PRODUCAO", "COD_ITEM_CARDAPIO", "dbo.ITEM_CARDAPIO", "COD_ITEM_CARDAPIO");
            AddForeignKey("dbo.ITEM_PEDIDO", "COD_ITEM_CARDAPIO", "dbo.ITEM_CARDAPIO", "COD_ITEM_CARDAPIO");
            AddForeignKey("dbo.OBSERVACAO_PRODUCAO_PERMITIDA_ITEM_CARDAPIO", "COD_ITEM_CARDAPIO", "dbo.ITEM_CARDAPIO", "COD_ITEM_CARDAPIO");
            AddForeignKey("dbo.OPCAO_EXTRA_ITEM_CARDAPIO", "COD_OPCAO_EXTRA", "dbo.OPCAO_EXTRA", "COD_OPCAO_EXTRA");
            AddForeignKey("dbo.OPCAO_EXTRA_ITEM_PEDIDO", "COD_OPCAO_EXTRA", "dbo.OPCAO_EXTRA", "COD_OPCAO_EXTRA");
            AddForeignKey("dbo.OPCAO_EXTRA_ITEM_PEDIDO", new[] { "COD_PEDIDO", "SEQ_ITEM" }, "dbo.ITEM_PEDIDO", new[] { "COD_PEDIDO", "SEQ_ITEM" });
            AddForeignKey("dbo.OBSERVACAO_PRODUCAO_ITEM_PEDIDO", new[] { "COD_PEDIDO", "SEQ_ITEM" }, "dbo.ITEM_PEDIDO", new[] { "COD_PEDIDO", "SEQ_ITEM" });
            AddForeignKey("dbo.ITEM_PEDIDO", "COD_PEDIDO", "dbo.PEDIDO", "COD_PEDIDO");
            AddForeignKey("dbo.OBSERVACAO_PRODUCAO_ITEM_PEDIDO", "COD_OBSERVACAO", "dbo.OBSERVACAO_PRODUCAO", "COD_OBSERVACAO");
            AddForeignKey("dbo.OBSERVACAO_PRODUCAO_PERMITIDA_ITEM_CARDAPIO", "COD_OBSERVACAO", "dbo.OBSERVACAO_PRODUCAO", "COD_OBSERVACAO");
            AddForeignKey("dbo.PEDIDO", "COD_SITUACAO", "dbo.SITUACAO_PEDIDO", "COD_SITUACAO");
            AddForeignKey("dbo.ITEM_CARDAPIO_IMPRESSORA_PRODUCAO", "COD_IMPRESSORA", "dbo.IMPRESSORA_PRODUCAO", "COD_IMPRESSORA");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ITEM_CARDAPIO_IMPRESSORA_PRODUCAO", "COD_IMPRESSORA", "dbo.IMPRESSORA_PRODUCAO");
            DropForeignKey("dbo.PEDIDO", "COD_SITUACAO", "dbo.SITUACAO_PEDIDO");
            DropForeignKey("dbo.OBSERVACAO_PRODUCAO_PERMITIDA_ITEM_CARDAPIO", "COD_OBSERVACAO", "dbo.OBSERVACAO_PRODUCAO");
            DropForeignKey("dbo.OBSERVACAO_PRODUCAO_ITEM_PEDIDO", "COD_OBSERVACAO", "dbo.OBSERVACAO_PRODUCAO");
            DropForeignKey("dbo.ITEM_PEDIDO", "COD_PEDIDO", "dbo.PEDIDO");
            DropForeignKey("dbo.OBSERVACAO_PRODUCAO_ITEM_PEDIDO", new[] { "COD_PEDIDO", "SEQ_ITEM" }, "dbo.ITEM_PEDIDO");
            DropForeignKey("dbo.OPCAO_EXTRA_ITEM_PEDIDO", new[] { "COD_PEDIDO", "SEQ_ITEM" }, "dbo.ITEM_PEDIDO");
            DropForeignKey("dbo.OPCAO_EXTRA_ITEM_PEDIDO", "COD_OPCAO_EXTRA", "dbo.OPCAO_EXTRA");
            DropForeignKey("dbo.OPCAO_EXTRA_ITEM_CARDAPIO", "COD_OPCAO_EXTRA", "dbo.OPCAO_EXTRA");
            DropForeignKey("dbo.OBSERVACAO_PRODUCAO_PERMITIDA_ITEM_CARDAPIO", "COD_ITEM_CARDAPIO", "dbo.ITEM_CARDAPIO");
            DropForeignKey("dbo.ITEM_PEDIDO", "COD_ITEM_CARDAPIO", "dbo.ITEM_CARDAPIO");
            DropForeignKey("dbo.ITEM_CARDAPIO_IMPRESSORA_PRODUCAO", "COD_ITEM_CARDAPIO", "dbo.ITEM_CARDAPIO");
            DropForeignKey("dbo.OPCAO_EXTRA_ITEM_CARDAPIO", "COD_ITEM_CARDAPIO", "dbo.ITEM_CARDAPIO");
            DropForeignKey("dbo.ITEM_CARDAPIO", "COD_CLASSE", "dbo.CLASSE_ITEM_CARDAPIO");
            DropForeignKey("dbo.CLASSE_ITEM_CARDAPIO", "COD_IMPRESSORA_PADRAO", "dbo.IMPRESSORA_PRODUCAO");
            DropIndex("dbo.CLASSE_ITEM_CARDAPIO", new[] { "COD_IMPRESSORA_PADRAO" });
            DropColumn("dbo.CLASSE_ITEM_CARDAPIO", "COD_IMPRESSORA_PADRAO");
            AddForeignKey("dbo.ITEM_CARDAPIO_IMPRESSORA_PRODUCAO", "COD_IMPRESSORA", "dbo.IMPRESSORA_PRODUCAO", "COD_IMPRESSORA", cascadeDelete: true);
            AddForeignKey("dbo.PEDIDO", "COD_SITUACAO", "dbo.SITUACAO_PEDIDO", "COD_SITUACAO", cascadeDelete: true);
            AddForeignKey("dbo.OBSERVACAO_PRODUCAO_PERMITIDA_ITEM_CARDAPIO", "COD_OBSERVACAO", "dbo.OBSERVACAO_PRODUCAO", "COD_OBSERVACAO", cascadeDelete: true);
            AddForeignKey("dbo.OBSERVACAO_PRODUCAO_ITEM_PEDIDO", "COD_OBSERVACAO", "dbo.OBSERVACAO_PRODUCAO", "COD_OBSERVACAO", cascadeDelete: true);
            AddForeignKey("dbo.ITEM_PEDIDO", "COD_PEDIDO", "dbo.PEDIDO", "COD_PEDIDO", cascadeDelete: true);
            AddForeignKey("dbo.OBSERVACAO_PRODUCAO_ITEM_PEDIDO", new[] { "COD_PEDIDO", "SEQ_ITEM" }, "dbo.ITEM_PEDIDO", new[] { "COD_PEDIDO", "SEQ_ITEM" }, cascadeDelete: true);
            AddForeignKey("dbo.OPCAO_EXTRA_ITEM_PEDIDO", new[] { "COD_PEDIDO", "SEQ_ITEM" }, "dbo.ITEM_PEDIDO", new[] { "COD_PEDIDO", "SEQ_ITEM" }, cascadeDelete: true);
            AddForeignKey("dbo.OPCAO_EXTRA_ITEM_PEDIDO", "COD_OPCAO_EXTRA", "dbo.OPCAO_EXTRA", "COD_OPCAO_EXTRA", cascadeDelete: true);
            AddForeignKey("dbo.OPCAO_EXTRA_ITEM_CARDAPIO", "COD_OPCAO_EXTRA", "dbo.OPCAO_EXTRA", "COD_OPCAO_EXTRA", cascadeDelete: true);
            AddForeignKey("dbo.OBSERVACAO_PRODUCAO_PERMITIDA_ITEM_CARDAPIO", "COD_ITEM_CARDAPIO", "dbo.ITEM_CARDAPIO", "COD_ITEM_CARDAPIO", cascadeDelete: true);
            AddForeignKey("dbo.ITEM_PEDIDO", "COD_ITEM_CARDAPIO", "dbo.ITEM_CARDAPIO", "COD_ITEM_CARDAPIO", cascadeDelete: true);
            AddForeignKey("dbo.ITEM_CARDAPIO_IMPRESSORA_PRODUCAO", "COD_ITEM_CARDAPIO", "dbo.ITEM_CARDAPIO", "COD_ITEM_CARDAPIO", cascadeDelete: true);
            AddForeignKey("dbo.OPCAO_EXTRA_ITEM_CARDAPIO", "COD_ITEM_CARDAPIO", "dbo.ITEM_CARDAPIO", "COD_ITEM_CARDAPIO", cascadeDelete: true);
            AddForeignKey("dbo.ITEM_CARDAPIO", "COD_CLASSE", "dbo.CLASSE_ITEM_CARDAPIO", "COD_CLASSE", cascadeDelete: true);
        }
    }
}
