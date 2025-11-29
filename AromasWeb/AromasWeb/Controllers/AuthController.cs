using Microsoft.AspNetCore.Mvc;
using AromasWeb.Abstracciones.ModeloUI;
using System;

namespace AromasWeb.Controllers
{
    public class AuthController : Controller
    {
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

            // Login básico - solo verificamos el correo
            if (cliente.Correo.ToLower() == "admin@gmail.com" && cliente.Contrasena == "admin123")
            {
                // Es administrador
                HttpContext.Session.SetString("UsuarioTipo", "admin");
                HttpContext.Session.SetString("UsuarioNombre", "Administrador");
                HttpContext.Session.SetString("UsuarioCorreo", cliente.Correo);

                TempData["Mensaje"] = "¡Bienvenido Administrador!";
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

        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Registro(Cliente cliente, string ConfirmarContrasena)
        {
            if (ModelState.IsValid)
            {
                if (cliente.Contrasena != ConfirmarContrasena)
                {
                    TempData["Error"] = "Las contraseñas no coinciden";
                    return View(cliente);
                }

                if (cliente.Contrasena.Length < 8)
                {
                    TempData["Error"] = "La contraseña debe tener al menos 8 caracteres";
                    return View(cliente);
                }

                cliente.Estado = true;
                cliente.FechaRegistro = DateTime.Now;

                TempData["Mensaje"] = "¡Registro exitoso! Ya puedes iniciar sesión";
                return RedirectToAction(nameof(Login));
            }

            return View(cliente);
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