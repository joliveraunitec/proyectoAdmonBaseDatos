using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using proyecto.Models;

namespace proyecto.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        usersContext db = new usersContext();
        Usuarios lastUser;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            //var data = db.Usuarios;
            //return View(data);

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string usuario, string clave)
        {
            //db = new usersContext();
            List<Usuarios> user = db.Usuarios.Where(u => u.Usuario == usuario /*&& u.Clave == clave*/).ToList();

            if (user.Count < 1)
            {
                ViewBag.mensaje = "usuario no encontrado";
                return View();
            }

            if (user[0].Clave == clave) {
                ViewBag.mensaje = "clave correcta";
                ViewBag.auth = true;
                //view , controller
                //return RedirectToAction("Dashboard", "Home", new {usuario=usuario });

                //lastUser = user[0];
                HttpContext.Response.Cookies.Append("login", user[0].Usuario);
                return RedirectToAction("Dashboard");
            } else
            {
                ViewBag.mensaje = "clave INCORRECTA";
                ViewBag.auth = false;
            }


            /*var data = db.Usuarios;

            return View(data);*/

            //ViewBag.usuario = usuario;
            //ViewBag.clave = clave;
            //return Content($"Hello {usuario}");

            return View();
        }

        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Register(string usuario, string clave, string correo)
        {

            //db = new usersContext();

            var usuarios = db.Usuarios;

            //chequear si ya existe
            List<Usuarios> encontrados = usuarios.Where(u => u.Usuario == usuario).ToList();
                
             if (encontrados.Count > 0) {
                ViewBag.error = true;
                ViewBag.mensaje = "usuario ya existe";
                return View();
            } else
            {
                ViewBag.error = false;
                ViewBag.mensaje = "usuario creado con exito.";
                usuarios.Add(new Usuarios()
                {

                    Usuario = usuario,
                    Clave = clave,
                    Correo = correo

                });
                await db.SaveChangesAsync();
            }



            return View();
        }

        public IActionResult Dashboard()
        {
            ViewBag.usuario = HttpContext.Request.Cookies["login"];
            return View();
        }

        /*
         [HttpGet]
        public IActionResult Dashboard(string usuario)
        {
            return Content($"dashboard GET: {usuario}");
        }*/

        /*[HttpPost]
        public IActionResult Dashboard()
        {
            return View();
            //return View();
        }*/

        public IActionResult Privacy()
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
