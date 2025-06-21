using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrabajadoresApp.Models
{
    [Table("Departamento")] // Mapear a tabla singular
    public class Departamento
    {
        [Key]
        public int Id { get; set; }

        [Column("NombreDepartamento")]
        [StringLength(500)]
        public string? NombreDepartamento { get; set; }

        // Navegación
        public virtual ICollection<Provincia> Provincias { get; set; } = new List<Provincia>();
        public virtual ICollection<Trabajador> Trabajadores { get; set; } = new List<Trabajador>();
    }
}