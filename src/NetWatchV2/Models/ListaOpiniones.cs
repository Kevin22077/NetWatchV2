using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetWatchV2.Models
{
    public class ListaOpiniones
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Usuario")]
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        [ForeignKey("Contenido")]
        public int ContenidoId { get; set; }
        public Contenido Contenido { get; set; }

        [Required(ErrorMessage = "La calificación de la opinión es requerida.")]
        public string CalificacionOpinion { get; set; }

        [DataType(DataType.MultilineText)]
        public string OpinionTexto { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}