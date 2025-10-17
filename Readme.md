HospitalSanVicente

Jerónimo Cárdenas, Hooper, Correo, Numero de identidad

Proyecto ASP.NET Core MVC que gestiona doctores, pacientes y citas médicas.
Utiliza Entity Framework Core (ORM) con MySQL para la persistencia de datos y permite la notificación automática por correo electrónico al registrar una nueva cita.

Características principales

CRUD completo para Doctores, Pacientes y Citas.

Validaciones de negocio:

No se permiten documentos duplicados entre pacientes o doctores.

Ni doctores ni pacientes pueden tener más de una cita en la misma fecha y hora.

Las citas no se eliminan, solo se cambian de estado a “Cancelado”.

Vista Details de cada doctor o paciente muestra sus citas asociadas.

Filtro por especialidad en la vista de citas.

Envío automático de correo electrónico al crear una cita, usando SMTP (configurable mediante appsettings.json).

Diseño limpio y responsivo con Bootstrap.

Tecnologías utilizadas

ASP.NET Core MVC 8

Entity Framework Core 8

MySQL

Bootstrap 5

System.Net.Mail (para el envío de correos)

.NET Dependency Injection y Options Pattern para configuración

Requisitos previos

Tener instalado:

.NET 8 SDK

MySQL Server

Visual Studio 2022
o VS Code

Crear una base de datos vacía en MySQL, por ejemplo:

CREATE DATABASE HospitalSanVicente;

Configuración del proyecto
1. Clonar el repositorio
   git clone https://github.com/ingjercar/HospitalSanVicente.git
   cd HospitalSanVicente

2. Configurar la cadena de conexión en appsettings.json
   "ConnectionStrings": {
   "DefaultConnection": "server=localhost;port=3306;database=HospitalSanVicente;user=root;password=tu_contraseña"
   }

3. Configurar el envío de correos

En el mismo archivo appsettings.json, agrega:

"EmailSettings": {
"Host": "smtp.gmail.com",
"Port": 587,
"EnableSSL": true,
"UserName": "tucorreo@gmail.com",
"Password": "tu_clave_de_aplicacion",
"FromName": "Hospital San Vicente"
}


Si usas Gmail, activa la autenticación en dos pasos y genera una Contraseña de Aplicación.

Configuración de Entity Framework Core

El proyecto utiliza EF Core como ORM para manejar las entidades y sus relaciones.

Modelos principales:

Doctor: Id, Name, Document, Specialty, Phone, Email.

Patient: Id, Name, Document, Phone, Email.

Appointment: relaciona Doctor y Patient, incluye fecha, estado y validaciones.

1. Instalar los paquetes de Entity Framework Core para MySQL
   dotnet add package Pomelo.EntityFrameworkCore.MySql
   dotnet add package Microsoft.EntityFrameworkCore.Design

2. Crear la base de datos y aplicar migraciones
   dotnet ef migrations add InitialCreate
   dotnet ef database update


Esto generará las tablas automáticamente a partir de los modelos.

Estructura del proyecto
HospitalSanVicente/
├── Controllers/
│   ├── AppointmentController.cs
│   ├── DoctorController.cs
│   └── PatientController.cs
│
├── Models/
│   ├── Doctor.cs
│   ├── Patient.cs
│   ├── Appointment.cs
│   ├── EmailSettings.cs
│   └── ErrorViewModel.cs
│
├── Views/
│   ├── Appointment/
│   │   ├── Index.cshtml
│   │   ├── Create.cshtml
│   ├── Doctor/
│   │   ├── Index.cshtml
│   │   ├── Details.cshtml
│   ├── Patient/
│   │   ├── Index.cshtml
│   │   ├── Details.cshtml
│
├── Data/
│   └── AppDbContext.cs
│
├── appsettings.json
└── Program.cs

Flujo general de la aplicación

Inicio:
La aplicación inicia en el Index de Appointment, donde se listan las citas actuales.

Creación de citas:
Desde Appointment/Create, se seleccionan un doctor, un paciente y la fecha.

Si alguno tiene otra cita a la misma hora, se muestra un mensaje de error.

Si es válido, se guarda en la base de datos y se envía un correo al paciente.

Gestión de doctores y pacientes:

Se pueden crear, editar y eliminar doctores y pacientes.

En Details, se listan las citas asociadas.

No se permiten documentos duplicados.

Cancelación de citas:

Las citas no se eliminan físicamente; su estado cambia a "Cancelado".

En la vista, las citas canceladas se muestran en rojo.