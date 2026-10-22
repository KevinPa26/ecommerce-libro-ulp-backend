
-- ============================================================
-- Ecommerce Libro ULP - Usuarios de prueba
-- ============================================================
-- SOLO PARA DESARROLLO / PRUEBAS.
--
-- ADMIN
-- Email: admin@ecommerce.local
-- Password: admin123
--
-- USUARIO
-- Email: usuario@ecommerce.local
-- Password: usuario123
-- ============================================================

-- Administrador
INSERT INTO usuarios (
    estado,
    avatar,
    email,
    fecha_registro,
    nombre,
    password_hash,
    rol,
    usuario_creador,
    usuario_delete
)
SELECT
    'ACTIVO',
    NULL,
    'admin@ecommerce.local',
    CURRENT_TIMESTAMP,
    'Administrador',
    '$2a$11$AAL6IVlFbTMq9VtWMRG1SOghjyQ17E0de92CpYR5FLavaLahfkvWa',
    'ADMIN',
    NULL,
    NULL
FROM DUAL
WHERE NOT EXISTS (
    SELECT 1
    FROM usuarios
    WHERE email = 'admin@ecommerce.local'
);

-- Usuario común
INSERT INTO usuarios (
    estado,
    avatar,
    email,
    fecha_registro,
    nombre,
    password_hash,
    rol,
    usuario_creador,
    usuario_delete
)
SELECT
    'ACTIVO',
    NULL,
    'usuario@ecommerce.local',
    CURRENT_TIMESTAMP,
    'Usuario de prueba',
    '$2a$11$Vk40oOeLeMu0/Fz1t6TB8O9LMs8aU/6V7tQAdTAxS7q3zlAJgXxPy',
    'USUARIO',
    NULL,
    NULL
FROM DUAL
WHERE NOT EXISTS (
    SELECT 1
    FROM usuarios
    WHERE email = 'usuario@ecommerce.local'
);

-- ============================================================
-- Fin del script
-- ============================================================
