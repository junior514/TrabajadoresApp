# Sistema de Mantenimiento de Trabajadores

## 📋 Descripción del Proyecto
Este proyecto fue desarrollado como una prueba técnica que implementa un sistema de mantenimiento web de trabajadores con las siguientes características principales:
- **Listado de trabajadores** con filtros por sexo
- **Creación de nuevos trabajadores** mediante formularios modales
- **Actualización de información** de trabajadores existentes
- **Eliminación de registros** con confirmación de seguridad
- **Validaciones en tiempo real** de documentos de identidad
- **Interfaz responsive** con Bootstrap
- **Diferenciación visual** por género (filas azules para masculino, naranjas para femenino)

## 🛠️ Tecnologías Utilizadas
- **.NET Core 8** - Framework principal
- **Entity Framework Core** - ORM para acceso a datos
- **SQL Server** - Base de datos
- **Bootstrap 5.3.7** - Framework CSS para diseño responsive
- **jQuery** - Interacciones del lado cliente
- **Font Awesome** - Iconografía

## 📁 Estructura del Proyecto
```
TrabajadoresApp/
├── Controllers/
│   └── TrabajadorController.cs      # Controlador principal con toda la lógica CRUD
├── Models/
│   ├── Trabajador.cs               # Modelo principal
│   ├── Departamento.cs             # Modelo de departamento
│   ├── Provincia.cs                # Modelo de provincia
│   └── Distrito.cs                 # Modelo de distrito
├── Views/
│   └── Trabajador/
│       ├── Index.cshtml            # Vista de listado con filtros
│       ├── Create.cshtml           # Vista de creación
│       ├── Edit.cshtml             # Vista de edición
│       └── Delete.cshtml           # Vista de confirmación de eliminación
└── Data/
    └── TrabajadoresContext.cs      # Contexto de Entity Framework
```

## 🗄️ Base de Datos
**Nombre de la base de datos:** `TrabajadoresPrueba`

### Tablas principales:
- **Trabajadores** - Información personal y laboral
- **Departamentos** - Divisiones administrativas nivel 1
- **Provincias** - Divisiones administrativas nivel 2
- **Distritos** - Divisiones administrativas nivel 3

## ⚡ Funcionalidades Implementadas

### ✅ Requisitos Cumplidos
1. **Listado de Trabajadores**
   - Visualización completa de todos los trabajadores registrados
   - Implementado mediante Entity Framework con relaciones incluidas

2. **Crear Trabajador**
   - Formulario completo con validaciones
   - Implementación mediante vista dedicada con diseño responsive

3. **Actualizar Trabajador**
   - Edición de información existente
   - Validaciones para evitar duplicados en documentos

4. **Eliminar Trabajador**
   - Confirmación de eliminación con mensaje "¿Está seguro de eliminar el registro?"
   - Manejo de errores y mensajes informativos

5. **Bonus - Bandeja de Trabajadores Registrados**
   - ✅ **Filtro por Sexo** - Dropdown para filtrar por Masculino/Femenino/Todos
   - ✅ **Código de colores** - Filas azules para masculino, naranjas para femenino

## 🚀 Instalación y Configuración

### Pasos de Instalación
1. **Clonar el repositorio**
   ```bash
   git clone [URL_DEL_REPOSITORIO]
   cd TrabajadoresApp
   ```

2. **Configurar la cadena de conexión**
   ```json
   // En appsettings.json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=TrabajadoresPrueba;User Id=sa;Password=admin;TrustServerCertificate=true;"
     }
   }
   ```

3. **Ejecutar migraciones**
   ```bash
   dotnet ef database update
   ```

4. **Ejecutar la aplicación**
   ```bash
   dotnet run
   ```

## 🎯 Características Destacadas
- Interfaz moderna y responsive
- Validaciones robustas en formularios
- Código limpio y bien estructurado
- Manejo adecuado de errores
- Experiencia de usuario optimizada
- Cumplimiento total de los requisitos técnicos