﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Migrations;
using WebApplication1.Models;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class ItemController : Controller
    {
        public ItemController(AppDbContext db ,  IHostingEnvironment host)
        {
            _db = db;
            _host = host;
        }
        private readonly IHostingEnvironment _host;
        private readonly AppDbContext _db;
        public IActionResult Index()
        {
            IEnumerable<Item> itemsList =_db.items.Include(x => x.Category).ToList();
                return View(itemsList);
        }

        // get
        public IActionResult New()
        {
            createSelectList();
            return View();
        }

        //post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult New(Item item )
        {
            if (ModelState.IsValid) {
                string fileName = string.Empty;
                if (item.clientFile != null)
                {
                    string myUpload = Path.Combine(_host.WebRootPath, "images");
                    fileName = item.clientFile.FileName;
                    string fullPath = Path.Combine(myUpload, fileName);
                    item.clientFile.CopyTo(new FileStream(fullPath, FileMode.Create));
                    item.ImgPath = fileName;
                }

                _db.items.Add(item);
            _db.SaveChanges();
            TempData["successData"] = "Item has been added successfully";
            return RedirectToAction("Index");
            }
            else
            {

                return View(item);  
            }
        }

        public void createSelectList(int selectId = 0)
        {
            List<Categorys> categories = _db.Categorys.ToList();
            SelectList listItems = new SelectList(categories, "Id", "Name", selectId);
            ViewBag.CategoryList = listItems;
        }
        //GET
        public IActionResult Edit(int? id)
        {
            if(id ==null || id==0)
            { return NotFound(); }
            var item = _db.items.Find(id);
            if (item == null)
            {
                return NotFound();
            }
            createSelectList(item.categoryId);
            return View(item);
        }
        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Item item)
        {
            if (ModelState.IsValid)
            {
                _db.items.Update(item);
                _db.SaveChanges();
                TempData["successData"] = "Item has been Update successfully";
                return RedirectToAction("Index");
            }
            else
            {
                createSelectList(item.categoryId);
                return View(item);
            }
        }


        //Delete
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            { return NotFound(); }
            var item = _db.items.Find(id);
            if (item == null)
            {
                return NotFound();
            }
            createSelectList();
            return View(item);
        }
        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Item item)
        {
         
                _db.items.Remove(item);
                _db.SaveChanges();
            TempData["successData"] = "Item has been Remove successfully";
            return RedirectToAction("Index");
            
        }

    }
}
