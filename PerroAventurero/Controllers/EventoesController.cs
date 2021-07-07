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
    public class EventoesController : Controller
    {
        private readonly PAContext _context;

        public EventoesController(PAContext context)
        {
            _context = context;
        }

        // GET: Eventoes
        public async Task<IActionResult> Index()
        {
            var pAContext = _context.Eventos.Include(e => e.CedulaNavigation);
            return View(await pAContext.ToListAsync());
        }

        // GET: Eventoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evento = await _context.Eventos
                .Include(e => e.CedulaNavigation)
                .FirstOrDefaultAsync(m => m.CodigoEvento == id);
            if (evento == null)
            {
                return NotFound();
            }

            return View(evento);
        }

        // GET: Eventoes/Create
        public IActionResult Create()
        {
            ViewData["Cedula"] = new SelectList(_context.UsuarioAdministradors, "Cedula", "Cedula");
            return View();
        }

        // POST: Eventoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CodigoEvento,Cedula,NombreEvento,Lugar,Direccion,Fecha,PrecioGeneral,PrecioNinno,CantidadAforo,CantidadGrupos,HoraInicio,HoraFinal,EnvioAnuncios,Comentarios")] Evento evento)
        {

            if (ModelState.IsValid)
            {
                evento.HoraInicio = new DateTime(evento.Fecha.Year, evento.Fecha.Month, evento.Fecha.Day, evento.HoraInicio.Hour, evento.HoraInicio.Minute, evento.HoraInicio.Second);
                evento.HoraFinal = new DateTime(evento.Fecha.Year, evento.Fecha.Month, evento.Fecha.Day, evento.HoraFinal.Hour, evento.HoraFinal.Minute, evento.HoraFinal.Second);

                
                if (DateTime.Compare(evento.HoraInicio,evento.HoraFinal) > 0 || DateTime.Compare(evento.HoraInicio, evento.HoraFinal) == 0)
                {
                    //Validar que la fecha hora final sea posterior que hora inicio, ver que hago cuando es mayor
                    ModelState.AddModelError("HoraFinal", "Hora final debe ser posterior a hora inicial");
                    return View(evento);
                }
                else
                {
                    _context.Add(evento);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));

                }
            }
            ViewData["Cedula"] = new SelectList(_context.UsuarioAdministradors, "Cedula", "Cedula", evento.Cedula);
            return View(evento);
        }

        // GET: Eventoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evento = await _context.Eventos.FindAsync(id);
            if (evento == null)
            {
                return NotFound();
            }
            ViewData["Cedula"] = new SelectList(_context.UsuarioAdministradors, "Cedula", "Cedula", evento.Cedula);
            return View(evento);
        }

        // POST: Eventoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CodigoEvento,Cedula,NombreEvento,Lugar,Direccion,Fecha,PrecioGeneral,PrecioNinno,CantidadAforo, CantidadGrupos,HoraInicio,HoraFinal,EnvioAnuncios,Comentarios")] Evento evento)
        {
            if (id != evento.CodigoEvento)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (DateTime.Compare(evento.HoraInicio, evento.HoraFinal) > 0 || DateTime.Compare(evento.HoraInicio, evento.HoraFinal) == 0)
                {
                    //Validar que la fecha hora final sea posterior que hora inicio, ver que hago cuando es mayor
                    ModelState.AddModelError("HoraFinal", "Hora final debe ser posterior a hora inicial");
                    return View(evento);
                }
                else
                {
                    try
                    {
                        _context.Update(evento);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!EventoExists(evento.CodigoEvento))
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
               
            }
            ViewData["Cedula"] = new SelectList(_context.UsuarioAdministradors, "Cedula", "Cedula", evento.Cedula);
            return View(evento);
        }

        // GET: Eventoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evento = await _context.Eventos
                .Include(e => e.CedulaNavigation)
                .FirstOrDefaultAsync(m => m.CodigoEvento == id);
            if (evento == null)
            {
                return NotFound();
            }

            return View(evento);
        }

        // POST: Eventoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var evento = await _context.Eventos.FindAsync(id);
            _context.Eventos.Remove(evento);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventoExists(int id)
        {
            return _context.Eventos.Any(e => e.CodigoEvento == id);
        }
    }
}
