namespace PaD.DataContexts.PaDMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class spGetDefaultProjectDayViewModel : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(
                "spGetDefaultProjectDayViewModel",
                p => new
                {
                    userName = p.String(),
                    date = p.DateTime()
                },
                @"SET NOCOUNT ON;

                DECLARE @firstDate AS DATE;
                DECLARE @lastDate AS DATE;
                DECLARE @aveRating AS FLOAT;

                SELECT @firstDate = MIN(p.[Date])
                FROM Projects pr
                    JOIN Photos p
                    ON pr.ProjectId = p.ProjectId
                WHERE pr.IdentityUserName LIKE @userName
                    AND pr.IsDefault = 1;

                SELECT @lastDate = MAX(p.[Date])
                FROM Projects pr
                    JOIN Photos p
                    ON pr.ProjectId = p.ProjectId
                WHERE pr.IdentityUserName LIKE @userName
                    AND pr.IsDefault = 1;

                SELECT @aveRating = SUM(Cast(r.Value as Float)) / COUNT(r.Value)
                FROM Projects pr
                    JOIN Photos p
                    ON pr.ProjectId = p.ProjectId
                        AND pr.IsDefault = 1
                        AND p.Date >= @date AND p.Date < dateadd(day, 1, @date)
                    LEFT JOIN PhotoRatings r
                    ON p.PhotoId = r.PhotoId
                WHERE pr.IdentityUserName LIKE @userName;

                SELECT
                    pr.ProjectId,
                    pr.IdentityUserName AS 'Username',
                    pr.Title AS 'ProjectTitle',
                    pr.IsDefault,
                    @firstDate AS 'FirstDate',
                    @lastDate AS 'LastDate',
                    p.PhotoId,
                    p.[Date],
                    p.Title AS 'PhotoTitle',
                    p.Alt,
                    p.Tags,
                    p.IsPhotoOfTheMonth,
                    p.IsPhotoOfTheYear,
                    @aveRating AS 'AveRating', 
                    i.Bytes,
                    i.ContentType
                FROM Projects pr
                    LEFT JOIN Photos p
                    ON pr.ProjectId = p.ProjectId
                        AND pr.IsDefault = 1
                        AND p.Date >= @date AND p.Date < DATEADD(DAY, 1, @date)
                    LEFT JOIN PhotoRatings r
                    ON p.PhotoId = r.PhotoId
                        LEFT JOIN PhotoImages i
                        ON p.PhotoId = i.PhotoId
                WHERE pr.IdentityUserName LIKE @userName
                GROUP BY pr.ProjectId, pr.Title, pr.IdentityUserName, pr.IsDefault,
                    p.PhotoId, p.ProjectId, p.Title, p.[Date], p.Alt, p.IsPhotoOfTheMonth, p.IsPhotoOfTheYear, p.Tags,
                    i.PhotoId, i.Bytes, i.ContentType;"
            );
        }
        
        public override void Down()
        {
            DropStoredProcedure("spGetDefaultProjectDayViewModel");
        }
    }
}
