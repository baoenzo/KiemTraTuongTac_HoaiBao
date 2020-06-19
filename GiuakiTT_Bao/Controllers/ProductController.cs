using GiuakiTT_Bao.EF;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace GiuakiTT_Bao.Views.Home
{
    public class ProductController : Controller
    {
        // GET: Product
        QLSPEntities1 db = new QLSPEntities1();
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;
            var products = from s in db.Product
                           select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                products = products.Where(s =>
               s.Name.ToUpper().Contains(searchString.ToUpper())
                ||
               s.Short_name.ToUpper().Contains(searchString.ToUpper()));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    products = products.OrderByDescending(s => s.Name);
                    break;

                default: // Name ascending
                    products = products.OrderBy(s => s.Name);
                    break;
            }
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(products.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Category, "ID", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Code,Name,Short_name,Note,CategoryID")] Product p)
        {
            if (ModelState.IsValid)
            {
              //  p.ca = 1;
                db.Product.Add(p);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryID = new SelectList(db.Category, "ID", "Name", p.CategoryID);
            return View(p);
        }

        public ActionResult Edit(int id)
        {
            ViewBag.CategoryID = new SelectList(db.Category, "ID", "Name");
            return View(db.Product.Find(id));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Code,Name,Short_name,Note")] Product p)
        {
            if (ModelState.IsValid)
            {
       
                db.Entry(p).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryID = new SelectList(db.Category, "ID", "Name", p.CategoryID);
            return View(p);
        }

        public ActionResult Delete(int id)
        {
            var user = db.Product.Find(id);
            db.Product.Remove(user);
            db.SaveChanges();
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