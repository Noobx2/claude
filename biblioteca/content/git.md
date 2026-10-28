# Git y GitHub

Git guarda el **historial** de tu proyecto: qué cambió, cuándo y por qué. Te permite volver atrás, probar ideas en ramas y colaborar. GitHub es un servicio que aloja repositorios Git; Git funciona sin él.

*Verificado: 2026-10-27. Tu Git es 2.53.0 y **no tiene configuración global** (no existe `~/.gitconfig`). Empieza por la sección 2. Referencia: [Pro Git](https://git-scm.com/book/es/v2) y [git-scm.com/docs](https://git-scm.com/docs).*

## 1. Ideas clave

```text
 Directorio de trabajo  ──git add──▶  Área de preparación  ──git commit──▶  Repositorio (historial)
 (tus archivos)                        (staging: lo que irá                  (commits guardados)
                                        en el próximo commit)
```

| Término | Significado |
|---|---|
| **Repositorio** | Carpeta con tu proyecto y su historial (la carpeta oculta `.git`) |
| **Commit** | Una «foto» del proyecto con un mensaje y un identificador único (hash) |
| **Rama** (*branch*) | Línea de trabajo independiente; `main` es la principal |
| **HEAD** | Dónde estás ahora (el commit o rama actual) |
| **Remoto** (*remote*) | Copia del repositorio en otro sitio (GitHub); por convención se llama `origin` |
| **Staging** | Zona donde eliges qué cambios entrarán en el siguiente commit |

## 2. Configuración inicial (hazlo una vez)

```bash
git config --global user.name "Tu Nombre"
git config --global user.email "tu-correo@example.com"
git config --global init.defaultBranch main
git config --global pull.rebase false
git config --global core.editor "nano"
git config --global --list
```

| Pieza | Significado |
|---|---|
| `git config` | Lee o escribe configuración |
| `--global` | Para tu usuario (se guarda en `~/.gitconfig`); `--local` solo el repositorio actual |
| `user.name`, `user.email` | Aparecen en cada commit. **Sin ellos Git no deja commitear** |
| `init.defaultBranch main` | Nombre de la rama inicial al crear repositorios |
| `pull.rebase false` | `git pull` hace una fusión (merge); con `true` haría rebase |
| `core.editor` | Editor para mensajes largos |
| `--list` | Muestra toda la configuración |

> **Ojo:** el correo que pongas es público en los commits que subas a GitHub. GitHub ofrece una dirección privada «noreply» en su configuración.

## 3. Tu primer repositorio

```bash
mkdir -p ~/projects/git-practice && cd ~/projects/git-practice
git init
echo "# Mi proyecto" > README.md
git status
git add README.md
git commit -m "Añade README inicial"
git log --oneline
```

| Comando | Significado |
|---|---|
| `git init` | Crea el repositorio (la carpeta `.git`) |
| `git status` | Estado: qué está modificado, preparado o sin seguimiento |
| `git add README.md` | Prepara ese archivo para el siguiente commit |
| `git commit -m "mensaje"` | Guarda un commit; `-m` pone el mensaje en la propia línea |
| `git log --oneline` | Historial en una línea por commit |

## 4. El ciclo diario

```bash
git status -sb                  # estado resumido
git diff                        # cambios aún sin preparar
git add -p                      # elige trozos de cambios para preparar
git add .                       # prepara todo lo de esta carpeta (¡revisa antes con status!)
git diff --staged               # lo que va a entrar en el commit
git commit -m "Corrige la validación del formulario"
git log --oneline --graph --decorate --all -n 15
```

| Pieza | Significado |
|---|---|
| `status -s` | Formato corto; `-b` añade la rama |
| `diff` | Diferencias entre el directorio de trabajo y el staging |
| `diff --staged` | Diferencias entre staging y el último commit (`--cached` es sinónimo) |
| `add -p` | *Patch*: te pregunta trozo a trozo (`y` sí, `n` no, `s` dividir, `q` salir) |
| `add .` | Añade todos los cambios de la carpeta actual |
| `commit -a` | Con `-a`, incluye automáticamente los archivos **ya seguidos** modificados (no los nuevos) |
| `log --oneline` | Un commit por línea |
| `log --graph` | Dibuja el árbol de ramas |
| `log --decorate` | Muestra ramas y etiquetas en cada commit |
| `log --all` | Todas las ramas, no solo la actual |
| `log -n 15` | Máximo 15 commits |

### Cómo escribir un buen mensaje

- Primera línea: en imperativo y de menos de ~72 caracteres. «Añade filtro por estado», no «añadido» ni «cambios».
- Si hace falta, una línea en blanco y un párrafo con el **porqué**.
- Un commit = un cambio con sentido. Evita los «varios arreglos».

## 5. `.gitignore`: lo que no se versiona

Crea `.gitignore` en la raíz:

```text
# Dependencias
node_modules/

# Compilados
dist/
build/
target/
__pycache__/
.venv/

# Secretos y entorno local
.env
.env.*
!.env.example

# Editor y sistema
.vscode/
.DS_Store
*.log
```

| Patrón | Significado |
|---|---|
| `carpeta/` | Ignora esa carpeta |
| `*.log` | Ignora por extensión |
| `.env.*` | Ignora cualquier `.env.algo` |
| `!.env.example` | La `!` **excepción**: este sí se versiona |
| `#` | Comentario |

> **Peligro:** si ya subiste un secreto, borrarlo en un commit posterior **no lo elimina del historial**. Cambia la credencial inmediatamente (revócala y crea otra). Para dejar de seguir un archivo ya versionado: `git rm --cached .env`.

## 6. Ramas

```bash
git switch -c feature/task-filter     # crear y moverte
# ...trabajas y haces commits...
git switch main                        # volver
git merge feature/task-filter          # traer los cambios
git branch -d feature/task-filter      # borrar la rama ya fusionada
```

| Comando | Significado |
|---|---|
| `git branch` | Lista ramas (`-a` incluye remotas, `-vv` detalle) |
| `git switch nombre` | Cambia de rama |
| `git switch -c nombre` | `-c` crea la rama y se mueve a ella |
| `git merge nombre` | Fusiona esa rama en la actual; `--no-ff` fuerza siempre un commit de fusión |
| `git branch -d nombre` | Borra una rama fusionada; `-D` fuerza (pierde commits no fusionados) |
| `git branch -m nuevo` | Renombra la rama actual |

Convención de nombres: `feature/…`, `fix/…`, `docs/…`.

### Conflictos de fusión

Si dos ramas cambian las mismas líneas, Git se detiene y marca el archivo:

```text
<<<<<<< HEAD
const limit = 10;
=======
const limit = 20;
>>>>>>> feature/task-filter
```

1. Abre el archivo y decide el resultado final: borra las marcas `<<<<<<<`, `=======`, `>>>>>>>`.
2. `git add archivo` para marcar el conflicto como resuelto.
3. `git commit` para terminar la fusión (o `git merge --abort` para cancelar).

## 7. Deshacer cosas

| Situación | Comando |
|---|---|
| Descartar cambios **sin preparar** de un archivo | `git restore archivo` |
| Sacar un archivo del staging (conserva los cambios) | `git restore --staged archivo` |
| Corregir el **último** commit (mensaje o archivos) | `git commit --amend` (solo si aún no lo has subido) |
| Deshacer un commit **ya compartido** creando uno nuevo | `git revert <hash>` |
| Mover la rama atrás conservando los cambios en staging | `git reset --soft HEAD~1` |
| Mover la rama atrás conservando los cambios sin preparar | `git reset HEAD~1` (equivale a `--mixed`) |
| Mover la rama atrás **y tirar los cambios** | `git reset --hard HEAD~1` |
| Guardar cambios a medias para cambiar de rama | `git stash push -m "wip"` y luego `git stash pop` |
| Recuperar algo «perdido» | `git reflog` muestra los movimientos recientes de HEAD |

> **Peligro:** `git reset --hard` y `git restore archivo` descartan trabajo sin posibilidad de recuperarlo salvo que estuviera commiteado. `git reflog` salva commits, no cambios sin guardar.

`HEAD~1` significa «el commit anterior al actual»; `HEAD~3`, tres atrás.

## 8. Remotos y GitHub

### Clave SSH para GitHub

```bash
ssh-keygen -t ed25519 -C "tu-correo@example.com" -f ~/.ssh/id_ed25519
cat ~/.ssh/id_ed25519.pub
```

| Pieza | Significado |
|---|---|
| `ssh-keygen` | Genera un par de claves |
| `-t ed25519` | Tipo de algoritmo (moderno y corto) |
| `-C "…"` | Comentario que identifica la clave |
| `-f ruta` | Archivo de salida; la clave **privada** es `id_ed25519` y la **pública** `id_ed25519.pub` |

Pega la clave **pública** (la que acaba en `.pub`) en GitHub → *Settings → SSH and GPG keys*. **Nunca compartas el archivo sin `.pub`.** Protege la privada con una frase de contraseña cuando te la pida. Prueba con `ssh -T git@github.com`.

### Conectar y subir

```bash
git remote add origin git@github.com:USUARIO/mi-proyecto.git
git push -u origin main
git pull
git fetch --prune
git clone git@github.com:USUARIO/mi-proyecto.git
```

| Comando | Significado |
|---|---|
| `git remote add origin URL` | Registra el remoto con el nombre `origin` |
| `git remote -v` | Lista los remotos |
| `git push -u origin main` | Sube la rama; `-u` recuerda el destino para futuros `git push` |
| `git pull` | `fetch` + fusión de los cambios remotos |
| `git fetch` | Trae información del remoto **sin** fusionar; `--prune` elimina referencias a ramas ya borradas |
| `git clone URL` | Copia un repositorio; `--depth 1` solo el último commit (más rápido) |

### Flujo de trabajo con ramas y Pull Request

1. `git switch -c feature/lo-que-sea`
2. Commits pequeños.
3. `git push -u origin feature/lo-que-sea`
4. En GitHub, abre un **Pull Request** hacia `main`: es la propuesta de fusión, donde se revisa el código.
5. Tras fusionarlo: `git switch main`, `git pull`, `git branch -d feature/lo-que-sea`.

## 9. Etiquetas y versiones

```bash
git tag -a v1.0.0 -m "Primera versión estable"
git push origin v1.0.0
git tag
```

`-a` crea una etiqueta anotada (con mensaje y autor). Se usan para marcar versiones (*versionado semántico*: `MAYOR.MENOR.PARCHE`).

## 10. Rebase, en una frase

`git rebase main` (estando en tu rama) vuelve a aplicar tus commits encima de la última versión de `main`, dejando un historial lineal. Nunca hagas rebase de ramas que otros ya hayan descargado. De momento, `merge` es suficiente.

## 11. Errores habituales

| Mensaje | Causa | Solución |
|---|---|---|
| `Author identity unknown` / `Please tell me who you are` | Falta `user.name` / `user.email` | Sección 2 |
| `nothing added to commit but untracked files present` | No hiciste `git add` | `git add archivo` |
| `Your branch is behind 'origin/main'` | Hay cambios remotos | `git pull` |
| `rejected … (non-fast-forward)` al hacer `push` | El remoto tiene commits que no tienes | `git pull` y vuelve a `push` |
| `CONFLICT (content): Merge conflict in …` | Mismas líneas cambiadas | Resolver (sección 6) |
| `Permission denied (publickey)` | GitHub no reconoce tu clave | Revisa que subiste la **pública** y prueba `ssh -T git@github.com` |
| `fatal: not a git repository` | Estás fuera de un repositorio | `cd` a la carpeta correcta o `git init` |
| `detached HEAD` | Estás en un commit, no en una rama | `git switch -c nueva-rama` para conservar el trabajo |

## 12. Ejercicios

> **Ejercicio:** crea un repositorio, haz tres commits, crea una rama, modifica el mismo archivo en `main` y en la rama, y provoca (y resuelve) un conflicto.

> **Ejercicio:** añade `node_modules/` a `.gitignore`, comprueba con `git status` que desaparece, y explica por qué no hay que subirlo.

> **Ejercicio:** genera una clave SSH, súbela a GitHub, crea un repositorio vacío, conéctalo con `git remote add` y haz tu primer `git push -u origin main`.

> **Ejercicio:** por error, commitea un archivo `.env` falso. Sin subirlo, quítalo del último commit con `git rm --cached` y `git commit --amend`. Explica por qué si lo hubieras subido no bastaría.

## 13. Documentación oficial

- [Pro Git (español)](https://git-scm.com/book/es/v2)
- [Referencia de comandos](https://git-scm.com/docs)
- [Documentación de GitHub](https://docs.github.com/es)
