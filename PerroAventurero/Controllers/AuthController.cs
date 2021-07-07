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

namespace PerroAventurero.Controllers
{
    public class AuthController : Controller
    {
        private readonly PAContext _context;

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
                return RedirectToAction("ModifyPassword", "Auth");
            }
            catch (Exception ex)
            {
                // TODO: handle exception
                throw new InvalidOperationException(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SignIn([Bind("CedulaCliente, Foto, Descripcion, Contrasenna")] UsuarioComun usuario, [Bind("CedulaCliente, NombreCompleto, FechaNacimiento, Genero, Telefono, Correo")] Cliente usuarioCliente)
        {
            //VALIDAR QUE LA CEDULA NO EXISTA!!
            // NOOOOOOO ME SIRVE!!!!!!!!!!!!!!!!!!!! VIENEN NULL
            if (ModelState.IsValid)
            {
                //empresasAfiliada.Cedula = session;
                _context.Add(usuario);
                _context.Add(usuarioCliente);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "EmpresasAfiliadas");
            }
            return View(usuario);
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
            var normal = CreateComun(usuarioComun);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, normal);

            return RedirectToAction("Create", "EmpresasAfiliadas");
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
