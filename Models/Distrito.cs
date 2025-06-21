using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrabajadoresApp.Models
{
    [Table("Distrito")] // Mapear a tabla singular
    public class Distrito
    {
        [Key]
        public int Id { get; set; }

        public int? IdProvincia { get; set; }

        [Column("NombreDistrito")]
        [StringLength(500)]
        public string? NombreDistrito { get; set; }

        // Navegación
        [ForeignKey("IdProvincia")]
        public virtual Provincia? Provincia { get; set; }
        public virtual ICollection<Trabajador> Trabajadores { get; set; } = new List<Trabajador>();
    }
}