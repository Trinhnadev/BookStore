using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FPTBook.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore.Metadata.Internal;


namespace FPTBook.Controllers
{
     [Authorize(Roles = "Admin,StoreOwner")]
    public class BookController : Controller
    {
        private readonly BookContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BookController(BookContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        private string GetUniqueFileName(string fileName)
        {
            fileName = Path.GetFileName(fileName);
            return Path.GetFileNameWithoutExtension(fileName)
                   + "_"
                   + Guid.NewGuid().ToString().Substring(0, 4)
                   + Path.GetExtension(fileName);
        }

        // GET: Book
        public async Task<IActionResult> Index()
        {
            var fptbookContext = _context.Books.Include(b => b.Cat);
            return View(await fptbookContext.ToListAsync());
        }

        // GET: Book/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Cat)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Book/Create
        public IActionResult Create()
        {
            var categories = _context.Categories.Where(c => c.Status == "Yes");
            ViewData["CatId"] = new SelectList(categories, "Id", "Name");
            return View();
        }

        // POST: Book/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Author,Quantity,Price,Description,Image,CatId,ImageFile")] Book Book)
        {
            if (Book.ImageFile == null)
            {
                string defaultImagePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "defaultBook.jpg");
                Book.Image = "defaultBook.jpg";
            }
            else
            {
                string uniqueFileName = GetUniqueFileName(Book.ImageFile.FileName);
                string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await Book.ImageFile.CopyToAsync(fileStream);
                }
                Book.Image = uniqueFileName;
            }

            if (ModelState.IsValid)
            {
                _context.Add(Book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // var categories = _context.Categories.Where(c => c.Status == "Yes");
            // Trinh function
            ViewData["CatId"] = new SelectList(_context.Categories, "Id", "Name", Book.CatId);
            return View(Book);
        }

        // GET: Book/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            ViewData["CatId"] = new SelectList(_context.Categories, "Id", "Name", book.CatId);
            return View(book);
        }

        // POST: Book/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Author,Quantity,Price,Description,Image,CatId,ImageFile")] Book book)
        {
            if (id != book.Id)
            {
                return NotFound();
            }
            if(book.ImageFile == null){
                ModelState.Remove("ImageFile");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    if (book.ImageFile != null) // Check if ImageFile is not null
                    {
                        string uniqueFileName = GetUniqueFileName(book.ImageFile.FileName);
                        string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await book.ImageFile.CopyToAsync(fileStream);
                        }
                        book.Image = uniqueFileName;
                    }

                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CatId"] = new SelectList(_context.Categories, "Id", "Name", book.CatId);
            return View(book);
        }
        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Author,Quantity,Price,Description,Image,CatId,ImageFile")] Book book)
        // {
        //     if (id != book.Id)
        //     {
        //         return NotFound();
        //     }

        //     if (!ModelState.IsValid)
        //     {
        //         if (book.ImageFile != null)
        //         {
        //             string uniqueFileName = GetUniqueFileName(book.ImageFile.FileName);
        //             string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", uniqueFileName);

        //             using (var fileStream = new FileStream(filePath, FileMode.Create))
        //             {
        //                 await book.ImageFile.CopyToAsync(fileStream);
        //             }
        //             book.Image = uniqueFileName;
        //         }

        //         _context.Update(book);
        //         await _context.SaveChangesAsync();
        //         return RedirectToAction(nameof(Index));
        //     }

        //     ViewData["CatId"] = new SelectList(_context.Categories, "Id", "Name", book.CatId);
        //     return View(book);
        // }

        // GET: Book/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            // if (id == null || _context.Books == null)
            // {
            //     return NotFound();
            // }

            // var book = await _context.Books
            //     .Include(b => b.Cat)
            //     .FirstOrDefaultAsync(m => m.Id == id);
            // if (book == null)
            // {
            //     return NotFound();
            // }

            // return View(book);
            if (_context.Books == null)
            {
                return Problem("Entity set 'FptbookContext.Books'  is null.");
            }
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Book/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Books == null)
            {
                return Problem("Entity set 'FptbookContext.Books'  is null.");
            }
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return (_context.Books?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
