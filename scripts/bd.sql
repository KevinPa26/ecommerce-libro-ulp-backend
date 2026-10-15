-- Crear la base de datos
CREATE DATABASE IF NOT EXISTS libreria_db CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE libreria_db;

-- 1. Tabla usuarios
CREATE TABLE usuarios (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    avatar VARCHAR(255),
    rol ENUM('ADMIN', 'USUARIO') NOT NULL DEFAULT 'USUARIO',
    email VARCHAR(150) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    fecha_registro TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    estado ENUM('ACTIVO', 'INACTIVO', 'SUSPENDIDO') NOT NULL DEFAULT 'ACTIVO',
    usuario_creador INT NULL,
    usuario_delete INT NULL
) ENGINE=InnoDB;

-- 2. Tabla libros (con los nuevos campos y los índices en título y autor)
CREATE TABLE libros (
    id INT AUTO_INCREMENT PRIMARY KEY,
    vendedor_id INT NOT NULL,
    isbn13 VARCHAR(13),
    titulo VARCHAR(150) NOT NULL,
    autor VARCHAR(100) NOT NULL,
    descripcion_vendedor TEXT,
    descripcion_libro TEXT,
    precio DECIMAL(10, 2) NOT NULL,
    stock INT NOT NULL DEFAULT 0,
    estado ENUM('DISPONIBLE', 'AGOTADO', 'INACTIVO') NOT NULL DEFAULT 'DISPONIBLE',
    usuario_creador INT NULL,
    usuario_delete INT NULL,
    CONSTRAINT fk_libros_vendedor FOREIGN KEY (vendedor_id) REFERENCES usuarios(id) ON DELETE CASCADE,
    CONSTRAINT fk_libros_creador FOREIGN KEY (usuario_creador) REFERENCES usuarios(id) ON DELETE SET NULL,
    CONSTRAINT fk_libros_delete FOREIGN KEY (usuario_delete) REFERENCES usuarios(id) ON DELETE SET NULL,
    -- Creación de índices solicitados para mejorar el rendimiento de búsqueda
    INDEX idx_titulo (titulo),
    INDEX idx_autor (autor)
) ENGINE=InnoDB;

-- 3. Tabla imagen_libro
CREATE TABLE imagen_libro (
    id INT AUTO_INCREMENT PRIMARY KEY,
    libro_id INT NOT NULL,
    path VARCHAR(255) NOT NULL,
    es_principal BOOLEAN DEFAULT FALSE,
    CONSTRAINT fk_imagen_libro FOREIGN KEY (libro_id) REFERENCES libros(id) ON DELETE CASCADE
) ENGINE=InnoDB;

-- 4. Tabla carrito
CREATE TABLE carrito (
    id INT AUTO_INCREMENT PRIMARY KEY,
    usuario_id INT NOT NULL,
    libro_id INT NOT NULL,
    cantidad INT NOT NULL DEFAULT 1,
    fecha_agregado DATE,
    CONSTRAINT fk_carrito_usuario FOREIGN KEY (usuario_id) REFERENCES usuarios(id) ON DELETE CASCADE,
    CONSTRAINT fk_carrito_libro FOREIGN KEY (libro_id) REFERENCES libros(id) ON DELETE CASCADE
) ENGINE=InnoDB;

-- 5. Tabla favorito
CREATE TABLE favorito (
    id INT AUTO_INCREMENT PRIMARY KEY,
    usuario_id INT NOT NULL,
    libro_id INT NOT NULL,
    fecha_agregado DATE,
    CONSTRAINT fk_favorito_usuario FOREIGN KEY (usuario_id) REFERENCES usuarios(id) ON DELETE CASCADE,
    CONSTRAINT fk_favorito_libro FOREIGN KEY (libro_id) REFERENCES libros(id) ON DELETE CASCADE
) ENGINE=InnoDB;

-- 6. Tabla compras
CREATE TABLE compras (
    id INT AUTO_INCREMENT PRIMARY KEY,
    usuario_id INT NOT NULL,
    fecha_compra DATE NOT NULL,
    CONSTRAINT fk_compras_usuario FOREIGN KEY (usuario_id) REFERENCES usuarios(id) ON DELETE CASCADE
) ENGINE=InnoDB;

-- 7. Tabla detalle_compras
CREATE TABLE detalle_compras (
    id INT AUTO_INCREMENT PRIMARY KEY,
    compra_id INT NOT NULL,
    libro_id INT NOT NULL,
    cantidad INT NOT NULL,
    precio_unitario DECIMAL(10, 2) NOT NULL,
    puntos INT DEFAULT 0,
    CONSTRAINT fk_detalle_compra FOREIGN KEY (compra_id) REFERENCES compras(id) ON DELETE CASCADE,
    CONSTRAINT fk_detalle_libro FOREIGN KEY (libro_id) REFERENCES libros(id) ON DELETE CASCADE
) ENGINE=InnoDB;

-- 8. Llaves foráneas autoreferenciales para la tabla usuarios (auditoría)
ALTER TABLE usuarios
    ADD CONSTRAINT fk_usuario_creador FOREIGN KEY (usuario_creador) REFERENCES usuarios(id) ON DELETE SET NULL,
    ADD CONSTRAINT fk_usuario_delete FOREIGN KEY (usuario_delete) REFERENCES usuarios(id) ON DELETE SET NULL;