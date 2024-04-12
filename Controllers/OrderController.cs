using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FPTBook.Models;
using Microsoft.AspNetCore.Authorization;

namespace FPTBook.Controllers
{
    public class OrderController : Controller
    {
        private readonly BookContext _context;

        public OrderController(BookContext context)
        {
            _context = context;
        }

         [Authorize(Roles = "Admin,StoreOwner")]
        // GET: Order
        public async Task<IActionResult> Index()
        {
            if (_context.Orders == null)
            {
                return Problem("Entity set 'BookContext.Orders' is null.");
            }

            var order = await _context.Orders.OrderBy(o => o.Status == "Waiting" ? 0 : 1).ToListAsync();
            return View(order);
        }
        

        // GET: Order/Details/5
        [Authorize(Roles = "Admin,StoreOwner")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Order/Create
        public IActionResult Create()
        {
            return View();
        }

        public async Task<IActionResult> IndexUser()
        {
            var bookContext = _context.Orders.Include(c => c.OrderDetails).Where(p => p.UserId == User.Identity.Name);
            return View(await bookContext.ToListAsync());
        }


        // POST: Order/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,Total,Address,Status")] Order order)
        {
            if (ModelState.IsValid)
            {
                order.Time = DateTime.Now;
                order.UserId = User.Identity.Name;
                order.Status = "Waiting";
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                var cart = _context.Carts.Include(c => c.IdBookNavigation).Where(p => p.UserId == User.Identity.Name);

                foreach (var crt in cart)
                {
                    var orderDetail = new OrderDetail();
                    orderDetail.BookId = crt.BookId;
                    orderDetail.Price = crt.Price;
                    orderDetail.OrderId = order.Id;
                    order.Total += crt.Price;
                    _context.OrderDetails.Add(orderDetail);
                }
                _context.Carts.RemoveRange(cart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(IndexUser));
            }
            return View(order);

        }

        // GET: Order/Edit/5
        [Authorize(Roles = "Admin,StoreOwner")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST: Order/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,StoreOwner")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,Time,Total,Address,Status")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
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
            return View(order);
        }

        // GET: Order/Delete/5
        [Authorize(Roles = "Admin,StoreOwner")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Order/Delete/5
        [Authorize(Roles = "Admin,StoreOwner")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Orders == null)
            {
                return Problem("Entity set 'BookContext.Orders'  is null.");
            }
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        //GET: Order/SetOrder/
         [Authorize(Roles = "Admin,StoreOwner")]
        public async Task<IActionResult> SetOrder(int? id)
        {
            var order = await _context.Orders.FindAsync(id);
            order.Status = "Approval";
            _context.Update(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return (_context.Orders?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
