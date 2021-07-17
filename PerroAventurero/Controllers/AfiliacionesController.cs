using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
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
            var pAContext = _context.Afiliacions.Where(a => a.EsAceptada == null).Include(a => a.CedulaClienteNavigation).Include(a => a.CedulaNavigation);
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

        public async Task<IActionResult> Accept(int id)
        {

            var afiliacion = await _context.Afiliacions.FindAsync(id);
           

            ////Enviar correo con el codigo de afiliación No está sirviendo, REVISAR
            //SmtpClient client = new SmtpClient("smtp.gmail.com");
            //client.Port = 587;
            //client.EnableSsl = true;
            //client.Timeout = 100000;
            //client.EnableSsl = true;
            //client.DeliveryMethod = SmtpDeliveryMethod.Network;
            //client.UseDefaultCredentials = false;
            //client.Credentials = new NetworkCredential("correo", "contraseña");
            //MailMessage msg = new MailMessage();
            ////msg.To.Add(Correo.ToString()); correo se recibía por parámetro, supuse que era el correo al que se envía el codigo como tal
            //msg.To.Add("allan.najera@gmail.com");
            //msg.From = new MailAddress("juanperez33op@gmail.com");
            //msg.Subject = "Prueba de correo";
            //msg.Body = "Estos es todo el texto que lleva el mensaje con las diferentes descripciones";
            ////Attachment data = new Attachment(textBox3.Text);
            ////msg.Attachments.Add(data);
            //client.Send(msg);//Aquí está el problema

            //Con esto hace modifica la afiliación
            afiliacion.EsAceptada = true;
            _context.Update(afiliacion);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Reject(int id)
        {
            //Enviar correo de que la solicitud de afiliación ha sido rechazada, aún no funciona

            //El rechazar lo que hace es eliminar la afiliación
            var afiliacion = await _context.Afiliacions.FindAsync(id);
            _context.Afiliacions.Remove(afiliacion);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
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
