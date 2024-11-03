namespace api.src.Helpers
{
    /// <summary>
    /// Clase QueryObjectUsers que representa la solicitud que permite el filtro y paginado de los usuarios.
    /// Contiene los parámetros necesarios para realizar filtros y paginar la lista de productos.
    /// </summary>
    public class QueryObjectUsers
    {
        /// <summary>
        /// Atributo de tipo string que representa el nombre del usuario que el administrador desea filtrar.
        /// </summary>
        public string? Name { get; set; } = string.Empty;

        /// <summary>
        /// Atributo de tipo int que representa el número de página que se desea ver.
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// Atributo de tipo int que representa el tamaño de la página.
        /// Define el número de elementos que se verán en cada página de resultados.
        /// </summary>
        public int PageSize { get; set; } = 10;
    }
}