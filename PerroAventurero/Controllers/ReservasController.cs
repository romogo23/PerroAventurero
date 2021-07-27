using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace PerroAventurero.Models
{
    public class ReservasController : Controller
    {
        private readonly PAContext _context;
        private static int code;
        private int AGE_CHILDREN = 12;

        public ReservasController(PAContext context)
        {
            _context = context;
        }

        // GET: Reservas
        public async Task<IActionResult> Index()
        {
            var pAContext = _context.Reservas.Where(r => r.EsAceptada == null && r.ComprobantePago != null).Include(r => r.CedulaClienteNavigation).Include(r => r.CedulaNavigation).Include(r => r.CodigoEventoNavigation);
            return View(await pAContext.ToListAsync());
        }


        public async Task<IActionResult> home()
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


        public async Task<IActionResult> Accept(int id)
        {
            var reserva = await _context.Reservas.FindAsync(id);
            //Enviar correo con el detalle de la reserva. No está sirviendo, REVISAR
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
            reserva.EsAceptada = true;
            _context.Update(reserva);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Reject(int id)
        {
            //Enviar correo de que la solicitud de reserva ha sido rechazada, aún no funciona
            var reserva = await _context.Reservas.FindAsync(id);
            Acompannante acompannate;
            while (_context.Acompannantes.Where(a => a.CodigoReserva == reserva.CodigoReserva).FirstOrDefault() != null)
            {
                acompannate = _context.Acompannantes.Where(a => a.CodigoReserva == reserva.CodigoReserva).FirstOrDefault();
                _context.Acompannantes.Remove(acompannate);
                await _context.SaveChangesAsync();
            }
            _context.Reservas.Remove(reserva);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // GET: Reservas/Create
        public IActionResult Create(int id)
        {
            if (id != 0) {
                code = id;
                List<String> GroupTime = groupTime(code);

                Evento evento = (Evento)_context.Eventos.Where(eventos => eventos.CodigoEvento == code).FirstOrDefault();

                ViewBag.EntradasNinnos = evento.PrecioNinno;
                ViewBag.EntradasGenerales = evento.PrecioGeneral;
                ViewBag.GroupTimeList = GroupTime;
            ViewData["Nav"] = "false";
            ViewData["CedulaCliente"] = new SelectList(_context.Clientes, "CedulaCliente", "CedulaCliente");
            ViewData["Cedula"] = new SelectList(_context.UsuarioAdministradors, "Cedula", "Cedula");
            ViewData["CodigoEvento"] = new SelectList(_context.Eventos, "CodigoEvento", "Cedula");
            return View();
            }
            return Redirect("~/Home/Index");
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EntradasGenerales,EntradasNinnos,ComprobantePago,Acompannantes,CedulaClienteNavigation")] Reserva reserva, List<char> Gender, List<short> Age, IFormFile files, string groupAndTime)
        {
            Evento evento = new Evento();
            evento = (Evento)_context.Eventos.Where(eventos => eventos.CodigoEvento == code).FirstOrDefault();
            reserva.CodigoEvento = evento.CodigoEvento;

            reserva.PrecioTotal = price(evento, Age);

            List <String> GT = divide(groupAndTime);
            reserva.Grupo = short.Parse(GT[0]);
            reserva.HoraEntrada = DateTime.Parse(GT[1]);

            reserva.FechaReserva = DateTime.Now;

            reserva.CedulaCliente = reserva.CedulaClienteNavigation.CedulaCliente;

            if (Age.Count() == 0)
            {
                reserva.EntradasNinnos = 0;
            }

            if (ModelState.IsValid)
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

                            reserva.ComprobantePago = target.ToArray();
                        }



                        // ViewBag.Image = ViewImage(objfiles.ComprobantePago);

                    }
                }
                List<int> capacity = available(evento.CantidadAforo, evento.CantidadGrupos);

                if (capacity[reserva.Grupo - 1] >= Age.Count() + 1)
                {

                    if (ValidateClient(reserva.CedulaCliente) == 0)
                    {


                        if (_context.Clientes.AsNoTracking().Where(cli => cli.CedulaCliente == reserva.CedulaCliente).Count() == 0)
                        {
                            _context.Add(reserva.CedulaClienteNavigation);
                            await _context.SaveChangesAsync();
                        }
                        else { 
                            reserva.CedulaClienteNavigation = null;
                        }
                        _context.Add(reserva);
                        await _context.SaveChangesAsync();

                        int code = _context.Reservas.Max(item => item.CodigoReserva);
                        for (int i = 0; i < Age.Count; i++)
                        {
                            Acompannante acompannante = new Acompannante();
                            acompannante.Codigo = i;
                            acompannante.CodigoReserva = code;
                            acompannante.Genero = Gender[i];
                            acompannante.Edad = Age[i];
                            _context.Add(acompannante);
                        }
                        _context.SaveChanges();

                        return Redirect("~/Home/Index");
                    }
                else
                {
                    ModelState.AddModelError("CedulaClienteNavigation.CedulaCliente", "Le informamos que ya existe una reserva a su nombre");
                    List<String> GroupTimeR = groupTime(code);

                      ViewBag.GroupTimeList = GroupTimeR;

                        ViewBag.EntradasNinnos = evento.PrecioNinno;
                        ViewBag.EntradasGenerales = evento.PrecioGeneral;
                        return View(reserva);
                }
            }
            else
                {
                    List<String> GroupTimeR = groupTime(code);

                    ViewBag.EntradasNinnos = evento.PrecioNinno;
                    ViewBag.EntradasGenerales = evento.PrecioGeneral;
                    ViewBag.GroupTimeList = GroupTimeR;
                    ModelState.AddModelError("Grupo", "Le informamos que no quedan entradas disponibles");
                    return View(reserva);

                }
            }
            ViewData["CedulaCliente"] = new SelectList(_context.Clientes, "CedulaCliente", "CedulaCliente", reserva.CedulaCliente);
            ViewData["Cedula"] = new SelectList(_context.UsuarioAdministradors, "Cedula", "Cedula", reserva.Cedula);
            ViewData["CodigoEvento"] = new SelectList(_context.Eventos, "CodigoEvento", "Cedula", reserva.CodigoEvento);
            List<String> GroupTime = groupTime(code);
            ViewBag.EntradasNinnos = evento.PrecioNinno;
            ViewBag.EntradasGenerales = evento.PrecioGeneral;
            ViewBag.GroupTimeList = GroupTime;
            return View(reserva);
        }


        
        private decimal price(Evento evento, List<short> reserva) { 
            decimal precioTotal = evento.PrecioGeneral;

            for (int i = 0; i < reserva.Count; i++) {
                if (reserva[i] > AGE_CHILDREN)
                {
                    precioTotal += evento.PrecioGeneral;
                }
                else {
                    precioTotal += evento.PrecioNinno;
                }
            }
            return precioTotal;
        }

        private List<int> AmountGroup(int totalCapacity, int groups)
        {
            int capacity = totalCapacity / groups;

            List<int> GroupsCapacity = new List<int>();

            for (int i = 1; i <= groups; i++)
            {
                GroupsCapacity.Add(capacity);

            }

            int validate = totalCapacity - (capacity * groups);

            if (validate > 1)
            {
                for (int i = 0; i < validate; i++)
                {
                    GroupsCapacity[i] = GroupsCapacity[i] + 1;
                }
            } else {
                for (int i = 0; i < -validate; i++)
                {
                    GroupsCapacity[i] = GroupsCapacity[i] + 1;
                }

            }

            return GroupsCapacity;
        }



        private List<int> available(int totalCapacity, int groups)
        {

            List<int> capacity = AmountGroup(totalCapacity, groups);

            for (int i = 1; i <= groups; i++)
            {

                capacity[i-1] = capacity[i-1] - acompannanteG(i);

            }

            return capacity;
        }

        private int acompannanteG(int group)
        {

            List<Reserva> reserva = _context.Reservas.Where(reserva => reserva.CodigoEvento == code && reserva.Grupo == group).ToList();
            int sum = 0;

            for (int i = 0; i < reserva.Count; i++)
            {
                sum += reserva[i].EntradasGenerales + (int) reserva[i].EntradasNinnos;


            }

            return sum;
        }



        private List<String> Entry(Evento evento)
        {
            List<String> entryList = new List<String>();
            int groupList = evento.CantidadGrupos;
            DateTime initTime = evento.HoraInicio;
            DateTime finalTime = evento.HoraFinal;
            TimeSpan diference = finalTime.Subtract(initTime);
            TimeSpan d = diference / groupList;
            DateTime newTime = initTime;
            for (int i = 1; i <= groupList; i++)
            {
                
                entryList.Add(newTime.ToString("HH:mm"));
                newTime += d;
            }
            return entryList;
        }

        private List<String> groupTime(int code)
        {
            Evento evento = new Evento();
            evento = (Evento)_context.Eventos.Where(eventos => eventos.CodigoEvento == code).FirstOrDefault();

            int num = evento.CantidadGrupos;

            List<int> capacity = available(evento.CantidadAforo, num);

            List<String> time = Entry(evento);

            List<String> gT = new List<String>();

            for (int i = 1; i <= num; i++)
            {
                if (capacity[i-1] > 0)
                {
                    gT.Add("Grupo " + i + " Entrada " + time[i - 1] + " disponibles " + capacity[i-1]);
                }
            }
            return gT;

        }

        private List<String> divide(string grouptime)
        {
            if (grouptime != null)
            {
                char[] separators = new char[] { ' ' };

                string[] subs = grouptime.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                List<string> GroupAndTime = new List<string>();
                GroupAndTime.Add(subs[1]);
                GroupAndTime.Add(subs[3]);
                return GroupAndTime;
            }
            return null;

        }

        //Validate client
        private int ValidateClient(String id)
        {

            int reserva = _context.Reservas.AsNoTracking().Where(re => re.CedulaCliente == id && re.CodigoEvento == code).Count();
            return reserva;

        }

        // POST: Reservas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("CedulaClienteNavigation")] Reserva reserva, IFormFile files)
        {
            if (ModelState.IsValid)
            {
                if(ValidateClient(reserva.CedulaClienteNavigation.CedulaCliente) !=0) {
                    Reserva reservaTempo = _context.Reservas.Where(re => re.CedulaCliente == reserva.CedulaClienteNavigation.CedulaCliente).FirstOrDefault();
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





          [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Insert([Bind("CedulaCliente")] Cliente cliente, IFormFile files)
        {

            Reserva reserva = new Reserva();
            if (ValidateClient(cliente.CedulaCliente) != 0) {
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

                            reserva.ComprobantePago = target.ToArray();
                        }


                        // ViewBag.Image = ViewImage(objfiles.ComprobantePago);

                    }
                }

            }
            return Redirect("~/Home/Index");

        }
        private bool ReservaExists(int id)
        {
            return _context.Reservas.Any(e => e.CodigoReserva == id);
        }
    }
}
