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

        [HttpPost]
        public async Task<IActionResult> SignIn([Bind("UsuarioComun.CedulaCliente, UsuarioComun.Descripcion, UsuarioComun.Contrasenna, UsuarioComun.Foto")] UsuarioComun usuario, [Bind("Cliente.NombreCompleto, Cliente.FechaNacimiento, Cliente.Genero, Cliente.Telefono, Cliente.Correo")] Cliente usuarioCliente)
        {
            //VALIDAR QUE LA CEDULA NO EXISTA!!
            // NOOOOOOO ME SIRVE!!!!!!!!!!!!!!!!!!!! VIENEN NULL
            if (ModelState.IsValid)
            {
                //empresasAfiliada.Cedula = session;
                usuario.CedulaCliente = usuarioCliente.CedulaCliente;
                _context.Add(usuario);
                await _context.SaveChangesAsync();
                _context.Add(usuarioCliente);
                await _context.SaveChangesAsync();
                return RedirectToAction("Auth", "Login");
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
        
        private UsuarioComun GetMyCOMUNUser (String email, String password)
        {
            UsuarioComun usuarioComun = null;
            usuarioComun = _context.UsuarioComuns.Where(u => u.Correo == email && u.Contrasenna == password).FirstOrDefault();
            return usuarioComun;
        }

        private ClaimsPrincipal CreatePrincipal(UsuarioAdministrador userAdmin)
        {
            var claims = new List<Claim>
            {
                /*new Claim("Correo", userAdmin.Correo),
                new Claim("Contrasenna", userAdmin.Contrasenna)*/
                new Claim(ClaimTypes.Name, userAdmin.Correo),
                new Claim("FullName", userAdmin.NombreCompleto),
                new Claim(ClaimTypes.Role, "Administrator"),
            };
            var principal = new ClaimsPrincipal();
            principal.AddIdentity(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
            return principal;
        }

        private ClaimsPrincipal CreateComun(UsuarioComun userComun)
        {
            var claims = new List<Claim>
            {
                /*new Claim("Correo", userAdmin.Correo),
                new Claim("Contrasenna", userAdmin.Contrasenna)*/
                new Claim(ClaimTypes.Name, userComun.Correo),
                new Claim("FullName", userComun.CedulaCliente),
                new Claim(ClaimTypes.Role, "Normal"),
            };
            var principal = new ClaimsPrincipal();
            principal.AddIdentity(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
            return principal;
        }
    }
}
