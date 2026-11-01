# ERROR404-
Juan David Mosquera Cabrera
Joan Manuel Londoño Restrepo
Samuel Maya Piedrahita

Nombre del proyecto:DevWork

En la actualidad existen diversas plataformas digitales orientadas a la búsqueda y publicación de oportunidades laborales. Sin embargo, estas plataformas abarcan múltiples sectores económicos y áreas profesionales, por lo que la búsqueda de oportunidades específicas para profesionales del área de sistemas puede resultar poco centralizada y requerir que los usuarios apliquen múltiples filtros o revisen una gran cantidad de ofertas que no necesariamente corresponden con su perfil profesional.

Esta situación puede dificultar la identificación de oportunidades laborales relacionadas específicamente con el desarrollo de software y otras áreas afines a los sistemas, tanto para estudiantes y profesionales que buscan empleo como para empresas que requieren incorporar talento especializado.

Adicionalmente, existe una necesidad diferente relacionada con la contratación de servicios de desarrollo de software. Personas, emprendimientos o empresas que requieren desarrollar una aplicación, página web, sistema de información u otro tipo de solución tecnológica pueden tener dificultades para encontrar profesionales que ofrezcan estos servicios y que se ajusten a las características de su proyecto.

Ante esta situación, se plantea el desarrollo de una plataforma especializada en el área de sistemas, orientada a centralizar las oportunidades laborales y facilitar la conexión entre profesionales y organizaciones. La plataforma también permitirá que personas o empresas interesadas en contratar servicios tecnológicos puedan buscar profesionales de acuerdo con sus conocimientos, habilidades, experiencia y servicios ofrecidos.

1. Instrucciones básicas para ejecutar la soluciónPrerrequisitos:Tener instalado .NET 10 SDK.   Tener configurada y en ejecución una instancia de SQL Server (o LocalDB).Configuración de la Base de Datos:Navegar a la carpeta del proyecto WebAPI.Verificar o actualizar la cadena de conexión (DefaultConnection) en el archivo appsettings.json.Ejecución del Backend (WebAPI):Abrir una terminal en el proyecto WebAPI y ejecutar:dotnet restore
dotnet ef database update
dotnet run Al iniciar, la aplicación aplicará las migraciones y ejecutará automáticamente el SeedDb para poblar los datos iniciales y el usuario administrador

2. Tecnologías utilizadasBackend: ASP.NET Core .NET 10 WebAPI.   Persistencia de Datos: Entity Framework Core (EF Core) con migraciones.   Frontend: Blazor WebAssembly con componentes Razor.   Modelos Compartidos: Biblioteca de clases Shared (.NET Standard / .NET 10).   Pruebas y Documentación API: Swagger UI y Postman.   Control de Versiones: Git y GitHub.

Entidades principales
   

User (Usuario): La cuenta con la que cada persona ingresa al sistema.   


Role (Rol): Controla qué puede hacer cada usuario en la plataforma (por ejemplo: Administrador, Desarrollador o Empleador).   


PerfilProfesional: La hoja de vida del desarrollador. Contiene su presentación, habilidades técnicas, precio por hora y enlaces a GitHub y LinkedIn.   


Educacion, Experiencia y Certificacion: Guardan el detalle de los estudios, empleos anteriores y certificados que respaldan el perfil del candidato.   


Publicacion: Las ofertas de empleo que las empresas publican para buscar talento en sistemas.   


Pregunta: Preguntas de filtro que la empresa agrega a una vacante para conocer mejor a los aspirantes.   


Aplicacion: La propuesta que envía el desarrollador para postularse a un empleo, incluyendo su mensaje y el estado de la solicitud (Pendiente, Aceptada o Rechazada).   


Relaciones entre entidades
Rol 
→ Usuario: Un rol se le asigna a varios usuarios para definir sus permisos.

Usuario 
→ Perfil Profesional: Cada cuenta de desarrollador tiene un único perfil profesional.

Usuario 
→ Publicación: Un usuario empleador puede crear todas las ofertas de empleo que necesite.

Perfil Profesional 
→ Educación / Experiencia / Certificación: Un solo perfil reúne múltiples títulos de estudio, trabajos previos y certificaciones.

Publicación 
→ Pregunta: Una oferta de trabajo puede incluir varias preguntas de filtro.

Publicación 
↔ Perfil Profesional (mediante Aplicación): La entidad Aplicación conecta la oferta laboral con el perfil del desarrollador que se postula a ella.

5. Descripción del SeedDb implementadoLa clase SeedDb inicializa la base de datos automáticamente al arrancar la WebAPI. Realiza las siguientes tareas:   Verificación de duplicados: Comprueba la existencia previa de registros (AnyAsync()) antes de realizar inserciones para evitar duplicaciones cada vez que se ejecute la aplicación.   Carga de datos iniciales del negocio: Pobla al menos dos entidades principales con datos de prueba iniciales (por ejemplo, categorías como Desarrollo de Software y Bases de Datos, y empresas de tecnología iniciales).   Usuario Administrador: Crea las credenciales e información del rol administrativo inicial para permitir el acceso total al sistema desde el primer arranque.

 
 6. Usuario administrador inicial creadoRol: AdministradorUsuario / Email: admin@buscadortrabajo.com (o el correo configurado en tu SeedDb)Contraseña temporal / inicial: Admin123* (o la clave configurada en el SeedDb)

  7. Endpoints principales del WebAPI(Ejemplos de la estructura REST de la API):   Ofertas (/api/offers):GET /api/offers - Obten el listado completo de ofertas laborales.   GET /api/offers/{id} - Obten los detalles de una oferta específica por ID.   POST /api/offers - Registra/Crea una nueva oferta de trabajo.   PUT /api/offers/{id} - Actualiza una oferta existente.   DELETE /api/offers/{id} - Elimina una oferta del sistema.   Empresas (/api/companies):Operaciones CRUD completas (GET, POST, PUT, DELETE) para la gestión de las empresas registradas.   Categorías (/api/categories):Endpoints REST para listar, crear, editar y eliminar categorías de empleo.

 8. Resumen de las funcionalidades implementadasBackend: WebAPI funcional conectado a SQL Server mediante Entity Framework Core, con validaciones de datos en Shared y pruebas documentadas en Swagger.   Base de Datos & SeedDb: Migraciones de base de datos aplicadas correctamente y datos iniciales generados para categorías, empresas y usuario administrador.   Frontend: Aplicación Blazor WebAssembly integrada con el WebAPI que permite listar, crear, editar y eliminar registros (CRUD completo) para las entidades principales del proyecto.   UI/UX: Formularios interactivos con validaciones visuales de campos obligatorios/formatos y mensajes dinámicos de feedback al usuario.

https://youtu.be/5NZIwV0dYi4
