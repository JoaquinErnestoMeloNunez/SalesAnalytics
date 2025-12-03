using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesAnalytics.Domain.Entities.Dwh.Dimensions
{
    [Table("Dim_Date", Schema = "Dimension")]
    public class DimDate
    {
        [Key]
        public int Date_Key { get; set; }

        public int Date_Id { get; set; }
        public DateTime Date { get; set; }
        public int Anio { get; set; }
        public int Trimestre { get; set; }
        public int Mes { get; set; }
        public string Nombre_Mes { get; set; } = null!;
        public int Semana { get; set; }
        public int Dia_Mes { get; set; }
        public int Dia_Semana { get; set; }
        public string Nombre_Dia { get; set; } = null!;
    }
}
