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
    public class ReportesController : Controller
    {

        private readonly PAContext _context;
        private int AGE_CHILDREN = 12;

        public ReportesController(PAContext context)
        {
            _context = context;
        }


        public IActionResult Index()
        {
            List<String> rp = reports();
            ViewBag.Reports = rp;
            return View();
        }

        public ActionResult RenderMenu(string selectReport, DateTime start, DateTime final)
        {
            List<String> rp = reports();
            ViewBag.Reports = rp;
            if (selectReport == "Comparación entre asistencia y reservaciones por evento")
            {
                if (start == default(DateTime) || final == default(DateTime))
                {
                    ModelState.AddModelError("Fecha_Evento", "Seleccione un rango de fechas");
                    return View("RenderMenu");
                }
                else
                {
                    return View("ReportAttendanceReservationsEvent", reportsAttendanceReservationsEvents(start, final));

                }

            }
            else if (selectReport == "Comparación entre asistencia y reservaciones de todos los eventos")
            {
                return View("ReportAllAtendanceReservatios", reportsAllAtendanceReservations());
            }
            else if (selectReport == "Comparación entre asistencia y reservaciones de niños")
            {
                return View("ReportsAllAtendanceReservationsChildren", reportsAllAtendanceReservationsChildren());


            }
            else if (selectReport == ("Top 10 usuarios que han asistido a más eventos"))
            {
                return View("ReportsTopUserAttending", reportsTopUserAttending());

            }
            else if (selectReport == ("Promedio de edad de asistencia por evento"))
            {
                //if (start == default(DateTime) || final == default(DateTime))
                //{
                //    ModelState.AddModelError("Fecha_Evento", "Seleccione un rango de fechas");
                //    return View("RenderMenu");
                //}
                //else
                //{
                //    return View("ReportAttendanceReservationsEvent", reportsAttendanceReservationsEvents());

                //}

            }
            else if (selectReport == ("Promedio de edad de asistencia por evento"))
            {
                //if (start == default(DateTime) || final == default(DateTime))
                //{
                //    ModelState.AddModelError("Fecha_Evento", "Seleccione un rango de fechas");
                //    return View("RenderMenu");
                //}
                //else
                //{
                //    return View("ReportAttendanceReservationsEvent", reportsAttendanceReservationsEvents());

                //}

            }
            else {
                // rp.Add("Género de asistentes");

            }


            return View();
            
        }


        //hay que agregar el rango de fechas
        private List<Reports> reportsAttendanceReservationsEvents(DateTime start, DateTime final)
        {
            List<Reports> ListReports = new List<Reports>();
            List<Evento> listEventos = _context.Eventos.Where(ev=> ev.Fecha >= start && ev.Fecha <= final).ToList();


            for (int i = 0; i < listEventos.Count; i++)
            {
                Reports report = new Reports();
                report.NombreEvento = listEventos[i].NombreEvento;
                report.Fecha_Evento = listEventos[i].Fecha;
                report.asistencia = attendance(listEventos[i].CodigoEvento);
                report.reservas = reservations(listEventos[i].CodigoEvento);
                ListReports.Add(report);

            }

            return ListReports;
        }

        private int attendance(int code)
        {
            List<Reserva> reserva = _context.Reservas.Where(re => re.CodigoEvento == code).ToList();

            int sumAttendance = 0;

            for (int i = 0; i < reserva.Count; i++)
            {
                if (reserva[i].Asistencia == true) {
                    sumAttendance++;
                }
                int acompannantes = _context.Acompannantes.Where(ac => ac.CodigoReserva == reserva[i].CodigoReserva && ac.Asistencia == true).Count();
                sumAttendance += acompannantes;

            }

            return sumAttendance;
        }

        private int reservations(int code)
        {
            List<Reserva> reserva = _context.Reservas.Where(re => re.CodigoEvento == code).ToList();

            int sumReservations = reserva.Count;

            for (int i = 0; i < reserva.Count; i++)
            {
                int acompannantes = _context.Acompannantes.Where(ac => ac.CodigoReserva == reserva[i].CodigoReserva).Count();
                sumReservations += acompannantes;

            }

            return sumReservations;
        }

        private List<String> reports()
        {
            List<String> rp = new List<String>();
            rp.Add("Comparación entre asistencia y reservaciones por evento");
            rp.Add("Comparación entre asistencia y reservaciones de todos los eventos");
            rp.Add("Comparación entre asistencia y reservaciones de niños");
            rp.Add("Top 10 usuarios que han asistido a más eventos");
            rp.Add("Promedio de edad de asistencia por evento");
            rp.Add("Género de los asistentes por evento");
            rp.Add("Género de asistentes");

            return rp;

        }


        //comparation Report attendace and reservations
        //Cantidad de reservaciones totales. 
        //Cantidad de asistentes totales.
        //Porcentaje de comparación: cantidad de asistentes por 100 dividido entre cantidad de reservaciones totales. 
        private Reports reportsAllAtendanceReservations()
        {
            Reports report = new Reports();
            List<Evento> listEventos = _context.Eventos.ToList();


            for (int i = 0; i < listEventos.Count; i++)
            {
                report.asistencia += attendance(listEventos[i].CodigoEvento);
                report.reservas += reservations(listEventos[i].CodigoEvento);

            }
            report.porcentaje = percentage(report.asistencia, report.reservas);
            return report;
        }

        private decimal percentage(int attendance, int reservations) {
            if (attendance == 0 || reservations == 0) {
                return 0;
            }
            return ((decimal)attendance * 100) / (decimal)reservations;
        
        }


        //Atendance and reservations of children
        //Cantidad de reservaciones de niños totales.
        //Cantidad de niños asistentes totales.
        //Porcentaje de comparación: cantidad de niños asistentes por 100 dividido entre cantidad de reservaciones de niños totales. 

        private Reports reportsAllAtendanceReservationsChildren()
        {
            Reports report = new Reports();
            List<Evento> listEventos = _context.Eventos.ToList();


            for (int i = 0; i < listEventos.Count; i++)
            {
                report.asistencia += attendanceChildren(listEventos[i].CodigoEvento);
                report.reservas += reservationsChildren(listEventos[i].CodigoEvento);

            }
            report.porcentaje = percentage(report.asistencia, report.reservas);
            return report;
        }

        private int attendanceChildren(int code)
        {
            List<Reserva> reserva = _context.Reservas.Where(re => re.CodigoEvento == code).ToList();

            int sumAttendance = 0;

            for (int i = 0; i < reserva.Count; i++)
            {
                
                int acompannantes = _context.Acompannantes.Where(ac => ac.CodigoReserva == reserva[i].CodigoReserva && ac.Asistencia == true &&  ac.Edad <= AGE_CHILDREN).Count();
                sumAttendance += acompannantes;

            }

            return sumAttendance;
        }

        private int reservationsChildren(int code)
        {
            List<Reserva> reserva = _context.Reservas.Where(re => re.CodigoEvento == code).ToList();

            int sumReservations = 0;

            for (int i = 0; i < reserva.Count; i++)
            {
                int acompannantes = _context.Acompannantes.Where(ac => ac.CodigoReserva == reserva[i].CodigoReserva && ac.Edad <= AGE_CHILDREN).Count();
                sumReservations += acompannantes;

            }

            return sumReservations;
        }


        //Report Users attending the most events
        //Cédula de usuario (se debe agrupar por este dato). 
        //Nombre de usuario.
        //Número de teléfono del usuario.
        //Cantidad de eventos asistidos: realizar una cuenta de los eventos a los que ha asistido el usuario (se debe ordenar por este dato). 

        private List<Reports> reportsTopUserAttending()
        {
            List<Cliente> listClientes = _context.Clientes.FromSqlRaw("Select top 10 RECEPCION_ANUNCIOS, NOMBRE_COMPLETO, GENERO, FECHA_NACIMIENTO, TELEFONO,correo, Cliente.CEDULA_CLIENTE,  count(Cliente.CEDULA_CLIENTE) as 'asistencia' from cliente inner join RESERVA on CLIENTE.CEDULA_CLIENTE = RESERVA.CEDULA_CLIENTE group by cliente.CEDULA_CLIENTE, correo, NOMBRE_COMPLETO, TELEFONO, GENERO, FECHA_NACIMIENTO, RECEPCION_ANUNCIOS order by asistencia desc").ToList();

             List<Reports> ListReport = new List<Reports>();


            for (int i = 0; i < listClientes.Count; i++)
            {
                Reports report = new Reports();
                report.cedulacliente = listClientes[i].CedulaCliente;
                report.NombreCliente = listClientes[i].NombreCompleto;
                report.telefono = listClientes[i].Telefono;

                report.AttendanceEvent = attendanceTop(listClientes[i].CedulaCliente);
                ListReport.Add(report);

            }
            
            return ListReport;
        }

        private int attendanceTop(string id)
        {
            int attendance = _context.Reservas.Where(re => re.CedulaCliente == id && re.Asistencia == true).Count();

            return attendance;
        }


        //Report average age attending event date range
        //Nombre del evento. 
        //Fecha del evento.
        //Promedio de edad por evento: suma de las edades de los asistentes dividido entre la cantidad total de asistencia


        //Gender attendance per event (date range)
        //Nombre del evento. 
        //Fecha del evento. 
        //Porcentaje de asistencia de género masculino: cantidad de asistentes de género masculino por 100 dividido entre asistentes totales. 
        //Porcentaje de asistencia de género femenino: cantidad de asistentes de género femenino por 100 dividido entre asistentes totales. 
        //Porcentaje de asistencia de género otro: cantidad de asistentes de género otro por 100 dividido entre asistentes totales. 

        //Gender attendance all events
        //Porcentaje de asistencia de género masculino: cantidad de asistentes de género masculino por 100 dividido entre asistentes totales. 
        //Porcentaje de asistencia de género femenino: cantidad de asistentes de género femenino por 100 dividido entre asistentes totales. 
        //Porcentaje de asistencia de género otro: cantidad de asistentes de género otro por 100 dividido entre asistentes totales. 


    }
}
