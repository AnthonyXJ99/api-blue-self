namespace BlueSelfCheckout.WebApi.Dtos
{
    /// <summary>
    /// Resultado paginado genérico para APIs.
    /// </summary>
    /// <typeparam name="T">Tipo de datos a paginar.</typeparam>
    public class PaginatedResult<T>
    {
        /// <summary>
        /// Lista de datos de la página actual.
        /// </summary>
        public IEnumerable<T> Data { get; set; }

        /// <summary>
        /// Número de página actual.
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Tamaño de página utilizado.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Total de registros disponibles (después del filtro).
        /// </summary>
        public int TotalRecords { get; set; }

        /// <summary>
        /// Total de páginas disponibles.
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Indica si existe una página siguiente.
        /// </summary>
        public bool HasNextPage { get; set; }

        /// <summary>
        /// Indica si existe una página anterior.
        /// </summary>
        public bool HasPreviousPage { get; set; }

        /// <summary>
        /// Filtro aplicado a los datos.
        /// </summary>
        public string Filter { get; set; }
    }
}
