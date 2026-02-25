using Microsoft.AspNetCore.Mvc;
using AromasWeb.Abstracciones.ModeloUI;
using AromasWeb.Abstracciones.Logica.Cliente;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using AromasWeb.Helpers;

namespace AromasWeb.Controllers
{
    public class AuthController : Controller
    {
        private ICrearCliente _crearCliente;

        public AuthController()
        {
            _crearCliente = new LogicaDeNegocio.Clientes.CrearCliente();
        }

        // ============================================
        // LOGIN
        // ============================================

        // GET: Auth/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(Cliente cliente)
        {
            if (string.IsNullOrEmpty(cliente.Correo) || string.IsNullOrEmpty(cliente.Contrasena))
            {
                TempData["Error"] = "Por favor, ingresa tu correo y contraseña";
                return View(cliente);
            }

            // Login básico - verificamos el correo y contraseña
            if (cliente.Correo.ToLower() == "admin@gmail.com" && cliente.Contrasena == "admin123")
            {
                // Es administrador
                HttpContext.Session.SetString("UsuarioTipo", "admin");
                HttpContext.Session.SetString("UsuarioNombre", "Administrador");
                HttpContext.Session.SetString("UsuarioCorreo", cliente.Correo);

                TempData["Mensaje"] = "¡Bienvenido Administrador!";
                return RedirectToAction("Index", "Home");
            }
            else if (cliente.Correo.ToLower() == "empleado@gmail.com" && cliente.Contrasena == "empleado123")
            {
                // Es empleado
                HttpContext.Session.SetString("UsuarioTipo", "empleado");
                HttpContext.Session.SetString("UsuarioNombre", "Empleado");
                HttpContext.Session.SetString("UsuarioCorreo", cliente.Correo);
                HttpContext.Session.SetInt32("IdEmpleado", 1);

                TempData["Mensaje"] = "¡Bienvenido Empleado!";
                return RedirectToAction("Index", "Home");
            }
            else if (cliente.Correo.ToLower() == "cliente@gmail.com" && cliente.Contrasena == "cliente123")
            {
                // Es cliente
                HttpContext.Session.SetString("UsuarioTipo", "cliente");
                HttpContext.Session.SetString("UsuarioNombre", "Cliente");
                HttpContext.Session.SetString("UsuarioCorreo", cliente.Correo);

                TempData["Mensaje"] = "¡Bienvenido! Has iniciado sesión correctamente";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["Error"] = "Correo o contraseña incorrectos";
                return View(cliente);
            }
        }

        // ============================================
        // LOGOUT
        // ============================================

        public IActionResult CerrarSesion()
        {
            HttpContext.Session.Clear();
            TempData["Mensaje"] = "Has cerrado sesión correctamente";
            return RedirectToAction("Index", "Home");
        }

        // ============================================
        // REGISTRO
        // ============================================

        // GET: Auth/Registro
        public IActionResult Registro()
        {
            return View();
        }

        // POST: Auth/Registro
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registro(Cliente cliente, string ConfirmarContrasena)
        {
            try
            {
                // Remover validación de campos calculados
                ModelState.Remove("EstadoTexto");
                ModelState.Remove("FechaRegistroFormateada");
                ModelState.Remove("UltimaReservaFormateada");
                ModelState.Remove("DiasDesdeUltimaReserva");
                ModelState.Remove("EsClienteFrecuente");
                ModelState.Remove("UltimaReserva");

                // Validación robusta de contraseña
                if (string.IsNullOrWhiteSpace(cliente.Contrasena))
                {
                    ModelState.AddModelError("Contrasena", "La contraseña es requerida");
                }
                else
                {
                    if (!PasswordValidator.EsContrasenaValida(cliente.Contrasena, out string mensajeError))
                    {
                        ModelState.AddModelError("Contrasena", mensajeError);
                    }
                }

                if (string.IsNullOrWhiteSpace(ConfirmarContrasena))
                {
                    ModelState.AddModelError("ConfirmarContrasena", "Debes confirmar la contraseña");
                }
                else if (cliente.Contrasena != ConfirmarContrasena)
                {
                    ModelState.AddModelError("ConfirmarContrasena", "Las contraseñas no coinciden");
                }

                if (!ModelState.IsValid)
                {
                    TempData["Error"] = "Por favor, corrige los errores del formulario";
                    return View(cliente);
                }

                cliente.Estado = true;
                cliente.FechaRegistro = DateTime.Now;

                int resultado = await _crearCliente.Crear(cliente);

                if (resultado > 0)
                {
                    TempData["Mensaje"] = "¡Registro exitoso! Ya puedes iniciar sesión con tu cuenta";
                    return RedirectToAction(nameof(Login));
                }
                else
                {
                    TempData["Error"] = "No se pudo completar el registro. Intenta nuevamente.";
                    return View(cliente);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en registro: {ex.Message}");

                if (ex.Message.Contains("identificación"))
                {
                    TempData["Error"] = "Ya existe un cliente registrado con esa identificación";
                }
                else if (ex.Message.Contains("correo"))
                {
                    TempData["Error"] = "Ya existe un cliente registrado con ese correo electrónico";
                }
                else
                {
                    TempData["Error"] = $"Error al registrar: {ex.Message}";
                }

                return View(cliente);
            }
        }

        // ============================================
        // OLVIDÉ CONTRASEÑA
        // ============================================

        public IActionResult OlvideContrasenna()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult OlvideContrasenna(string correo)
        {
            if (string.IsNullOrEmpty(correo))
            {
                TempData["Error"] = "Por favor, ingresa tu correo electrónico";
                return View();
            }

            TempData["Mensaje"] = "Te hemos enviado un correo con las instrucciones para recuperar tu contraseña. Revisa tu bandeja de entrada y spam.";
            return View();
        }

        public IActionResult RestablecerContrasenna(string token = null)
        {
            ViewBag.Token = token;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RestablecerContrasenna(string token, string nuevaContrasena, string confirmarNuevaContrasena)
        {
            if (string.IsNullOrEmpty(token))
            {
                TempData["Error"] = "Token inválido";
                return RedirectToAction(nameof(OlvideContrasenna));
            }

            if (nuevaContrasena != confirmarNuevaContrasena)
            {
                TempData["Error"] = "Las contraseñas no coinciden";
                ViewBag.Token = token;
                return View();
            }

            if (nuevaContrasena.Length < 8)
            {
                TempData["Error"] = "La contraseña debe tener al menos 8 caracteres";
                ViewBag.Token = token;
                return View();
            }

            TempData["Mensaje"] = "¡Contraseña restablecida exitosamente! Ya puedes iniciar sesión";
            return RedirectToAction(nameof(Login));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["Mensaje"] = "Has cerrado sesión correctamente";
            return RedirectToAction("Index", "Home");
        }
    }
}