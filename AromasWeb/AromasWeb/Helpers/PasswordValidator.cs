using System.Text.RegularExpressions;

namespace AromasWeb.Helpers
{
    public static class PasswordValidator
    {
        public static bool EsContrasenaValida(string contrasena, out string mensajeError)
        {
            mensajeError = string.Empty;

            if (string.IsNullOrWhiteSpace(contrasena))
            {
                mensajeError = "La contraseña es requerida";
                return false;
            }

            if (contrasena.Length < 8)
            {
                mensajeError = "La contraseña debe tener al menos 8 caracteres";
                return false;
            }

            // Validar al menos una mayúscula
            if (!Regex.IsMatch(contrasena, @"[A-Z]"))
            {
                mensajeError = "La contraseña debe contener al menos una letra mayúscula";
                return false;
            }

            // Validar al menos una minúscula
            if (!Regex.IsMatch(contrasena, @"[a-z]"))
            {
                mensajeError = "La contraseña debe contener al menos una letra minúscula";
                return false;
            }

            // Validar al menos un número
            if (!Regex.IsMatch(contrasena, @"[0-9]"))
            {
                mensajeError = "La contraseña debe contener al menos un número";
                return false;
            }

            // Validar al menos un carácter especial
            if (!Regex.IsMatch(contrasena, @"[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]"))
            {
                mensajeError = "La contraseña debe contener al menos un carácter especial (!@#$%^&*()_+-=[]{}etc.)";
                return false;
            }

            return true;
        }
    }
}