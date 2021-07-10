using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            List<String> GroupTime = Both(2);

            ViewBag.GroupTimeList = GroupTime;
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EntradasGenerales,EntradasNinnos,ComprobantePago,Acompannantes,CedulaClienteNavigation")] Reserva reserva, List<char> Gender, List<short> Age, IFormFile files, string groupTime)
        {
            reserva.CodigoEvento = 1;
            List<String> GT = divide(groupTime);
            reserva.Grupo = short.Parse(GT[0]);
            reserva.HoraEntrada = DateTime.Parse(GT[1]);
            reserva.FechaReserva = DateTime.Now;
            reserva.CedulaCliente = reserva.CedulaClienteNavigation.CedulaCliente;
            reserva.PrecioTotal = 3500 * (decimal)reserva.EntradasGenerales + 2599 * (decimal)reserva.EntradasNinnos;
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

                if (ValidateClient(reserva.CedulaCliente) == 0)
                {

                    if (_context.Clientes.Find(reserva.CedulaCliente) == null)
                    {
                        _context.Add(reserva.CedulaClienteNavigation);
                        await _context.SaveChangesAsync();
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
                    ModelState.AddModelError("CedulaClienteNavigation.CedulaCliente", "Querido usuario le informamos que ya existe una reserva a su nombre");
                    return View(reserva);
                }
            }
            ViewData["CedulaCliente"] = new SelectList(_context.Clientes, "CedulaCliente", "CedulaCliente", reserva.CedulaCliente);
            ViewData["Cedula"] = new SelectList(_context.UsuarioAdministradors, "Cedula", "Cedula", reserva.Cedula);
            ViewData["CodigoEvento"] = new SelectList(_context.Eventos, "CodigoEvento", "Cedula", reserva.CodigoEvento);
            return View(reserva);
        }





        private List<int> AmountGroups(int code)
        {
            Evento evento = new Evento();
            evento = (Evento)_context.Eventos.Where(eventos => eventos.CodigoEvento == code).FirstOrDefault();
            List<int> groupList = new List<int>();
            for (int i = 1; i <= evento.CantidadGrupos; i++)
            {
                groupList.Add(i);
            }
            return groupList;
        }

        private List<String> Entry(int groupList,int code)
        {
            Evento evento = new Evento();
            evento = (Evento)_context.Eventos.Where(eventos => eventos.CodigoEvento == code).FirstOrDefault();
            List<String> entryList = new List<String>();
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

        private List<String> Both(int code)
        {
            int num = AmountGroups(code).Count;
            List<String> time = Entry(num , 2);
            List<String> gT = new List<String>();

            for (int i = 1; i <= num; i++)
            {

                gT.Add("Grupo " + i + " entrada "+ time[i-1]);
                
            }
            return gT;

        }

        private List<String> divide(string grouptime)
        {
            char[] separators = new char[] {' '};

            string[] subs = grouptime.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            List<string> GroupAndTime = new List<string>();
            GroupAndTime.Add(subs[1]);
            GroupAndTime.Add(subs[3]);
            return GroupAndTime;

        }

        //Validate client
        private int ValidateClient(String id)
        {

            int reserva = _context.Reservas.Where(re => re.CedulaCliente == id).Count();
            return reserva;

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
