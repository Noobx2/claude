# Android nativo (Kotlin)

Android nativo es la vía directa a **todas** las APIs del teléfono: sensores, cámara, Bluetooth, ubicación, servicios en segundo plano, notificaciones. Se programa en **Kotlin** (el lenguaje recomendado por Google; Java sigue siendo válido) con **Jetpack Compose** para la interfaz, y se construye con **Gradle**.

> **Ojo:** este módulo **no se ha podido ejecutar** desde aquí. El entorno de pruebas no puede descargar el Android Gradle Plugin ni el SDK de Android, y no se pudo instalar el compilador de Kotlin. Todo el código está escrito con construcciones estables y conocidas, pero **compílalo en Android Studio** antes de fiarte de él. Los números de versión de plugins deben copiarse de la plantilla que genere Android Studio.

*Verificado: 2026-10-27 solo a nivel de conocimiento; sin ejecución. Tu tabla declara Android SDK 36.0.0, pero `adb` no está en el PATH y no hay `kotlinc`. Referencia: [developer.android.com](https://developer.android.com/) y [kotlinlang.org](https://kotlinlang.org/docs/).*

## 1. Android nativo frente a Flutter

| | Flutter ([módulo](#flutter)) | Android nativo |
|---|---|---|
| Lenguaje | Dart | Kotlin (o Java) |
| Plataformas | Android, iOS, web, escritorio | Solo Android |
| APIs del dispositivo | Plugins; si falta, *platform channel* con código Kotlin | Directas y completas, la primera vez que salen |
| Interfaz | Propia de Flutter (se dibuja ella misma) | Jetpack Compose (o vistas XML clásicas) |
| Cuándo | Una app para varias plataformas; interfaz personalizada | APIs muy específicas o nuevas de Android, servicios en segundo plano, widgets, rendimiento máximo |

Puedes combinar ambos: una app Flutter con un módulo Kotlin para lo que ningún plugin cubra.

## 2. Lo que necesitas

| Herramienta | Para qué | Estado en tu equipo |
|---|---|---|
| **Android Studio** | IDE oficial: editor, emulador, depurador, gestor de SDK | Pendiente (en el anfitrión con interfaz gráfica; la VM puede no soportar el emulador) |
| **Android SDK** (36) | Plataformas, `build-tools`, `platform-tools` (`adb`) | Declarado, `adb` fuera del PATH |
| JDK | Gradle y el compilador de Kotlin lo necesitan | Java 25 comprobado (Android Studio incluye el suyo) |
| **Dispositivo o emulador** | Ejecutar la app | Pendiente |

> **Ojo:** tu equipo Linux es una máquina virtual. El **emulador de Android** necesita virtualización por hardware, que dentro de una VM suele no estar disponible. Lo más práctico: Android Studio en tu anfitrión, o un teléfono real conectado por USB o Wi-Fi.

### `adb` (Android Debug Bridge)

```bash
adb devices
adb install -r app-debug.apk
adb logcat -s MiApp:D
adb shell
adb reverse tcp:3000 tcp:3000
```

| Comando | Significado |
|---|---|
| `adb devices` | Lista dispositivos y emuladores conectados (`unauthorized` = acepta la depuración en el teléfono) |
| `adb install -r X.apk` | Instala una app; `-r` reinstala conservando los datos |
| `adb logcat` | Registro del sistema; `-s Etiqueta:Nivel` filtra (`V` verbose, `D` debug, `I`, `W`, `E`); `-c` limpia |
| `adb shell` | Terminal del dispositivo |
| `adb push local remoto` / `adb pull remoto local` | Copiar archivos |
| `adb reverse tcp:3000 tcp:3000` | Hace que `localhost:3000` **en el teléfono** apunte al puerto 3000 de tu equipo: la app puede llamar a `http://localhost:3000/api` sin saber tu IP |
| `adb tcpip 5555` / `adb connect IP:5555` | Depuración por Wi-Fi |
| `adb kill-server` / `adb start-server` | Reinicia el servidor de `adb` (escucha en el puerto 5037) |

El puerto 5037 en escucha que aparece en tu informe es muy probablemente este servidor.

## 3. Archivos y convenciones de nombres

| Elemento | Convención |
|---|---|
| Archivos Kotlin | `PascalCase.kt` (suele coincidir con la clase principal) |
| Clases, objetos, interfaces | `PascalCase` |
| Funciones y propiedades | `camelCase` |
| Funciones `@Composable` | `PascalCase` (son «widgets») |
| Constantes | `UPPER_SNAKE_CASE` (`const val MAX = 3`) |
| Paquetes | minúsculas: `com.example.tasks` |
| Recursos (`res/`) | `snake_case` (`ic_launcher.png`, `activity_main.xml`) |

## 4. Kotlin: fundamentos

```kotlin
// Basics.kt
const val MAX_TRIES = 3            // constante en tiempo de compilación

fun add(a: Int, b: Int = 0): Int = a + b       // parámetro con valor por defecto, cuerpo de expresión

fun grade(score: Int): String = when {         // when: el "switch" potente, es una expresión
    score >= 90 -> "A"
    score >= 70 -> "B"
    else -> "C"
}

fun main() {
    val name = "Ana"               // val: no se reasigna (como final)
    var count = 0                  // var: se puede reasignar
    val age: Int = 31
    val height = 1.68              // Double deducido
    count += 1

    println("$name $age $height $count")       // plantillas: $nombre o ${expresión}
    println("${name.uppercase()} mide ${"%.1f".format(height)} m")

    val maybe: String? = null      // el ? permite null (null safety)
    println(maybe?.length)         // ?. acceso seguro
    println(maybe ?: "sin valor")  // ?: valor por defecto (operador Elvis)

    println("42".toInt() + 1)
    println("abc".toIntOrNull())   // null si no es válido

    println(grade(95) + grade(72) + grade(40))
    println(add(2, 3).toString() + " " + add(5) + " " + add(b = 1, a = 2))

    for (i in 1..3) println("vuelta $i")           // 1..3 incluye el 3
    for ((index, letter) in "abc".withIndex()) println("$index $letter")
    var remaining = 3
    while (remaining > 0) remaining--
}
```

| Concepto | Kotlin |
|---|---|
| Variable inmutable / mutable | `val` / `var` |
| Tipos | `Int`, `Long`, `Double`, `Boolean`, `String`, `Char` (todos son objetos; el compilador optimiza) |
| Tipos que admiten `null` | `String?` |
| Condicional como expresión | `if (x) a else b`, `when (x) { … }` |
| Rango | `1..10`, `1 until 10`, `10 downTo 1`, `1..10 step 2` |
| Conversión | Siempre explícita: `n.toDouble()`, `"7".toInt()` |

### Colecciones y funciones de orden superior

```kotlin
data class Employee(
    val id: Int,
    val name: String,
    val department: String,
    val salary: Int,
    val age: Int,
    val active: Boolean,
)

val employees = listOf(
    Employee(1, "Ana", "ventas", 2400, 31, true),
    Employee(2, "Luis", "tecnología", 3600, 28, true),
    Employee(3, "Marta", "tecnología", 4100, 41, false),
    Employee(4, "Pablo", "ventas", 2100, 24, true),
    Employee(5, "Sara", "recursos", 2800, 36, true),
    Employee(6, "Iván", "tecnología", 3300, 33, true),
)

val active = employees.filter { it.active }.map { it.name }          // ["Ana", "Luis", "Pablo", "Sara", "Iván"]
val total = employees.sumOf { it.salary }                             // 18300
val average = employees.map { it.salary }.average()                   // 3050.0
val top3 = employees.sortedByDescending { it.salary }.take(3).map { it.name }
val firstOver40 = employees.firstOrNull { it.age > 40 }?.name         // "Marta"
val allWell = employees.all { it.salary > 2000 }
val anyInactive = employees.any { !it.active }
val names = employees.joinToString(", ") { it.name }
val payroll = employees.groupBy { it.department }
    .mapValues { (_, group) -> group.sumOf { it.salary } }            // {ventas=4500, tecnología=11000, recursos=2800}
```

| Operación | Kotlin |
|---|---|
| Filtrar / transformar | `filter { }`, `map { }` (`it` es el parámetro implícito) |
| Sumar / media | `sumOf { }`, `average()` |
| Ordenar (sin mutar) | `sorted()`, `sortedBy { }`, `sortedByDescending { }` |
| Agrupar | `groupBy { }` |
| Alguno / todos | `any { }`, `all { }` |
| Primero | `first { }`, `firstOrNull { }` |
| Acumular | `fold(inicial) { acc, x -> … }`, `reduce` |
| Listas mutables | `mutableListOf()`, `mutableMapOf()` |

`listOf` y `mapOf` devuelven colecciones de **solo lectura**. `data class` genera `equals`, `hashCode`, `toString`, `copy(...)` y desestructuración.

### Clases, herencia y clases selladas

```kotlin
open class Account(val owner: String, balance: Double = 0.0) {
    var balance: Double = balance
        private set                                     // solo la clase puede modificarlo

    fun deposit(amount: Double) {
        require(amount > 0) { "El importe debe ser positivo" }     // lanza IllegalArgumentException
        balance += amount
    }

    open fun withdraw(amount: Double) {
        check(amount <= balance) { "Saldo insuficiente: $balance" }  // lanza IllegalStateException
        balance -= amount
    }
}

class SavingsAccount(owner: String, balance: Double, val rate: Double) : Account(owner, balance) {
    fun addInterest() = deposit(balance * rate)
}

// Clase sellada: un conjunto cerrado de casos. El "when" debe cubrirlos todos.
sealed interface UiState {
    data object Loading : UiState
    data class Error(val message: String) : UiState
    data class Success(val tasks: List<String>) : UiState
}

fun describe(state: UiState): String = when (state) {
    UiState.Loading -> "Cargando…"
    is UiState.Error -> "Error: ${state.message}"
    is UiState.Success -> "${state.tasks.size} tareas"
}
```

Las clases son `final` por defecto: para heredar hay que marcarlas `open`. Los `object` son *singletons*. Las extensiones (`fun String.shout() = uppercase() + "!"`) añaden métodos a tipos existentes.

### Excepciones y corrutinas

```kotlin
import kotlinx.coroutines.async
import kotlinx.coroutines.awaitAll
import kotlinx.coroutines.coroutineScope
import kotlinx.coroutines.delay
import kotlinx.coroutines.runBlocking

suspend fun fetch(name: String, millis: Long): String {
    delay(millis)                    // suspende sin bloquear el hilo
    return "$name listo"
}

fun main() = runBlocking {
    try {
        val results = coroutineScope {
            listOf(async { fetch("a", 200) }, async { fetch("b", 200) }).awaitAll()   // en paralelo
        }
        println(results)
        "abc".toInt()
    } catch (e: NumberFormatException) {
        println("Error: ${e.message}")
    }
}
```

Las **corrutinas** son la herramienta de asincronía de Kotlin: `suspend` marca funciones que pueden esperar; `launch` y `async` las lanzan en un ámbito (`CoroutineScope`). En Android, se usan con `viewModelScope` y `lifecycleScope`. Requieren la librería `kotlinx-coroutines`.

## 5. Proyecto Android: estructura

Se crea con Android Studio: *New Project → Empty Activity* (Jetpack Compose).

```text
TasksApp/
  settings.gradle.kts            módulos del proyecto
  build.gradle.kts               plugins comunes
  gradle/libs.versions.toml      catálogo de versiones de dependencias
  gradlew, gradlew.bat           Gradle Wrapper (descarga la versión correcta de Gradle)
  gradle/wrapper/gradle-wrapper.properties
  app/
    build.gradle.kts             configuración del módulo: SDK, dependencias, firma
    src/main/
      AndroidManifest.xml        permisos, actividades, nombre de la app
      kotlin/com/example/tasks/  tu código (o java/)
        MainActivity.kt
      res/                       recursos: textos, iconos, colores, temas
    src/test/                    pruebas unitarias (JUnit, en la JVM)
    src/androidTest/             pruebas instrumentadas (en el dispositivo)
```

| Archivo | Qué es |
|---|---|
| `settings.gradle.kts` | Qué módulos hay (`include(":app")`) y de dónde se descargan los plugins |
| `build.gradle.kts` (raíz) | Declara los plugins del proyecto |
| `app/build.gradle.kts` | `compileSdk`, `minSdk`, `targetSdk`, `applicationId`, dependencias |
| `libs.versions.toml` | Lista central de versiones: se usa como `libs.androidx.core.ktx` |
| `AndroidManifest.xml` | Identidad y capacidades: permisos, actividades, servicios |
| `gradle.properties` | Opciones de Gradle (memoria, AndroidX) |
| `local.properties` | **Ruta de tu Android SDK**; específico de tu máquina, **no se versiona** |
| `res/values/strings.xml` | Textos (para traducir) |

Campos clave de `app/build.gradle.kts` (los valores exactos los pone la plantilla de Android Studio):

| Campo | Significado |
|---|---|
| `namespace` / `applicationId` | Identificador único de la app (`com.example.tasks`) |
| `compileSdk` | Versión del SDK con la que se compila (p. ej. 36) |
| `minSdk` | Versión mínima de Android soportada |
| `targetSdk` | Versión para la que está probada la app |
| `versionCode` / `versionName` | Número interno y texto de la versión |
| `buildFeatures { compose = true }` | Activa Jetpack Compose |

### Gradle: lo justo para leer y arreglar proyectos

```bash
./gradlew assembleDebug
./gradlew installDebug
./gradlew test
./gradlew lint
./gradlew clean
./gradlew :app:dependencies
```

| Comando | Significado |
|---|---|
| `./gradlew` | Usa el *wrapper* del proyecto (no instales Gradle a mano) |
| `assembleDebug` | Genera el APK de depuración en `app/build/outputs/apk/debug/` |
| `installDebug` | Instala en el dispositivo conectado |
| `test` | Pruebas unitarias |
| `connectedAndroidTest` | Pruebas instrumentadas en un dispositivo |
| `lint` | Análisis de calidad de Android |
| `clean` | Borra `build/` |
| `--stacktrace` / `--info` | Más detalle cuando falla |
| `--offline` | Sin red |

## 6. Interfaz con Jetpack Compose

```kotlin
@Composable
fun Counter() {
    var count by remember { mutableIntStateOf(0) }          // estado que provoca redibujado
    Button(onClick = { count++ }) {
        Text("Clics: $count")
    }
}

@Composable
fun TaskItem(title: String, done: Boolean, onToggle: () -> Unit, modifier: Modifier = Modifier) {
    Row(modifier = modifier.fillMaxWidth().padding(8.dp), verticalAlignment = Alignment.CenterVertically) {
        Checkbox(checked = done, onCheckedChange = { onToggle() })
        Text(title, textDecoration = if (done) TextDecoration.LineThrough else null)
    }
}

@Composable
fun TaskList(tasks: List<String>) {
    LazyColumn {                                  // lista que construye solo lo visible
        items(tasks) { title -> TaskItem(title, done = false, onToggle = {}) }
    }
}
```

| Concepto | Significado |
|---|---|
| `@Composable` | Función que describe una parte de la interfaz |
| `remember { mutableStateOf(…) }` | Estado que sobrevive a redibujados |
| `Modifier` | Cadena de ajustes: tamaño, relleno, clics |
| *State hoisting* | El estado vive arriba y los hijos reciben valor + `onXxx`, como las props y eventos de React |
| `LazyColumn` | Lista eficiente |

### ViewModel y estado de pantalla

```kotlin
class TasksViewModel : ViewModel() {
    private val _uiState = MutableStateFlow<UiState>(UiState.Loading)
    val uiState: StateFlow<UiState> = _uiState.asStateFlow()     // la UI solo lee

    init { load() }

    fun load() {
        viewModelScope.launch {                                   // corrutina ligada al ciclo de vida
            _uiState.value = UiState.Loading
            _uiState.value = try {
                UiState.Success(listOf("Estudiar Kotlin"))
            } catch (e: Exception) {
                UiState.Error(e.message ?: "Error desconocido")
            }
        }
    }
}
```

El `ViewModel` conserva el estado ante rotaciones de pantalla; `StateFlow` emite cambios que Compose observa (`collectAsStateWithLifecycle()`). Es la misma separación por capas que en las demás tecnologías: pantalla ← estado ← repositorio ← API.

## 7. Acceso al dispositivo

### Permisos

Se declaran en `AndroidManifest.xml` **y** se piden en ejecución cuando son «peligrosos».

```xml
<uses-permission android:name="android.permission.INTERNET" />
<uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" />
<uses-permission android:name="android.permission.CAMERA" />
```

```kotlin
@Composable
fun LocationPermissionButton(onResult: (Boolean) -> Unit) {
    val launcher = rememberLauncherForActivityResult(
        ActivityResultContracts.RequestPermission()
    ) { granted -> onResult(granted) }

    Button(onClick = { launcher.launch(Manifest.permission.ACCESS_FINE_LOCATION) }) {
        Text("Permitir ubicación")
    }
}
```

> **Regla:** pide el permiso en el momento en que el usuario entiende por qué (al pulsar «usar ubicación»), maneja el caso «denegado» y degrada la función en lugar de bloquear la app.

### Sensores

```kotlin
class AccelerometerReader(context: Context) : SensorEventListener {
    private val sensorManager = context.getSystemService(Context.SENSOR_SERVICE) as SensorManager
    private val accelerometer: Sensor? = sensorManager.getDefaultSensor(Sensor.TYPE_ACCELEROMETER)

    var onChange: (x: Float, y: Float, z: Float) -> Unit = { _, _, _ -> }

    fun start() {
        accelerometer?.let { sensorManager.registerListener(this, it, SensorManager.SENSOR_DELAY_UI) }
    }

    fun stop() = sensorManager.unregisterListener(this)      // siempre: ahorra batería

    override fun onSensorChanged(event: SensorEvent) {
        onChange(event.values[0], event.values[1], event.values[2])
    }

    override fun onAccuracyChanged(sensor: Sensor?, accuracy: Int) = Unit
}
```

### Otras APIs habituales

| Necesidad | API / librería |
|---|---|
| Ubicación | `FusedLocationProviderClient` (Google Play Services) |
| Cámara | CameraX (`androidx.camera`) |
| Bluetooth de bajo consumo | `BluetoothLeScanner`, `BluetoothGatt` (permisos `BLUETOOTH_SCAN`, `BLUETOOTH_CONNECT`) |
| USB / dispositivos conectados | `UsbManager` (ver [Hardware](#hardware)) |
| Notificaciones | `NotificationManager` (permiso `POST_NOTIFICATIONS` en Android 13+) |
| Segundo plano | WorkManager; servicios en primer plano |
| Almacenamiento | DataStore, Room (SQLite) |
| Red | OkHttp, Retrofit, Ktor Client |

### Red hacia tu API

La app habla con la misma API de los demás módulos. Con `adb reverse tcp:3000 tcp:3000`, `http://localhost:3000/api` funciona desde el teléfono. El tráfico HTTP sin cifrar está bloqueado por defecto en Android: en desarrollo se declara `android:usesCleartextTraffic="true"` en `<application>` (o un `network_security_config`); en producción, HTTPS.

## 8. Pruebas y depuración

| Qué | Cómo |
|---|---|
| Lógica pura (modelos, ViewModel) | JUnit en `src/test/`, con `./gradlew test` |
| Interfaz | `compose-ui-test` en `src/androidTest/` |
| Depurador | Android Studio: puntos de interrupción y *Debug* |
| Registro | `Log.d("TAG", "mensaje")` y `adb logcat -s TAG:D` |
| Inspección | *Layout Inspector*, *Profiler* (CPU, memoria, red) |

## 9. Empaquetar

- **APK de depuración**: `./gradlew assembleDebug` → instalable con `adb install`.
- **Versión de publicación**: `./gradlew bundleRelease` genera un `.aab` para Google Play; requiere una **clave de firma** que debes guardar fuera del repositorio y con copia de seguridad.
- `R8` reduce y ofusca el código en `release` (`isMinifyEnabled = true`).

## 10. Errores habituales

| Mensaje | Causa | Solución |
|---|---|---|
| `SDK location not found` | Falta `local.properties` con `sdk.dir` | Android Studio lo crea; o `sdk.dir=/ruta/Android/Sdk` |
| `Could not resolve com.android.tools.build:gradle` | Sin acceso a `google()` en `repositories` | Revisa `settings.gradle.kts` y la red |
| `Unsupported class file major version` | JDK incompatible con la versión de Gradle/AGP | Usa el JDK que pide el proyecto |
| `INSTALL_FAILED_UPDATE_INCOMPATIBLE` | Hay una versión con otra firma | `adb uninstall paquete` |
| `device unauthorized` | No se aceptó la depuración USB | Acepta el aviso en el teléfono |
| `SecurityException: Permission Denial` | Falta declarar o conceder un permiso | Manifest + petición en ejecución |
| `NetworkOnMainThreadException` | Red en el hilo principal | Usa corrutinas (`Dispatchers.IO`) |
| `Cleartext HTTP traffic … not permitted` | HTTP sin cifrar | HTTPS, o `usesCleartextTraffic` solo en desarrollo |
| `ActivityNotFoundException` | La actividad no está en el Manifest | Decláralo |
| `Unresolved reference` | Falta `import` o dependencia | Añádelos |

## 11. Ejercicios

> **Ejercicio:** reproduce los ejercicios 1–15 de [lógica y programación](#ejercicios-basicos) en Kotlin, en un archivo `.kt` con `fun main()`.

> **Ejercicio:** crea una app Compose con la lista de tareas guardada en memoria y un `ViewModel` con `StateFlow`; después conéctala a tu API con Retrofit u OkHttp.

> **Ejercicio:** muestra los valores del acelerómetro en pantalla y detén el sensor cuando la app pase a segundo plano.

> **Ejercicio:** pide el permiso de ubicación y muestra latitud y longitud; maneja el caso «denegado» con un mensaje claro.

> **Ejercicio:** usa `adb reverse` para que la app llame a `localhost:3000` y comprueba la conexión con `adb logcat`.

## 12. Documentación oficial

- [Android Developers: empezar](https://developer.android.com/get-started)
- [Aprende Kotlin](https://kotlinlang.org/docs/getting-started.html)
- [Jetpack Compose](https://developer.android.com/compose)
- [Guía de permisos](https://developer.android.com/training/permissions)
- [Sensores](https://developer.android.com/develop/sensors-and-location/sensors/sensors_overview)
- [adb](https://developer.android.com/tools/adb)
