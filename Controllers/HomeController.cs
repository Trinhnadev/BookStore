using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FPTBook.Models;
using Microsoft.EntityFrameworkCore;

namespace FPTBook.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    private readonly BookContext _context;

    public HomeController(BookContext context, ILogger<HomeController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 6)
    {
        var dbtestContext = _context.Books.Include(b => b.Cat).Skip((page - 1) * pageSize).Take(pageSize);
        ViewBag.TotalPage = Math.Ceiling((double)_context.Books.Count() / pageSize);
        ViewBag.Page = page;
        return View(await dbtestContext.ToListAsync());
    }

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
    public async Task<IActionResult> Show(int page = 1, int pageSize = 6)
    {
        var dbtestContext = _context.Books.Include(b => b.Cat).Skip((page - 1) * pageSize).Take(pageSize);
        ViewBag.TotalPage = Math.Ceiling((double)_context.Books.Count() / pageSize);
        ViewBag.Page = page;
        return View(await dbtestContext.ToListAsync());
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
