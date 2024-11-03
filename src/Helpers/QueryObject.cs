namespace api.src.Helpers
{
    /// <summary>
    /// Clase QueryObject que representa la solicitud que permite el filtro y paginado de los productos.
    /// Contiene los parámetros necesarios para realizar filtros, ordenar y paginar la lista de productos.
    /// </summary>
    public class QueryObject
    {
        /// <summary>
        /// Atributo de tipo string que representa el texto por el cual el usuario desea filtrar los productos.
        /// Permite buscar productos que coincidan con este texto.
        /// </summary>
        public string? textFilter { get; set; } = string.Empty;

        /// <summary>
        /// Atributo de tipo string que representa el tipo de producto por el cual se desea filtrar.
        /// Este filtro permite seleccionar productos que pertenecen a una categoría específica.
        /// </summary>
        public string? productType { get; set; } = string.Empty;

        /// <summary>
        /// Atributo de tipo string que representa el criterio para ordenar los productos por precio.
        /// </summary>
        public string? sortByPrice { get; set; } = string.Empty;

        /// <summary>
        /// Atributo de tipo bool que indica si se desea ordenar de forma descendente.
        /// Al ser verdadero, se ordenarán los productos de mayor a menor; de lo contrario, de menor a mayor.
        /// </summary>
        public bool IsDescending { get; set; } = false;

        /// <summary>
        /// Atributo de tipo int que representa el número de página que se desea ver.
        /// </summary>
        public int pageNumber { get; set; } = 1;

        /// <summary>
        /// Atributo de tipo int que representa el tamaño de la página.
        /// Define el número de elementos que se verán en cada página de resultados.
        /// </summary>
        public int pageSize { get; set; } = 10;
    }
}