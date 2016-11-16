using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PaD.DataContexts;
using PaD.Models;

namespace PaD.Controllers
{
    public class PhotosController : Controller
    {
        private PaDDb db = new PaDDb();

        // GET: Photos
        public async Task<ActionResult> Index()
        {
            var photo = db.Photo.Include(p => p.PhotoImage).Include(p => p.Project).Include(p => p.ThumbnailImage);
            return View(await photo.ToListAsync());
        }

        // GET: Photos/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Photo photo = await db.Photo.FindAsync(id);
            if (photo == null)
            {
                return HttpNotFound();
            }
            return View(photo);
        }

        // GET: Photos/Create
        public ActionResult Create()
        {
            ViewBag.PhotoId = new SelectList(db.PhotoImage, "PhotoId", "ContentType");
            ViewBag.ProjectId = new SelectList(db.Project, "ProjectId", "IdentityUserName");
            ViewBag.PhotoId = new SelectList(db.ThumbnailImage, "PhotoId", "ContentType");
            return View();
        }

        // POST: Photos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "PhotoId,ProjectId,Date,Title,Alt,Tags,IsPhotoOfTheMonth,IsPhotoOfTheYear,Timestamp")] Photo photo)
        {
            if (ModelState.IsValid)
            {
                db.Photo.Add(photo);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.PhotoId = new SelectList(db.PhotoImage, "PhotoId", "ContentType", photo.PhotoId);
            ViewBag.ProjectId = new SelectList(db.Project, "ProjectId", "IdentityUserName", photo.ProjectId);
            ViewBag.PhotoId = new SelectList(db.ThumbnailImage, "PhotoId", "ContentType", photo.PhotoId);
            return View(photo);
        }

        // GET: Photos/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Photo photo = await db.Photo.FindAsync(id);
            if (photo == null)
            {
                return HttpNotFound();
            }
            ViewBag.PhotoId = new SelectList(db.PhotoImage, "PhotoId", "ContentType", photo.PhotoId);
            ViewBag.ProjectId = new SelectList(db.Project, "ProjectId", "IdentityUserName", photo.ProjectId);
            ViewBag.PhotoId = new SelectList(db.ThumbnailImage, "PhotoId", "ContentType", photo.PhotoId);
            return View(photo);
        }

        // POST: Photos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "PhotoId,ProjectId,Date,Title,Alt,Tags,IsPhotoOfTheMonth,IsPhotoOfTheYear,Timestamp")] Photo photo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(photo).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.PhotoId = new SelectList(db.PhotoImage, "PhotoId", "ContentType", photo.PhotoId);
            ViewBag.ProjectId = new SelectList(db.Project, "ProjectId", "IdentityUserName", photo.ProjectId);
            ViewBag.PhotoId = new SelectList(db.ThumbnailImage, "PhotoId", "ContentType", photo.PhotoId);
            return View(photo);
        }

        // GET: Photos/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Photo photo = await db.Photo.FindAsync(id);
            if (photo == null)
            {
                return HttpNotFound();
            }
            return View(photo);
        }

        // POST: Photos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Photo photo = await db.Photo.FindAsync(id);
            db.Photo.Remove(photo);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
