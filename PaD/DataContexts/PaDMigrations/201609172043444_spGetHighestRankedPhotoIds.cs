namespace PaD.DataContexts.PaDMigrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class spGetHighestRankedPhotoIds : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(
                "spGetHighestRankedPhotoIds",
                p => new
                {
                    numberOfPhotos = p.Int()
                },
                @"
                SET NOCOUNT ON;

                -- How heavily weighted the variables are
                -- relative to each other.
                declare @aveRatingWeight float
                declare @ratingCounterWeight float
                declare @hitCounterWeight float

                set @aveRatingWeight = .5
                set @ratingCounterWeight = .3
                set @hitCounterWeight = .2

                -- Need the min and max for the variables to
                -- normalize the data.
                declare @aveRatingMin float
                declare @aveRatingMax float
                declare @ratingCounterMin float
                declare @ratingCounterMax float
                declare @hitCounterMin float
                declare @hitCounterMax float

                ;WITH MinMaxCTE(PhotoId, AveRating, RatingCount, HitCount)
                AS
                (
	                SELECT ph.PhotoId, 
		                AVG(pr.Value) AS AveRating,
		                COUNT(pr.PhotoId) AS RatingCount,
		                pt.[Counter] As HitCount
	                FROM Photos ph
		                inner join PhotoRatings pr on ph.PhotoId = pr.PhotoId
		                inner join PhotoTrackers pt on ph.PhotoId = pt.PhotoId
	                GROUP BY ph.PhotoId, pt.[Counter]
                )

                SELECT 
	                @aveRatingMin = MIN(AveRating),
	                @aveRatingMax = MAX(AveRating),
	                @ratingCounterMin = MIN(RatingCount),
	                @ratingCounterMax = MAX(RatingCount),
	                @hitCounterMin = MIN(HitCount),
	                @hitCounterMax = MAX(HitCount)
                FROM MinMaxCTE;

                -- Get the data with normalized values for the
                -- Rating and the Counter
                ;WITH NormalizedCTE(PhotoId, AveRating, RatingCount, HitCount)
                AS
                (
	                SELECT 
		                ph.PhotoId, 
		                (AVG(pr.Value) - @aveRatingMin) / (@aveRatingMax - @aveRatingMin) AS AveRating,
		                (COUNT(pr.PhotoId) - @ratingCounterMin) / (@ratingCounterMax - @ratingCounterMin) AS RatingCount,
		                (pt.[Counter] - @hitCounterMin) / (@hitCounterMax - @hitCounterMin) AS HitCount
	                FROM 
		                Photos ph
			                inner join PhotoRatings pr on ph.PhotoId = pr.PhotoId
			                inner join PhotoTrackers pt on ph.PhotoId = pt.PhotoId
	                GROUP BY ph.PhotoId, pt.[Counter]
                ),
                -- Apply the weighting to the normalized data
                WeightedCTE(PhotoId, [Rank])
                AS
                (
	                SELECT
		                PhotoId,
		                (AveRating * @aveRatingWeight)  +
		                (RatingCount * @ratingCounterWeight) +
		                (HitCount * @hitCounterWeight) AS [Rank]
	                FROM NormalizedCTE
                )

                -- Return the PhotoId and Rank of the highest ranked
                -- photo in the passed date range.
                SELECT TOP (@numberOfPhotos)
	                PhotoId,
	                [Rank]
                FROM
	                WeightedCTE
                ORDER BY
	                [Rank] DESC"
                );
        }

        public override void Down()
        {
            DropStoredProcedure("spGetHighestRankedPhotoIds");
        }
    }
}
