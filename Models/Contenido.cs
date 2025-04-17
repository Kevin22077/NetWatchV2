using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NetWatchV2.Models
{
    public class Contenido
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El tipo es requerido.")]
        public string Tipo { get; set; } // Serie o Película

        [Required(ErrorMessage = "El nombre es requerido.")]
        public string Nombre { get; set; }

        public string Genero { get; set; }

        public int? Año { get; set; }

        public string Plataforma { get; set; }

        [DataType(DataType.MultilineText)]
        public string Sinopsis { get; set; }

        [Display(Name = "Link de Portada")]
        public string LinkPortada { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "La calificación debe tener un formato como '8.7' o '9.50'.")]
        public decimal? Calificacion { get; set; }

        public string Duracion { get; set; } // Ejemplo: "1h 30min" o "30 min"

        public int? Temporada { get; set; }

        public int? Capitulo { get; set; }
    }
}
