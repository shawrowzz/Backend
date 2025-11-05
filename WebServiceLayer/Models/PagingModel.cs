namespace WebServiceLayer.Models;

public class PagingModel<T>
{
    public string First { get; set; }
    public string Prev { get; set; }
    public string Next { get; set; }
    public string Last { get; set; }
    public string Current { get; set; }
    public int NumberOfPages { get; set; }
    public int NumberOfItems { get; set; }
    public List<T> Items { get; set; }
}