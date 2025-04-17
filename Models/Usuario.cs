using System.ComponentModel.DataAnnotations;

namespace NetWatchV2.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "El nombre de usuario es requerido.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "El nombre de usuario solo puede contener letras y espacios.")]
        [Display(Name = "Nombre de Usuario")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "El correo electrónico es requerido.")]
        [EmailAddress(ErrorMessage = "Por favor, introduce un correo electrónico válido.")]
        [Display(Name = "Correo Electrónico")]
        public string Correo { get; set; }
        [Required(ErrorMessage = "La contraseña es requerida.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Contrasena { get; set; } // Aquí guardaremos el hash
        public DateTime FechaRegistro { get; set; }
        public bool EsAdmin { get; set; }
    }
}
