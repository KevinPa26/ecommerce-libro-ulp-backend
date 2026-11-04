# ecommerce-libro-ulp-backend

Proyecto backend para TUDS-26 - Laboratorio de Programación II.

- **Diagrama Entidad-Relación (DER):**  
  [Ver diagrama en Lucidchart](https://lucid.app/lucidchart/0c66a41f-50d8-4e97-9b10-7b6bda7d771b/edit?page=0_0&invitationId=inv_d6f15f92-a569-4629-a0b7-1f3a63f70877#)

---

## 📌 Descripción General del Proyecto

Este proyecto consiste en el desarrollo de una **plataforma web de comercio electrónico especializada en la compra y venta de libros**.

El sistema permite a los usuarios registrarse, iniciar sesión, consultar un catálogo de libros, gestionar sus favoritos y administrar un carrito de compras. También contempla la gestión de usuarios con distintos roles, perfiles con avatar y el registro de compras con su correspondiente detalle.

La aplicación utiliza una arquitectura basada en ASP.NET Core MVC, una base de datos relacional MySQL/TiDB y una Web API protegida mediante autenticación JWT.

## 💡 Problema que Resuelve

El proyecto busca resolver la necesidad de contar con una plataforma centralizada para administrar la comercialización de libros, organizando tanto la información de los productos como las operaciones realizadas por los usuarios.

Entre sus principales objetivos se encuentran:

1. **Administración del catálogo:** permite registrar y consultar libros, sus precios, stock e información bibliográfica.
2. **Gestión de usuarios:** incorpora registro, autenticación, roles y administración del perfil personal.
3. **Carrito y favoritos:** permite guardar libros de interés y gestionar los productos que se desean adquirir.
4. **Registro de compras:** mantiene un historial de operaciones y sus respectivos detalles, conservando el precio de cada libro al momento de la compra.
5. **Seguridad y trazabilidad:** utiliza contraseñas protegidas mediante hash, autenticación JWT y campos de auditoría para registrar determinadas operaciones.

---

## 📋 Matriz de Evidencias de Requerimientos Mínimos

La siguiente matriz identifica dónde se implementa cada requerimiento y qué evidencia permite comprobar su funcionamiento.

Las capturas, GIF y videos se organizarán dentro de la carpeta `docs/evidencias/`, separados por requerimiento.

| Requerimiento mínimo                | Estado      | Enlace al código / controladores                                                                                                                                                                                                                                                                       | Evidencia (captura / GIF / video)         |
| ----------------------------------- | ----------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ | ----------------------------------------- |
| **1. Cuatro tablas y relación 1:N** | ✅ Cumplido | [AppDbContext.cs](Data/DataContext.cs) · [Models/](Models/)                                                                                                                                                                                                                                            | [DER] · `docs/evidencias/01-modelo-relacional/Diagrama-der.png/`                   |
| **2. Login, roles y avatar**        | ✅ Cumplido | [AccountController.cs](Controllers/AccountController.cs) → API · [AuthController.cs](Controllers/AuthController.cs) → Vistas                                                                                                                                                                           | `docs/evidencias/02-login-roles-avatar/`  |
| **3. Manejo de archivos (extra)**   | ✅ Cumplido | [AccountController.cs](Controllers/AccountController.cs) → `ActualizarAvatar()` · Carga y almacenamiento de avatares                                                                                                                                                                                   | `docs/evidencias/03-manejo-archivos/`     |
| **4. ABM en Vue.js + AJAX**         | ✅ Cumplido | [LibrosController.cs](Controllers/LibrosController.cs) · [Index.cshtml](Views/Libros/Index.cshtml) · [Crear.cshtml](Views/Libros/Crear.cshtml) · [Editar.cshtml](Views/Libros/Editar.cshtml) · [Eliminar.cshtml](Views/Libros/Eliminar.cshtml) · [LibroRepository.cs](Repositories/LibroRepository.cs) | `docs/evidencias/04-abm-vue-ajax/`        |
| **5. Paginado por servidor**        | ✅ Cumplido | [UsuariosController.cs](Controllers/UsuariosController.cs) → `Index()`                                                                                                                                                                                                                                 | `docs/evidencias/05-paginado-servidor/`   |
| **6. Selección asíncrona (AJAX)**   | ✅ Cumplido | [CarritosController.cs](Controllers/CarritosController.cs) · [CarritoRepository.cs](Repositories/CarritoRepository.cs) · [ICarritoRepository.cs](Repositories/ICarritoRepository.cs) · [Index.cshtml](Views/Carritos/Index.cshtml)                                                                     | `docs/evidencias/06-seleccion-asincrona/` |
| **7. Web API + JWT**                | ✅ Cumplido | [AccountController.cs](Controllers/AccountController.cs)                                                                                                                                                                                                                                               | `docs/evidencias/07-api-jwt/`             |

**Referencias de estado:**

- ✅ Cumplido: funcionalidad implementada.
- 🟡 En proceso: implementación parcial o pendiente de completar.
- ⬜ Pendiente: falta implementar o documentar la evidencia correspondiente.

> Los estados y las rutas de la matriz deben actualizarse según la implementación final y los archivos reales del repositorio. Los enlaces relativos son orientativos y deben ajustarse si la estructura del proyecto utiliza otras rutas.

---

## 📁 Organización de las evidencias

Se propone organizar las evidencias de la entrega de la siguiente manera:

```text
docs/
├── evidencias/
│   ├── 01-modelo-relacional/
│   │   └── diagrama-der.png
│   │
│   ├── 02-login-roles-avatar/
│   │   ├── login.png
│   │   ├── perfil-avatar.png
│   │   └── roles.png
│   │
│   ├── 03-manejo-archivos/
│   │   └── carga-avatar.gif
│   │
│   ├── 04-abm-vue-ajax/
│   │   └── abm-dinamico.gif
│   │
│   ├── 05-paginado-servidor/
│   │   └── paginado-red.png
│   │
│   ├── 06-seleccion-asincrona/
│   │   └── seleccion-ajax.gif
│   │
│   └── 07-api-jwt/
│       └── autenticacion-postman.png
```

Los nombres de los archivos son ejemplos. Se pueden reemplazar por las capturas y grabaciones reales de cada funcionalidad.

---

## 📋 Listado y Descripción de Entidades

- **`usuarios`**: Almacena la información central de las cuentas del sistema. En este modelo de marketplace, **un usuario tiene un rol dual**: puede actuar como comprador (adquiriendo libros) y simultáneamente como vendedor (publicando su propio catálogo). Controla perfiles, credenciales (`password_hash`), roles y campos de auditoría.
- **`libros`**: Contiene el inventario de productos. Cada libro está vinculado obligatoriamente a un usuario a través de `vendedor_id`, lo que permite que múltiples usuarios ofrezcan libros de forma independiente. Registra datos bibliográficos, stock, precios y estados.
- **`limagenLibro`**: Almacena las rutas digitales (`path`) de las fotografías asociadas a cada libro, permitiendo gestionar una galería por producto y definir cuál actúa como imagen principal.
- **`carrito`**: Tabla temporal que asocia a un usuario con los libros que desea adquirir, registrando la cantidad y la fecha en que se agregaron antes de proceder al pago.
- **`favorito`**: Gestiona la lista de deseos (_wishlists_) de los usuarios, relacionando qué libros han marcado como de su interés.
- **`compras`**: Actúa como el registro maestro de cada transacción de compra finalizada, vinculando al usuario comprador con la fecha en que se efectuó la orden.
- **`detalle_compras`**: Desglosa de manera granular cada orden de compra. Almacena qué libros específicos se compraron, la cantidad, los puntos generados y el **precio unitario histórico** al momento de la transacción.

---

## 🔐 Configuración local con User Secrets

Los secretos de conexión y autenticación se configuran localmente mediante .NET User Secrets. No deben guardarse en el repositorio.

Ejecutar los siguientes comandos desde la carpeta que contiene `ecommerce-libro-ulp-backend.csproj`.

### 1. Conexión a la base de datos

Configurar la conexión a MySQL/TiDB con las credenciales correspondientes al entorno:

```powershell
dotnet user-secrets set "ConnectionStrings:MySqlConnectionString" "Server=TU_SERVIDOR;Port=4000;Database=TU_BASE;User=TU_USUARIO;Password=TU_PASSWORD;SslMode=Required;"
```

La clave debe coincidir con la que lee `Program.cs`.

### 2. Configuración JWT

Generar una clave aleatoria segura para firmar los tokens JWT:

```powershell
$rng = [System.Security.Cryptography.RandomNumberGenerator]::Create()
$bytes = New-Object byte[] 64
$rng.GetBytes($bytes)
$jwtSecret = [Convert]::ToBase64String($bytes)
$rng.Dispose()

dotnet user-secrets set "Jwt:Secret" "$jwtSecret"
```

Configurar el emisor, la audiencia y la duración del token:

```powershell
dotnet user-secrets set "Jwt:Issuer" "ecommerce-libro-ulp-backend"
dotnet user-secrets set "Jwt:Audience" "ecommerce-libro-ulp-client"
dotnet user-secrets set "Jwt:ExpiresMinutes" "60"
```

Para verificar la configuración:

```powershell
dotnet user-secrets list
```

No publicar ni compartir el valor de `Jwt:Secret` o las credenciales de la base de datos.

### 3. Usuarios de prueba

El repositorio incluye el script `scripts/crear-usuarios-prueba.sql`, que crea dos cuentas para probar los permisos y el flujo de autenticación.

| Rol           | Email                     | Contraseña   |
| ------------- | ------------------------- | ------------ |
| Administrador | `admin@ecommerce.local`   | `admin123`   |
| Usuario común | `usuario@ecommerce.local` | `usuario123` |

Los hashes BCrypt correspondientes están definidos en el script. Las contraseñas no se almacenan en texto plano en la base de datos.

**Cómo ejecutar el script:**

1. Configurar y comprobar el acceso a la base de datos.
2. Asegurarse de que el esquema y la tabla `usuarios` ya estén creados.
3. Ejecutar `scripts/crear-usuarios-prueba.sql` sobre la base de datos configurada.
4. Iniciar la aplicación y probar el inicio de sesión con ambas cuentas.

El script evita insertar nuevamente las cuentas si sus correos ya existen.

**Importante:** estas credenciales son exclusivamente para desarrollo y pruebas. No utilizarlas en producción. Si se cambian las contraseñas, también deben generarse y guardarse sus correspondientes hashes BCrypt.

### 4. Ejecutar el proyecto

```powershell
dotnet restore
dotnet run
```

---

## 🛠️ Tecnologías utilizadas

- **Backend:** ASP.NET Core MVC / C#
- **Base de datos:** MySQL / TiDB
- **ORM:** Entity Framework Core
- **Proveedor de base de datos:** Pomelo.EntityFrameworkCore.MySql
- **Autenticación:** JWT y BCrypt
- **Frontend:** HTML, CSS, Bootstrap y JavaScript
- **Interactividad:** Vue.js y AJAX
