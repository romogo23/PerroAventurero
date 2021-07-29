using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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

            Cliente cliente = await _context.Clientes.FindAsync(afiliacion.CedulaCliente);
            DateTime date = new DateTime(afiliacion.Fecha.Value.Year + 1, afiliacion.Fecha.Value.Month, afiliacion.Fecha.Value.Day, afiliacion.Fecha.Value.Hour, afiliacion.Fecha.Value.Minute, afiliacion.Fecha.Value.Second);

            try
            {
                SmtpClient client = new SmtpClient("smtp.gmail.com");
                client.Port = 587;
                client.EnableSsl = true;
                client.Timeout = 100000;
                client.EnableSsl = true;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential("juanperez33op@gmail.com", "Juanitoperez33");
                MailMessage msg = new MailMessage();
                msg.To.Add(cliente.Correo.ToString());
                msg.From = new MailAddress("juanperez33op@gmail.com");
                msg.Subject = "Estado de afiliacion a club Aventuras con Descuentos";
                msg.Body = "Su afiliación al club Aventuras con Descuentos ha sido ACEPTADA.\n\n" +
                    "Su código de afiliación es: " + afiliacion.Codigo.ToString() + "\n"+
                    "Su afiliación comienza el día: " + afiliacion.Fecha.ToString() + "\n" +
                    "Su afiliación vence el día: " + date.ToString();
                client.Send(msg);
            }
            catch (Exception ex)
            {
                // TODO: handle exception
                throw new InvalidOperationException(ex.Message);
            }
            

            //Con esto hace modifica la afiliación
            afiliacion.EsAceptada = true;
            _context.Update(afiliacion);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Reject(int id)
        {

            var afiliacion = await _context.Afiliacions.FindAsync(id);
            Cliente cliente = await _context.Clientes.FindAsync(afiliacion.CedulaCliente);

            try
            {
                SmtpClient client = new SmtpClient("smtp.gmail.com");
                client.Port = 587;
                client.EnableSsl = true;
                client.Timeout = 100000;
                client.EnableSsl = true;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential("juanperez33op@gmail.com", "Juanitoperez33");
                MailMessage msg = new MailMessage();
                msg.To.Add(cliente.Correo.ToString());
                msg.From = new MailAddress("juanperez33op@gmail.com");
                msg.Subject = "Estado de afiliacion a club Aventuras con Descuentos";
                msg.Body = "Lo sentimos, su afiliación fue rechazada. \n" +
                    "Encontramos algunos problemas con el comprobante de pago enviado.  \n" +
                    "Agradecemos el apoyo. \n" +
                    "Si desea realizar una nueva solicitud por favor vuelva a realizar el proceso de afiliación";
                client.Send(msg);
            }
            catch (Exception ex)
            {
                // TODO: handle exception
                throw new InvalidOperationException(ex.Message);
            }


            //El rechazar lo que hace es eliminar la afiliación
            
            _context.Afiliacions.Remove(afiliacion);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // GET: Afiliaciones/Create
        public IActionResult Create()
        {
            string CedulaCliente = null;
            CedulaCliente = User.Identity.Name;
            if (CedulaCliente != null)
            {

                ViewData["CedulaCliente"] = new SelectList(_context.UsuarioComuns, "CedulaCliente", "CedulaCliente");
                ViewData["Cedula"] = new SelectList(_context.UsuarioAdministradors, "Cedula", "Cedula");
                return View();
            }
            else {
                return Redirect("/Auth/Login");
            }

        }

        // POST: Afiliaciones/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile files)
        {

            var CedulaCliente = User.Identity.Name;
            if (_context.Afiliacions.Where(af => af.CedulaCliente == CedulaCliente).FirstOrDefault() == null)
            {
                if (files != null)
                {
                    Afiliacion afiliacionTemporal = new Afiliacion();

                    //Agregar codigo desde bd
                    //afiliacionTemporal.Codigo = 
                    afiliacionTemporal.CedulaCliente = CedulaCliente;
                    afiliacionTemporal.Fecha = DateTime.Today;
                    //afiliacionTemporal.ComprobantePago = 
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
                            afiliacionTemporal.ComprobantePago = target.ToArray();
                        }

                        int code;


                        //Agregar codigo ramdon verificando los que hay en la bd
                        do
                        {
                            code = generateCode();
                        } while (_context.Afiliacions.Where(af => af.Codigo == code).FirstOrDefault() != null);

                        afiliacionTemporal.Codigo = code;
                        afiliacionTemporal.CedulaCliente = CedulaCliente;
                        afiliacionTemporal.Fecha = DateTime.Today;

                        _context.Add(afiliacionTemporal);
                        await _context.SaveChangesAsync();
                        ViewBag.r = "s";

                    }
                }
                //Ver a donde lo mando, solo puse eso para que dejara de molestar
                return Redirect("~/Home/Index");
            }
            else
            {
                //Ver a donde lo mando, solo puse eso para que dejara de molestar
                //Ya tiene una afiliacion a su nombre
                return Redirect("~/Home/Index");
            }
        }


        private int generateCode()
        {
            return new Random().Next(1000, 9999);
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

        public ActionResult getImage(int id)
        {
            var eventoVieja = _context.Afiliacions.Find(id);
            byte[] byteImage = eventoVieja.ComprobantePago;

            MemoryStream memoryStream = new MemoryStream(byteImage);
            Image image = Image.FromStream(memoryStream);

            memoryStream = new MemoryStream();
            image.Save(memoryStream, ImageFormat.Jpeg);
            memoryStream.Position = 0;

            return File(memoryStream, "image/jpg");
        }
    }
}
