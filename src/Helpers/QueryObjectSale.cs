namespace api.src.Helpers

{
    public class QueryObjectSale
    {
        /// <summary>
        /// Atributo de tipo bool que representa si se desea ordenar los resultados por fecha.
        /// </summary>
        /// <value></value>
        public bool? IsDecendingDate { get; set; }
        
        /// <summary>
        /// Atributo de tipo string que representa el nombre de usuario del cliente por el cual se desea filtrar.
        /// </summary>
        /// <value></value>
        public string? UserName { get; set; } = string.Empty;

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