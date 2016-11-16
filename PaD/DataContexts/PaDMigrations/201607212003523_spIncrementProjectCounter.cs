namespace PaD.DataContexts.PaDMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class spIncrementProjectCounter : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(
                "spIncrementProjectCounter",
                p => new
                {
                    projectId = p.Int(),
                    year = p.Int(),
                    month = p.Int()
                },
                @"
                SET NOCOUNT ON;
                
                DECLARE @ProjectTrackerId int;
                
                -- Get existing ProjectTrackerId if it exists
                SELECT TOP(1) @ProjectTrackerId=ProjectTrackerId
                FROM ProjectTrackers
                WHERE ProjectId = @projectId AND Year = @year AND Month = @month;
                
                -- If no ProjectTrackerId, insert a new one, otherwise update the existing one
                IF @ProjectTrackerId IS NULL
                    INSERT INTO ProjectTrackers(ProjectId, Year, Month, [Counter]) 
                    VALUES (@projectId, @year, @month, 1);
                ELSE
                    UPDATE ProjectTrackers SET [Counter] = [Counter] + 1 
                    WHERE ProjectId = @projectId AND Year = @year AND Month = @month;

                -- Return the new counter
                SELECT [Counter] FROM ProjectTrackers WHERE ProjectId=@projectId AND Year = @year AND Month = @month;"
            );
        }
        
        public override void Down()
        {
            DropStoredProcedure("spIncrementProjectCounter");
        }
    }
}
