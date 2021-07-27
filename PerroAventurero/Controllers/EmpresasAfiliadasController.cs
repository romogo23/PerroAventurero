using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PerroAventurero.Models;
using System.Drawing;
using System.Drawing.Imaging;

namespace PerroAventurero.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class EmpresasAfiliadasController : Controller
    {
        private readonly PAContext _context;

        public EmpresasAfiliadasController(PAContext context)
        {
            _context = context;
        }

      

        //[Authorize(Roles = "Administrator")]
        // GET: EmpresasAfiliadas
        public async Task<IActionResult> Index(string searchString)
        {
            var pAContext = _context.EmpresasAfiliadas.Include(e => e.CedulaNavigation);
            if (!String.IsNullOrEmpty(searchString))
            {
                pAContext = _context.EmpresasAfiliadas.Where(e => e.NombreEmpresa.Contains(searchString) || e.NombreContacto.Contains(searchString)).Include(e => e.CedulaNavigation);
            }
            List<byte[]> listP = new List<byte[]>();

            foreach (var empr in pAContext.ToList())
            {
                listP.Add(empr.Logo);
              
            }

            List<String> listt = ViewImage2(listP);

           ViewBag.List = listt;
           


            return View(await pAContext.ToListAsync());
        }

        private string ViewImage(byte[] arrayImage)

        {

            string base64String = Convert.ToBase64String(arrayImage, 0, arrayImage.Length);

            return "data:image/png;base64," + base64String;

        }

        private List<String> ViewImage2(List<byte[]> arrayImage)

        {
            List<String> listas = new List<String>();
            string base64String;
            foreach (var a in arrayImage)
            {
               base64String = "data:image/png;base64," + Convert.ToBase64String(a, 0, a.Length);
                listas.Add(base64String);
            }



            return listas;

        }

        // GET: EmpresasAfiliadas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empresasAfiliada = await _context.EmpresasAfiliadas
                .Include(e => e.CedulaNavigation)
                .FirstOrDefaultAsync(m => m.CodigoEmpresa == id);
            ViewBag.Image = ViewImage(empresasAfiliada.Logo);
            if (empresasAfiliada == null)
            {
                return NotFound();
            }

            return View(empresasAfiliada);
        }

        // GET: EmpresasAfiliadas/Create
        public IActionResult Create()
        {
            ViewData["Cedula"] = new SelectList(_context.UsuarioAdministradors, "Cedula", "Cedula");
            return View();
        }

        // POST: EmpresasAfiliadas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CodigoEmpresa,Cedula,NombreEmpresa,Correo,Logo,Categoria,Telefono,NombreContacto")] EmpresasAfiliada empresasAfiliada, IFormFile files)
        {
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

                            empresasAfiliada.Logo = target.ToArray();
                        }



                        // ViewBag.Image = ViewImage(objfiles.ComprobantePago);

                    }
                }
                //empresasAfiliada.Cedula = session;
                _context.Add(empresasAfiliada);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Cedula"] = new SelectList(_context.UsuarioAdministradors, "Cedula", "Cedula", empresasAfiliada.Cedula);
            //ViewData["Categoria"] = new SelectList(_context.EmpresasAfiliadas, "Categoria", "Categoria", empresasAfiliada.Categoria);
            return View(empresasAfiliada);
        }

        // GET: EmpresasAfiliadas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empresasAfiliada = await _context.EmpresasAfiliadas.FindAsync(id);
            if (empresasAfiliada == null)
            {
                return NotFound();
            }
            ViewBag.Image = ViewImage(empresasAfiliada.Logo);
            
            ViewData["Cedula"] = new SelectList(_context.UsuarioAdministradors, "Cedula", "Cedula", empresasAfiliada.Cedula);
            return View(empresasAfiliada);
        }

        // POST: EmpresasAfiliadas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CodigoEmpresa,Cedula,NombreEmpresa,Correo,Logo,Categoria,Telefono,NombreContacto")] EmpresasAfiliada empresasAfiliada, IFormFile files)
        {
            if (id != empresasAfiliada.CodigoEmpresa)
            {
                return NotFound();
            }

            var empresasAfiliadaVieja = _context.EmpresasAfiliadas.Find(id);
            byte[] logoEmp = ViewBag.Image = empresasAfiliadaVieja.Logo;
            _context.Entry(empresasAfiliadaVieja).State = EntityState.Detached;

            if (ModelState.IsValid)
            {


                try
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

                                empresasAfiliada.Logo = target.ToArray();
                            }

                        }
                        else
                        {
                            empresasAfiliada.Logo = logoEmp;
                        }
                    }
                    else
                    {
                        empresasAfiliada.Logo = logoEmp;
                    }
                    _context.Update(empresasAfiliada);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmpresasAfiliadaExists(empresasAfiliada.CodigoEmpresa))
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
            ViewData["Cedula"] = new SelectList(_context.UsuarioAdministradors, "Cedula", "Cedula", empresasAfiliada.Cedula);
            return View(empresasAfiliada);
        }

        // GET: EmpresasAfiliadas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empresasAfiliada = await _context.EmpresasAfiliadas
                .Include(e => e.CedulaNavigation)
                .FirstOrDefaultAsync(m => m.CodigoEmpresa == id);
            if (empresasAfiliada == null)
            {
                return NotFound();
            }

            return View(empresasAfiliada);
        }

        // POST: EmpresasAfiliadas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int CodigoEmpresa)
        {
            var empresasAfiliada = await _context.EmpresasAfiliadas.FindAsync(CodigoEmpresa);
            _context.EmpresasAfiliadas.Remove(empresasAfiliada);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmpresasAfiliadaExists(int id)
        {
            return _context.EmpresasAfiliadas.Any(e => e.CodigoEmpresa == id);
        }

        public ActionResult getImage(int id)
        {
            var empresasAfiliadaVieja = _context.EmpresasAfiliadas.Find(id);
            byte[] byteImage = empresasAfiliadaVieja.Logo;

            MemoryStream memoryStream = new MemoryStream(byteImage);
            Image image = Image.FromStream(memoryStream);

            memoryStream = new MemoryStream();
            image.Save(memoryStream, ImageFormat.Jpeg);
            memoryStream.Position = 0;

            return File(memoryStream, "image/jpg");
        }

    }
}
