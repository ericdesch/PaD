namespace PaD.DataContexts.PaDMigrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System.Threading.Tasks;
    using System.Web.Hosting;
    using System.IO;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Drawing;
    using System.Reflection;
    using System.Web;
    using System.Web.Mvc;

    using EntityFramework.BulkInsert.Extensions;

    using Fooz.Logging;
    using Fooz.Caching;

    using PaD.Models;
    using PaD.DAL;
    using PaD.Infrastructure;
    using System.Text.RegularExpressions;

    internal sealed class Configuration : DbMigrationsConfiguration<PaD.DataContexts.PaDDb>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            MigrationsDirectory = @"DataContexts\PaDMigrations";
        }

        protected override void Seed(PaD.DataContexts.PaDDb context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            //// Launch debugger so we can step through seed code when run from package manager console.
            //if (System.Diagnostics.Debugger.IsAttached == false)
            //    System.Diagnostics.Debugger.Launch();

            // Import the dictionary values
            ImportDictionaryValues(context);

            // Import photos from old PaD database for user "eric"
            // 5AB4242A-1D92-4B38-9D07-17E4743E1F2C is userid 'eric' in the old database.
            int ericProjectId = ImportFromLegacyDatabase(context, new Guid("5AB4242A-1D92-4B38-9D07-17E4743E1F2C"));

            SetFeaturedProject(context, ericProjectId);

            // Add hit counters from old website that were taken from google analytics as a CSV.
            AddLegacyHits(context);

            // bailey and dave are in role "ProjectOwner", so add projects/photos for them using photos in their dirs
            // in Test Images folder.
            string[] usernames = new string[] { "bailey", "dave" };

            foreach (string username in usernames)
            {
                string testProjectTitle = username + "'s Test Project";

                // Check to see if the text project exists. If it does, no need to proceed.
                var testProject = context.Project.FirstOrDefault(p => p.Title == testProjectTitle);

                if (testProject != null)
                    continue;

                IdentityDb identityContext = new IdentityDb();
                var user = identityContext.Users.FirstOrDefault(u => u.UserName == username);

                // Get a list of Photos to add to the project that we are creating
                var photos = GetProjectPhotos(user.UserName);

                // Uniqueness is determined by both UserName and Title.
                // Note that this will add child elements when inserting, but will not update
                // children when updating; you have to do that manually.
                context.Project.AddOrUpdate(
                    p => new { p.IdentityUserName, p.Title },
                    new Project
                    {
                        IdentityUserName = user.UserName,
                        IsDefault = true,
                        Title = testProjectTitle,
                        Photos = photos
                    });
            }
        }

        private void ImportDictionaryValues(PaD.DataContexts.PaDDb context)
        {
            ILoggerProvider logger = (ILoggerProvider)DependencyResolver.Current.GetService(typeof(ILoggerProvider));
            ICacheProvider cache = (ICacheProvider)DependencyResolver.Current.GetService(typeof(ICacheProvider));

            DictionaryManager dicManager = new DictionaryManager(context, logger, cache);

            // Return if dictionary table already populated
            if (dicManager.Count() > 0)
            {
                return;
            }

            // Words to add
            List<Dictionary> dicWords = new List<Dictionary>();

            // Open dictionary.txt file
            string path = MapPath("~/dictionary/dictionary.txt");

            // Loop through file and add words to List<Dictionary>
            using (StreamReader reader = File.OpenText(path))
            {
                string s = "";
                while ((s = reader.ReadLine()) != null)
                {
                    dicWords.Add(new Dictionary()
                    {
                        Word = s.Trim()
                    });
                }
            }

            // Add the words to the database using BulkInsert.
            // HUGE performance impvrovement over EF.
            context.BulkInsert(dicWords);
        }

        /// <summary>
        /// Gets a list of Photo objects (including Photo, PhotoThubnail,
        /// and a generated rating for each) from the ~/TestImages folder. 
        /// Folder names indicate the date for each.
        /// UserName is used when creating the Ratings.
        /// </summary>
        /// <param name="identityUserName"></param>
        /// <returns></returns>
        private ICollection<Photo> GetProjectPhotos(string identityUserName)
        {
            // Path to test images folder
            string testImagesFolder = MapPath("~/TestImages/" + identityUserName);

            // Get all the images in the test folder recursively
            DirectoryInfo di = new DirectoryInfo(testImagesFolder);
            FileInfo[] fileInfos = di.GetFiles("*.jpg", SearchOption.AllDirectories);

            // Keep track of added dates. We only want one photo per date.
            HashSet<DateTime> addedDates = new HashSet<DateTime>();

            // List of Photo to insert.
            List<Photo> photosToAdd = new List<Photo>();

            foreach (FileInfo fi in fileInfos)
            {
                string imagePath = fi.FullName;
                string directoryName = fi.Directory.Name;
                string fileName = fi.Name;
                string ext = fi.Extension;

                // Get Title from image path
                string title = GetTitleFromFileName(fileName);

                // Get Date from image path
                DateTime date;
                try
                {
                    date = DateTime.ParseExact(directoryName, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                }
                catch
                {
                    // Not a folder we care about, so continue
                    continue;
                }

                // Only want 1 from each date...
                if (addedDates.Contains(date))
                {
                    continue;
                }
                // Add to addedDates so we don't add again
                addedDates.Add(date);

                // Create Photo for this image path
                Image image = Image.FromFile(imagePath);
                byte[] bytes = image.ToByteArray();

                string contentType = fi.GetContentTypeFromExtension();

                PhotoImage photoImage = new PhotoImage
                {
                    ContentType = contentType,
                    Bytes = bytes
                };

                // Rate determined by number of underscores in fileName
                // + 1 for titles with no underscores so range will be 1 to 5.
                int rating = fileName.Count(f => f == '_') + 1;
                // Max of 5 stars
                rating = rating > 5 ? 5 : rating;
                PhotoRating photoRating = new PhotoRating
                {
                    IdentityUserName = identityUserName,
                    Value = (short)rating
                };

                var photoRatings = new[] { photoRating };

                Photo photo = new Photo
                {
                    Title = title,
                    Alt = title,
                    Date = date,
                    PhotoImage = photoImage,
                    PhotoRatings = photoRatings
                };
                // Call extension method to create the thumbnail from the Photo
                photo.GenerateThumbnail();

                photosToAdd.Add(photo);
            }

            return photosToAdd;
        }

        private int ImportFromLegacyDatabase(PaD.DataContexts.PaDDb context, Guid userId)
        {
            IdentityDb identityContext = new IdentityDb();
            var user = identityContext.Users.FirstOrDefault(u => u.UserName == "eric");

            if (user == null)
            {
                throw new Exception("User 'eric' not found");
            }

            string projectTitle = "Eric's Photo A Day Project";

            // Check to see if the text project exists. If it does, no need to proceed.
            var project = context.Project.FirstOrDefault(p => p.Title == projectTitle);
            if (project != null)
            {
                // Already seeded
                return project.ProjectId;
            }

            // Add the project; we need the ProjectId when creating Photo objects.
            project = context.Project.Add(
                new Project
                {
                    IdentityUserName = user.UserName,
                    IsDefault = true,
                    Title = projectTitle
                });
            context.SaveChanges();

            // Context for legacy data
            LegacyEntities legacyContext = new LegacyEntities();

            // Get start and end dates
            DateTime startDate = legacyContext.LegacyPhotos.Where(p => p.UserId == userId).Min(p => p.Date);
            DateTime endDate = legacyContext.LegacyPhotos.Where(p => p.UserId == userId).Max(p => p.Date);

            // loop through dates and build a list of Photos to add to the database
            List<Photo> photosToAdd = new List<Photo>();

            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                // Get the photo from the old database
                LegacyPhoto legacyPhoto = legacyContext.LegacyPhotos.Where(p => p.Date == date).FirstOrDefault();

                if (legacyPhoto == null)
                    continue;

                // Make a new Photo from the old one.
                Photo photo = new Photo()
                {
                    Alt = legacyPhoto.Alt,
                    Date = legacyPhoto.Date,
                    IsPhotoOfTheMonth = legacyPhoto.IsPhotoOfTheMonth,
                    IsPhotoOfTheYear = legacyPhoto.IsPhotoOfTheYear,
                    PhotoImage = new PhotoImage()
                    {
                        Bytes = legacyPhoto.Image,
                        ContentType = legacyPhoto.ContentType
                    },
                    ProjectId = project.ProjectId,
                    Tags = legacyPhoto.Tags,
                    Title = legacyPhoto.Title,
                    PhotoRatings = GetPhotoRatings(legacyContext, legacyPhoto.Date) 
                };

                // generate a new thumbnail instead of copying existing thumbnail (in case size is different, improved algorythm, etc)
                photo.GenerateThumbnail();

                photosToAdd.Add(photo);
            }

            // Now add all the photos
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    context.Photo.AddRange(photosToAdd);
                    context.SaveChanges();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    // rethrow the exception, no point continuing.
                    throw ex;
                }
            }

            return project.ProjectId;
        }

        private void SetFeaturedProject(PaD.DataContexts.PaDDb context, int projectId)
        {
            context.FeaturedProject.AddOrUpdate(f => f.ProjectId, new FeaturedProject() { ProjectId = projectId });
        }

        private List<PhotoRating> GetPhotoRatings(LegacyEntities legacyContext, DateTime date)
        {
            List<PhotoRating> ratingsToAdd = new List<PhotoRating>();

            // Now get the lst of ratings for this photo and add them.
            List<LegacyRating> legacyRatings = legacyContext.LegacyRatings.Where(r => r.Date == date).ToList();

            foreach (LegacyRating legacyRating in legacyRatings)
            {
                // Use the legacy userId as the username for the rating unless it is user "eric"
                string username = legacyRating.UserId.ToString();
                if (username.ToLower() == "5AB4242A-1D92-4B38-9D07-17E4743E1F2C".ToLower())
                {
                    username = "eric";
                }

                PhotoRating rating = new PhotoRating()
                {
                    IdentityUserName = username,
                    //PhotoId = photo.PhotoId,
                    Value = legacyRating.Value ?? 0
                };

                ratingsToAdd.Add(rating);
            }

            return ratingsToAdd;
        }

        // Hits from old Photo-A-Day site downloaded from google analytics as a CSV.
        private void AddLegacyHits(PaD.DataContexts.PaDDb context)
        {
            // Need the project for 'eric' so we can later find the photoId each date.
            Project project = context.Project.FirstOrDefault(p => p.IdentityUserName == "eric");

            if (project == null)
                throw new Exception("Project for user 'eric' not found");

            // Check to see if trackers were already updated with legacy data.
            PhotoTracker photoTracker = context.PhotoTracker.FirstOrDefault(t => t.PhotoTrackerId > 0);

            if (photoTracker != null)
                return;

            // loop through dates and build a list of Photos to add to the database
            List<PhotoTracker> trackersToAdd = new List<PhotoTracker>();

            // Path to test images folder
            string testImagesFolder = MapPath("~/TestImages/");
            string csvFilePath = testImagesFolder + "legacyhits.csv";

            var reader = new StreamReader(File.OpenRead(csvFilePath));

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                // Skip lines that are blank or commented out or are the header line (header line contains "Pageviews")
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#") || line.Contains("Pageviews"))
                    continue;

                var values = line.Split(',');

                // We only care about the first 2 columns, which look like this:
                // /PhotoDetail.aspx?date=12/12/2010,78
                string pattern = "^/PhotoDetail\\.aspx\\?date=(\\d{2})/(\\d{2})/(\\d{4})$";
                Regex regex = new Regex(pattern);

                int photoId = 0;

                Match match = regex.Match(values[0]);
                if (match.Success)
                {
                    // match.Groups[0] is always the entire matched string. So start with index 1 for month.
                    int month = 0;
                    int.TryParse(match.Groups[1].Value, out month);
                    int day = 0;
                    int.TryParse(match.Groups[2].Value, out day);
                    int year = 0;
                    int.TryParse(match.Groups[3].Value, out year);

                    DateTime date = new DateTime(year, month, day);

                    // Get the PhotoId for eric's project for this date.
                    Photo photo = context.Photo.FirstOrDefault(p => p.ProjectId == project.ProjectId && p.Date == date);

                    if (photo == null)
                        continue;

                    photoId = photo.PhotoId;
                }
                else
                {
                    // No match. Bad data so skip.
                    continue;
                }

                long counter = 0;
                long.TryParse(values[1], out counter);

                // Is there already a tracker for this photoid?
                PhotoTracker tracker = trackersToAdd.Find(t => t.PhotoId == photoId);
                if (tracker == null)
                {
                    // Create a new tracker with these values.
                    tracker = new PhotoTracker()
                    {
                        PhotoId = photoId,
                        Counter = counter
                    };
                }
                else
                {
                    // Already exists, add this count to existing count
                    tracker.Counter += counter;
                }

                // Add it to the list of trackers to add.
                trackersToAdd.Add(tracker);
            }

            // Now add the trackers
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    context.PhotoTracker.AddRange(trackersToAdd);
                    context.SaveChanges();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    // rethrow the exception, no point continuing.
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Maps path to directory. Works when run from web app or from seed method.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private string MapPath(string filePath)
        {
            if (HttpContext.Current != null)
            {
                return HostingEnvironment.MapPath(filePath);
            }

            var absolutePath = new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath;
            var directoryName = Path.GetDirectoryName(absolutePath);
            var path = Path.Combine(directoryName, ".." + filePath.TrimStart('~').Replace('/', '\\'));

            return path;
        }

        /// <summary>
        /// Just replaces underscore with spaces from filename (ignoring extension)
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private string GetTitleFromFileName(string fileName)
        {
            fileName = Path.GetFileNameWithoutExtension(fileName);
            return fileName.Replace("_", " ");
        }
    }
}
