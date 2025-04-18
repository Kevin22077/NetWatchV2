using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetWatchV2.Models
{
    public class ContenidoVisto
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Usuario")]
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        [ForeignKey("Contenido")]
        public int ContenidoId { get; set; }
        public Contenido Contenido { get; set; }

        public DateTime FechaVisto { get; set; }
    }
}