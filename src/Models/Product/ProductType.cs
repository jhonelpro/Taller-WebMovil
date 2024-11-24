namespace api.src.Models
{
    /// <summary>
    /// Clase que representa un tipo de producto en el sistema.
    /// </summary>
    public class ProductType
    {
        /// <summary>
        /// Atributo de tipo int que representa el identificador único del tipo de producto.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Atributo de tipo string que representa el nombre del tipo de producto.
        /// </summary>
        public string Name { get; set; } = string.Empty;
    }
}