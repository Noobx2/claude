// Monitor de acceso a una carpeta con fanotify (Linux).
// Muestra QUÉ proceso (pid y ejecutable) abre, lee o modifica archivos de la carpeta vigilada.
// Compilar:  gcc -Wall -Wextra -O2 -o watch watch.c
// Ejecutar:  sudo ./watch /ruta/a/la/carpeta      (fanotify exige privilegios de administrador)
#define _GNU_SOURCE
#include <errno.h>
#include <fcntl.h>
#include <limits.h>
#include <signal.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <sys/fanotify.h>
#include <time.h>
#include <unistd.h>

static volatile sig_atomic_t running = 1;
static void stop(int sig) { (void)sig; running = 0; }

// Convierte la máscara de eventos en texto legible. El kernel puede FUSIONAR varios eventos
// (mismo proceso y mismo archivo) en uno solo, por eso la máscara puede traer varios bits a la vez.
static void describe(unsigned long long mask, char *out, size_t size) {
    out[0] = '\0';
    if (mask & FAN_OPEN)        strncat(out, "ABRIR ", size - strlen(out) - 1);
    if (mask & FAN_ACCESS)      strncat(out, "LEER ", size - strlen(out) - 1);
    if (mask & FAN_MODIFY)      strncat(out, "ESCRIBIR ", size - strlen(out) - 1);
    if (mask & FAN_CLOSE_WRITE) strncat(out, "CERRAR ", size - strlen(out) - 1);
}

int main(int argc, char **argv) {
    if (argc != 2) { fprintf(stderr, "uso: %s <carpeta>\n", argv[0]); return 2; }
    signal(SIGINT, stop);
    signal(SIGTERM, stop);

    // 1) Crear el "grupo" de notificaciones. CLASS_NOTIF = solo observar (no bloquear)
    int fan = fanotify_init(FAN_CLOEXEC | FAN_CLASS_NOTIF, O_RDONLY | O_LARGEFILE);
    if (fan < 0) { perror("fanotify_init (¿eres root?)"); return 1; }

    // 2) Marcar la carpeta: eventos de ella y de sus hijos
    unsigned long long mask = FAN_OPEN | FAN_ACCESS | FAN_MODIFY | FAN_CLOSE_WRITE | FAN_EVENT_ON_CHILD;
    if (fanotify_mark(fan, FAN_MARK_ADD, mask, AT_FDCWD, argv[1]) < 0) { perror("fanotify_mark"); return 1; }
    fprintf(stderr, "vigilando %s (Ctrl+C para salir)\n", argv[1]);

    char buffer[8192];
    while (running) {
        ssize_t length = read(fan, buffer, sizeof buffer);
        if (length < 0) { if (errno == EINTR) continue; perror("read"); break; }

        // 3) Recorrer los eventos recibidos
        for (struct fanotify_event_metadata *ev = (struct fanotify_event_metadata *)buffer;
             FAN_EVENT_OK(ev, length); ev = FAN_EVENT_NEXT(ev, length)) {
            if (ev->vers != FANOTIFY_METADATA_VERSION) { fprintf(stderr, "versión de fanotify inesperada\n"); return 1; }
            if (ev->fd < 0) continue;                                  // desbordamiento de cola

            char fd_path[64], file[PATH_MAX], exe_path[64], exe[PATH_MAX];
            // El evento trae un descriptor ya abierto: /proc/self/fd/N nos dice qué archivo es
            snprintf(fd_path, sizeof fd_path, "/proc/self/fd/%d", ev->fd);
            ssize_t n = readlink(fd_path, file, sizeof file - 1);
            file[n > 0 ? n : 0] = '\0';
            // Y /proc/<pid>/exe nos dice qué programa lo hizo
            snprintf(exe_path, sizeof exe_path, "/proc/%d/exe", ev->pid);
            n = readlink(exe_path, exe, sizeof exe - 1);
            if (n > 0) exe[n] = '\0'; else strcpy(exe, "(ya terminó)");

            char what[64];
            describe(ev->mask, what, sizeof what);
            time_t now = time(NULL);
            char when[32];
            strftime(when, sizeof when, "%H:%M:%S", localtime(&now));
            printf("%s  pid=%-6d %-22s %s  <-  %s\n", when, ev->pid, what, strrchr(file, '/') ? strrchr(file, '/') + 1 : file, exe);
            fflush(stdout);
            close(ev->fd);                                             // IMPORTANTE: cada evento abre un descriptor
        }
    }
    close(fan);
    return 0;
}
