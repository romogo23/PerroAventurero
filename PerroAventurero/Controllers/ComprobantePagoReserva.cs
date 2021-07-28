using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PerroAventurero.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PerroAventurero.Controllers
{
    public class ComprobantePagoReserva : Controller
    {

        private readonly PAContext _context;
        private static int code;
        private static Reserva cliente;
        private static bool opc;
        public ComprobantePagoReserva(PAContext context)
        {
            _context = context;
        }

        public IActionResult Index(int id)
        {
            opc = true;
            if (id != 0)
            {
                code = id;
                return View();
            }
            return Redirect("~/Home/Index");
        }

        private int ValidateClient(String id)
        {
            int reserva = _context.Reservas.Where(re => re.CedulaCliente == id && re.CodigoEvento == code).Count();
            return reserva;
        }



        private Evento showEvento()
        {
            Evento evento = new Evento();
            evento = (Evento)_context.Eventos.Where(eventos => eventos.CodigoEvento == code).FirstOrDefault();
            return evento;

        }

        public async Task<ActionResult> _Evento()
        {
            return PartialView("_Evento", showEvento());

        }




        // POST: Reservas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string CedulaCliente, IFormFile files)
        {
            if (opc != true)
            {
                if (cliente != null)
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

                                cliente.ComprobantePago = target.ToArray();
                            }

                            // ViewBag.Image = ViewImage(objfiles.ComprobantePago);

                        }
                        _context.Update(cliente);
                        await _context.SaveChangesAsync();
                        return Redirect("~/Home/Index");

                    }
                    else
                    {
                        ModelState.AddModelError("ComprobantePago", "Debe adjuntar el comprobante de pago");
                        return View("ComprobantePago", cliente);
                    }
                }
            }
            else
            {
                if (ValidateClient(CedulaCliente) != 0)
                {
                    cliente = _context.Reservas.Where(re => re.CedulaCliente == CedulaCliente && re.CodigoEvento == code).FirstOrDefault();
                    opc = false;
                    return View("ComprobantePago", cliente);
                }
                else
                {
                    ModelState.AddModelError("CedulaClienteNavigation.CedulaCliente", "Le informamos que no existe una reserva a su nombre");
                    return View("ComprobantePago");
                }
            }
            return null;
        }

    }
}




