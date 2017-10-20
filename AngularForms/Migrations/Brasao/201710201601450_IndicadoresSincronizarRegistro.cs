namespace BrasaoHamburgueria.Web.Migrations.Brasao
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IndicadoresSincronizarRegistro : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CLASSE_ITEM_CARDAPIO", "SINCRONIZAR", c => c.Boolean(nullable: false));
            AddColumn("dbo.ITEM_CARDAPIO", "SINCRONIZAR", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ITEM_CARDAPIO", "SINCRONIZAR");
            DropColumn("dbo.CLASSE_ITEM_CARDAPIO", "SINCRONIZAR");
        }
    }
}
