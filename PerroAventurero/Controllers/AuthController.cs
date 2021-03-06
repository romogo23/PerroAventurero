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
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.EntityFrameworkCore;
using MailKit;
using MimeKit;
using MailKit.Net.Smtp;
namespace PerroAventurero.Controllers
{
    public class AuthController : Controller
    {
        private readonly PAContext _context;

        private static string emailModify;

        private static Cliente_UsuarioComun Client_Comun_User = new Cliente_UsuarioComun();

        private static Cliente_UsuarioComun userComun;

        private static UsuarioAdministrador userAdmin;

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

        public ActionResult ModifyPass(string NewPassword, string ConfirmPassword)
        {
            if (NewPassword.Equals(ConfirmPassword)) {

                if (userAdmin != null)
                {
                    userAdmin.Contrasenna = Encriptar(NewPassword);
                    userAdmin.CodigoTemporal = null;
                    _context.Update(userAdmin);
                    _context.SaveChanges();
                }
                else if (userComun != null)
                {
                    userComun.UsuarioComun.Contrasenna = Encriptar(NewPassword);
                    userComun.UsuarioComun.CodigoTemporal = null;
                    _context.Update(userComun.UsuarioComun);
                    _context.SaveChanges();
                }
                ViewBag.r = "Su contrase" + '\u00F1' + "a se actualiz"+ '\u00f3' + " correctamente";
                return View("ModifyPassword_4");

            }
            else
            {
                ModelState.AddModelError("UsuarioComun.Contrasenna", "Las contraseñas no coinciden");
                return View("ModifyPassword_4");
            }
        }

        public ActionResult ValidateCode(short Code)
        {

            if (emailModify != null)
            {
                if (userAdmin != null)
                {
                    if (userAdmin.CodigoTemporal == Code)
                    {
                        return RedirectToAction("ModifyPassword_4", "Auth");

                    }
                    else {
                        ModelState.AddModelError("UsuarioComun.CodigoTemporal", "Código incorrecto");
                        return View("ModifyPassword_3");
                    }

                }
                else if (userComun != null)
                {
                    if (userComun.UsuarioComun.CodigoTemporal == Code)
                    {
                        return RedirectToAction("ModifyPassword_4", "Auth");

                    }
                    else {
                        ModelState.AddModelError("UsuarioComun.CodigoTemporal", "Código incorrecto");
                        return View("ModifyPassword_3");
                    }


                }
                return View("Login");


            }
            else {
                return View("Login");
            }
        }


        public ActionResult ValidateSendCode()
        {
            if (emailModify != null)
            {

                short codeMod = generateCode();
                if (userAdmin != null)
                {
                    userAdmin.CodigoTemporal = codeMod;
                    _context.Update(userAdmin);
                }
                else if (userComun != null)
                {
                    userComun.UsuarioComun.CodigoTemporal = codeMod;
                    _context.Update(userComun.UsuarioComun);
                    _context.SaveChanges();
                }
                string message = "Su contraseña temporal es: " + codeMod;
                string subject = "Cambio de contraseña Perro Aventurero";
                SendMessage(subject, message, emailModify);
                return RedirectToAction("ModifyPassword_3", "Auth");
            }
            else
            {
                return View("Login");
            }
        }



      
        private short generateCode()
        {
            return (short)new Random().Next(1000, 9999);
        }

        public ActionResult ValidateEmail(string correo)
        {
            if (correo != null)
            {
                Cliente_UsuarioComun cliente = null;
                UsuarioAdministrador admin = null;
                cliente = EmailNUser(correo);
                admin = EmailAdminUser(correo);

                if (cliente != null || admin != null)
                {
                    emailModify = correo;
                    userComun= cliente;
                    userAdmin = admin;
                   return RedirectToAction("ModifyPassword_2", "Auth");

                }else
                {
                    ModelState.AddModelError("Correo", "Correo inválido");
                    return View("ModifyPassword");
                }
                
            }
            else {
                ModelState.AddModelError("Correo", "Correo inválido");
                return View("ModifyPassword");
            }
           
        }




        private UsuarioAdministrador EmailAdminUser(String email)
        {
            UsuarioAdministrador usuarioAdmin = null;
            usuarioAdmin = _context.UsuarioAdministradors.Where(u => u.Correo == email).FirstOrDefault();
            return usuarioAdmin;
        }

        private Cliente_UsuarioComun EmailNUser(String email)
        {
            Cliente cliente = null;
            UsuarioComun usuarioC = null;
            Cliente_UsuarioComun c_U = null;
            cliente = _context.Clientes.Where(u => u.Correo == email).FirstOrDefault();
            if (cliente != null) {
                usuarioC = _context.UsuarioComuns.Where(u => u.CedulaCliente.Equals(cliente.CedulaCliente)).FirstOrDefault();
                if (usuarioC != null)
                {
                    c_U = new Cliente_UsuarioComun();
                    c_U.UsuarioComun = usuarioC;
                    c_U.Cliente = cliente;
                }
            }
            return c_U;
        }



        public void SendMessage(string subject, string messageBody, string emailClient)
        {
            MimeMessage message = new MimeMessage();

            message.From.Add(new MailboxAddress("Perro Aventurero", "perroaventurero.info@gmail.com"));
            message.To.Add(MailboxAddress.Parse(emailClient));
            message.Subject = subject;

            message.Body = new TextPart("plain")
            {
                Text = messageBody,
            };

            SmtpClient client = new SmtpClient();


            string email = "perroaventurero.info@gmail.com";
            string password = "paonline08";

            try
            {

                client.Connect("smtp.gmail.com", 465, true);
                client.Authenticate(email, password);
                client.Send(message);

            }
            catch (Exception ex)
            {


            }
            finally
            {

                client.Disconnect(true);
                client.Dispose();
            }
        }

        [HttpPost]
        public async Task<IActionResult> VerifyCodeRegister(int code)
        {
            if (code > 0) {
                if (Client_Comun_User.UsuarioComun.CodigoTemporal == code)
                {

                    if (ValidateClient(Client_Comun_User.Cliente.CedulaCliente) == 0)
                    {

                        _context.Add(Client_Comun_User.Cliente);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        _context.Update(Client_Comun_User.Cliente);
                        await _context.SaveChangesAsync();
                    }
                    Client_Comun_User.UsuarioComun.CodigoTemporal = 0;
                    string conTempo = Client_Comun_User.UsuarioComun.Contrasenna;
                    Client_Comun_User.UsuarioComun.Contrasenna = Encriptar(conTempo);
                    _context.Add(Client_Comun_User.UsuarioComun);
                    await _context.SaveChangesAsync();
                    ViewBag.r = "Se ha registrado correctamente";
                    return View("RegistroValidacion");
                }
                else {
                    ViewBag.r = "El código es incorrecto";
                    return View("RegistroValidacion");
                }
            } else {
                ViewBag.r = "Debe ingresar un código";
                return View("RegistroValidacion");
            }
           
        }


        [HttpPost]
        public async Task<IActionResult> SignIn([Bind("UsuarioComun", "Cliente")] Cliente_UsuarioComun usuario, IFormFile files)
        {
            

            if (!TryValidateModel(usuario, nameof(Cliente_UsuarioComun)))
            {
                usuario.UsuarioComun.CedulaCliente = usuario.Cliente.CedulaCliente;


               if (ValidateUser(usuario.Cliente.CedulaCliente) == 0) {

                    if (ValidateEmailClient(usuario.Cliente.Correo) == 0)
                    {
                        //save the client in the local atribute
                        Client_Comun_User.Cliente = usuario.Cliente;

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

                                    usuario.UsuarioComun.Foto = target.ToArray();
                                }

                            }
                        }

                        usuario.UsuarioComun.CodigoTemporal = generateCode();
                        string message = "Hola "+ usuario.Cliente.NombreCompleto + " su código de verificación para la cuenta perro aventurero es: " + usuario.UsuarioComun.CodigoTemporal;
                        string subject = "Código de verificación Perro Aventurero";
                        SendMessage(subject, message, usuario.Cliente.Correo);
                        Client_Comun_User.UsuarioComun = usuario.UsuarioComun;
                        return View("RegistroValidacion");

                    }
                    else {
                        ModelState.AddModelError("Cliente.Correo", "Ya existe un usuario con el correo");
                        return View(usuario);
                        
                    }

                    
                }
                else
                {
                    ModelState.AddModelError("Cliente.CedulaCliente", "Ya existe un usuario con la cédula ingresada");
                    return View(usuario);
                }   
                

            }
            return View(usuario);
        }

        private int ValidateClient(String id)
        {
            
            int cliente = _context.Clientes.Where(c => c.CedulaCliente == id).Count();
            return cliente;

        }

        

        private int ValidateEmailClient(String email)
        {

            Cliente cliente = _context.Clientes.AsNoTracking().Where(c => c.Correo == email).FirstOrDefault();
            if (cliente != null)
            {
                int userC = _context.UsuarioComuns.Where(c => c.CedulaCliente == cliente.CedulaCliente).Count();
                return userC;

            }
            
            int user = _context.UsuarioAdministradors.Where(c => c.Correo == email).Count();
            if (user >= 1) {
                return user;
            }
            
            return 0;
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
            if (usuarioComun != null)
            {
                var normal = CreateComun(usuarioComun);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, normal);

                return Redirect("~/Home/Index");
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
            string conTempo = Encriptar(password);
            UsuarioAdministrador usuarioAdmin = null;
            usuarioAdmin = _context.UsuarioAdministradors.Where(u => u.Correo == email && u.Contrasenna == conTempo).FirstOrDefault();
            return usuarioAdmin;
        }
        
        private Cliente_UsuarioComun GetMyCOMUNUser (String email, String password)
        {
            Cliente_UsuarioComun ret = null;
            UsuarioComun usuarioComun = null;
            Cliente cliente = null;
            cliente = _context.Clientes.Where(u => u.Correo == email).FirstOrDefault();
            if (cliente != null) {
                string conTempo = Encriptar(password);

                usuarioComun = _context.UsuarioComuns.Where(u => u.CedulaCliente.Equals(cliente.CedulaCliente) && u.Contrasenna == conTempo).FirstOrDefault();
                if (usuarioComun != null)
                {
                    if (cliente.CedulaCliente.Equals(usuarioComun.CedulaCliente))
                    {
                        ret = new Cliente_UsuarioComun();
                        ret.Cliente = cliente;
                        ret.UsuarioComun = usuarioComun;
                        return ret;
                    }
                }
            }
            return null;
        }

        private ClaimsPrincipal CreatePrincipal(UsuarioAdministrador userAdmin)
        {
            var claims = new List<Claim>
            {
                /*new Claim("Correo", userAdmin.Correo),
                new Claim("Contrasenna", userAdmin.Contrasenna)*/
                new Claim(ClaimTypes.Name, userAdmin.Cedula),
                new Claim("FullName", userAdmin.NombreCompleto),
                 new Claim("FullName", userAdmin.NombreCompleto),
                new Claim("Email", userAdmin.Correo),
                new Claim("Tel", userAdmin.Telefono.ToString()),
                new Claim("Fecha", userAdmin.FechaNacimiento.ToString()),
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
                new Claim("Email", userComun.Cliente.Correo),
                new Claim("Tel", userComun.Cliente.Telefono.ToString()),
                new Claim("Fecha", userComun.Cliente.FechaNacimiento.ToString()),
                new Claim(ClaimTypes.Role, "Normal"),
            };
            var principal = new ClaimsPrincipal();
            principal.AddIdentity(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
            return principal;
        }




        public static string Encriptar(string _cadenaAencriptar)
        {
            string result = string.Empty;
            byte[] encryted = System.Text.Encoding.Unicode.GetBytes(_cadenaAencriptar);
            result = Convert.ToBase64String(encryted);
            return result;
        }


    }
}
