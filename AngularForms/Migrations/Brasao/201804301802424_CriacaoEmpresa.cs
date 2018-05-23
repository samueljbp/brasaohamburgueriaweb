namespace BrasaoHamburgueria.Web.Migrations.Brasao
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CriacaoEmpresa : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.FUNCIONAMENTO_ESTABELECIMENTO");
            DropPrimaryKey("dbo.PARAMETRO_SISTEMA");
            CreateTable(
                "dbo.EMPRESA",
                c => new
                    {
                        COD_EMPRESA = c.Int(nullable: false),
                        COD_EMPRESA_MATRIZ = c.Int(),
                        RAZAO_SOCIAL = c.String(nullable: false),
                        NOME_FANTASIA = c.String(nullable: false),
                        CNPJ = c.String(nullable: false),
                        INSCRICAO_ESTADUAL = c.String(),
                        COD_BAIRRO = c.Int(nullable: false),
                        LOGRADOURO = c.String(nullable: false),
                        NUMERO = c.String(nullable: false),
                        COMPLEMENTO = c.String(),
                        TELEFONE = c.String(nullable: false),
                        LOGOMARCA = c.String(),
                    })
                .PrimaryKey(t => t.COD_EMPRESA)
                .ForeignKey("dbo.BAIRRO", t => t.COD_BAIRRO)
                .Index(t => t.COD_BAIRRO);

            CreateTable(
                "dbo.BAIRRO",
                c => new
                    {
                        COD_BAIRRO = c.Int(nullable: false),
                        NOME = c.String(nullable: false),
                        CIDADE = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.COD_BAIRRO)
                .ForeignKey("dbo.CIDADE", t => t.CIDADE)
                .Index(t => t.CIDADE);
            
            CreateTable(
                "dbo.CIDADE",
                c => new
                    {
                        COD_CIDADE = c.Int(nullable: false),
                        NOME = c.String(nullable: false),
                        ESTADO = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.COD_CIDADE);

            base.Sql(@"INSERT INTO CIDADE (COD_CIDADE, NOME, ESTADO) VALUES (1, 'Cataguases', 'MG')");

            base.Sql(@"INSERT INTO BAIRRO (COD_BAIRRO, NOME, CIDADE) VALUES (1, 'Centro', 1)");

            base.Sql(@"INSERT INTO EMPRESA (COD_EMPRESA, RAZAO_SOCIAL, NOME_FANTASIA, CNPJ, COD_BAIRRO, LOGRADOURO, NUMERO, TELEFONE, EMPRESA_ATIVA, CASA_ABERTA) VALUES 
                    (1, 'Nome da empresa', 'Nome fantasia', '00.000.000/0000-00', 1, 'Logradouro', 'Numero', 'Telefone', 1, 0)");

            AddColumn("dbo.IMPRESSORA_PRODUCAO", "COD_EMPRESA", c => c.Int());
            AddColumn("dbo.ITEM_CARDAPIO", "COD_EMPRESA", c => c.Int());
            AddColumn("dbo.COMBO", "COD_EMPRESA", c => c.Int());
            AddColumn("dbo.PEDIDO", "COD_EMPRESA", c => c.Int(nullable: false, identity: false, defaultValue: 1));
            AddColumn("dbo.ENTREGADOR", "COD_EMPRESA", c => c.Int());
            AddColumn("dbo.PROGRAMA_FIDELIDADE", "COD_EMPRESA", c => c.Int());
            AddColumn("dbo.PROMOCAO_VENDA", "COD_EMPRESA", c => c.Int());
            AddColumn("dbo.FUNCIONAMENTO_ESTABELECIMENTO", "COD_EMPRESA", c => c.Int(nullable: false, identity: false, defaultValue: 1));
            AddColumn("dbo.PARAMETRO_SISTEMA", "COD_EMPRESA", c => c.Int(nullable: false, identity: false, defaultValue: 1));
            AddPrimaryKey("dbo.FUNCIONAMENTO_ESTABELECIMENTO", new[] { "DIA_SEMANA", "ABERTURA", "COD_EMPRESA" });
            AddPrimaryKey("dbo.PARAMETRO_SISTEMA", new[] { "COD_PARAMETRO", "COD_EMPRESA" });
            CreateIndex("dbo.IMPRESSORA_PRODUCAO", "COD_EMPRESA");
            CreateIndex("dbo.ITEM_CARDAPIO", "COD_EMPRESA");
            CreateIndex("dbo.COMBO", "COD_EMPRESA");
            CreateIndex("dbo.PEDIDO", "COD_EMPRESA");
            CreateIndex("dbo.ENTREGADOR", "COD_EMPRESA");
            CreateIndex("dbo.PROGRAMA_FIDELIDADE", "COD_EMPRESA");
            CreateIndex("dbo.PROMOCAO_VENDA", "COD_EMPRESA");
            CreateIndex("dbo.FUNCIONAMENTO_ESTABELECIMENTO", "COD_EMPRESA");
            CreateIndex("dbo.PARAMETRO_SISTEMA", "COD_EMPRESA");
            AddForeignKey("dbo.IMPRESSORA_PRODUCAO", "COD_EMPRESA", "dbo.EMPRESA", "COD_EMPRESA");
            AddForeignKey("dbo.COMBO", "COD_EMPRESA", "dbo.EMPRESA", "COD_EMPRESA");
            AddForeignKey("dbo.ITEM_CARDAPIO", "COD_EMPRESA", "dbo.EMPRESA", "COD_EMPRESA");
            AddForeignKey("dbo.PEDIDO", "COD_EMPRESA", "dbo.EMPRESA", "COD_EMPRESA");
            AddForeignKey("dbo.ENTREGADOR", "COD_EMPRESA", "dbo.EMPRESA", "COD_EMPRESA");
            AddForeignKey("dbo.PROGRAMA_FIDELIDADE", "COD_EMPRESA", "dbo.EMPRESA", "COD_EMPRESA");
            AddForeignKey("dbo.PROMOCAO_VENDA", "COD_EMPRESA", "dbo.EMPRESA", "COD_EMPRESA");
            AddForeignKey("dbo.FUNCIONAMENTO_ESTABELECIMENTO", "COD_EMPRESA", "dbo.EMPRESA", "COD_EMPRESA");
            AddForeignKey("dbo.PARAMETRO_SISTEMA", "COD_EMPRESA", "dbo.EMPRESA", "COD_EMPRESA");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PARAMETRO_SISTEMA", "COD_EMPRESA", "dbo.EMPRESA");
            DropForeignKey("dbo.FUNCIONAMENTO_ESTABELECIMENTO", "COD_EMPRESA", "dbo.EMPRESA");
            DropForeignKey("dbo.PROMOCAO_VENDA", "COD_EMPRESA", "dbo.EMPRESA");
            DropForeignKey("dbo.PROGRAMA_FIDELIDADE", "COD_EMPRESA", "dbo.EMPRESA");
            DropForeignKey("dbo.ENTREGADOR", "COD_EMPRESA", "dbo.EMPRESA");
            DropForeignKey("dbo.PEDIDO", "COD_EMPRESA", "dbo.EMPRESA");
            DropForeignKey("dbo.ITEM_CARDAPIO", "COD_EMPRESA", "dbo.EMPRESA");
            DropForeignKey("dbo.COMBO", "COD_EMPRESA", "dbo.EMPRESA");
            DropForeignKey("dbo.IMPRESSORA_PRODUCAO", "COD_EMPRESA", "dbo.EMPRESA");
            DropForeignKey("dbo.EMPRESA", "COD_BAIRRO", "dbo.BAIRRO");
            DropForeignKey("dbo.BAIRRO", "CIDADE", "dbo.CIDADE");
            DropIndex("dbo.PARAMETRO_SISTEMA", new[] { "COD_EMPRESA" });
            DropIndex("dbo.FUNCIONAMENTO_ESTABELECIMENTO", new[] { "COD_EMPRESA" });
            DropIndex("dbo.PROMOCAO_VENDA", new[] { "COD_EMPRESA" });
            DropIndex("dbo.PROGRAMA_FIDELIDADE", new[] { "COD_EMPRESA" });
            DropIndex("dbo.ENTREGADOR", new[] { "COD_EMPRESA" });
            DropIndex("dbo.PEDIDO", new[] { "COD_EMPRESA" });
            DropIndex("dbo.COMBO", new[] { "COD_EMPRESA" });
            DropIndex("dbo.ITEM_CARDAPIO", new[] { "COD_EMPRESA" });
            DropIndex("dbo.BAIRRO", new[] { "CIDADE" });
            DropIndex("dbo.EMPRESA", new[] { "COD_BAIRRO" });
            DropIndex("dbo.IMPRESSORA_PRODUCAO", new[] { "COD_EMPRESA" });
            DropPrimaryKey("dbo.PARAMETRO_SISTEMA");
            DropPrimaryKey("dbo.FUNCIONAMENTO_ESTABELECIMENTO");
            DropColumn("dbo.PARAMETRO_SISTEMA", "COD_EMPRESA");
            DropColumn("dbo.FUNCIONAMENTO_ESTABELECIMENTO", "COD_EMPRESA");
            DropColumn("dbo.PROMOCAO_VENDA", "COD_EMPRESA");
            DropColumn("dbo.PROGRAMA_FIDELIDADE", "COD_EMPRESA");
            DropColumn("dbo.ENTREGADOR", "COD_EMPRESA");
            DropColumn("dbo.PEDIDO", "COD_EMPRESA");
            DropColumn("dbo.COMBO", "COD_EMPRESA");
            DropColumn("dbo.ITEM_CARDAPIO", "COD_EMPRESA");
            DropColumn("dbo.IMPRESSORA_PRODUCAO", "COD_EMPRESA");
            DropTable("dbo.CIDADE");
            DropTable("dbo.BAIRRO");
            DropTable("dbo.EMPRESA");
            AddPrimaryKey("dbo.PARAMETRO_SISTEMA", "COD_PARAMETRO");
            AddPrimaryKey("dbo.FUNCIONAMENTO_ESTABELECIMENTO", new[] { "DIA_SEMANA", "ABERTURA" });
        }
    }
}
