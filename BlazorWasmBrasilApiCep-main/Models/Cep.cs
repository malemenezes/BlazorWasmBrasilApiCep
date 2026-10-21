namespace BlazorWasmBrasilApiCep.Models
{
    public class Cep
    {
        public string? cep { get; set; }
        public string? state { get; set; }
        public string? city { get; set; }
        public string? neighborhood { get; set; }
        public string? street { get; set; }
        public string? service { get; set; }
        public Ibge? ibge { get; set; }
    }
}
