using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace TrabajadoresApp.Models
{
    public class ValidacionDocumentoAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is not Trabajador trabajador)
                return true; // Si no es Trabajador, no aplicamos validación

            var tipoDocumento = trabajador.TipoDocumento?.Trim();
            var numeroDocumento = trabajador.NumeroDocumento?.Trim();

            if (string.IsNullOrEmpty(tipoDocumento) || string.IsNullOrEmpty(numeroDocumento))
                return true; // Dejamos que Required se encargue de campos vacíos

            return tipoDocumento switch
            {
                "DNI" => ValidarDNI(numeroDocumento),
                "Pasaporte" => ValidarPasaporte(numeroDocumento),
                "Carnet Extranjeria" => ValidarCarnetExtranjeria(numeroDocumento),
                _ => false
            };
        }

        private static bool ValidarDNI(string numero)
        {
            return numero.Length == 8 && numero.All(char.IsDigit);
        }

        private static bool ValidarPasaporte(string numero)
        {
            return numero.Length == 9 && numero.All(c => char.IsLetterOrDigit(c));
        }

        private static bool ValidarCarnetExtranjeria(string numero)
        {
            return numero.Length == 20 && numero.All(char.IsDigit);
        }

        public override string FormatErrorMessage(string name)
        {
            return ErrorMessage ?? "El número de documento no cumple con el formato requerido.";
        }
    }
}