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
