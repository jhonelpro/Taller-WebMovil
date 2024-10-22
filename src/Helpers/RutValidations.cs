
namespace Api.src.Helpers
{
    public class RutValidations
    {
        public static bool IsValidRut(string rut)
        {
            if (string.IsNullOrWhiteSpace(rut))
                return false;

            rut = rut.Replace(".", "").Replace("-", "");

            if (rut.Length < 9 || rut.Length > 10)
                return false;

            string dv = rut.Substring(rut.Length - 1, 1);
            string rutBody = rut.Substring(0, rut.Length - 1);

            if (!int.TryParse(rutBody, out int rutNumber))
                return false;

            return dv.ToUpper() == CalculateRutCheckDigit(rutNumber);
        }

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