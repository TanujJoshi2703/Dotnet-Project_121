using ExpenseTracker.Models;
using ExpenseTracker.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ExpenseTracker.Controllers
{
    public class ExpensesController : Controller
    {
        private readonly IExpenseService _svc;
        public ExpensesController(IExpenseService svc) { _svc = svc; }

        // GET: /Expenses
        public async Task<IActionResult> Index()
        {
            var list = await _svc.GetAllAsync();
            // compute totals for display
            ViewBag.Total = list.Count == 0 ? 0 : Math.Round((double)list.Sum(e => e.Amount), 2);
            return View(list);
        }

        // GET: /Expenses/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var e = await _svc.GetAsync(id);
            if (e == null) return NotFound();
            return View(e);
        }

        // GET: /Expenses/Create
        public IActionResult Create()
        {
            var model = new Expense { Date = DateTime.Today };
            return View(model);
        }

        // POST: /Expenses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Expense expense)
        {
            if (!ModelState.IsValid) return View(expense);
            await _svc.AddAsync(expense);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Expenses/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var e = await _svc.GetAsync(id);
            if (e == null) return NotFound();
            return View(e);
        }

        // POST: /Expenses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Expense expense)
        {
            if (id != expense.Id) return BadRequest();
            if (!ModelState.IsValid) return View(expense);
            await _svc.UpdateAsync(expense);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Expenses/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var e = await _svc.GetAsync(id);
            if (e == null) return NotFound();
            return View(e);
        }

        // POST: /Expenses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _svc.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
