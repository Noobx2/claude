#!/usr/bin/env bash
# Verifica TaskFlow de punta a punta, en un solo comando:
#   compilar → pruebas unitarias e integración → migraciones → API → interfaz Blazor en un navegador real.
# Requisitos: .NET 10 SDK, PostgreSQL en localhost:5432 con el rol «taskflow», Node + Playwright (solo para el último paso).
set -uo pipefail
cd "$(dirname "$0")"

PUERTOS=(5099 5200)
# Apaga lo que escucha en los puertos que usó ESTE script. («dotnet run» lanza un proceso hijo: matar solo al padre dejaría al hijo vivo.)
limpiar() { for puerto in "${PUERTOS[@]}"; do for pid in $(ss -ltnpH "sport = :$puerto" 2>/dev/null | grep -oP 'pid=\K[0-9]+' | sort -u); do kill "$pid" 2>/dev/null; done; done; }
trap limpiar EXIT

paso() { printf '\n== %s ==\n' "$1"; }
falla() { printf '\nFALLO en el paso: %s\n' "$1"; exit 1; }
esperar() { for _ in $(seq 1 40); do curl -s -o /dev/null "$1" && return 0; sleep 0.5; done; return 1; }   # espera a que un servidor responda

export PGPASSWORD=taskflow_dev
PSQL=(psql -h localhost -U taskflow -d taskflow -qAt)

paso "1/6 Compilar toda la solución (Release)"
dotnet build -c Release --nologo -v:q 2>&1 | grep -E "error|Warn|Error" | head -3
dotnet build -c Release --nologo -v:q >/dev/null 2>&1 || falla "compilación"

paso "2/6 Pruebas unitarias y de integración (PostgreSQL real)"
dotnet test -c Release --no-build --nologo 2>&1 | grep -E "Passed!|Failed!" | sed -E 's/, Duration.*//'
dotnet test -c Release --no-build --nologo >/dev/null 2>&1 || falla "pruebas"

paso "3/6 Migraciones: la base se deja al día"
dotnet ef database update --project src/TaskFlow.Infrastructure --startup-project src/TaskFlow.Api --no-build 2>&1 | grep -E "Done|No migrations|already" | head -1 || true
"${PSQL[@]}" -c "select count(*) || ' migraciones aplicadas' from \"__EFMigrationsHistory\"" || falla "migraciones"
"${PSQL[@]}" -c "truncate tasks, users restart identity"          # base limpia para el recorrido

paso "4/6 Arrancar la API (entorno Development) y comprobar su estado"
# OJO: con «dotnet App.dll» la raíz de contenido es la carpeta ACTUAL (no la del programa) y ahí no estaría appsettings.Development.json.
# Por eso se entra primero en la carpeta de la aplicación. («dotnet run» no tiene este problema: usa la carpeta del proyecto.)
(cd src/TaskFlow.Api/bin/Release/net10.0 && ASPNETCORE_ENVIRONMENT=Development exec dotnet TaskFlow.Api.dll --urls http://localhost:5099 >/tmp/tf-api.log 2>&1) &
esperar http://localhost:5099/health || falla "la API no arrancó (mira /tmp/tf-api.log)"
printf 'GET /health → %s\n' "$(curl -s http://localhost:5099/health)"
printf 'GET /tasks sin token → HTTP %s\n' "$(curl -s -o /dev/null -w '%{http_code}' http://localhost:5099/tasks)"

paso "5/6 Arrancar la interfaz Blazor"
(cd src/TaskFlow.Web && dotnet run -c Release --urls http://localhost:5200 >/tmp/tf-web.log 2>&1) &
esperar http://localhost:5200/ || falla "la interfaz no arrancó (mira /tmp/tf-web.log)"
printf 'GET / → HTTP %s\n' "$(curl -s -o /dev/null -w '%{http_code}' http://localhost:5200/)"

paso "6/6 Recorrido de extremo a extremo en Chromium (Playwright)"
NODE_PATH="$(npm root -g)" node e2e/e2e.js || falla "recorrido E2E"

printf '\nTODO CORRECTO\n'
