namespace api.src.Helpers
{
    /// <summary>
    /// Clase CloudinarySettings que contiene la configuración para conectarse a la API de Cloudinary.
    /// Incluye las credenciales necesarias para autenticar la conexión.
    /// </summary>
    public class CloudinarySettings
    {
        /// <summary>
        /// Atributo de tipo string que representa el nombre del Cloud en Cloudinary.
        /// </summary>
        public string CloudName { get; set; } = string.Empty;

        /// <summary>
        /// Atributo de tipo string que representa la clave de la API del Cloud.
        /// </summary> 
        public string ApiKey { get; set; } = string.Empty;

        /// <summary>
        /// Atributo de tipo string que representa el secreto de la API del Cloud.
        /// </summary>
        public string ApiSecret { get; set; } = string.Empty;
    }
}