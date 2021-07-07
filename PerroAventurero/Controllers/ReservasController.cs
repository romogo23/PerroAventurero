using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace PerroAventurero.Models
{
    public class ReservasController : Controller
    {
        private readonly PAContext _context;

        public ReservasController(PAContext context)
        {
            _context = context;
        }

        // GET: Reservas
        public async Task<IActionResult> Index()
        {
            var pAContext = _context.Reservas.Include(r => r.CedulaClienteNavigation).Include(r => r.CedulaNavigation).Include(r => r.CodigoEventoNavigation);
            return View(await pAContext.ToListAsync());
        }

        // GET: Reservas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas
                .Include(r => r.CedulaClienteNavigation)
                .Include(r => r.CedulaNavigation)
                .Include(r => r.CodigoEventoNavigation)
                .FirstOrDefaultAsync(m => m.CodigoReserva == id);
            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }


        // GET: Reservas/Create
        public IActionResult Create()
        {
            ViewData["Nav"] = "false";
            ViewData["CedulaCliente"] = new SelectList(_context.Clientes, "CedulaCliente", "CedulaCliente");
            ViewData["Cedula"] = new SelectList(_context.UsuarioAdministradors, "Cedula", "Cedula");
            ViewData["CodigoEvento"] = new SelectList(_context.Eventos, "CodigoEvento", "Cedula");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddAcompannante([Bind("Acompannantes")] Reserva acompannante)
        {
            acompannante.Acompannantes.Add(new Acompannante());
            return PartialView("Acompannantes", acompannante);

        }


        // POST: Reservas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EntradasGenerales,EntradasNinnos,Grupo,HoraEntrada,ComprobantePago,Acompannantes,CedulaClienteNavigation")] Reserva reserva)
        {
            reserva.FechaReserva = DateTime.Now;
            reserva.CedulaCliente = reserva.CedulaClienteNavigation.CedulaCliente;
            reserva.CodigoEvento = 1;
            reserva.PrecioTotal = 3500 * (decimal)reserva.EntradasGenerales + 2599 * (decimal)reserva.EntradasNinnos;
             
            if (ModelState.IsValid)
            {
                _context.Add(reserva.CedulaClienteNavigation);
                await _context.SaveChangesAsync();

                _context.Add(reserva);
                await _context.SaveChangesAsync();

                for (int i = 0; i < reserva.Acompannantes.Count(); i++) {
                    reserva.Acompannantes[i].Codigo = i;
                    reserva.Acompannantes[i].CodigoReserva = _context.Reservas.Max(item => item.CodigoReserva);
                    _context.Add(reserva.Acompannantes[i]);
                }
                
                _context.SaveChanges();


                return Redirect("~/Home/Index");
            }
            ViewData["CedulaCliente"] = new SelectList(_context.Clientes, "CedulaCliente", "CedulaCliente", reserva.CedulaCliente);
            ViewData["Cedula"] = new SelectList(_context.UsuarioAdministradors, "Cedula", "Cedula", reserva.Cedula);
            ViewData["CodigoEvento"] = new SelectList(_context.Eventos, "CodigoEvento", "Cedula", reserva.CodigoEvento);
            return View(reserva);
        }

        // GET: Reservas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva == null)
            {
                return NotFound();
            }
            ViewData["CedulaCliente"] = new SelectList(_context.Clientes, "CedulaCliente", "CedulaCliente", reserva.CedulaCliente);
            ViewData["Cedula"] = new SelectList(_context.UsuarioAdministradors, "Cedula", "Cedula", reserva.Cedula);
            ViewData["CodigoEvento"] = new SelectList(_context.Eventos, "CodigoEvento", "Cedula", reserva.CodigoEvento);
            return View(reserva);
        }

        // POST: Reservas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CodigoReserva,CodigoEvento,CedulaCliente,Cedula,EntradasGenerales,EntradasNinnos,FechaReserva,Grupo,HoraEntrada,EsAceptada,ComprobantePago,PrecioTotal,Asistencia")] Reserva reserva)
        {
            if (id != reserva.CodigoReserva)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reserva);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservaExists(reserva.CodigoReserva))
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
            ViewData["CedulaCliente"] = new SelectList(_context.Clientes, "CedulaCliente", "CedulaCliente", reserva.CedulaCliente);
            ViewData["Cedula"] = new SelectList(_context.UsuarioAdministradors, "Cedula", "Cedula", reserva.Cedula);
            ViewData["CodigoEvento"] = new SelectList(_context.Eventos, "CodigoEvento", "Cedula", reserva.CodigoEvento);
            return View(reserva);
        }

        // GET: Reservas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas
                .Include(r => r.CedulaClienteNavigation)
                .Include(r => r.CedulaNavigation)
                .Include(r => r.CodigoEventoNavigation)
                .FirstOrDefaultAsync(m => m.CodigoReserva == id);
            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }

        // POST: Reservas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reserva = await _context.Reservas.FindAsync(id);
            _context.Reservas.Remove(reserva);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservaExists(int id)
        {
            return _context.Reservas.Any(e => e.CodigoReserva == id);
        }
    }
}
