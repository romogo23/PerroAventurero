using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PerroAventurero.Models;

namespace PerroAventurero.Controllers
{
    public class AfiliacionesController : Controller
    {
        private readonly PAContext _context;

        public AfiliacionesController(PAContext context)
        {
            _context = context;
        }

        // GET: Afiliaciones
        public async Task<IActionResult> Index()
        {
            var pAContext = _context.Afiliacions.Include(a => a.CedulaClienteNavigation).Include(a => a.CedulaNavigation);
            return View(await pAContext.ToListAsync());
        }

        // GET: Afiliaciones/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var afiliacion = await _context.Afiliacions
                .Include(a => a.CedulaClienteNavigation)
                .Include(a => a.CedulaNavigation)
                .FirstOrDefaultAsync(m => m.Codigo == id);
            if (afiliacion == null)
            {
                return NotFound();
            }

            return View(afiliacion);
        }

        // GET: Afiliaciones/Create
        public IActionResult Create()
        {
            ViewData["CedulaCliente"] = new SelectList(_context.UsuarioComuns, "CedulaCliente", "CedulaCliente");
            ViewData["Cedula"] = new SelectList(_context.UsuarioAdministradors, "Cedula", "Cedula");
            return View();
        }

        // POST: Afiliaciones/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Codigo,CedulaCliente,Cedula,Fecha,ComprobantePago,EsAceptada")] Afiliacion afiliacion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(afiliacion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CedulaCliente"] = new SelectList(_context.UsuarioComuns, "CedulaCliente", "CedulaCliente", afiliacion.CedulaCliente);
            ViewData["Cedula"] = new SelectList(_context.UsuarioAdministradors, "Cedula", "Cedula", afiliacion.Cedula);
            return View(afiliacion);
        }

        // GET: Afiliaciones/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var afiliacion = await _context.Afiliacions.FindAsync(id);
            if (afiliacion == null)
            {
                return NotFound();
            }
            ViewData["CedulaCliente"] = new SelectList(_context.UsuarioComuns, "CedulaCliente", "CedulaCliente", afiliacion.CedulaCliente);
            ViewData["Cedula"] = new SelectList(_context.UsuarioAdministradors, "Cedula", "Cedula", afiliacion.Cedula);
            return View(afiliacion);
        }

        // POST: Afiliaciones/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Codigo,CedulaCliente,Cedula,Fecha,ComprobantePago,EsAceptada")] Afiliacion afiliacion)
        {
            if (id != afiliacion.Codigo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(afiliacion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AfiliacionExists(afiliacion.Codigo))
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
            ViewData["CedulaCliente"] = new SelectList(_context.UsuarioComuns, "CedulaCliente", "CedulaCliente", afiliacion.CedulaCliente);
            ViewData["Cedula"] = new SelectList(_context.UsuarioAdministradors, "Cedula", "Cedula", afiliacion.Cedula);
            return View(afiliacion);
        }

        // GET: Afiliaciones/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var afiliacion = await _context.Afiliacions
                .Include(a => a.CedulaClienteNavigation)
                .Include(a => a.CedulaNavigation)
                .FirstOrDefaultAsync(m => m.Codigo == id);
            if (afiliacion == null)
            {
                return NotFound();
            }

            return View(afiliacion);
        }

        // POST: Afiliaciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var afiliacion = await _context.Afiliacions.FindAsync(id);
            _context.Afiliacions.Remove(afiliacion);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AfiliacionExists(int id)
        {
            return _context.Afiliacions.Any(e => e.Codigo == id);
        }
    }
}
