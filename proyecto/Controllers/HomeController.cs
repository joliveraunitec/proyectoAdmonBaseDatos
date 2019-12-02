using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using proyecto.Models;
using System.Web;

namespace proyecto.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        usersContext db = new usersContext();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            //var data = db.Usuarios;
            //return View(data);

            if (Request.Cookies["login"] != null)
            {
                return RedirectToAction("Dashboard");
            }

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
                ViewBag.mensaje = "usuario ya existe";
                ViewBag.registered = true;
                return View();
            } else
            {
                ViewBag.registered = false;
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

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            if (Request.Cookies["login"] != null)
            {
                foreach (var cookie in Request.Cookies.Keys)
                {
                    Response.Cookies.Delete(cookie);
                }
            }
            return RedirectToAction("Index");

        }

        public IActionResult Dashboard()
        {

            if (Request.Cookies["login"] == null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.usuario = HttpContext.Request.Cookies["login"];
            return View();
        }

        public IActionResult NuevaOrden()
        {
            if (Request.Cookies["login"] == null)
            {
                return RedirectToAction("Index");
            }

            List<Clientes> clientes = db.Clientes.ToList();
            ViewBag.clientes = clientes;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> NuevaOrden(int cliente, string titulo, string fechaEntrega, string descripccion)
        {


            var ordenes = db.Ordenes;

            var clientes = db.Clientes;
            Clientes clienteEncontrado = null;

            List<Clientes> clientesEncontrados = clientes.Where(c => c.Id == cliente).ToList();
            if (clientesEncontrados.Count < 1)
            {
                ViewBag.mensaje = "ese cliente no existe";
                ViewBag.error = true;
                return View();
            } else
            {
                clienteEncontrado = clientesEncontrados[0];
            }

            //chequear si ya existe
            List<Ordenes> encontrados = ordenes.Where(o => o.Cliente == clienteEncontrado.Id && o.Titulo == titulo).ToList();

            if (encontrados.Count > 0)
            {
                ViewBag.mensaje = "orden de trabajo ya existe, posible titulo duplicado";
                ViewBag.registered = true;
                return View();
            }
            else
            {
                ViewBag.registered = false;
                ViewBag.mensaje = "orden de trabajo creada con exito.";
                ordenes.Add(new Ordenes()
                {

                    Cliente = cliente,
                    Titulo = titulo,
                    FechaEntrega = DateTime.Parse(fechaEntrega),
                    Descripccion = descripccion,
                    Estado = "En Proceso"

                });
                await db.SaveChangesAsync();
            }


            List<Clientes> cs = db.Clientes.ToList();
            ViewBag.clientes = cs;

            return View();
        }

        public async Task<IActionResult> VerOrdenes()
        {

            if (Request.Cookies["login"] == null)
            {
                return RedirectToAction("Index");
            }

            var ordenes = db.Ordenes;
            List<Ordenes> ordenesEncontradas = ordenes.ToList();

            ViewBag.ordenes = ordenesEncontradas;


            return View();
        }

        [HttpPost]
        public async Task<IActionResult> VerOrdenes(string estado, int id)
        {

            var ordenes = db.Ordenes;
            List<Ordenes> ordenesEncontradas = ordenes.Where(o => o.Id == id).ToList();
            ordenesEncontradas[0].Estado = estado;

            if (ordenesEncontradas.Count > 0)
            {
                
                db.Update<Ordenes>(ordenesEncontradas[0]);
                ViewBag.error = false;
                ViewBag.mensaje = "orden actualizada correctamente.";

                db.SaveChanges();
            } else
            {
                ViewBag.error = true;
                ViewBag.mensaje = "no pudimos encontrar la orden bajo su id.";
            }

            ViewBag.ordenes = ordenes;

            return View();
        }

        public async Task<IActionResult> Clientes()
        {

            if (Request.Cookies["login"] == null)
            {
                return RedirectToAction("Index");
            }

            List<Clientes> clientes = db.Clientes.ToList();

            ViewBag.clientes = clientes;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Clientes(int id, string nombre, string rtn, string direccion, string telefono, string correo, bool agregar)
        {


            //return Content($"id: {id}, nombre: {nombre}, rtn: {rtn}, direccion: {direccion}, telefono: {telefono}, correo: {correo}, agregar: {agregar}");

            if (agregar)
            {

                db.Clientes.Add(new Clientes() { Id = id, Nombre = nombre, Rtn = rtn, Direccion = direccion, Telefono = telefono, Correo = correo });
                db.SaveChanges();

                ViewBag.mensaje = "usuario CREADO con exito.";
                ViewBag.error = false;
            } else {
                var clientes = db.Clientes;
                List<Clientes> encontrados = clientes.Where(e => e.Id == id).ToList();
                /*foreach(Clientes clienteActualizado in encontrados)
                {
                    if (clienteActualizado.Id == id)
                    {
                        clienteActualizado.Nombre = nombre;
                        clienteActualizado.Rtn = rtn;
                        clienteActualizado.Direccion = direccion;
                        clienteActualizado.Telefono = telefono;
                        clienteActualizado.Correo = correo;
                        db.Update<Clientes>(clienteActualizado);
                        db.SaveChanges();
                        break;
                    }
                }*/

                Clientes clienteActualizado = encontrados[0];
                clienteActualizado.Nombre = nombre;
                clienteActualizado.Rtn = rtn;
                clienteActualizado.Direccion = direccion;
                clienteActualizado.Telefono = telefono;
                clienteActualizado.Correo = correo;
                db.Update<Clientes>(clienteActualizado);
                db.SaveChanges();

                ViewBag.mensaje = "usuario ACTUALIADO con exito.";
                ViewBag.error = false;

                // return Content("ENCONTRADOS: " + encontrados.Count);

                /*Clientes clienteActualizado = encontrados.Where(c => c.Id == id).First();
                return Content("clienteActualizado: " + clienteActualizado.Nombre);*/

                //logger.LogInformation("ENCONTRADOS: " + encontrados.Count);

                /*Clientes clienteActualizado = encontrados;
                clienteActualizado.Nombre = nombre;
                clienteActualizado.Rtn = rtn;
                clienteActualizado.Direccion = direccion;
                clienteActualizado.Telefono = telefono;
                clienteActualizado.Correo = correo;
                db.Update<Clientes>(clienteActualizado);
                db.SaveChanges();*/
            }

            List<Clientes> cs = db.Clientes.ToList();
            ViewBag.clientes = cs;
            return View();
        }
        
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
