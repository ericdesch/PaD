namespace PaD.DataContexts.PaDMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class spAddOrUpdateRating : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(
                "spAddOrUpdateRating",
                p => new
                {
                    photoId = p.Int(),
                    userName = p.String(),
                    value = p.Double()
                },
                @"
                SET NOCOUNT ON;
                
                DECLARE @photoRatingId int;

                -- Get the existing photoRatingId if there is one
                SELECT TOP(1) @photoRatingId = PhotoRatingId FROM PhotoRatings
                    WHERE PhotoId = @photoId AND IdentityUserName = @userName;

                -- If there isn't an existing rating for this photoId and userName
                -- insert a new one. Otherwise update it with the value
                IF @photoRatingId IS NULL
                	INSERT INTO PhotoRatings(PhotoId, IdentityUserName, Value) VALUES (@photoId, @userName, @value);
                ELSE
                	UPDATE PhotoRatings SET Value = @value WHERE PhotoRatingId = @photoRatingId;

                -- Return the new average rating for this photoId
                SELECT AVG(CAST(Value AS FLOAT)) AS NewAverage FROM PhotoRatings
                    WHERE PhotoId = @photoId;"
            );
        }
        
        public override void Down()
        {
            DropStoredProcedure("spAddOrUpdateRating");
        }
    }
}
