# ecommerce-libro-ulp-backend
proyecto backend para TUDS-26 Laboratorio de Programación II

- url diagrama ER: 
https://lucid.app/lucidchart/0c66a41f-50d8-4e97-9b10-7b6bda7d771b/edit?page=0_0&invitationId=inv_d6f15f92-a569-4629-a0b7-1f3a63f70877#

## 📌 Descripción General del Proyecto

Este proyecto es el diseño e implementación de la base de datos relacional para una **plataforma web de comercio electrónico (marketplace) especializada en la compra y venta de libros**. 

El sistema cubre todo el ciclo de vida de la interacción de los usuarios en la plataforma: desde el registro y navegación en el catálogo hasta la gestión de favoritos, carritos de compra activos y la posterior facturación e historial de compras.

---

## 💡 Problema que Resuelve

En las plataformas de comercio electrónico orientadas al sector editorial o marketplaces de libros, es necesario estructurar eficientemente la información para garantizar consistencia, escalabilidad e integridad de los datos. Este modelo resuelve los siguientes desafíos:

1. **Gestión Multivendedor e Inventarios:**
   * Permite que múltiples usuarios actúen como vendedores manteniendo un registro claro del stock disponible, precios e información bibliográfica normalizada (ISBN-13, título, autor).
   
2. **Persistencia del Carrito de Compras y Favoritos:**
   * Resuelve el problema de mantener un historial de artículos guardados para más tarde (*wishlists*) y carritos de compras activos por usuario, facilitando la conversión de ventas.

3. **Inmutabilidad y Trazabilidad de Ventas:**
   * Evita inconsistencias históricas al separar las cabeceras de órdenes de compra (`compras`) de sus ítems específicos (`detalle_compras`). Registra el precio unitario exacto al momento de la transacción para proteger la integridad contable frente a futuros cambios de precios en el catálogo.

4. **Auditoría y Trazabilidad:**
   * Incorpora campos de auditoría (`usuario_creador`, `usuario_delete`) para soportar bajas lógicas (*soft delete*) y tener un control de seguridad sobre las modificaciones realizadas por los distintos roles de usuario.

## 📋 Listado y Descripción de Entidades

* **`usuarios`**: Almacena la información central de las cuentas del sistema. En este modelo de marketplace, **un usuario tiene un rol dual**: puede actuar como comprador (adquiriendo libros) y simultáneamente como vendedor (publicando su propio catálogo). Controla perfiles, credenciales (`password_hash`), roles y campos de auditoría.
* **`libros`**: Contiene el inventario de productos. Cada libro está vinculado obligatoriamente a un usuario a través de `vendedor_id`, lo que permite que múltiples usuarios ofrezcan libros de forma independiente. Registra datos bibliográficos, stock, precios y estados.
* **`limagenLibro`**: Almacena las rutas digitales (`path`) de las fotografías asociadas a cada libro, permitiendo gestionar una galería por producto y definir cuál actúa como imagen principal.
* **`carrito`**: Tabla temporal que asocia a un usuario con los libros que desea adquirir, registrando la cantidad y la fecha en que se agregaron antes de proceder al pago.
* **`favorito`**: Gestiona la lista de deseos (*wishlist*) de los usuarios, relacionando qué libros han marcado como de su interés.
* **`compras`**: Actúa como el registro maestro de cada transacción de compra finalizada, vinculando al usuario comprador con la fecha en que se efectuó la orden.
* **`detalle_compras`**: Desglosa de manera granular cada orden de compra. Almacena qué libros específicos se compraron, la cantidad, los puntos generados y el **precio unitario histórico** al momento de la transacción.

---

## 🔗 Descripción de Relaciones (No Triviales)

* **`usuarios` ↔ `libros` (Modelo Marketplace / Vendedor):** 
  * Un usuario de la plataforma puede publicar y gestionar muchos libros para la venta (`vendedor_id`). 
  * Adicionalmente, las llaves de auditoría (`usuario_creador`, `usuario_delete`) conectan con la tabla de usuarios para mantener trazabilidad sobre quién registró o dio de baja lógica un libro en el sistema.

* **`usuarios` ↔ `compras` (Modelo Comprador):** 
  * Un usuario, en su rol de cliente, puede realizar múltiples transacciones u órdenes de compra a lo largo del tiempo. Esto demuestra que **el mismo usuario que vende productos en la plataforma también puede comprar otros**.

* **`libros` ↔ `limagenLibro` (1 a N):** 
  * Un libro ofertado por un usuario vendedor puede tener asociadas múltiples imágenes de galería, pero cada imagen pertenece a un único libro.

* **`usuarios` ↔ `libros` a través de `carrito` y `favorito` (Muchos a Muchos):** 
  * Tablas intermedias que permiten a cualquier usuario gestionar su carro de compras y su lista de deseos de forma independiente al hecho de que él mismo también sea vendedor.

* **`compras` ↔ `detalle_compras` (1 a N):** 
  * Relación maestro-detalle. Una compra general agrupa uno o varios ítems específicos en el detalle, permitiendo procesar transacciones que incluyan libros de diferentes vendedores en una sola orden.
