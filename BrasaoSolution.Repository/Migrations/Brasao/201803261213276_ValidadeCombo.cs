namespace BrasaoSolution.Repository.Migrations.Brasao
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ValidadeCombo : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DIA_SEMANA_COMBO",
                c => new
                    {
                        COD_COMBO = c.Int(nullable: false),
                        DIA_SEMANA = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.COD_COMBO, t.DIA_SEMANA })
                .ForeignKey("dbo.COMBO", t => t.COD_COMBO)
                .Index(t => t.COD_COMBO);
            
            AddColumn("dbo.COMBO", "DATA_INICIO", c => c.DateTime(nullable: false));
            AddColumn("dbo.COMBO", "DATA_FIM", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DIA_SEMANA_COMBO", "COD_COMBO", "dbo.COMBO");
            DropIndex("dbo.DIA_SEMANA_COMBO", new[] { "COD_COMBO" });
            DropColumn("dbo.COMBO", "DATA_FIM");
            DropColumn("dbo.COMBO", "DATA_INICIO");
            DropTable("dbo.DIA_SEMANA_COMBO");
        }
    }
}
