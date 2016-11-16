namespace PaD.DataContexts.PaDMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class spIncrementPhotoCounter : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(
                "spIncrementPhotoCounter",
                p => new
                {
                    photoId = p.Int()
                },
                @"
                SET NOCOUNT ON;
                
                DECLARE @PhotoTrackerId int;

                -- Get existing PhotoTrackerId if it exists
                SELECT TOP(1) @PhotoTrackerId=PhotoTrackerId
                FROM PhotoTrackers WHERE PhotoId = @photoId;

                -- If no PhotoTrackerId, insert a new one, otherwise update the existing one
                IF @PhotoTrackerId IS NULL
                    INSERT INTO PhotoTrackers(PhotoId, [Counter]) 
                    VALUES (@photoId, 1);
                ELSE
                    UPDATE PhotoTrackers SET [Counter] = [Counter] + 1 
                    WHERE PhotoId = @photoId;

                -- Return the new counter
                SELECT [Counter] FROM PhotoTrackers WHERE PhotoId=@photoId;"
            );
        }
        
        public override void Down()
        {
            DropStoredProcedure("spIncrementPhotoCounter");
        }
    }
}
