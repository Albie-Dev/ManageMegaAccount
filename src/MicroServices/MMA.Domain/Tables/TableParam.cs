namespace MMA.Domain
{
    public class TableParam<T> : PagingParam
    {
        public string SearchQuery { get; set; } = string.Empty;
        public T? Filter { get; set; }
        public SorterParam? Sorter { get; set; } = null;
    }
}