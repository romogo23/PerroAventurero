using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PerroAventurero.Models;

namespace PerroAventurero.Controllers
{
    public class EmpresasAfiliadasController : Controller
    {
        private readonly PAContext _context;

        public EmpresasAfiliadasController(PAContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Administrator")]
        // GET: EmpresasAfiliadas
        public async Task<IActionResult> Index()
        {
            var pAContext = _context.EmpresasAfiliadas.Include(e => e.CedulaNavigation);
            return View(await pAContext.ToListAsync());
        }

        // GET: EmpresasAfiliadas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empresasAfiliada = await _context.EmpresasAfiliadas
                .Include(e => e.CedulaNavigation)
                .FirstOrDefaultAsync(m => m.CodigoEmpresa == id);
            if (empresasAfiliada == null)
            {
                return NotFound();
            }

            return View(empresasAfiliada);
        }

        // GET: EmpresasAfiliadas/Create
        public IActionResult Create()
        {
            ViewData["Cedula"] = new SelectList(_context.UsuarioAdministradors, "Cedula", "Cedula");
            return View();
        }

        // POST: EmpresasAfiliadas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CodigoEmpresa,Cedula,NombreEmpresa,Correo,Logo,Categoria,Telefono")] EmpresasAfiliada empresasAfiliada)
        {
            if (ModelState.IsValid)
            {
                //empresasAfiliada.Cedula = session;
                _context.Add(empresasAfiliada);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Cedula"] = new SelectList(_context.UsuarioAdministradors, "Cedula", "Cedula", empresasAfiliada.Cedula);
            //ViewData["Categoria"] = new SelectList(_context.EmpresasAfiliadas, "Categoria", "Categoria", empresasAfiliada.Categoria);
            return View(empresasAfiliada);
        }

        // GET: EmpresasAfiliadas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empresasAfiliada = await _context.EmpresasAfiliadas.FindAsync(id);
            if (empresasAfiliada == null)
            {
                return NotFound();
            }
            ViewData["Cedula"] = new SelectList(_context.UsuarioAdministradors, "Cedula", "Cedula", empresasAfiliada.Cedula);
            return View(empresasAfiliada);
        }

        // POST: EmpresasAfiliadas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CodigoEmpresa,Cedula,NombreEmpresa,Correo,Logo,Categoria,Telefono")] EmpresasAfiliada empresasAfiliada)
        {
            if (id != empresasAfiliada.CodigoEmpresa)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(empresasAfiliada);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmpresasAfiliadaExists(empresasAfiliada.CodigoEmpresa))
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
            ViewData["Cedula"] = new SelectList(_context.UsuarioAdministradors, "Cedula", "Cedula", empresasAfiliada.Cedula);
            return View(empresasAfiliada);
        }

        // GET: EmpresasAfiliadas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empresasAfiliada = await _context.EmpresasAfiliadas
                .Include(e => e.CedulaNavigation)
                .FirstOrDefaultAsync(m => m.CodigoEmpresa == id);
            if (empresasAfiliada == null)
            {
                return NotFound();
            }

            return View(empresasAfiliada);
        }

        // POST: EmpresasAfiliadas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var empresasAfiliada = await _context.EmpresasAfiliadas.FindAsync(id);
            _context.EmpresasAfiliadas.Remove(empresasAfiliada);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmpresasAfiliadaExists(int id)
        {
            return _context.EmpresasAfiliadas.Any(e => e.CodigoEmpresa == id);
        }
    }
}
