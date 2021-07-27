using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
        public async Task<IActionResult> Index(string searchString)
        {
            var pAContext = _context.Eventos.Include(e => e.CedulaNavigation);
            if (!String.IsNullOrEmpty(searchString))
            {
                pAContext = _context.Eventos.Where(e => e.NombreEvento.Contains(searchString)).Include(e => e.CedulaNavigation);
            }
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
            ViewBag.Image = ViewImage(evento.Imagen);
            if (evento == null)
            {
                return NotFound();
            }

            return View(evento);
        }

        private string ViewImage(byte[] arrayImage)

        {
            string base64String = Convert.ToBase64String(arrayImage, 0, arrayImage.Length);
            return "data:image/png;base64," + base64String;
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
        public async Task<IActionResult> Create([Bind("CodigoEvento,Cedula,NombreEvento,Lugar,Direccion,Fecha,PrecioGeneral,PrecioNinno,CantidadAforo,CantidadGrupos,HoraInicio,HoraFinal,EnvioAnuncios,Comentarios")] Evento evento, IFormFile files)
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
                    if (files != null)
                    {
                        if (files.Length > 0)
                        {
                            //Getting FileName
                            var fileName = Path.GetFileName(files.FileName);
                            //Getting file Extension
                            var fileExtension = Path.GetExtension(fileName);
                            // concatenating  FileName + FileExtension
                            var newFileName = String.Concat(Convert.ToString(Guid.NewGuid()), fileExtension);

                            using (var target = new MemoryStream())
                            {
                                files.CopyTo(target);

                                evento.Imagen = target.ToArray();
                            }
                        }
                    }
                    else
                    {
                        //agregar imagen por defecto pero aun no se cual o como se va tratar
                    }
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
            if (evento.Imagen != null)
            {
                ViewBag.Image = ViewImage(evento.Imagen);
            }

            ViewData["Cedula"] = new SelectList(_context.UsuarioAdministradors, "Cedula", "Cedula", evento.Cedula);
            return View(evento);
        }

        // POST: Eventoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CodigoEvento,Cedula,NombreEvento,Lugar,Direccion,Fecha,PrecioGeneral,PrecioNinno,CantidadAforo,CantidadGrupos,HoraInicio,HoraFinal,EnvioAnuncios,Comentarios")] Evento evento, IFormFile files)
        {
            if (id != evento.CodigoEvento)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                evento.HoraInicio = new DateTime(evento.Fecha.Year, evento.Fecha.Month, evento.Fecha.Day, evento.HoraInicio.Hour, evento.HoraInicio.Minute, evento.HoraInicio.Second);
                evento.HoraFinal = new DateTime(evento.Fecha.Year, evento.Fecha.Month, evento.Fecha.Day, evento.HoraFinal.Hour, evento.HoraFinal.Minute, evento.HoraFinal.Second);

                if (DateTime.Compare(evento.HoraInicio, evento.HoraFinal) > 0 || DateTime.Compare(evento.HoraInicio, evento.HoraFinal) == 0)
                {
                    //Validar que la fecha hora final sea posterior que hora inicio, ver que hago cuando es mayor
                    ModelState.AddModelError("HoraFinal", "Hora final debe ser posterior a hora inicial");
                    return View(evento);
                }
                else
                {
                    if (files != null)
                    {
                        if (files.Length > 0)
                        {
                            //Getting FileName
                            var fileName = Path.GetFileName(files.FileName);
                            //Getting file Extension
                            var fileExtension = Path.GetExtension(fileName);
                            // concatenating  FileName + FileExtension
                            var newFileName = String.Concat(Convert.ToString(Guid.NewGuid()), fileExtension);

                            using (var target = new MemoryStream())
                            {
                                files.CopyTo(target);

                                evento.Imagen = target.ToArray();
                            }
                        }
                    }
                    else
                    {
                        //supongo que no pasa nada, pero queria preguntar, creo que este else se podria borrar
                    }
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

        public ActionResult getImage(int id)
        {
            var eventoVieja = _context.Eventos.Find(id);
            byte[] byteImage = eventoVieja.Imagen;

            MemoryStream memoryStream = new MemoryStream(byteImage);
            Image image = Image.FromStream(memoryStream);

            memoryStream = new MemoryStream();
            image.Save(memoryStream, ImageFormat.Jpeg);
            memoryStream.Position = 0;

            return File(memoryStream, "image/jpg");
        }
    }
}
