using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrabajadoresApp.Models
{
    [Table("Provincia")] // Mapear a tabla singular
    public class Provincia
    {
        [Key]
        public int Id { get; set; }

        public int? IdDepartamento { get; set; }

        [Column("NombreProvincia")]
        [StringLength(500)]
        public string? NombreProvincia { get; set; }

        // Navegación
        [ForeignKey("IdDepartamento")]
        public virtual Departamento? Departamento { get; set; }
        public virtual ICollection<Distrito> Distritos { get; set; } = new List<Distrito>();
        public virtual ICollection<Trabajador> Trabajadores { get; set; } = new List<Trabajador>();
    }
}