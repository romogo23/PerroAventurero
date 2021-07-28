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

        public async Task<IActionResult> SendReminders(int codigoEvento)
        {
            Reserva currentReservation;
            Evento evento = _context.Eventos.Where(e => e.CodigoEvento == codigoEvento).Include(e => e.CedulaNavigation).FirstOrDefault();
            Cliente cliente;
            List<Reserva> listOfReservations = new List<Reserva>();
            if (_context.Reservas.Where(r => r.CodigoEvento == codigoEvento && r.EsAceptada == true).FirstOrDefault() != null)
            {
                listOfReservations = await (_context.Reservas.Where(r => r.CodigoEvento == codigoEvento).Include(e => e.CedulaNavigation)).ToListAsync();
            }
            else
            {
                //No se tienen a quien mandar recordatorios, preguntar que se hace en estos casos, se tiene que terminar para que no de errores
            }
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
                msg.From = new MailAddress("juanperez33op@gmail.com");
                msg.Subject = "Recordatorio de evento Perro Aventurero";

                do
                {
                    msg.To.Clear();
                    currentReservation = listOfReservations.First();
                    cliente = _context.Clientes.Where(c => c.CedulaCliente == currentReservation.CedulaCliente).FirstOrDefault();

                    msg.Body = "Perro aventurero le recuerda que el día " + evento.Fecha.ToString() + " se llevará a cabo el evento " + evento.NombreEvento +
                        " el cual tomará lugar en " + evento.Lugar + ". Poseemos una reservación con los siguientes datos: \n" +
                        "\nNombre de la persona: " + cliente.NombreCompleto + "\n" +
                        "Cédula: " + cliente.CedulaCliente + "\n" +
                        "Cantidad entradas generales: " + currentReservation.EntradasGenerales.ToString() + "\n" +
                        "Cantidad entradas niños: " + currentReservation.EntradasNinnos.ToString() + "\n" +
                        "Grupo : " + currentReservation.Grupo.ToString() + "\n" +
                        "Hora de entrada : " + currentReservation.HoraEntrada.ToString();

                    msg.To.Add(cliente.Correo.ToString());
                    //Attachment data = new Attachment(textBox3.Text);
                    //msg.Attachments.Add(data);
                    client.Send(msg);//Aquí está el problema

                    listOfReservations.Remove(currentReservation);

                } while (listOfReservations.Count() >= 1);
            }
            catch (Exception ex)
            {
                // TODO: handle exception
                throw new InvalidOperationException(ex.Message);
            }
            
            
            return RedirectToAction(nameof(Index));//hay que cambiarlo
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

            var eventoVieja = _context.Eventos.Find(id);
            byte[] logoEmp = ViewBag.Image = eventoVieja.Imagen;
            _context.Entry(eventoVieja).State = EntityState.Detached;

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
                        else
                        {
                            evento.Imagen = logoEmp;
                        }
                    }
                    else
                    {
                        evento.Imagen = logoEmp;
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
