using AromasWeb.Abstracciones.Logica.Cliente;
using AromasWeb.Abstracciones.ModeloUI;
using AromasWeb.Abstracciones.Servicios;
using AromasWeb.LogicaDeNegocio.Bitacoras;
using AromasWeb.AccesoADatos.Modulos;
using AromasWeb.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace AromasWeb.Controllers
{
    public class AuthController : Controller
    {
        private ICrearCliente _crearCliente;
        private IBuscarClientePorCorreo _buscarClientePorCorreo;
        private IActualizarCliente _actualizarCliente;
        private readonly IEmailService _emailService;
        private readonly CrearBitacora _crearBitacora;

        public AuthController(IEmailService emailService)
        {
            _crearCliente = new LogicaDeNegocio.Clientes.CrearCliente();
            _buscarClientePorCorreo = new LogicaDeNegocio.Clientes.BuscarClientePorCorreo();
            _actualizarCliente = new LogicaDeNegocio.Clientes.ActualizarCliente();
            _emailService = emailService;
            _crearBitacora = new CrearBitacora();
        }

        // ============================================
        // HELPERS DE SESIÓN
        // ============================================

        // Devuelve el IdEmpleado de la sesión activa, o null si no hay sesión válida.
        private int? ObtenerIdEmpleadoSesion()
        {
            int? id = HttpContext.Session.GetInt32("IdEmpleado");
            return (id.HasValue && id.Value > 0) ? id : (int?)null;
        }

        // Guarda en sesión la lista de IDs de permisos del rol.
        private void CargarPermisosEnSesion(int idRol)
        {
            try
            {
                using (var contexto = new AccesoADatos.Contexto())
                {
                    // IDs que tiene el rol
                    var idsPermisos = contexto.RolPermiso
                        .Where(rp => rp.IdRol == idRol)
                        .Select(rp => rp.IdPermiso)
                        .ToList();

                    HttpContext.Session.SetString("PermisosRol", JsonSerializer.Serialize(idsPermisos));

                    // mapa completo nombre → id de TODOS los permisos
                    var mapaPermisos = contexto.Permiso
                        .Select(p => new { p.Nombre, p.IdPermiso })
                        .ToDictionary(p => p.Nombre, p => p.IdPermiso);

                    HttpContext.Session.SetString("MapaPermisos", JsonSerializer.Serialize(mapaPermisos));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AUTH] Error al cargar permisos: {ex.Message}");
                HttpContext.Session.SetString("PermisosRol", "[]");
                HttpContext.Session.SetString("MapaPermisos", "{}");
            }
        }

        // ============================================
        // LOGIN
        // ============================================

        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(Cliente cliente)
        {
            if (string.IsNullOrEmpty(cliente.Correo) || string.IsNullOrEmpty(cliente.Contrasena))
            {
                TempData["Error"] = "Por favor, ingresa tu correo y contraseña";
                return View(cliente);
            }

            try
            {
                using (var contexto = new AccesoADatos.Contexto())
                {
                    var empleadoAD = contexto.Empleado
                        .FirstOrDefault(e => e.Correo.ToLower() == cliente.Correo.ToLower().Trim());

                    if (empleadoAD != null)
                    {
                        if (!empleadoAD.Estado)
                        {
                            TempData["Error"] = "Tu cuenta de empleado está inactiva. Contacta al administrador.";
                            return View(cliente);
                        }

                        if (empleadoAD.Contrasena != cliente.Contrasena)
                        {
                            TempData["Error"] = "Correo o contraseña incorrectos";
                            return View(cliente);
                        }

                        string tipoUsuario = empleadoAD.IdRol == 1 ? "admin" : "empleado";

                        string nombreRol = "Empleado";
                        try
                        {
                            var rol = contexto.Rol.FirstOrDefault(r => r.IdRol == empleadoAD.IdRol);
                            if (rol != null) nombreRol = rol.Nombre;
                        }
                        catch { }

                        HttpContext.Session.SetString("UsuarioTipo", tipoUsuario);
                        HttpContext.Session.SetString("UsuarioNombre", $"{empleadoAD.Nombre} {empleadoAD.Apellidos}");
                        HttpContext.Session.SetString("UsuarioCorreo", empleadoAD.Correo);
                        HttpContext.Session.SetInt32("IdEmpleado", empleadoAD.IdEmpleado);
                        HttpContext.Session.SetInt32("IdRol", empleadoAD.IdRol);
                        HttpContext.Session.SetString("NombreRol", nombreRol);

                        CargarPermisosEnSesion(empleadoAD.IdRol);

                        _crearBitacora.RegistrarAccion(
                            idEmpleado: empleadoAD.IdEmpleado,
                            idModulo: ObtenerModulo.ObtenerIdPorNombre("Autenticación"),
                            accion: Bitacora.Acciones.Login,
                            tablaAfectada: "Empleado",
                            descripcion: $"Inicio de sesión ({empleadoAD.Correo}) — Rol: {nombreRol}"
                        );

                        TempData["Mensaje"] = $"¡Bienvenido {empleadoAD.Nombre}!";
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AUTH LOGIN] {ex.Message}");
            }

            // Buscar como Cliente
            var clienteBD = _buscarClientePorCorreo.ObtenerPorCorreo(cliente.Correo);

            if (clienteBD != null)
            {
                if (!clienteBD.Estado)
                {
                    TempData["Error"] = "Debes verificar tu correo antes de iniciar sesión.";
                    return View(cliente);
                }

                if (clienteBD.Contrasena == cliente.Contrasena)
                {
                    HttpContext.Session.SetString("UsuarioTipo", "cliente");
                    HttpContext.Session.SetString("UsuarioNombre", $"{clienteBD.Nombre} {clienteBD.Apellidos}");
                    HttpContext.Session.SetString("UsuarioCorreo", clienteBD.Correo);
                    HttpContext.Session.SetInt32("IdCliente", clienteBD.IdCliente);

                    TempData["Mensaje"] = $"¡Bienvenido {clienteBD.Nombre}!";
                    return RedirectToAction("Index", "Home");
                }

                TempData["Error"] = "Correo o contraseña incorrectos";
                return View(cliente);
            }

            TempData["Error"] = "Correo o contraseña incorrectos";
            return View(cliente);
        }

        // ============================================
        // LOGOUT
        // ============================================

        public IActionResult CerrarSesion()
        {
            RegistrarLogout();
            HttpContext.Session.Clear();
            TempData["Mensaje"] = "Has cerrado sesión correctamente";
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            RegistrarLogout();
            HttpContext.Session.Clear();
            TempData["Mensaje"] = "Has cerrado sesión correctamente";
            return RedirectToAction("Index", "Home");
        }

        private void RegistrarLogout()
        {
            string tipo = HttpContext.Session.GetString("UsuarioTipo");
            if (tipo != "admin" && tipo != "empleado") return;

            int? idEmp = ObtenerIdEmpleadoSesion();
            if (!idEmp.HasValue) return;

            string correo = HttpContext.Session.GetString("UsuarioCorreo");
            _crearBitacora.RegistrarAccion(
                idEmpleado: idEmp.Value,
                idModulo: ObtenerModulo.ObtenerIdPorNombre("Autenticación"),
                accion: Bitacora.Acciones.Logout,
                tablaAfectada: "Empleado",
                descripcion: $"Cierre de sesión ({correo})"
            );
        }

        // ============================================
        // REGISTRO
        // ============================================

        public IActionResult Registro() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registro(Cliente cliente, string ConfirmarContrasena)
        {
            try
            {
                ModelState.Remove("EstadoTexto");
                ModelState.Remove("FechaRegistroFormateada");
                ModelState.Remove("UltimaReservaFormateada");
                ModelState.Remove("DiasDesdeUltimaReserva");
                ModelState.Remove("EsClienteFrecuente");
                ModelState.Remove("UltimaReserva");

                if (string.IsNullOrWhiteSpace(cliente.Contrasena))
                    ModelState.AddModelError("Contrasena", "La contraseña es requerida");
                else if (!PasswordValidator.EsContrasenaValida(cliente.Contrasena, out string msgPass))
                    ModelState.AddModelError("Contrasena", msgPass);

                if (string.IsNullOrWhiteSpace(ConfirmarContrasena))
                    ModelState.AddModelError("ConfirmarContrasena", "Debes confirmar la contraseña");
                else if (cliente.Contrasena != ConfirmarContrasena)
                    ModelState.AddModelError("ConfirmarContrasena", "Las contraseñas no coinciden");

                if (!ModelState.IsValid)
                {
                    TempData["Error"] = "Por favor, corrige los errores del formulario";
                    return View(cliente);
                }

                cliente.Estado = false;
                cliente.FechaRegistro = DateTime.Now;

                int resultado = await _crearCliente.Crear(cliente);
                if (resultado <= 0)
                {
                    TempData["Error"] = "No se pudo completar el registro. Intenta nuevamente.";
                    return View(cliente);
                }

                string codigo = new Random().Next(100000, 999999).ToString();
                DateTime expiracion = DateTime.UtcNow.AddMinutes(15);

                var contexto = new AccesoADatos.Contexto();
                var clienteAD = contexto.Cliente.FirstOrDefault(c => c.Correo.ToLower() == cliente.Correo.ToLower());
                if (clienteAD != null)
                {
                    clienteAD.CodigoRecuperacion = codigo;
                    clienteAD.CodigoExpiracion = expiracion;
                    contexto.SaveChanges();
                }

                bool emailEnviado = await _emailService.EnviarCodigoVerificacion(
                    cliente.Correo, $"{cliente.Nombre} {cliente.Apellidos}", codigo);

                if (emailEnviado)
                {
                    TempData["Mensaje"] = "Te hemos enviado un código de verificación. Revisa tu bandeja de entrada y spam.";
                    return RedirectToAction(nameof(VerificarEmail), new { correo = cliente.Correo });
                }

                TempData["Error"] = "Cuenta creada pero hubo un problema al enviar el email. Contacta a soporte.";
                return View(cliente);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message.Contains("identificación")
                    ? "Ya existe un cliente registrado con esa identificación"
                    : ex.Message.Contains("correo")
                        ? "Ya existe un cliente registrado con ese correo electrónico"
                        : $"Error al registrar: {ex.Message}";
                return View(cliente);
            }
        }

        // ============================================
        // OLVIDÉ CONTRASEÑA
        // ============================================

        public IActionResult OlvideContrasenna() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OlvideContrasenna(string correo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(correo))
                {
                    TempData["Error"] = "Por favor, ingresa tu correo electrónico";
                    return View();
                }

                var cliente = _buscarClientePorCorreo.ObtenerPorCorreo(correo);
                if (cliente == null)
                {
                    TempData["Mensaje"] = "Si el correo existe en nuestro sistema, recibirás un código en los próximos minutos.";
                    return View();
                }

                if (!cliente.Estado)
                {
                    TempData["Error"] = "Esta cuenta está inactiva. Contacta a soporte.";
                    return View();
                }

                string codigo = new Random().Next(100000, 999999).ToString();
                DateTime expiracion = DateTime.UtcNow.AddMinutes(15);

                var contexto = new AccesoADatos.Contexto();
                var clienteAD = contexto.Cliente.FirstOrDefault(c => c.IdCliente == cliente.IdCliente);
                if (clienteAD == null) { TempData["Error"] = "Error al procesar la solicitud"; return View(); }

                clienteAD.CodigoRecuperacion = codigo;
                clienteAD.CodigoExpiracion = expiracion;
                contexto.SaveChanges();

                bool emailEnviado = await _emailService.EnviarCodigoRecuperacion(
                    cliente.Correo, $"{cliente.Nombre} {cliente.Apellidos}", codigo);

                if (emailEnviado)
                {
                    TempData["Mensaje"] = "Te hemos enviado un código de verificación. Revisa tu bandeja de entrada y spam.";
                    return RedirectToAction(nameof(VerificarCodigo), new { correo = cliente.Correo });
                }

                TempData["Error"] = "Hubo un problema al enviar el correo. Intenta nuevamente.";
                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Ocurrió un error: {ex.Message}";
                return View();
            }
        }

        // ============================================
        // VERIFICAR CÓDIGO
        // ============================================

        public IActionResult VerificarCodigo(string correo)
        {
            if (string.IsNullOrWhiteSpace(correo)) return RedirectToAction(nameof(OlvideContrasenna));
            ViewBag.Correo = correo;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult VerificarCodigo(string correo, string codigo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(codigo))
                {
                    TempData["Error"] = "Por favor, ingresa el código de verificación";
                    ViewBag.Correo = correo;
                    return View();
                }

                var cliente = _buscarClientePorCorreo.ObtenerPorCorreo(correo);
                if (cliente == null) return RedirectToAction(nameof(OlvideContrasenna));

                var contexto = new AccesoADatos.Contexto();
                var clienteAD = contexto.Cliente.FirstOrDefault(c => c.IdCliente == cliente.IdCliente);

                if (clienteAD == null || string.IsNullOrWhiteSpace(clienteAD.CodigoRecuperacion))
                {
                    TempData["Error"] = "No se encontró un código válido";
                    ViewBag.Correo = correo; return View();
                }

                if (clienteAD.CodigoRecuperacion != codigo.Trim())
                {
                    TempData["Error"] = "El código ingresado es incorrecto";
                    ViewBag.Correo = correo; return View();
                }

                if (clienteAD.CodigoExpiracion == null || DateTime.Now > clienteAD.CodigoExpiracion)
                {
                    TempData["Error"] = "El código ha expirado. Solicita uno nuevo.";
                    return RedirectToAction(nameof(OlvideContrasenna));
                }

                return RedirectToAction(nameof(RestablecerContrasenna), new { correo, codigo });
            }
            catch
            {
                TempData["Error"] = "Ocurrió un error al verificar el código";
                ViewBag.Correo = correo;
                return View();
            }
        }

        // ============================================
        // RESTABLECER CONTRASEÑA
        // ============================================

        public IActionResult RestablecerContrasenna(string correo, string codigo)
        {
            if (string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(codigo))
                return RedirectToAction(nameof(OlvideContrasenna));
            ViewBag.Correo = correo; ViewBag.Codigo = codigo;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RestablecerContrasenna(string correo, string codigo, string nuevaContrasena, string confirmarNuevaContrasena)
        {
            try
            {
                ViewBag.Correo = correo; ViewBag.Codigo = codigo;

                if (string.IsNullOrWhiteSpace(nuevaContrasena))
                { TempData["Error"] = "La contraseña es requerida"; return View(); }

                if (!PasswordValidator.EsContrasenaValida(nuevaContrasena, out string msgError))
                { TempData["Error"] = msgError; return View(); }

                if (nuevaContrasena != confirmarNuevaContrasena)
                { TempData["Error"] = "Las contraseñas no coinciden"; return View(); }

                var cliente = _buscarClientePorCorreo.ObtenerPorCorreo(correo);
                if (cliente == null) return RedirectToAction(nameof(OlvideContrasenna));

                var contexto = new AccesoADatos.Contexto();
                var clienteAD = contexto.Cliente.FirstOrDefault(c => c.IdCliente == cliente.IdCliente);

                if (clienteAD == null || clienteAD.CodigoRecuperacion != codigo)
                { TempData["Error"] = "Código inválido"; return RedirectToAction(nameof(OlvideContrasenna)); }

                if (clienteAD.CodigoExpiracion == null || DateTime.Now > clienteAD.CodigoExpiracion)
                { TempData["Error"] = "El código ha expirado"; return RedirectToAction(nameof(OlvideContrasenna)); }

                cliente.Contrasena = nuevaContrasena;
                _actualizarCliente.Actualizar(cliente);
                clienteAD.CodigoRecuperacion = null;
                clienteAD.CodigoExpiracion = null;
                contexto.SaveChanges();

                TempData["Mensaje"] = "¡Contraseña restablecida! Ya puedes iniciar sesión";
                return RedirectToAction(nameof(Login));
            }
            catch
            {
                TempData["Error"] = "Ocurrió un error al restablecer la contraseña";
                return View();
            }
        }

        // ============================================
        // VERIFICAR EMAIL
        // ============================================

        public IActionResult VerificarEmail(string correo)
        {
            if (string.IsNullOrWhiteSpace(correo)) return RedirectToAction(nameof(Registro));
            ViewBag.Correo = correo;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult VerificarEmail(string correo, string codigo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(codigo))
                {
                    TempData["Error"] = "Por favor, ingresa el código de verificación";
                    ViewBag.Correo = correo; return View();
                }

                var cliente = _buscarClientePorCorreo.ObtenerPorCorreo(correo);
                if (cliente == null) return RedirectToAction(nameof(Registro));
                if (cliente.Estado)
                {
                    TempData["Mensaje"] = "Tu cuenta ya está verificada. Puedes iniciar sesión.";
                    return RedirectToAction(nameof(Login));
                }

                var contexto = new AccesoADatos.Contexto();
                var clienteAD = contexto.Cliente.FirstOrDefault(c => c.IdCliente == cliente.IdCliente);

                if (clienteAD == null || string.IsNullOrWhiteSpace(clienteAD.CodigoRecuperacion))
                {
                    TempData["Error"] = "No se encontró un código válido";
                    ViewBag.Correo = correo; return View();
                }

                if (clienteAD.CodigoRecuperacion != codigo.Trim())
                {
                    TempData["Error"] = "El código ingresado es incorrecto";
                    ViewBag.Correo = correo; return View();
                }

                if (clienteAD.CodigoExpiracion == null || DateTime.UtcNow > clienteAD.CodigoExpiracion)
                {
                    TempData["Error"] = "El código ha expirado. Solicita uno nuevo.";
                    ViewBag.Correo = correo; return View();
                }

                clienteAD.Estado = true;
                clienteAD.CodigoRecuperacion = null;
                clienteAD.CodigoExpiracion = null;
                contexto.SaveChanges();

                TempData["Mensaje"] = "¡Cuenta verificada exitosamente! Ya puedes iniciar sesión";
                return RedirectToAction(nameof(Login));
            }
            catch
            {
                TempData["Error"] = "Ocurrió un error al verificar el código";
                ViewBag.Correo = correo; return View();
            }
        }

        public async Task<IActionResult> ReenviarCodigoVerificacion(string correo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(correo)) return RedirectToAction(nameof(Registro));

                var cliente = _buscarClientePorCorreo.ObtenerPorCorreo(correo);
                if (cliente == null) return RedirectToAction(nameof(Registro));
                if (cliente.Estado) return RedirectToAction(nameof(Login));

                string codigo = new Random().Next(100000, 999999).ToString();
                DateTime expiracion = DateTime.UtcNow.AddMinutes(15);

                var contexto = new AccesoADatos.Contexto();
                var clienteAD = contexto.Cliente.FirstOrDefault(c => c.IdCliente == cliente.IdCliente);
                if (clienteAD != null)
                {
                    clienteAD.CodigoRecuperacion = codigo;
                    clienteAD.CodigoExpiracion = expiracion;
                    contexto.SaveChanges();
                }

                bool ok = await _emailService.EnviarCodigoVerificacion(
                    cliente.Correo, $"{cliente.Nombre} {cliente.Apellidos}", codigo);

                TempData[ok ? "Mensaje" : "Error"] = ok
                    ? "Hemos reenviado el código a tu correo"
                    : "Hubo un problema al reenviar el código";

                return RedirectToAction(nameof(VerificarEmail), new { correo });
            }
            catch
            {
                TempData["Error"] = "Ocurrió un error al reenviar el código";
                return RedirectToAction(nameof(VerificarEmail), new { correo });
            }
        }
    }
}