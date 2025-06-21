using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TrabajadoresApp.Data;
using TrabajadoresApp.Models;

namespace TrabajadoresApp.Controllers
{
    public class TrabajadorController : Controller
    {
        private readonly TrabajadoresContext _context;

        public TrabajadorController(TrabajadoresContext context)
        {
            _context = context;
        }

        // GET: Trabajador - Con filtro por sexo
        public async Task<IActionResult> Index(string filtroSexo)
        {
            // Crear lista de opciones para el filtro de sexo
            ViewBag.FiltroSexo = new SelectList(new[]
            {
                new { Value = "", Text = "Todos" },
                new { Value = "M", Text = "Masculino" },
                new { Value = "F", Text = "Femenino" }
            }, "Value", "Text", filtroSexo);

            // Obtener trabajadores con includes
            var trabajadoresQuery = _context.Trabajadores
                .Include(t => t.Departamento)
                .Include(t => t.Distrito)
                .Include(t => t.Provincia)
                .AsQueryable();

            // Aplicar filtro si se especifica
            if (!string.IsNullOrEmpty(filtroSexo))
            {
                trabajadoresQuery = trabajadoresQuery.Where(t => t.Sexo == filtroSexo);
            }

            var trabajadores = await trabajadoresQuery.ToListAsync();
            return View(trabajadores);
        }

        // GET: Trabajador/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trabajador = await _context.Trabajadores
                .Include(t => t.Departamento)
                .Include(t => t.Distrito)
                .Include(t => t.Provincia)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trabajador == null)
            {
                return NotFound();
            }

            return View(trabajador);
        }

        // Método para validar el formato del documento
        private bool ValidarDocumento(string tipoDocumento, string numeroDocumento)
        {
            if (string.IsNullOrEmpty(tipoDocumento) || string.IsNullOrEmpty(numeroDocumento))
                return false;

            return tipoDocumento switch
            {
                "DNI" => ValidarDNI(numeroDocumento),
                "Carnet Extranjeria" => ValidarCarnetExtranjeria(numeroDocumento),
                "Pasaporte" => ValidarPasaporte(numeroDocumento),
                _ => false
            };
        }

        private bool ValidarDNI(string numero)
        {
            return numero.Length == 8 && numero.All(char.IsDigit);
        }

        private bool ValidarCarnetExtranjeria(string numero)
        {
            return numero.Length == 9 && numero.All(c => char.IsLetterOrDigit(c));
        }

        private bool ValidarPasaporte(string numero)
        {
            return numero.Length >= 6 && numero.Length <= 12 &&
                   numero.All(c => char.IsLetterOrDigit(c));
        }

        // Método para verificar si ya existe un documento
        private async Task<bool> DocumentoExiste(string numeroDocumento, int? idExcluir = null)
        {
            var query = _context.Trabajadores.Where(t => t.NumeroDocumento == numeroDocumento);

            if (idExcluir.HasValue)
            {
                query = query.Where(t => t.Id != idExcluir.Value);
            }

            return await query.AnyAsync();
        }

        // Método para añadir validaciones personalizadas
        private async Task ValidarTrabajador(Trabajador trabajador, bool esEdicion = false)
        {
            // Validar formato de documento
            if (!ValidarDocumento(trabajador.TipoDocumento, trabajador.NumeroDocumento))
            {
                string mensaje = trabajador.TipoDocumento switch
                {
                    "DNI" => "El DNI debe tener exactamente 8 dígitos.",
                    "Carnet Extranjeria" => "El Carnet de Extranjería debe tener 9 caracteres alfanuméricos.",
                    "Pasaporte" => "El Pasaporte debe tener entre 6 y 12 caracteres alfanuméricos.",
                    _ => "Tipo de documento no válido."
                };
                ModelState.AddModelError("NumeroDocumento", mensaje);
            }

            // Validar que el documento no esté duplicado
            int? idExcluir = esEdicion ? trabajador.Id : null;
            if (await DocumentoExiste(trabajador.NumeroDocumento, idExcluir))
            {
                ModelState.AddModelError("NumeroDocumento", "Ya existe un trabajador con este número de documento.");
            }

            // Validar que los nombres no contengan números
            if (!string.IsNullOrEmpty(trabajador.Nombres) && trabajador.Nombres.Any(char.IsDigit))
            {
                ModelState.AddModelError("Nombres", "Los nombres no pueden contener números.");
            }

            // Validar que los nombres tengan al menos 2 caracteres
            if (!string.IsNullOrEmpty(trabajador.Nombres) && trabajador.Nombres.Trim().Length < 2)
            {
                ModelState.AddModelError("Nombres", "Los nombres deben tener al menos 2 caracteres.");
            }

            // Validar relaciones jerárquicas (Departamento -> Provincia -> Distrito)
            if (trabajador.IdProvincia.HasValue && trabajador.IdDepartamento.HasValue)
            {
                var provincia = await _context.Provincias.FindAsync(trabajador.IdProvincia);
                if (provincia != null && provincia.IdDepartamento != trabajador.IdDepartamento)
                {
                    ModelState.AddModelError("IdProvincia", "La provincia seleccionada no pertenece al departamento indicado.");
                }
            }

            if (trabajador.IdDistrito.HasValue && trabajador.IdProvincia.HasValue)
            {
                var distrito = await _context.Distritos.FindAsync(trabajador.IdDistrito);
                if (distrito != null && distrito.IdProvincia != trabajador.IdProvincia)
                {
                    ModelState.AddModelError("IdDistrito", "El distrito seleccionado no pertenece a la provincia indicada.");
                }
            }
        }

        // Método para cargar las listas de ViewData de forma segura
        private async Task LoadViewDataLists(Trabajador trabajador = null)
        {
            // Crear lista de opciones para Tipo de Documento
            ViewData["TipoDocumento"] = new SelectList(new[]
            {
                new { Value = "DNI", Text = "DNI" },
                new { Value = "Pasaporte", Text = "Pasaporte" },
                new { Value = "Carnet Extranjeria", Text = "Carnet de Extranjería" }
            }, "Value", "Text", trabajador?.TipoDocumento);

            // Crear lista de opciones para Sexo (usando valores de 1 caracter como en el modelo)
            ViewData["Sexo"] = new SelectList(new[]
            {
                new { Value = "M", Text = "Masculino" },
                new { Value = "F", Text = "Femenino" }
            }, "Value", "Text", trabajador?.Sexo);

            // Cargar departamentos de forma segura - CORREGIR nombre de propiedad
            var departamentos = await _context.Departamentos.ToListAsync();
            ViewData["IdDepartamento"] = new SelectList(
                departamentos ?? new List<Departamento>(),
                "Id",
                "NombreDepartamento",
                trabajador?.IdDepartamento
            );

            // Cargar provincias de forma segura - CORREGIR nombre de propiedad
            var provincias = await _context.Provincias.ToListAsync();
            ViewData["IdProvincia"] = new SelectList(
                provincias ?? new List<Provincia>(),
                "Id",
                "NombreProvincia",
                trabajador?.IdProvincia
            );

            // Cargar distritos de forma segura - CORREGIR nombre de propiedad
            var distritos = await _context.Distritos.ToListAsync();
            ViewData["IdDistrito"] = new SelectList(
                distritos ?? new List<Distrito>(),
                "Id",
                "NombreDistrito",
                trabajador?.IdDistrito
            );
        }

        // GET: Trabajador/Create
        public async Task<IActionResult> Create()
        {
            await LoadViewDataLists();
            return View();
        }

        // POST: Trabajador/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TipoDocumento,NumeroDocumento,Nombres,Sexo,IdDepartamento,IdProvincia,IdDistrito")] Trabajador trabajador)
        {
            // Aplicar validaciones personalizadas
            await ValidarTrabajador(trabajador);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(trabajador);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Trabajador creado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al guardar el trabajador: " + ex.Message);
                }
            }

            // Recargar las listas si hay errores de validación
            await LoadViewDataLists(trabajador);
            return View(trabajador);
        }

        // GET: Trabajador/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trabajador = await _context.Trabajadores.FindAsync(id);
            if (trabajador == null)
            {
                return NotFound();
            }

            await LoadViewDataLists(trabajador);
            return View(trabajador);
        }

        // POST: Trabajador/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TipoDocumento,NumeroDocumento,Nombres,Sexo,IdDepartamento,IdProvincia,IdDistrito")] Trabajador trabajador)
        {
            if (id != trabajador.Id)
            {
                return NotFound();
            }

            // Aplicar validaciones personalizadas (indicando que es edición)
            await ValidarTrabajador(trabajador, esEdicion: true);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(trabajador);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Trabajador actualizado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrabajadorExists(trabajador.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        ModelState.AddModelError("", "Error de concurrencia. El registro fue modificado por otro usuario.");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al actualizar el trabajador: " + ex.Message);
                }
            }

            await LoadViewDataLists(trabajador);
            return View(trabajador);
        }

        // GET: Trabajador/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trabajador = await _context.Trabajadores
                .Include(t => t.Departamento)
                .Include(t => t.Distrito)
                .Include(t => t.Provincia)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trabajador == null)
            {
                return NotFound();
            }

            return View(trabajador);
        }

        // POST: Trabajador/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var trabajador = await _context.Trabajadores.FindAsync(id);
                if (trabajador != null)
                {
                    _context.Trabajadores.Remove(trabajador);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Trabajador eliminado exitosamente.";
                }
                else
                {
                    TempData["ErrorMessage"] = "El trabajador no fue encontrado.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al eliminar el trabajador: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        private bool TrabajadorExists(int id)
        {
            return _context.Trabajadores.Any(e => e.Id == id);
        }

        // Métodos AJAX para carga dinámica (opcional)
        [HttpGet]
        public async Task<JsonResult> GetProvinciasByDepartamento(int departamentoId)
        {
            var provincias = await _context.Provincias
                .Where(p => p.IdDepartamento == departamentoId)
                .Select(p => new { Id = p.Id, Nombre = p.NombreProvincia })
                .ToListAsync();

            return Json(provincias);
        }

        [HttpGet]
        public async Task<JsonResult> GetDistritosByProvincia(int provinciaId)
        {
            var distritos = await _context.Distritos
                .Where(d => d.IdProvincia == provinciaId)
                .Select(d => new { Id = d.Id, Nombre = d.NombreDistrito })
                .ToListAsync();

            return Json(distritos);
        }

        // Método AJAX para validar documento en tiempo real
        [HttpGet]
        public async Task<JsonResult> ValidarNumeroDocumento(string numeroDocumento, string tipoDocumento, int? idTrabajador = null)
        {
            var resultado = new { esValido = true, mensaje = "" };

            // Validar formato
            if (!ValidarDocumento(tipoDocumento, numeroDocumento))
            {
                string mensaje = tipoDocumento switch
                {
                    "DNI" => "El DNI debe tener exactamente 8 dígitos.",
                    "Carnet Extranjeria" => "El Carnet de Extranjería debe tener 9 caracteres alfanuméricos.",
                    "Pasaporte" => "El Pasaporte debe tener entre 6 y 12 caracteres alfanuméricos.",
                    _ => "Formato de documento inválido."
                };
                return Json(new { esValido = false, mensaje });
            }

            // Validar duplicado
            if (await DocumentoExiste(numeroDocumento, idTrabajador))
            {
                return Json(new { esValido = false, mensaje = "Ya existe un trabajador con este número de documento." });
            }

            return Json(resultado);
        }
    }
}