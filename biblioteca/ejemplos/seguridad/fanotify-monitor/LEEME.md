# Monitor de acceso a carpeta (Linux, fanotify)

Muestra qué proceso abre, lee o escribe archivos de una carpeta.

```bash
gcc -Wall -Wextra -O2 -o watch watch.c
sudo ./watch /ruta/a/la/carpeta
```

Comprobado en Linux (kernel 6.18) como administrador. Límites: es solo de observación y, si un proceso termina antes de consultarlo, el ejecutable sale como "(ya terminó)". Ver el módulo «Roadmap», sección 4B.
