using Microsoft.AspNetCore.Mvc;
using PerroAventurero.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace PerroAventurero.Controllers
{
    public class AuthController : Controller
    {
        private readonly PAContext _context;

        private static string emailModify;

        private static string codeMod;

        public AuthController(PAContext context)
        {
            _context = context;
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult SignIn()
        {
            return View();
        }

        public ActionResult Mamo()
        {
            return View();
        }

        public ActionResult ModifyPassword()
        {
            return View();
        }

        public ActionResult ModifyPassword_2()
        {
            ViewData["Email"] = emailModify;
            return View();
        }

        public ActionResult ModifyPassword_3()
        {
            return View();
        }

        public ActionResult ModifyPassword_4()
        {
            ViewData["Code"] = codeMod;
            return View();
        }

        public ActionResult ModifyPass(string NewPassword)
        {
            return RedirectToAction("Login", "Auth");
        }

        public ActionResult ValidateCode(string Code)
        {
            codeMod = Code;
            return RedirectToAction("ModifyPassword_4", "Auth");
        }

        public ActionResult ValidateEmail(string correo)
        {
            emailModify = correo;
            return RedirectToAction("ModifyPassword_2", "Auth");
        }

        [HttpPost]
        public ActionResult SendCode(string Correo)
        {
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
                msg.To.Add(Correo.ToString());
                msg.From = new MailAddress("juanperez33op@gmail.com");
                msg.Subject = "Prueba de correo";
                msg.Body = "Prueba de correo";
                //Attachment data = new Attachment(textBox3.Text);
                //msg.Attachments.Add(data);
                client.Send(msg);
                return RedirectToAction("ModifyPassword_3", "Auth");
            }
            catch (Exception ex)
            {
                // TODO: handle exception
                throw new InvalidOperationException(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SignIn([Bind("CedulaCliente, Foto, Descripcion, Contrasenna")] UsuarioComun usuario, [Bind("CedulaCliente, NombreCompleto, FechaNacimiento, Genero, Telefono, Correo")] Cliente usuarioCliente, IFormFile files)
        {
            if (ModelState.IsValid)
            {
                if (ValidateClient(usuario.CedulaCliente) == 0)
                {
                    if (files != null)
                    {
                        if (files.Length > 0)
                        {
                            var fileName = Path.GetFileName(files.FileName);
                            //Getting file Extension
                            var fileExtension = Path.GetExtension(fileName);
                            // concatenating  FileName + FileExtension
                            var newFileName = String.Concat(Convert.ToString(Guid.NewGuid()), fileExtension);

                            using (var target = new MemoryStream())
                            {
                                files.CopyTo(target);

                                usuario.Foto = target.ToArray();
                            }

                        }
                    }
                    _context.Add(usuarioCliente);
                    _context.Add(usuario);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    ModelState.AddModelError("UsuarioComun.CedulaCliente", "Ya existe un usuario con esa cédula ingresada");
                }
                
                return RedirectToAction("Index", "EmpresasAfiliadas");

            }
            return View(usuario);
        }

        private int ValidateClient(String id)
        {
            
            int cliente = _context.Clientes.Where(c => c.CedulaCliente == id).Count();
            return cliente;

        }

        private int ValidateUser(String id)
        {

            int cliente = _context.UsuarioComuns.Where(c => c.CedulaCliente == id).Count();
            return cliente;

        }

        /*[HttpPost]
        public ActionResult Login(UsuarioAdministrador usuarioAdmin, string ReturnUrl)
        {
            if(IsValid(usuarioAdmin))
            {
                FormsAuthentication.SetAuthCookie(usuarioAdmin.Correo, false);
                if(ReturnUrl != null)
                {
                    return Redirect(ReturnUrl);
                }
                return RedirectToAction("Index", "Home");
            }
            return View(usuarioAdmin);
        }

        private bool IsValid(UsuarioAdministrador usuarioAdmin)
        {
            if(usuarioAdmin.Correo == "usuarioA@gmail.com" && usuarioAdmin.Contrasenna == "admin")
            {
                return true;
            }
            return false;
        }*/

        [HttpPost]
        public async Task<IActionResult> Login(string Correo, string Contrasenna)
        {
            var usuarioAdmin = GetMyADMINUser(Correo, Contrasenna);
            var usuarioComun = GetMyCOMUNUser(Correo, Contrasenna);
            // Todo: Check for no user with these credentials

            if (usuarioAdmin != null)
            {
                /*
                return RedirectToAction("Index", "Home");^*/

                var principal = CreatePrincipal(usuarioAdmin);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return RedirectToAction("Index", "EmpresasAfiliadas");
            }
            if (usuarioComun.Cliente != null)
            {
                var normal = CreateComun(usuarioComun);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, normal);

                return RedirectToAction("Create", "EmpresasAfiliadas");
            }
            ModelState.AddModelError("correo", "Correo o contraseña incorrecta");
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        private UsuarioAdministrador GetMyADMINUser (String email, String password)
        {
            UsuarioAdministrador usuarioAdmin = null;
            usuarioAdmin = _context.UsuarioAdministradors.Where(u => u.Correo == email && u.Contrasenna == password).FirstOrDefault();
            return usuarioAdmin;
        }
        
        private Cliente_UsuarioComun GetMyCOMUNUser (String email, String password)
        {
            Cliente_UsuarioComun ret = null;
            UsuarioComun usuarioComun = null;
            Cliente cliente = null;
            usuarioComun = _context.UsuarioComuns.Where(u => u.Contrasenna == password).FirstOrDefault();
            cliente = _context.Clientes.Where(u => u.Correo == email).FirstOrDefault();

            ret = new Cliente_UsuarioComun(usuarioComun, cliente);

            return ret;
        }

        private ClaimsPrincipal CreatePrincipal(UsuarioAdministrador userAdmin)
        {
            var claims = new List<Claim>
            {
                /*new Claim("Correo", userAdmin.Correo),
                new Claim("Contrasenna", userAdmin.Contrasenna)*/
                new Claim(ClaimTypes.Name, userAdmin.Cedula),
                new Claim("FullName", userAdmin.NombreCompleto),
                new Claim(ClaimTypes.Role, "Administrator"),
            };
            var principal = new ClaimsPrincipal();
            principal.AddIdentity(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
            return principal;
        }

        private ClaimsPrincipal CreateComun(Cliente_UsuarioComun userComun)
        {
            var claims = new List<Claim>
            {
                /*new Claim("Correo", userAdmin.Correo),
                new Claim("Contrasenna", userAdmin.Contrasenna)*/
                new Claim(ClaimTypes.Name, userComun.UsuarioComun.CedulaCliente),
                new Claim("FullName", userComun.Cliente.NombreCompleto),
                new Claim(ClaimTypes.Role, "Normal"),
            };
            var principal = new ClaimsPrincipal();
            principal.AddIdentity(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
            return principal;
        }
    }
}
