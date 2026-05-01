# TicketSystem - Instrucciones de instalación y ejecución

## Requisitos previos
- Visual Studio 2022 o superior
- .NET 8 SDK
- SQL Server Express o LocalDB

## Paso 1 — Clonar el repositorio
Clonar el repositorio y abrir el archivo `TicketSystem.sln` en Visual Studio.

## Paso 2 — Configurar la cadena de conexión
Verificar que el archivo `Presentation/appsettings.json` tenga el connection string correcto:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TicketSystemDB;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

## Paso 3 — Ejecutar las migraciones
Abrir la Consola del Administrador de paquetes, seleccionar **Infrastructure** como proyecto predeterminado y ejecutar el comando Update-Database.
Esto creará la base de datos y todas las tablas automáticamente.

## Paso 4 — Compilar y ejecutar
Compilar la solución con **Ctrl+Shift+B** y ejecutar con **F5**.
La documentación de la API estará disponible en Swagger: `https://localhost:7223/swagger`

## Paso 5 — Ejecutar el frontend
Abrir el archivo `Frontend/index.html` en el navegador con el backend corriendo.

## Notas
- Al ejecutar por primera vez la base de datos se inicializa automáticamente con 1 evento, 2 sectores y 50 butacas por sector.