using Microsoft.AspNetCore.Mvc;
using AromasWeb.Abstracciones.ModeloUI;
using AromasWeb.Abstracciones.Logica.Cliente;
using AromasWeb.Abstracciones.Servicios;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using AromasWeb.Helpers;

namespace AromasWeb.Controllers
{
    public class AuthController : Controller
    {
        private ICrearCliente _crearCliente;
        private IBuscarClientePorCorreo _buscarClientePorCorreo;
        private IActualizarCliente _actualizarCliente;
        private readonly IEmailService _emailService;

        public AuthController(IEmailService emailService)
        {
            _crearCliente = new LogicaDeNegocio.Clientes.CrearCliente();
            _buscarClientePorCorreo = new LogicaDeNegocio.Clientes.BuscarClientePorCorreo();
            _actualizarCliente = new LogicaDeNegocio.Clientes.ActualizarCliente();
            _emailService = emailService;
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

            // ⭐ FUTURO: Buscar cliente en la base de datos
            var clienteBD = _buscarClientePorCorreo.ObtenerPorCorreo(cliente.Correo);

            if (clienteBD != null)
            {
                // ⭐ Verificar que el cliente haya verificado su email
                if (!clienteBD.Estado)
                {
                    TempData["Error"] = "Debes verificar tu correo antes de iniciar sesión. Revisa tu bandeja de entrada.";
                    return View(cliente);
                }

                // ⭐ Verificar contraseña
                if (clienteBD.Contrasena == cliente.Contrasena)
                {
                    // Login exitoso
                    HttpContext.Session.SetString("UsuarioTipo", "cliente");
                    HttpContext.Session.SetString("UsuarioNombre", $"{clienteBD.Nombre} {clienteBD.Apellidos}");
                    HttpContext.Session.SetString("UsuarioCorreo", clienteBD.Correo);
                    HttpContext.Session.SetInt32("IdCliente", clienteBD.IdCliente);

                    TempData["Mensaje"] = $"¡Bienvenido {clienteBD.Nombre}!";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["Error"] = "Correo o contraseña incorrectos";
                    return View(cliente);
                }
            }

            // Si no encontró el cliente en BD, verificar usuarios hardcodeados (admin, empleado)
            if (cliente.Correo.ToLower() == "admin@gmail.com" && cliente.Contrasena == "admin123")
            {
                HttpContext.Session.SetString("UsuarioTipo", "admin");
                HttpContext.Session.SetString("UsuarioNombre", "Administrador");
                HttpContext.Session.SetString("UsuarioCorreo", cliente.Correo);
                TempData["Mensaje"] = "¡Bienvenido Administrador!";
                return RedirectToAction("Index", "Home");
            }
            else if (cliente.Correo.ToLower() == "empleado@gmail.com" && cliente.Contrasena == "empleado123")
            {
                HttpContext.Session.SetString("UsuarioTipo", "empleado");
                HttpContext.Session.SetString("UsuarioNombre", "Empleado");
                HttpContext.Session.SetString("UsuarioCorreo", cliente.Correo);
                HttpContext.Session.SetInt32("IdEmpleado", 1);
                TempData["Mensaje"] = "¡Bienvenido Empleado!";
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
                System.Diagnostics.Debug.WriteLine($"[REGISTRO] Inicio para: {cliente.Correo}");

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

                // Estado = false (no verificado hasta el código de verificación)
                cliente.Estado = false;
                cliente.FechaRegistro = DateTime.Now;

                System.Diagnostics.Debug.WriteLine($"[REGISTRO] Creando cliente en BD...");
                int resultado = await _crearCliente.Crear(cliente);

                if (resultado > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"[REGISTRO] Cliente creado exitosamente");

                    // Generar código de verificación de 6 dígitos
                    Random random = new Random();
                    string codigo = random.Next(100000, 999999).ToString();
                    DateTime expiracion = DateTime.UtcNow.AddMinutes(15);

                    System.Diagnostics.Debug.WriteLine($"[REGISTRO] Código generado: {codigo}");

                    // Guardar código en la base de datos
                    var contexto = new AccesoADatos.Contexto();
                    var clienteAD = contexto.Cliente.FirstOrDefault(c => c.Correo.ToLower() == cliente.Correo.ToLower());

                    if (clienteAD != null)
                    {
                        clienteAD.CodigoRecuperacion = codigo;
                        clienteAD.CodigoExpiracion = expiracion;
                        contexto.SaveChanges();
                        System.Diagnostics.Debug.WriteLine($"[REGISTRO] Código guardado en BD");
                    }

                    // Enviar email de verificación
                    System.Diagnostics.Debug.WriteLine($"[REGISTRO] Enviando email de verificación...");
                    bool emailEnviado = await _emailService.EnviarCodigoVerificacion(
                        cliente.Correo,
                        $"{cliente.Nombre} {cliente.Apellidos}",
                        codigo
                    );

                    if (emailEnviado)
                    {
                        System.Diagnostics.Debug.WriteLine($"[REGISTRO] Email enviado correctamente");
                        TempData["Mensaje"] = "Te hemos enviado un código de verificación a tu correo. Revisa tu bandeja de entrada y spam.";
                        return RedirectToAction(nameof(VerificarEmail), new { correo = cliente.Correo });
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"[REGISTRO] ERROR al enviar email");
                        TempData["Error"] = "Cuenta creada pero hubo un problema al enviar el email. Contacta a soporte.";
                        return View(cliente);
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"[REGISTRO] No se pudo crear el cliente");
                    TempData["Error"] = "No se pudo completar el registro. Intenta nuevamente.";
                    return View(cliente);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[REGISTRO] EXCEPCION: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[REGISTRO] STACK: {ex.StackTrace}");

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

        // GET: Auth/OlvideContrasenna
        public IActionResult OlvideContrasenna()
        {
            return View();
        }

        // POST: Auth/OlvideContrasenna
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OlvideContrasenna(string correo)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"[RECUPERACION] Inicio - Correo: {correo}");

                if (string.IsNullOrWhiteSpace(correo))
                {
                    TempData["Error"] = "Por favor, ingresa tu correo electrónico";
                    return View();
                }

                // Buscar cliente por correo
                System.Diagnostics.Debug.WriteLine($"[RECUPERACION] Buscando cliente...");
                var cliente = _buscarClientePorCorreo.ObtenerPorCorreo(correo);

                if (cliente == null)
                {
                    System.Diagnostics.Debug.WriteLine($"[RECUPERACION] Cliente NO encontrado");
                    // Por seguridad, no revelar si el correo existe o no
                    TempData["Mensaje"] = "Si el correo existe en nuestro sistema, recibirás un código de verificación en los próximos minutos. Revisa tu bandeja de entrada y spam.";
                    return View();
                }

                System.Diagnostics.Debug.WriteLine($"[RECUPERACION] Cliente encontrado: {cliente.Nombre} {cliente.Apellidos}");

                // Verificar que el cliente esté activo
                if (!cliente.Estado)
                {
                    System.Diagnostics.Debug.WriteLine($"[RECUPERACION] Cliente INACTIVO");
                    TempData["Error"] = "Esta cuenta está inactiva. Contacta a soporte.";
                    return View();
                }

                // Generar código aleatorio de 6 dígitos
                Random random = new Random();
                string codigo = random.Next(100000, 999999).ToString();
                System.Diagnostics.Debug.WriteLine($"[RECUPERACION] Código generado: {codigo}");

                // Establecer expiración de 15 minutos
                DateTime expiracion = DateTime.UtcNow.AddMinutes(15);
                System.Diagnostics.Debug.WriteLine($"[RECUPERACION] Expiración: {expiracion}");

                // Guardar código y expiración usando AccesoADatos directamente
                System.Diagnostics.Debug.WriteLine($"[RECUPERACION] Guardando código en BD...");
                var contexto = new AccesoADatos.Contexto();
                var clienteAD = contexto.Cliente.FirstOrDefault(c => c.IdCliente == cliente.IdCliente);

                if (clienteAD != null)
                {
                    clienteAD.CodigoRecuperacion = codigo;
                    clienteAD.CodigoExpiracion = expiracion;
                    int filasAfectadas = contexto.SaveChanges();
                    System.Diagnostics.Debug.WriteLine($"[RECUPERACION] Código guardado. Filas afectadas: {filasAfectadas}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"[RECUPERACION] ERROR: No se pudo obtener clienteAD");
                    TempData["Error"] = "Error al procesar la solicitud";
                    return View();
                }

                // Enviar email con el código
                System.Diagnostics.Debug.WriteLine($"[RECUPERACION] Enviando email...");
                bool emailEnviado = await _emailService.EnviarCodigoRecuperacion(
                    cliente.Correo,
                    $"{cliente.Nombre} {cliente.Apellidos}",
                    codigo
                );

                System.Diagnostics.Debug.WriteLine($"[RECUPERACION] Email enviado: {emailEnviado}");

                if (emailEnviado)
                {
                    TempData["CorreoRecuperacion"] = cliente.Correo;
                    TempData["Mensaje"] = "Te hemos enviado un código de verificación a tu correo. Revisa tu bandeja de entrada y spam.";
                    System.Diagnostics.Debug.WriteLine($"[RECUPERACION] Redirigiendo a VerificarCodigo");
                    return RedirectToAction(nameof(VerificarCodigo), new { correo = cliente.Correo });
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"[RECUPERACION] ERROR al enviar email");
                    TempData["Error"] = "Hubo un problema al enviar el correo. Intenta nuevamente.";
                    return View();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[RECUPERACION] EXCEPCION: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[RECUPERACION] STACK TRACE: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"[RECUPERACION] INNER EXCEPTION: {ex.InnerException.Message}");
                }
                TempData["Error"] = $"Ocurrió un error: {ex.Message}";
                return View();
            }
        }

        // ============================================
        // VERIFICAR CÓDIGO
        // ============================================

        // GET: Auth/VerificarCodigo
        public IActionResult VerificarCodigo(string correo)
        {
            if (string.IsNullOrWhiteSpace(correo))
            {
                return RedirectToAction(nameof(OlvideContrasenna));
            }

            ViewBag.Correo = correo;
            return View();
        }

        // POST: Auth/VerificarCodigo
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

                // Buscar cliente
                var cliente = _buscarClientePorCorreo.ObtenerPorCorreo(correo);

                if (cliente == null)
                {
                    TempData["Error"] = "No se encontró la cuenta";
                    return RedirectToAction(nameof(OlvideContrasenna));
                }

                // Obtener código y expiración de la base de datos
                var contexto = new AccesoADatos.Contexto();
                var clienteAD = contexto.Cliente.FirstOrDefault(c => c.IdCliente == cliente.IdCliente);

                if (clienteAD == null || string.IsNullOrWhiteSpace(clienteAD.CodigoRecuperacion))
                {
                    TempData["Error"] = "No se encontró un código de recuperación válido";
                    ViewBag.Correo = correo;
                    return View();
                }

                // Verificar que el código coincida
                if (clienteAD.CodigoRecuperacion != codigo.Trim())
                {
                    TempData["Error"] = "El código ingresado es incorrecto";
                    ViewBag.Correo = correo;
                    return View();
                }

                // Verificar que no haya expirado
                if (clienteAD.CodigoExpiracion == null || DateTime.Now > clienteAD.CodigoExpiracion)
                {
                    TempData["Error"] = "El código ha expirado. Solicita uno nuevo.";
                    return RedirectToAction(nameof(OlvideContrasenna));
                }

                // Código válido, redirigir a restablecer contraseña
                TempData["CodigoVerificado"] = codigo;
                return RedirectToAction(nameof(RestablecerContrasenna), new { correo = correo, codigo = codigo });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en VerificarCodigo: {ex.Message}");
                TempData["Error"] = "Ocurrió un error al verificar el código";
                ViewBag.Correo = correo;
                return View();
            }
        }

        // ============================================
        // RESTABLECER CONTRASEÑA
        // ============================================

        // GET: Auth/RestablecerContrasenna
        public IActionResult RestablecerContrasenna(string correo, string codigo)
        {
            if (string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(codigo))
            {
                return RedirectToAction(nameof(OlvideContrasenna));
            }

            ViewBag.Correo = correo;
            ViewBag.Codigo = codigo;
            return View();
        }

        // POST: Auth/RestablecerContrasenna
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RestablecerContrasenna(string correo, string codigo, string nuevaContrasena, string confirmarNuevaContrasena)
        {
            try
            {
                ViewBag.Correo = correo;
                ViewBag.Codigo = codigo;

                if (string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(codigo))
                {
                    TempData["Error"] = "Datos inválidos";
                    return RedirectToAction(nameof(OlvideContrasenna));
                }

                // Validar nueva contraseña
                if (string.IsNullOrWhiteSpace(nuevaContrasena))
                {
                    TempData["Error"] = "La contraseña es requerida";
                    return View();
                }

                if (!PasswordValidator.EsContrasenaValida(nuevaContrasena, out string mensajeError))
                {
                    TempData["Error"] = mensajeError;
                    return View();
                }

                if (nuevaContrasena != confirmarNuevaContrasena)
                {
                    TempData["Error"] = "Las contraseñas no coinciden";
                    return View();
                }

                // Buscar cliente
                var cliente = _buscarClientePorCorreo.ObtenerPorCorreo(correo);

                if (cliente == null)
                {
                    TempData["Error"] = "No se encontró la cuenta";
                    return RedirectToAction(nameof(OlvideContrasenna));
                }

                // Verificar código nuevamente
                var contexto = new AccesoADatos.Contexto();
                var clienteAD = contexto.Cliente.FirstOrDefault(c => c.IdCliente == cliente.IdCliente);

                if (clienteAD == null || clienteAD.CodigoRecuperacion != codigo)
                {
                    TempData["Error"] = "Código inválido";
                    return RedirectToAction(nameof(OlvideContrasenna));
                }

                if (clienteAD.CodigoExpiracion == null || DateTime.Now > clienteAD.CodigoExpiracion)
                {
                    TempData["Error"] = "El código ha expirado";
                    return RedirectToAction(nameof(OlvideContrasenna));
                }

                // Actualizar contraseña
                cliente.Contrasena = nuevaContrasena;
                _actualizarCliente.Actualizar(cliente);

                // Limpiar código de recuperación
                clienteAD.CodigoRecuperacion = null;
                clienteAD.CodigoExpiracion = null;
                contexto.SaveChanges();

                TempData["Mensaje"] = "¡Contraseña restablecida exitosamente! Ya puedes iniciar sesión";
                return RedirectToAction(nameof(Login));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en RestablecerContrasenna: {ex.Message}");
                TempData["Error"] = "Ocurrió un error al restablecer la contraseña";
                return View();
            }
        }

        // ============================================
        // VERIFICAR EMAIL
        // ============================================

        // GET: Auth/VerificarEmail
        public IActionResult VerificarEmail(string correo)
        {
            if (string.IsNullOrWhiteSpace(correo))
            {
                return RedirectToAction(nameof(Registro));
            }

            ViewBag.Correo = correo;
            return View();
        }

        // POST: Auth/VerificarEmail
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult VerificarEmail(string correo, string codigo)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"[VERIFICAR EMAIL] Verificando código para: {correo}");

                if (string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(codigo))
                {
                    TempData["Error"] = "Por favor, ingresa el código de verificación";
                    ViewBag.Correo = correo;
                    return View();
                }

                // Buscar cliente
                var cliente = _buscarClientePorCorreo.ObtenerPorCorreo(correo);

                if (cliente == null)
                {
                    TempData["Error"] = "No se encontró la cuenta";
                    return RedirectToAction(nameof(Registro));
                }

                // Si ya está verificado
                if (cliente.Estado)
                {
                    TempData["Mensaje"] = "Tu cuenta ya está verificada. Puedes iniciar sesión.";
                    return RedirectToAction(nameof(Login));
                }

                // Obtener código de la base de datos
                var contexto = new AccesoADatos.Contexto();
                var clienteAD = contexto.Cliente.FirstOrDefault(c => c.IdCliente == cliente.IdCliente);

                if (clienteAD == null || string.IsNullOrWhiteSpace(clienteAD.CodigoRecuperacion))
                {
                    TempData["Error"] = "No se encontró un código de verificación válido";
                    ViewBag.Correo = correo;
                    return View();
                }

                // Verificar que el código coincida
                if (clienteAD.CodigoRecuperacion != codigo.Trim())
                {
                    System.Diagnostics.Debug.WriteLine($"[VERIFICAR EMAIL] Código incorrecto");
                    TempData["Error"] = "El código ingresado es incorrecto";
                    ViewBag.Correo = correo;
                    return View();
                }

                // Verificar que no haya expirado
                if (clienteAD.CodigoExpiracion == null || DateTime.UtcNow > clienteAD.CodigoExpiracion)
                {
                    System.Diagnostics.Debug.WriteLine($"[VERIFICAR EMAIL] Código expirado");
                    TempData["Error"] = "El código ha expirado. Solicita uno nuevo.";
                    ViewBag.Correo = correo;
                    return View();
                }

                // ⭐ Activar cuenta (Estado = true)
                clienteAD.Estado = true;
                clienteAD.CodigoRecuperacion = null;
                clienteAD.CodigoExpiracion = null;
                contexto.SaveChanges();

                System.Diagnostics.Debug.WriteLine($"[VERIFICAR EMAIL] Cuenta verificada exitosamente");

                TempData["Mensaje"] = "¡Tu cuenta ha sido verificada exitosamente! Ya puedes iniciar sesión";
                return RedirectToAction(nameof(Login));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[VERIFICAR EMAIL] EXCEPCION: {ex.Message}");
                TempData["Error"] = "Ocurrió un error al verificar el código";
                ViewBag.Correo = correo;
                return View();
            }
        }

        // GET: Auth/ReenviarCodigoVerificacion
        public async Task<IActionResult> ReenviarCodigoVerificacion(string correo)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"[REENVIAR CODIGO] Para: {correo}");

                if (string.IsNullOrWhiteSpace(correo))
                {
                    return RedirectToAction(nameof(Registro));
                }

                var cliente = _buscarClientePorCorreo.ObtenerPorCorreo(correo);

                if (cliente == null)
                {
                    TempData["Error"] = "No se encontró la cuenta";
                    return RedirectToAction(nameof(Registro));
                }

                // Si ya está verificado
                if (cliente.Estado)
                {
                    TempData["Mensaje"] = "Tu cuenta ya está verificada";
                    return RedirectToAction(nameof(Login));
                }

                // Generar nuevo código
                Random random = new Random();
                string codigo = random.Next(100000, 999999).ToString();
                DateTime expiracion = DateTime.UtcNow.AddMinutes(15);

                System.Diagnostics.Debug.WriteLine($"[REENVIAR CODIGO] Nuevo código: {codigo}");

                // Actualizar en BD
                var contexto = new AccesoADatos.Contexto();
                var clienteAD = contexto.Cliente.FirstOrDefault(c => c.IdCliente == cliente.IdCliente);

                if (clienteAD != null)
                {
                    clienteAD.CodigoRecuperacion = codigo;
                    clienteAD.CodigoExpiracion = expiracion;
                    contexto.SaveChanges();
                }

                // Enviar email
                bool emailEnviado = await _emailService.EnviarCodigoVerificacion(
                    cliente.Correo,
                    $"{cliente.Nombre} {cliente.Apellidos}",
                    codigo
                );

                if (emailEnviado)
                {
                    TempData["Mensaje"] = "Hemos reenviado el código a tu correo";
                }
                else
                {
                    TempData["Error"] = "Hubo un problema al reenviar el código";
                }

                return RedirectToAction(nameof(VerificarEmail), new { correo = correo });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[REENVIAR CODIGO] ERROR: {ex.Message}");
                TempData["Error"] = "Ocurrió un error al reenviar el código";
                return RedirectToAction(nameof(VerificarEmail), new { correo = correo });
            }
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