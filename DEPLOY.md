# Despliegue de TechStore en producción (Ubuntu + Docker + Nginx)

La aplicación se ejecuta con **Docker Compose** (app ASP.NET Core + SQL Server) y
se publica detrás de **Nginx** como reverse proxy con HTTPS en
`https://techstore.yanes.xyz`.

- La app escucha dentro del contenedor en el puerto **8080**.
- Se expone solo en **127.0.0.1:8080** del host (no accesible desde fuera salvo por Nginx).
- Las **migraciones se aplican solas** al iniciar (crea la BD, tablas, relación y datos semilla).

---

## 1. Requisitos en el servidor (una sola vez)

Instalar Docker y el plugin de Compose:

```bash
sudo apt update
sudo apt install -y docker.io docker-compose-plugin
sudo systemctl enable --now docker
sudo usermod -aG docker $USER   # luego cierra y vuelve a abrir la sesión SSH
```

> SQL Server para Linux requiere una CPU **x86-64** y ~2 GB de RAM libres.

---

## 2. Obtener el proyecto en el servidor

```bash
cd ~
git clone -b unidad-2 https://github.com/xJonnyxD/techstore.git techstore
cd techstore
```

(En actualizaciones posteriores: `cd ~/techstore && git pull`.)

---

## 3. Configurar la contraseña de la base de datos

```bash
cp .env.example .env
nano .env        # cambia SA_PASSWORD por una contraseña fuerte
```

---

## 4. Levantar la aplicación

```bash
docker compose up -d --build
```

Verifica:

```bash
docker compose ps
docker compose logs -f web      # Ctrl+C para salir
curl -I http://127.0.0.1:8080/  # debe responder 200
```

La primera vez, la app espera a que SQL Server acepte conexiones (reintenta ~60 s)
y luego aplica las migraciones automáticamente.

---

## 5. Nginx (reverse proxy + HTTPS)

Crea el sitio `/etc/nginx/sites-available/techstore.yanes.xyz`:

```nginx
server {
    listen 80;
    server_name techstore.yanes.xyz;

    location / {
        proxy_pass         http://127.0.0.1:8080;
        proxy_http_version 1.1;
        proxy_set_header   Host              $host;
        proxy_set_header   X-Real-IP         $remote_addr;
        proxy_set_header   X-Forwarded-For   $proxy_add_x_forwarded_for;
        proxy_set_header   X-Forwarded-Proto $scheme;
        proxy_set_header   Upgrade           $http_upgrade;
        proxy_set_header   Connection        keep-alive;
    }
}
```

Actívalo y agrega el certificado HTTPS con Let's Encrypt:

```bash
sudo ln -s /etc/nginx/sites-available/techstore.yanes.xyz /etc/nginx/sites-enabled/
sudo nginx -t && sudo systemctl reload nginx
sudo certbot --nginx -d techstore.yanes.xyz    # genera/renueva el certificado
```

Certbot reescribe el bloque anterior para servir en HTTPS (443) y redirigir el 80.

---

## 6. Verificar

Abre **https://techstore.yanes.xyz** — deberías ver TechStore funcionando con los
productos y categorías cargados desde la base de datos.

---

## Comandos útiles

```bash
docker compose logs -f web      # ver logs de la app
docker compose restart web      # reiniciar solo la app
docker compose down             # detener (los datos de la BD se conservan en el volumen)
docker compose up -d --build    # reconstruir tras un git pull
```

## Notas

- Los datos de SQL Server persisten en el volumen `mssql-data` aunque se recreen
  los contenedores.
- Para actualizar la app tras cambios: `git pull && docker compose up -d --build`.
- El archivo `.env` contiene la contraseña y **no** se sube a git.
