# Biblioteca técnica personal

Guía de consulta en español (47 módulos) para recuperar autonomía programando.

## Cómo abrirla (sin instalar nada)

1. Descarga o clona este repositorio.
2. Haz doble clic en **`biblioteca/biblioteca.html`**. Es un solo archivo: funciona sin internet y sin servidor (solo las tipografías usan la red; si faltan, se ve igual).

## Qué hay aquí

| Ruta | Contenido |
|---|---|
| `biblioteca.html` | La biblioteca completa en un archivo |
| `index.html` + `content/*.md` | La misma biblioteca en fuentes (necesita servidor: `python3 -m http.server` dentro de esta carpeta) |
| `ejemplos/inventory-java/` | Java sin frameworks: capturar, mostrar y gestionar datos (consola, CSV, Swing, pruebas) |
| `ejemplos/inventory-cs/` | C# sin frameworks: lo mismo (consola, JSON/CSV, WinForms, xUnit) |

## Ejecutar los ejemplos

```bash
# Java (JDK 17+)
cd ejemplos/inventory-java
javac -Xlint:all -d out src/inventory/*.java
java -Dstdout.encoding=UTF-8 -cp out inventory.Main products.csv

# C# (.NET 10 SDK)
cd ejemplos/inventory-cs
dotnet run --project src/Inventory.Console -- products.json
dotnet test
```

La explicación de cada comando y cada opción está en los módulos **Java sin frameworks** y **C# sin frameworks**.
