using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PerroAventurero.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace PerroAventurero.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly PAContext _context;

        public HomeController(ILogger<HomeController> logger, PAContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["Nav"] = "true";
            deleteReservas();
            var pAContext = _context.Eventos.Include(e => e.CedulaNavigation);
            return View(await pAContext.ToListAsync());
        }
        private void deleteReservas() {
            List<Evento> evento = _context.Eventos.Where(ev => ev.Fecha >= DateTime.Now).ToList();
            for (int i = 0; i < evento.Count(); i++) {
                dReserva(evento[i].CodigoEvento);
            }
        }


        private void dReserva(int code) {
            List<Reserva> reserva = _context.Reservas.Where(ev => ev.CodigoEvento >= code && ev.ComprobantePago == null && ev.FechaReserva.AddMinutes(20) <= DateTime.Now).ToList();
            for (int i = 0; i < reserva.Count(); i++) {
                dAcompanante(reserva[i].CodigoReserva);
                _context.Reservas.Remove(reserva[i]);
            }
            _context.SaveChanges();
        }

        private void dAcompanante(int code)
        {
            List<Acompannante> acompannantes = _context.Acompannantes.Where(ev => ev.CodigoReserva == code).ToList();
            for (int i = 0; i<acompannantes.Count(); i++) {
                _context.Acompannantes.Remove(acompannantes[i]);
                    
            }
            _context.SaveChanges();

        }



        [Authorize]
        public IActionResult Aventuras()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
