namespace BrasaoHamburgueria.Web.Migrations.Brasao
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemCardapioAtivo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ITEM_CARDAPIO", "ATIVO", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ITEM_CARDAPIO", "ATIVO");
        }
    }
}
