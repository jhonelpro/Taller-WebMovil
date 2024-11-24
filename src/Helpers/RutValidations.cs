namespace api.src.Helpers
{
    /// <summary>
    /// Clase RutValidations que contiene métodos para la validación y cálculo del dígito verificador de un RUT chileno.
    /// </summary>
    public class RutValidations
    {
        /// <summary>
        /// Valida si un RUT chileno es correcto verificando el formato y el dígito verificador.
        /// </summary>
        /// <param name="rut">Parametro de tipo string qu representa un RUT chileno, con puntos y guión.</param>
        /// <returns>Devuelve true si el RUT es válido; de lo contrario, false.</returns>
        public static bool IsValidRut(string rut)
        {
            // Verifica que el rut no este vacio.
            if (string.IsNullOrWhiteSpace(rut))
                return false;
            
            // Verifica que el rut tenga el guion, y que el digito verificador sea de tamanio 1.
            int dashIndex = rut.LastIndexOf('-');
            if (dashIndex < 0 || rut.Length - dashIndex != 2)
                return false;

            // Quitar puntos y guión del RUT
            rut = rut.Replace(".", "").Replace("-", "");

            // Verifica que el rut sea de un tamanio correcto.
            if (rut.Length < 9 || rut.Length > 10)
                return false;

            // Separar el cuerpo del RUT y el dígito verificador
            string dv = rut.Substring(rut.Length - 1, 1);
            string rutBody = rut.Substring(0, rut.Length - 1);

            // Convierte de string rutBody a int rutNumber.
            if (!int.TryParse(rutBody, out int rutNumber))
                return false;

            //Compara el digito verificador con el resultado del calculo de este mismo a partir del cuerpo del rut. 
            return dv.ToUpper() == CalculateRutCheckDigit(rutNumber);
        }

        /// <summary>
        /// Calcula el dígito verificador de un RUT chileno basado en el algoritmo de validación.
        /// </summary>
        /// <param name="rut">Parametro de tipo int que representa el Número del RUT sin el dígito verificador.</param>
        /// <returns>Devuelve el dígito verificador como string ("0"-"9" o "K").</returns>
        public static string CalculateRutCheckDigit(int rut)
        {
            int sum = 0;
            int multiplier = 2;

            while (rut > 0)
            {
                int digit = rut % 10;
                sum += digit * multiplier;
                multiplier = (multiplier == 7) ? 2 : multiplier + 1;
                rut /= 10;
            }

            int remainder = 11 - (sum % 11);

            if (remainder == 11) return "0";
            if (remainder == 10) return "K";

            return remainder.ToString();
        }
    }
}