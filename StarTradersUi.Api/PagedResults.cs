namespace StarTradersUi.Api;

public class PagedResults<T>
{
    public T[] Data { get; set; }
    public PagedMeta Meta { get; set; }
}