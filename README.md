# GestorTransporteAPI – CRUD con ASP.NET Core Web API

API REST hecha con **ASP.NET Core Web API (Controllers)** que gestiona **Choferes** y **Vehículos** de una flotilla de transporte. Las dos entidades están relacionadas (un chofer tiene muchos vehículos) y se guardan en **SQL Server** con **Entity Framework Core**.

## Tecnologías

- .NET 10 / ASP.NET Core Web API con Controllers
- Entity Framework Core 10 + SQL Server (LocalDB)
- Migraciones de EF Core (Code First)
- OpenAPI + Swagger UI para probar los endpoints

## Estructura del proyecto

```
GestorTransporteAPI.slnx
dotnet-tools.json                  -> herramienta dotnet-ef (local)
GestorTransporteAPI/
├── Context/
│   └── TransporteContext.cs       -> DbContext, configuración Fluent API y datos iniciales
├── Controllers/
│   ├── ChoferesController.cs      -> CRUD de choferes
│   └── VehiculosController.cs     -> CRUD de vehículos
├── DTOs/
│   ├── ChoferDTO.cs               -> respuesta
│   ├── ChoferCreateDTO.cs         -> entrada (POST)
│   ├── ChoferUpdateDTO.cs         -> entrada (PUT)
│   ├── VehiculoDTO.cs             -> respuesta
│   ├── VehiculoCreateDTO.cs       -> entrada (POST)
│   └── VehiculoUpdateDTO.cs       -> entrada (PUT)
├── Migrations/                    -> migración Inicial generada con EF Core
├── Models/
│   ├── Chofer.cs
│   └── Vehiculo.cs
├── Properties/
│   └── launchSettings.json
├── appsettings.json               -> cadena de conexión "DefaultConnection"
├── appsettings.Development.json
├── GestorTransporteAPI.http       -> peticiones de prueba
└── Program.cs                     -> registro de servicios y pipeline
```

## Modelo de datos

**Chofer** (tabla `Choferes`)

| Campo | Tipo | Reglas |
|---|---|---|
| Id | int | Llave primaria (identity) |
| Nombre | nvarchar(100) | Obligatorio |
| Licencia | nvarchar(20) | Obligatoria, **única** |
| Telefono | nvarchar(20) | Opcional |

**Vehiculo** (tabla `Vehiculos`)

| Campo | Tipo | Reglas |
|---|---|---|
| Id | int | Llave primaria (identity) |
| Marca | nvarchar(50) | Obligatoria |
| Modelo | nvarchar(50) | Obligatorio |
| Placa | nvarchar(10) | Obligatoria, **única** |
| Capacidad | int | Entre 1 y 60 asientos |
| ChoferId | int | Llave foránea a `Choferes` (ON DELETE NO ACTION) |

Relación: **Chofer 1 — N Vehiculo**.

## Endpoints

| Método | Ruta | Descripción | Respuestas |
|---|---|---|---|
| GET | `/api/Choferes` | Lista todos los choferes | 200 |
| GET | `/api/Choferes/{id}` | Obtiene un chofer | 200, 404 |
| GET | `/api/Choferes/{id}/vehiculos` | Vehículos asignados a un chofer | 200, 404 |
| POST | `/api/Choferes` | Crea un chofer | 201, 400, 409 |
| PUT | `/api/Choferes/{id}` | Actualiza un chofer | 204, 400, 404, 409 |
| DELETE | `/api/Choferes/{id}` | Elimina un chofer | 204, 404, 409 |
| GET | `/api/Vehiculos` | Lista todos los vehículos | 200 |
| GET | `/api/Vehiculos/{id}` | Obtiene un vehículo | 200, 404 |
| POST | `/api/Vehiculos` | Crea un vehículo | 201, 400, 409 |
| PUT | `/api/Vehiculos/{id}` | Actualiza un vehículo | 204, 400, 404, 409 |
| DELETE | `/api/Vehiculos/{id}` | Elimina un vehículo | 204, 404 |

### Ejemplos de cuerpo (JSON)

Crear chofer:

```json
{
  "nombre": "Carlos Martínez",
  "licencia": "LIC-00003",
  "telefono": "849-555-0303"
}
```

Crear vehículo:

```json
{
  "marca": "Nissan",
  "modelo": "Urvan",
  "placa": "A100003",
  "capacidad": 12,
  "choferId": 1
}
```

## Reglas y validaciones

- Los DTOs de entrada validan campos obligatorios, longitudes y rangos con DataAnnotations. Si algo no cumple, la API responde **400** con el detalle de cada error.
- La licencia del chofer y la placa del vehículo no se pueden repetir: **409 Conflict**.
- Al crear o editar un vehículo, el `choferId` tiene que existir; si no, **400**.
- No se puede eliminar un chofer que todavía tiene vehículos asignados: **409 Conflict**.
- Un Id que no existe devuelve **404 Not Found**.
- La API nunca devuelve las entidades directamente, siempre DTOs de respuesta.


## Pruebas

El archivo `GestorTransporteAPI/GestorTransporteAPI.http` trae peticiones listas para cada endpoint, incluidos los casos de error (400, 404 y 409). Se puede ejecutar desde Visual Studio o VS Code (extensión REST Client).
