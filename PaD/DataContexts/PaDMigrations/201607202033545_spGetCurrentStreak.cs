namespace PaD.DataContexts.PaDMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GetCurrentStreakSP : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(
                "spGetCurrentStreak",
                p => new
                {
                    projectId = p.Int()
                },
                @"
                SET NOCOUNT ON;
                
                DECLARE @lastDate Date;
                -- Get the project's last date
                
                SELECT @lastDate = MAX([Date])
                FROM Photos
                WHERE ProjectId = @projectId;
                
                -- If the project's last date isn't today or yesterday (less than 2 days ago), there is no streak
                IF @lastDate < DATEADD (d, -2, GetDate())
                BEGIN
                    SELECT 0;
                    RETURN;
                END;

                -- Get the number of days in the streak
                WITH distinctDatesCTE AS (
                    SELECT DISTINCT ProjectId, [Date] AS Day
                    FROM   Photos
                    WHERE ProjectId = @projectId
                ), numberingCTE AS (
                    SELECT ProjectId, Day,
                            row_number() OVER(PARTITION BY ProjectId ORDER BY Day DESC) AS RowNo
                    FROM   distinctDatesCTE
                )
                SELECT MIN(CAST(a.RowNo AS INT)) AS Streak
                FROM   numberingCTE a
                LEFT   JOIN numberingCTE b ON b.ProjectId = a.ProjectId
                                        AND b.RowNo = a.RowNo + 1
                WHERE  b.Day IS NULL OR
                        DATEDIFF(DAY, b.Day, a.Day) > 1
                GROUP  BY a.ProjectId;"
            );
        }
        
        public override void Down()
        {
            DropStoredProcedure("spGetCurrentStreak");
        }
    }
}
