# Práctica de Laboratorio: Fluent API y Configuraciones

Este repositorio contiene la solución completa a la práctica de Entity Framework Core utilizando Fluent API. El proyecto demuestra cómo separar las reglas de negocio y restricciones de la base de datos fuera de los modelos (clases POCO) y del DbContext, utilizando clases de configuración independientes (`IEntityTypeConfiguration<T>`).

## Instrucciones para descargar y probar el proyecto

Para que el docente o cualquier desarrollador pueda ejecutar y probar este proyecto localmente, seguir estos pasos:

1. **Clonar el repositorio:**
   ```bash
   git clone https://github.com/F4SG/Fluent-API.git
   cd Fluent-API
   ```

2. **Preparar la Base de Datos (SQL Server):**
   Asegúrese de tener una instancia local de SQL Server (por ejemplo, `.\SQLEXPRESS`). 
   El proyecto utiliza una base de datos llamada `HeroesDb`. Si la base de datos y las tablas de Ciudades, Villanos, Cuarteles y Vehiculos no existen, ejecute el script SQL correspondiente en SQL Server Management Studio (SSMS) para crear el esquema y poblarlas con datos de prueba.

3. **Verificar cadena de conexión:**
   Revise el archivo `appsettings.json` y asegúrese de que la cadena de conexión apunte a su instancia local correcta:
   `"Server=localhost\\SQLEXPRESS;Database=HeroesDb;Trusted_Connection=True;TrustServerCertificate=True;"`

4. **Compilar y Ejecutar:**
   Abra el proyecto en Visual Studio y presione **F5** (o ejecute `dotnet run` en la consola).
   
5. **Pruebas de los Endpoints:**
   Con el proyecto en ejecución, puede navegar a las siguientes rutas para comprobar el correcto funcionamiento de las relaciones e inclusiones (.Include):
   - `https://localhost:<puerto>/LaboratorioEf/Ciudad/1`
   - `https://localhost:<puerto>/LaboratorioEf/Cuartel/1`
   - `https://localhost:<puerto>/LaboratorioEf/ModeloVillano`

## Características y Refactorización

- **Modelos Limpios:** Las clases `Cuartel`, `Vehiculo`, `Ciudad` y `Villano` se mantienen como objetos puros de C#, libres de Data Annotations.
- **Configuraciones Aisladas:** Cada entidad posee su propio archivo de configuración dentro de la carpeta `Data/Configurations/`.
- **Carga de Configuraciones por Ensamblado:** El archivo `HeroesContext.cs` ha sido optimizado utilizando el método `ApplyConfigurationsFromAssembly`, lo que permite aplicar automáticamente todas las configuraciones existentes sin necesidad de registrarlas manualmente una por una.
- **Implementación de Controladores:** Se creó `LaboratorioEfController` con endpoints que retornan respuestas en JSON (`/Cuartel/1` y `/Ciudad/1`), haciendo uso de `.Include()` para demostrar el mapeo correcto de las relaciones Uno a Muchos.

## Restricciones Aplicadas a Nivel de Base de Datos

Mediante Fluent API se configuraron de manera exitosa las siguientes restricciones que fueron probadas y validadas en SQL Server:
- `HasColumnName`: Mapeo de propiedades a columnas físicas diferentes (ej. `Placa` a `Nro_Placa`).
- `HasCheckConstraint`: Validaciones a nivel de base de datos (ej. Capacidad > 0, NivelAmenaza entre 1 y 5).
- `IsUnique`: Prevención de duplicados mediante índices únicos (ej. Placa del vehículo).
- `DeleteBehavior.SetNull` y `DeleteBehavior.NoAction`: Comportamiento seguro ante el borrado de registros padre para evitar datos huérfanos o pérdida de historial.
- `HasDefaultValueSql`: Valores automáticos a nivel de motor de base de datos (ej. `GETDATE()`).

## Alumno
- **Nombre:** Franco Andres Salas Gutierrez

---

## Anexo: Respuestas a Preguntas de Comprensión

---------------------------------------------------------
I. PREGUNTAS DE COMPRENSIÓN (Ciudad y Villano)
---------------------------------------------------------

**1. ¿Qué ventaja tiene poner la configuración en clases IEntityTypeConfiguration en vez de escribirla en OnModelCreating?**
Separar la configuración en clases nos ayuda a mantener el archivo DbContext mucho más limpio y ordenado. Si ponemos todo en OnModelCreating, el archivo se hace muy largo y difícil de leer. De esta forma, cada tabla tiene su propio archivo con sus propias reglas.

**2. ¿Qué diferencia hay entre ApplyConfiguration y ApplyConfigurationsFromAssembly?**
ApplyConfiguration sirve para agregar las configuraciones una por una, escribiendo una línea por cada clase que tengamos. Por otro lado, ApplyConfigurationsFromAssembly escanea todo el proyecto y carga todas las configuraciones automáticamente con una sola línea de código.

**3. ¿Por qué falló la consulta al comentar VillanoConfiguration?**
Falló porque en nuestro modelo de C# la propiedad se llamaba NivelAmenaza, pero en la base de datos la columna es Nivel_Amenaza. Al comentar la configuración, Entity Framework intentó buscar la columna con el mismo nombre que en el modelo y, al no encontrarla, lanzó una excepción.

**4. Si HasCheckConstraint no crea la restricción en esta práctica, ¿qué la impone?**
La restricción ya existe y es impuesta directamente por SQL Server, porque se creó mediante el script al construir la base de datos. El uso de HasCheckConstraint en C# solo sirve para informarle a Entity Framework que esa regla existe en la base de datos.

**5. ¿Por qué la relación se configura solo en VillanoConfiguration?**
Porque si la configuramos en ambas clases (Villano y Ciudad) estaríamos duplicando código innecesariamente. Además, configurarla doble puede causar que Entity Framework se confunda y cree llaves foráneas duplicadas. Siempre es mejor hacerlo desde el lado de "muchos", que es donde se guarda la llave foránea.

**6. ¿Qué pasa al intentar borrar una ciudad con villanos, y por qué?**
La base de datos no permitirá borrarla y mostrará un error. Esto sucede porque configuramos el comportamiento de borrado como NoAction, lo que indica que no se puede eliminar la ciudad si todavía existen villanos que dependen de ella, evitando así dejar datos huérfanos.

---------------------------------------------------------
II. PREGUNTAS (Cuarteles y Vehículos)
---------------------------------------------------------

**1. ¿Por qué CuartelId es int? y Villano.CiudadId es int?**
El atributo CuartelId se definió como nullable (int?) porque la regla de negocio indicaba que un vehículo podía quedarse sin cuartel. Sin embargo, CiudadId es un int normal porque la regla establecía que un villano debe pertenecer siempre a una ciudad obligatoriamente, por lo que no puede ser nulo.

**2. ¿Qué diferencia hay entre SetNull y NoAction al borrar el principal?**
Con SetNull, si se borra el cuartel principal, la base de datos no elimina los vehículos, sino que actualiza su llave foránea a NULL. Con NoAction, si se intenta borrar el cuartel y este tiene vehículos asignados, la base de datos detiene la operación y lanza un error.

**3. ¿Por qué el CHECK de capacidad va en CuartelConfiguration y no en VehiculoConfiguration?**
Porque el campo Capacidad pertenece exclusivamente a la tabla de Cuarteles. Cada archivo de configuración debe ocuparse únicamente de las propiedades y restricciones de su propia tabla.

**4. ¿Por qué la prueba de comentar la configuración falla con Placa y no con Modelo?**
Porque la propiedad Modelo tiene el mismo nombre tanto en C# como en SQL Server, por lo que Entity Framework la mapea automáticamente por defecto. Sin embargo, Placa se llama Nro_Placa en la base de datos, así que al quitar la configuración, el programa intenta buscar la columna Placa en SQL, no la encuentra y falla.
