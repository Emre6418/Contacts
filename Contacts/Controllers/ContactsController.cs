using Contacts.Data;
using Contacts.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Contacts.Controllers
{
    public class ContactsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ContactsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        //Get ALL LİST
        public IActionResult Index()
        {
            var data = _context.contacts.ToList();  
            return View(data);
        }

        //GET CREATE
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(contacts contact, IFormFile photo)
        {
            if (ModelState.IsValid)
            {
                // Dosya işlemleri
                if (photo != null && photo.Length > 0)
                {
                    var fileName = Path.GetFileName(photo.FileName);
                    var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "upload");
                    var filePath = Path.Combine(uploadPath, fileName);

                    // Resmi kaydet
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await photo.CopyToAsync(stream);
                    }

                    // Veritabanına dosya yolunu kaydet
                    contact.ImagePath = "upload/" + fileName;
                }

                _context.contacts.Add(contact);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(contact);
        }

        //GET EDIT
        public IActionResult Edit(int Id)
        {
            var contact = _context.contacts.Find(Id);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(contacts contact)
        {
            if (ModelState.IsValid)
            {
                _context.Update(contact);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(contact);
        }

        //GET REMOVE
        public IActionResult Delete(int id)
        {
            var contact = _context.contacts.Find(id);
            if (contact == null)
            {
                return NotFound();
            }
            return View(contact);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contact = await _context.contacts.FindAsync(id);
            if (contact != null)
            {
                _context.contacts.Remove(contact);
                await _context.SaveChangesAsync();  
            }
           return RedirectToAction(nameof(Index));
        }

        private bool ContactExists(int id)
        {
            return _context.contacts.Any(e => e.Id == id);
        }



    }
}
