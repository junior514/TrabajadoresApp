using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace TrabajadoresApp.Models
{
    [Table("Trabajadores")]
    [ValidacionDocumento(ErrorMessage = "El número de documento no cumple con el formato requerido para el tipo seleccionado.")]
    public class Trabajador
    {
        [Key]
        public int Id { get; set; }

        [StringLength(20)] // Aumentado para "Carnet Extranjeria"
        [Display(Name = "Tipo Documento")]
        [Required(ErrorMessage = "El tipo de documento es requerido")]
        public string? TipoDocumento { get; set; }

        [StringLength(50)]
        [Display(Name = "Número Documento")]
        [Required(ErrorMessage = "El número de documento es requerido")]
        public string? NumeroDocumento { get; set; }

        [StringLength(500)]
        [Display(Name = "Nombres")]
        [Required(ErrorMessage = "Los nombres son requeridos")]
        public string? Nombres { get; set; }

        [StringLength(1)]
        [Display(Name = "Sexo")]
        [Required(ErrorMessage = "El sexo es requerido")]
        public string? Sexo { get; set; }

        [Display(Name = "Departamento")]
        public int? IdDepartamento { get; set; }

        [Display(Name = "Provincia")]
        public int? IdProvincia { get; set; }

        [Display(Name = "Distrito")]
        public int? IdDistrito { get; set; }

        // Navegación
        [ForeignKey("IdDepartamento")]
        public virtual Departamento? Departamento { get; set; }

        [ForeignKey("IdProvincia")]
        public virtual Provincia? Provincia { get; set; }

        [ForeignKey("IdDistrito")]
        public virtual Distrito? Distrito { get; set; }

        // Propiedad adicional para mostrar el sexo completo
        [NotMapped]
        public string SexoCompleto => Sexo == "M" ? "Masculino" : "Femenino";
    }
}