namespace StageSpotter.Web.Models
{
    public class VacatureOverzichtViewModel
    {
        public string? Zoekterm { get; set; }
        public int HuidigePagina { get; set; }
        public int TotaalPaginas { get; set; }
        public int TotaalVacatures { get; set; }

        public List<VacatureLijstItem> Vacatures { get; set; } = new();
        
        public VacatureDetailItem? GeselecteerdeVacature { get; set; }
    }
}