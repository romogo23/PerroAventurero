using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using PerroAventurero.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PerroAventurero.Controllers
{
    public class Demo : Controller
    {
        private readonly PAContext _context;
        public Demo(PAContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

  

        private string ViewImage(byte[] arrayImage)

        {

            string base64String = Convert.ToBase64String(arrayImage, 0, arrayImage.Length);

            return "data:image/png;base64," + base64String;

        }



        [HttpPost]
        public IActionResult Index(IFormFile files)
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

                    var objfiles = new Afiliacion();
                   

                    using (var target = new MemoryStream())
                    {
                        files.CopyTo(target);
                        objfiles.Codigo = 123;
                        objfiles.Fecha = DateTime.Now; //aquí esto se puede dejar por afuera para cuando sea el form oficial
                        objfiles.ComprobantePago = target.ToArray();
                    }
                  
                    _context.Afiliacions.Add(objfiles);
                    _context.SaveChanges();
                   
                    ViewBag.Image = ViewImage(objfiles.ComprobantePago);
                    
                }
            }
            return View();
        }
    }
}

