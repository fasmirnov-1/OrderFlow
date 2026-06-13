namespace OrderFlow.Domain
{
    public class Page
    {
        public string? Header { get; set; }
        public Dictionary<Types, dynamic>? Children = new Dictionary<Types, dynamic>();
    }
}
