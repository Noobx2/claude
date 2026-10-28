# Biblioteca técnica personal

Una guía de consulta para volver a programar con autonomía: saber **cómo se crea, se ejecuta, se prueba y se mantiene** un proyecto en cada tecnología de tu equipo, de lo básico a lo avanzado.

Está pensada para tenerla abierta mientras programas. Cada módulo tiene ejemplos pequeños y ejecutables, explica **cada comando y cada opción** y termina con ejercicios sin solución.

## Cómo está organizada

- **Índice lateral**: todos los módulos, agrupados por tema.
- **Búsqueda** (arriba): busca en el texto completo de todos los módulos.
- **Bloques de código**: tienen botón *Copiar*. Los comandos son para que **tú** los ejecutes; esta biblioteca no toca tu máquina.
- **Tablas «Pieza / Significado»** bajo cada comando: desglosan cada flag y cada argumento.

## Qué promete cada módulo

1. **Lo esencial primero**, luego lo intermedio, luego lo avanzado.
2. **Nombres de código en inglés, comentarios en español.**
3. **Versiones con fecha** de comprobación.
4. **Comprobado ≠ supuesto**: la cabecera de cada módulo dice qué se ejecutó de verdad y qué no.
5. **Ejemplos mínimos y ejecutables**, no aplicaciones gigantes.

## Leyenda de estados

| Etiqueta | Significado |
|---|---|
| <span class="tag ok">comprobado</span> | Aparece en la salida de un comando de tu informe |
| <span class="tag warn">declarado</span> | Lo indicaste tú, pero ningún comando lo confirma todavía |
| <span class="tag bad">no disponible</span> | No está instalado o no se puede usar hoy |

## Por dónde empezar

| Si quieres… | Ve a… |
|---|---|
| Entender qué tienes instalado y qué falta | [Inventario](#inventario) y [Script de verificación](#entorno-verificacion) |
| Dominar la terminal | [La terminal](#entorno-terminal) → [Comandos, flag a flag](#comandos-explicados) → [Scripts de Bash](#terminal-scripts) |
| Recuperar lógica de programación | [Ejercicios de lógica](#ejercicios-basicos) y [Comparativa de lenguajes](#lenguajes-comparativa) |
| Construir una app web de principio a fin | [HTML5](#html5) → [CSS](#css) → [JavaScript](#js) → [TypeScript](#ts) → [Node](#node) → [MongoDB](#mongo) → [React](#react) o [Vue](#vue) → [Proyecto de tareas](#todo) |
| Capturar, mostrar y gestionar datos en Java o C# sin frameworks | [Java sin frameworks](#java-datos) · [C# sin frameworks](#csharp-datos) |
| Aprender otro lenguaje | [Python](#python), [Java](#java), [C#](#csharp), [Go](#go), [Rust](#rust) |
| Acceder a APIs del móvil | [Dart y Flutter](#flutter), [Android nativo](#android) |
| Controlar hardware del ordenador | [Hardware](#hardware) |
| Entender la seguridad | [Marco ético](#seguridad-marco) → [Redes](#redes) → [Criptografía](#cripto) → [Endurecimiento](#hardening) |
| Resolver una tarea concreta | [Recetario](#recetas) |
| Descifrar un archivo de configuración | [Manifiestos y configuración](#config) |

## Todos los módulos

| Bloque | Módulos |
|---|---|
| **Inicio** | [Inventario](#inventario) · [Ruta de aprendizaje](#ruta) · [Normas](#normas) |
| **Entorno y terminal** | [Terminal](#entorno-terminal) · [Comandos flag a flag](#comandos-explicados) · [PATH y variables](#entorno-path) · [Gestores de versiones](#entorno-versiones) · [Scripts de Bash](#terminal-scripts) · [CLI, streams y binarios](#cli-args-streams) · [Script de verificación](#entorno-verificacion) |
| **Práctica** | [Ejercicios de lógica](#ejercicios-basicos) · [Comparativa de lenguajes](#lenguajes-comparativa) |
| **Web base** | [HTML5](#html5) · [CSS](#css) · [JavaScript](#js) · [TypeScript](#ts) |
| **Stack de apps rápidas** | [Git](#git) · [Node.js](#node) · [Docker](#docker) · [MongoDB](#mongo) · [React](#react) · [Vue](#vue) · [Proyecto de tareas](#todo) |
| **Lenguajes** | [Python](#python) · [Java](#java) · [Java sin frameworks](#java-datos) · [C#](#csharp) · [C# sin frameworks](#csharp-datos) · [Go](#go) · [Rust](#rust) |
| **Dispositivos y hardware** | [Flutter](#flutter) · [Android](#android) · [Hardware](#hardware) |
| **Seguridad y redes** | [Marco ético](#seguridad-marco) · [Redes](#redes) · [Criptografía](#cripto) · [Monitorización](#monitoreo) · [VPN](#vpn) · [Endurecimiento](#hardening) · [Laboratorio](#lab-etico) · [Honeypots](#honeypots) |
| **Datos y avanzado** | [SQL](#sql) · [Anexo A: tipos de BD](#anexo-bd-tipos) · [Anexo B: curso de SQL](#anexo-sql-curso) · [Anexo C: NoSQL y ORM](#anexo-nosql-orm) · [Spring Boot](#spring) · [Recetario](#recetas) · [Configuración](#config) · [Glosario](#glosario) · [Glosario moderno](#glosario-moderno) |

## Meta

Poder crear por tu cuenta un proyecto mínimo en cada stack, ejecutar su *Hola mundo*, entender su estructura y saber dónde consultar cada operación habitual. Después: algoritmos y una lista de tareas de extremo a extremo.

> **Ejercicio:** abre el [script de verificación](#entorno-verificacion), ejecútalo de nuevo cuando instales algo, y compara la salida con el [inventario](#inventario).
