using Microsoft.AspNetCore.Mvc;
using DepartmentManagementSystem.Data;
using DepartmentManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using OfficeOpenXml;
using System.IO;

namespace DepartmentManagementSystem.Controllers
{
    [Authorize]
    public class DepartmentsController : Controller
    {
        private readonly AppDbContext _context;

        public DepartmentsController(AppDbContext context)
        {
            _context = context;
        }

        
        public async Task<IActionResult> Index(string? searchString)
        {
            if (_context.Departments == null)
            {
                return Problem("Entity set 'AppDbContext.Departments' is null.");
            }

            var departments = from d in _context.Departments select d;

            if (!string.IsNullOrEmpty(searchString))
            {
                departments = departments.Where(s => s.Name.Contains(searchString));
            }

            return View(await departments!.ToListAsync());
        }

        public async Task<IActionResult> ExportExcel()
        {
            var departments = await _context.Departments.ToListAsync();

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Departments");

            worksheet.Cells[1, 1].Value = "ID";
            worksheet.Cells[1, 2].Value = "Name";
            worksheet.Cells[1, 3].Value = "Description";

            for (int i = 0; i < departments.Count; i++)
            {
                worksheet.Cells[i + 2, 1].Value = departments[i].Id;
                worksheet.Cells[i + 2, 2].Value = departments[i].Name;
                worksheet.Cells[i + 2, 3].Value = departments[i].Description;
            }

            var stream = new MemoryStream(package.GetAsByteArray());
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Departments.xlsx");
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var department = await _context.Departments.FirstOrDefaultAsync(m => m.Id == id);
            if (department == null) return NotFound();

            return View(department);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description")] Department department)
        {
            if (ModelState.IsValid)
            {
                _context.Departments.Add(department);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Department created successfully!";
                return RedirectToAction(nameof(Index));
            }

            return View(department);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var department = await _context.Departments.FindAsync(id);
            if (department == null) return NotFound();

            return View(department);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] Department department)
        {
            if (id != department.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(department);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Departments.Any(e => e.Id == id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            return View(department);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var department = await _context.Departments.FirstOrDefaultAsync(m => m.Id == id);

            if (department == null) return NotFound();

            return View(department);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department != null)
            {
                _context.Departments.Remove(department);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
