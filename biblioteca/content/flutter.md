# Dart y Flutter

**Dart** es un lenguaje estático, con *null safety* y sintaxis cercana a Java, C# y TypeScript. **Flutter** es el framework de interfaces de Google que lo usa para crear aplicaciones nativas desde un único código para Android, iOS, web, Windows, macOS y Linux. Con sus **plugins** accedes a las APIs del dispositivo (sensores, cámara, ubicación, Bluetooth, almacenamiento) y, cuando falta un plugin, puedes llamar a código nativo (Kotlin/Swift) con *platform channels*.

*Verificado: 2026-10-27 con Flutter 3.47.8 y Dart 3.13.7 (las versiones de tu tabla): los ejemplos pasan `dart analyze`/`flutter analyze`, se ejecutan, tienen pruebas de widgets que pasan y la app se compiló para web y llamó a la API. No se pudo ejecutar en un teléfono ni en un emulador de Android. Referencia: [dart.dev](https://dart.dev/) y [docs.flutter.dev](https://docs.flutter.dev/).*

## 1. Para qué sirve

| Uso | Ejemplo |
|---|---|
| Apps móviles multiplataforma | Una base de código para Android e iOS |
| Apps que usan el dispositivo | Cámara, GPS, acelerómetro, Bluetooth, notificaciones |
| Aplicaciones de escritorio y web | `flutter run -d linux`, `flutter build web` |
| Prototipos rápidos de interfaz | *Hot reload*: ves el cambio sin reiniciar |

## 2. Instalación y versiones

Tu equipo declara Flutter 3.47.8 con FVM 4.3.2 (verificado) y Dart 3.13.7. `dart` y `flutter` no están en el PATH: se llaman a través de **FVM**, que fija la versión por proyecto.

```bash
fvm --version
fvm list
fvm install 3.47.8
cd ~/projects/mi-app
fvm use 3.47.8
fvm flutter --version
fvm flutter doctor
fvm dart --version
```

| Comando | Significado |
|---|---|
| `fvm install X` | Descarga esa versión de Flutter |
| `fvm use X` | La fija para el proyecto actual (crea `.fvmrc`) |
| `fvm list` | Versiones instaladas |
| `fvm flutter …` | Ejecuta `flutter` con la versión del proyecto |
| `fvm dart …` | Ejecuta el `dart` que viene con ese Flutter |
| `flutter doctor` | Diagnostica el entorno: SDK, Android, navegadores, dispositivos |
| `flutter doctor -v` | Detallado |

> **Ojo:** en un proyecto con FVM escribe `fvm flutter run`, no `flutter run`. VS Code puede usar el SDK de `.fvm/flutter_sdk` si lo indicas en la extensión de Flutter.

`flutter doctor` te dirá qué falta. Para Android necesitas el **Android SDK** (declarado en tu tabla pero `adb` no está en el PATH) y, para ejecutar en Linux de escritorio, `clang`, `cmake`, `ninja-build`, `pkg-config` y las bibliotecas de GTK 3 (ya tienes `clang` y `cmake`). Instálalos solo cuando `doctor` lo pida. Con `flutter config --android-sdk RUTA` indicas dónde está el SDK de Android.

## 3. Archivos y convenciones de nombres

| Elemento | Convención |
|---|---|
| Archivos | `snake_case.dart` |
| Clases, enums y typedefs | `UpperCamelCase` |
| Variables, métodos y funciones | `lowerCamelCase` |
| Constantes | `lowerCamelCase` (`const maxTries = 3;`) |
| Paquetes | `snake_case` |
| Privado | Prefijo `_` (privado **al archivo**, no a la clase) |
| Formato | `dart format` (obligatorio en la práctica) |

## 4. Hola mundo en Dart

```bash
cd ~/projects
fvm dart create -t console hello_dart
cd hello_dart
fvm dart run
```

```dart
// bin/hello_dart.dart
void main() {
  print('Hola mundo');
}
```

| Pieza | Significado |
|---|---|
| `dart create -t console` | Proyecto de línea de comandos; `-t` elige plantilla (`package`, `server-shelf`…) |
| `dart run` | Compila y ejecuta `bin/…dart` |
| `void main()` | Punto de entrada |
| `print` | Imprime con salto de línea |

## 5. Variables, tipos y conversiones

```dart
// basics.dart: variables, tipos, condiciones, bucles y funciones
const maxTries = 3; // constante en tiempo de compilación

int add(int a, [int b = 0]) =>
    a + b; // parámetro posicional opcional con valor por defecto

String greet({required String name, int times = 1}) =>
    List.filled(times, 'Hola $name').join(' '); // parámetros con nombre

String? lookup(String key) =>
    key == 'ana' ? 'Ana' : null; // devuelve null si no existe

String grade(int score) => switch (score) {
  >= 90 => 'A',
  >= 70 => 'B',
  _ => 'C',
};

void main() {
  String name = 'Ana';
  int age = 31;
  double height = 1.68;
  bool student = false;
  var inferred = 42; // tipo deducido: int
  final created = DateTime(
    2026,
    10,
    27,
  ); // final: se asigna una sola vez, en ejecución
  String? maybe = lookup('pedro'); // el ? permite null (null safety)

  print('$name $age $height $student $inferred $maxTries ${created.year}');
  print('$name mide ${height.toStringAsFixed(1)} m'); // ${...} para expresiones
  print(maybe ?? 'sin valor'); // ?? valor por defecto
  print(maybe?.length); // ?. no falla si es null

  // Conversiones
  print(int.parse('42') + 1);
  print(int.tryParse('abc')); // null si no es válido
  print(7 / 2); // 3.5: en Dart "/" siempre es decimal
  print(7 ~/ 2); // 3: "~/" es la división entera
  print(7 % 2);
  print('${7.toString()}!');

  print('${grade(95)}${grade(72)}${grade(40)}');
  print('${add(2, 3)} ${add(5)}');
  print(greet(name: 'Ana', times: 2));

  for (var i = 1; i <= 3; i++) {
    print('vuelta $i');
  }
  for (final letter in ['a', 'b']) {
    print(letter);
  }
  var remaining = 3;
  while (remaining > 0) {
    remaining--;
  }

  // Funciones como valores
  int square(int x) => x * x;
  print([1, 2, 3].map(square).toList());

  // Records (tuplas con nombre) y desestructuración
  final (x, y) = (3, 4);
  final point = (x: 3, y: 4);
  print('$x $y ${point.x}');
}
```

| Tipo | Ejemplo | Notas |
|---|---|---|
| `int`, `double`, `num` | `42`, `1.5` | `7 / 2` es `3.5`; `7 ~/ 2` es `3` |
| `String` | `'texto'` | `'Hola $name'`, `'${a + b}'` |
| `bool` | `true` | |
| `List<T>`, `Set<T>`, `Map<K,V>` | `[1,2]`, `{1,2}`, `{'a':1}` | Literales |
| `T?` | `String?` | Admite `null`; sin `?` nunca es `null` (**null safety**) |
| `var` | `var n = 42;` | Tipo deducido |
| `final` | `final x = f();` | Se asigna una vez, en ejecución |
| `const` | `const x = 3;` | Constante en tiempo de compilación |
| `dynamic` | | Desactiva la comprobación; evítalo |
| Record | `(int, int)` | Tupla ligera con nombre opcional |

Operadores de null safety: `?.` (acceso seguro), `??` (valor por defecto), `??=` (asignar si es nulo), `!` (afirmar que no es nulo; evítalo).

## 6. Colecciones

```dart
// collections.dart: List, Map, Set y métodos funcionales con el ejemplo de empleados
class Employee {
  const Employee(
    this.id,
    this.name,
    this.department,
    this.salary,
    this.age,
    this.active,
  );

  final int id;
  final String name;
  final String department;
  final int salary;
  final int age;
  final bool active;
}

void main() {
  const employees = [
    Employee(1, 'Ana', 'ventas', 2400, 31, true),
    Employee(2, 'Luis', 'tecnología', 3600, 28, true),
    Employee(3, 'Marta', 'tecnología', 4100, 41, false),
    Employee(4, 'Pablo', 'ventas', 2100, 24, true),
    Employee(5, 'Sara', 'recursos', 2800, 36, true),
    Employee(6, 'Iván', 'tecnología', 3300, 33, true),
  ];

  final active = employees.where((e) => e.active).map((e) => e.name).toList();
  print(active);

  final total = employees.fold<int>(0, (sum, e) => sum + e.salary);
  print('$total ${total / employees.length}');

  // Ordenar una copia: sort() modifica la lista
  final sorted = [...employees]..sort((a, b) => b.salary.compareTo(a.salary));
  print(sorted.take(3).map((e) => e.name).toList());

  final firstOver40 = employees.where((e) => e.age > 40).firstOrNull?.name;
  print(
    '$firstOver40 ${employees.every((e) => e.salary > 2000)} ${employees.any((e) => !e.active)}',
  );
  print(employees.map((e) => e.name).join(', '));

  // Agrupar
  final payroll = <String, int>{};
  for (final e in employees) {
    payroll.update(
      e.department,
      (value) => value + e.salary,
      ifAbsent: () => e.salary,
    );
  }
  print(payroll);

  // collection if / for: construir listas con lógica dentro del literal
  final labels = [
    for (final e in employees)
      if (e.active) '${e.name} (${e.age})',
  ];
  print(labels.take(2).toList());

  final numbers = [3, 1, 4, 1, 5, 9, 2, 6, 5, 3, 5];
  final unique = numbers.toSet().toList()..sort();
  final frequency = <int, int>{};
  for (final n in numbers) {
    frequency[n] = (frequency[n] ?? 0) + 1;
  }
  print('$unique $frequency');

  final ages = {'Ana': 31, 'Luis': 28};
  ages['Sara'] = 36;
  ages.forEach((name, age) => print('$name $age'));
  print(ages['Pedro'] ?? 'desconocido');
}
```

| Operación | Dart |
|---|---|
| Transformar | `.map((e) => …).toList()` |
| Filtrar | `.where((e) => …)` |
| Acumular | `.fold(inicial, (acc, e) => …)`, `.reduce(…)` |
| Alguno / todos | `.any(…)`, `.every(…)` |
| Primero | `.firstWhere(…)`, `.firstOrNull` |
| Ordenar | `.sort((a, b) => …)` (**modifica**; copia antes con `[...lista]`) |
| Quitar repetidos | `.toSet()` |
| Unir texto | `.join(', ')` |
| Construir con lógica | `[for (final x in xs) if (cond) …]` |

Los métodos como `map` y `where` son **perezosos**: devuelven un `Iterable`; usa `.toList()` para materializar.

## 7. Clases, errores y JSON

```dart
// classes_errors.dart: clases, herencia, enums, excepciones y JSON
import 'dart:convert';
import 'dart:io';

class InsufficientFundsException implements Exception {
  InsufficientFundsException(this.message);
  final String message;

  @override
  String toString() => 'InsufficientFundsException: $message';
}

class Account {
  Account(this.owner, [double balance = 0]) : _balance = balance;

  final String owner;
  double _balance; // el guion bajo hace el miembro PRIVADO a nivel de archivo

  double get balance => _balance;

  void deposit(double amount) {
    if (amount <= 0) {
      throw ArgumentError('El importe debe ser positivo');
    }
    _balance += amount;
  }

  void withdraw(double amount) {
    if (amount > _balance) {
      throw InsufficientFundsException('Saldo insuficiente: $_balance');
    }
    _balance -= amount;
  }

  @override
  String toString() => '$owner: ${_balance.toStringAsFixed(2)} €';
}

class SavingsAccount extends Account {
  SavingsAccount(super.owner, super.balance, this.rate);
  final double rate;
  void addInterest() => deposit(balance * rate);
}

enum Status { todo, doing, done } // enums

class Task {
  Task({required this.id, required this.title, this.done = false});

  // Constructor de fábrica: crear desde un mapa JSON
  factory Task.fromJson(Map<String, dynamic> json) => Task(
    id: json['id'] as int,
    title: json['title'] as String,
    done: json['done'] as bool? ?? false,
  );

  final int id;
  final String title;
  final bool done;

  Map<String, dynamic> toJson() => {'id': id, 'title': title, 'done': done};

  Task copyWith({String? title, bool? done}) =>
      Task(id: id, title: title ?? this.title, done: done ?? this.done);

  @override
  String toString() => 'Task($id, $title, $done)';
}

void main() {
  final account = SavingsAccount('Ana', 1000, 0.02)
    ..deposit(500) // cascada: encadena llamadas sobre el mismo objeto
    ..addInterest();
  print(account);
  try {
    account.withdraw(5000);
  } on InsufficientFundsException catch (e) {
    print('No se pudo: ${e.message}');
  } catch (e) {
    print('Otro error: $e');
  } finally {
    print('finally se ejecuta siempre');
  }

  print('${Status.doing.name} ${Status.values.length}');

  // JSON
  final task = Task(id: 1, title: 'Estudiar Dart');
  final text = jsonEncode(task.toJson());
  print(text);
  final back = Task.fromJson(jsonDecode(text) as Map<String, dynamic>);
  print(back.copyWith(done: true));

  try {
    jsonDecode('{"id":');
  } on FormatException catch (e) {
    print('JSON inválido: ${e.message}');
  }

  // Archivos
  final file = File('demo.txt')..writeAsStringSync('uno\ndos\n');
  print(file.readAsLinesSync());
  file.deleteSync();
}
```

- Constructores con parámetros con nombre (`{required this.id}`) hacen el código legible.
- `factory` permite constructores que devuelven una instancia existente o transformada (`Task.fromJson`).
- `get` define propiedades calculadas.
- La cascada `..` encadena llamadas sobre el mismo objeto.
- `jsonEncode` / `jsonDecode` (de `dart:convert`) convierten entre texto JSON y `Map`/`List`. No hay serialización automática sin código generado (`json_serializable`): se escribe `fromJson`/`toJson` a mano en proyectos pequeños.

## 8. Asincronía

```dart
// async_demo.dart: Future, async/await, Stream e isolates
import 'dart:async';
import 'dart:isolate';

Future<String> fetch(String name, int milliseconds) async {
  await Future.delayed(Duration(milliseconds: milliseconds));
  return '$name listo';
}

Stream<int> countdown(int from) async* {
  for (var i = from; i > 0; i--) {
    await Future.delayed(const Duration(milliseconds: 20));
    yield i; // emite un valor
  }
}

int heavySum(int n) {
  var total = 0;
  for (var i = 0; i < n; i++) {
    total += i;
  }
  return total;
}

Future<void> main() async {
  final watch = Stopwatch()..start();
  final results = await Future.wait([
    fetch('a', 200),
    fetch('b', 200),
    fetch('c', 200),
  ]);
  print('$results ${watch.elapsedMilliseconds ~/ 100}00 ms aprox.');

  await for (final value in countdown(3)) {
    print(value);
  }

  // Isolate: otro "hilo" con memoria propia (para cálculo pesado sin bloquear la interfaz)
  final result = await Isolate.run(() => heavySum(1000000));
  print(result);

  try {
    await Future.error(StateError('fallo'));
  } on StateError catch (e) {
    print('capturado: ${e.message}');
  }
}
```

| Concepto | Significado |
|---|---|
| `Future<T>` | Valor que llegará más tarde (como `Promise`) |
| `async` / `await` | Espera sin bloquear |
| `Future.wait([…])` | Espera varias en paralelo |
| `Stream<T>` | Secuencia de valores a lo largo del tiempo (`async*`, `yield`; se consume con `await for` o `.listen`) |
| `Isolate.run(…)` | Cálculo pesado en otro hilo con memoria propia: **evita congelar la interfaz** |

Dart es de un solo hilo por *isolate*; no hay memoria compartida entre ellos.

## 9. Flutter: ideas clave

En Flutter **todo es un widget**: botones, texto, márgenes, la propia aplicación. La interfaz es un árbol de widgets que se describe en `build`.

| Concepto | Significado |
|---|---|
| `StatelessWidget` | Widget sin estado propio: depende solo de sus parámetros |
| `StatefulWidget` + `State` | Widget con estado mutable; `setState(() {…})` pide redibujar |
| `build(BuildContext context)` | Devuelve el árbol de widgets; se puede ejecutar muchas veces |
| `Row`, `Column`, `Stack` | Disposición horizontal, vertical y apilada |
| `Expanded`, `Flexible`, `SizedBox`, `Padding`, `Center` | Espacio y tamaño |
| `ListView.builder` | Lista que construye solo los elementos visibles |
| `Scaffold` | Estructura de pantalla (barra superior, cuerpo, botón flotante) |
| `Navigator` / `go_router` | Navegación entre pantallas |
| `ThemeData` | Colores y estilos (Material 3) |

> **Regla:** los widgets son baratos y se reconstruyen a menudo. No guardes datos en variables locales de `build`; ponlos en el `State` o en un controlador. Libera siempre controladores y suscripciones en `dispose`.

## 10. Crear el proyecto

```bash
cd ~/projects
fvm flutter create --platforms=android,linux,web --project-name tasks_app tasks_app
cd tasks_app
fvm use 3.47.8
fvm flutter pub add http
fvm flutter run -d linux
```

| Pieza | Significado |
|---|---|
| `flutter create` | Genera un proyecto de ejemplo |
| `--platforms=android,linux,web` | Plataformas a incluir (puedes añadir más después con `flutter create .`) |
| `--project-name` | Nombre del paquete (`snake_case`) |
| `-e` / `--empty` | Genera un `main.dart` casi vacío |
| `flutter pub add http` | Añade una dependencia a `pubspec.yaml` y la descarga |
| `flutter run -d X` | Ejecuta en el dispositivo X (`flutter devices` los lista; `chrome`, `linux`, id del móvil) |

Estructura:

```text
tasks_app/
  pubspec.yaml       manifiesto: nombre, SDK, dependencias, assets
  pubspec.lock       bloqueo de versiones (se versiona en aplicaciones)
  analysis_options.yaml   reglas del analizador
  lib/               TODO tu código Dart
    main.dart        punto de entrada
  test/              pruebas
  android/           proyecto nativo de Android (Gradle, Kotlin)
  linux/  web/       proyectos de cada plataforma
  build/             generado (no se versiona)
```

| Archivo | Qué es | ¿Se edita? | ¿Git? |
|---|---|---|---|
| `pubspec.yaml` | **Manifiesto**: dependencias, versión, assets, fuentes | Sí (o con `flutter pub add`) | Sí |
| `pubspec.lock` | **Bloqueo** | No | Sí |
| `analysis_options.yaml` | Reglas del linter (`flutter_lints`) | Sí | Sí |
| `.fvmrc` | Versión de Flutter del proyecto (FVM) | Con `fvm use` | Sí |
| `android/app/build.gradle*` | Configuración de Android (Gradle) | Con cuidado | Sí |
| `build/`, `.dart_tool/` | Generados | No | **No** |

## 11. La aplicación de tareas

Misma API y mismo diseño por capas que en [React](#react), [Vue](#vue) y los backends. Estructura de `lib/`:

```text
lib/
  main.dart                   arranque
  models/task.dart            modelo (inmutable) y filtro
  api/tasks_api.dart          comunicación HTTP con la API
  state/tasks_controller.dart estado y lógica (ChangeNotifier)
  screens/tasks_screen.dart   pantalla
```

```dart
/// Modelo de dominio. Inmutable: para "modificar" se crea una copia.
class Task {
  const Task({required this.id, required this.title, this.done = false});

  /// Constructor de fábrica: crea una Task a partir del JSON de la API.
  factory Task.fromJson(Map<String, dynamic> json) => Task(
    id: json['id'] as String,
    title: json['title'] as String,
    done: json['done'] as bool,
  );

  final String id;
  final String title;
  final bool done;

  Task copyWith({String? title, bool? done}) =>
      Task(id: id, title: title ?? this.title, done: done ?? this.done);
}

enum TaskFilter { all, pending, done }
```

```dart
import 'dart:convert';

import 'package:http/http.dart' as http;

import '../models/task.dart';

/// Error de la API con el mensaje que envía el servidor.
class ApiException implements Exception {
  ApiException(this.message);
  final String message;

  @override
  String toString() => message;
}

/// Capa de red. La URL base llega por --dart-define para poder cambiarla sin tocar el código.
class TasksApi {
  TasksApi({http.Client? client, String? baseUrl})
    : _client = client ?? http.Client(),
      _baseUrl =
          baseUrl ??
          const String.fromEnvironment(
            'API_URL',
            defaultValue: 'http://localhost:3000/api',
          );

  final http.Client _client;
  final String _baseUrl;

  Uri _uri(String path) => Uri.parse('$_baseUrl$path');

  static const _jsonHeaders = {'Content-Type': 'application/json'};

  /// Comprueba el código de estado: http NO lanza error con 404 o 500.
  void _check(http.Response response) {
    if (response.statusCode >= 200 && response.statusCode < 300) {
      return;
    }
    String message = 'Error HTTP ${response.statusCode}';
    try {
      final body = jsonDecode(response.body) as Map<String, dynamic>;
      message = body['error'] as String? ?? message;
    } catch (_) {
      // El cuerpo no era JSON: nos quedamos con el mensaje genérico.
    }
    throw ApiException(message);
  }

  Future<List<Task>> list() async {
    final response = await _client.get(_uri('/tasks'));
    _check(response);
    final data = jsonDecode(response.body) as List<dynamic>;
    return data
        .map((item) => Task.fromJson(item as Map<String, dynamic>))
        .toList();
  }

  Future<Task> create(String title) async {
    final response = await _client.post(
      _uri('/tasks'),
      headers: _jsonHeaders,
      body: jsonEncode({'title': title}),
    );
    _check(response);
    return Task.fromJson(jsonDecode(response.body) as Map<String, dynamic>);
  }

  Future<Task> update(String id, {bool? done, String? title}) async {
    final response = await _client.patch(
      _uri('/tasks/$id'),
      headers: _jsonHeaders,
      body: jsonEncode({'done': ?done, 'title': ?title}),
    );
    _check(response);
    return Task.fromJson(jsonDecode(response.body) as Map<String, dynamic>);
  }

  Future<void> remove(String id) async {
    final response = await _client.delete(_uri('/tasks/$id'));
    _check(response);
  }
}
```

| Pieza | Significado |
|---|---|
| `String.fromEnvironment('API_URL', defaultValue: …)` | Configuración en **tiempo de compilación**: `flutter run --dart-define=API_URL=http://…` |
| `http.Client` inyectable | Permite sustituirlo por uno falso en las pruebas |
| `_check(response)` | `http` **no lanza error** con 404 o 500: hay que comprobar el código |
| `{'done': ?done, 'title': ?title}` | Elementos nulos opcionales: solo se incluyen los no nulos |

```dart
import 'package:flutter/foundation.dart';

import '../api/tasks_api.dart';
import '../models/task.dart';

/// Estado de la pantalla. ChangeNotifier avisa a la interfaz con notifyListeners().
class TasksController extends ChangeNotifier {
  TasksController(this._api);

  final TasksApi _api;

  List<Task> _tasks = [];
  bool _loading = true;
  String? _error;
  TaskFilter _filter = TaskFilter.all;

  bool get loading => _loading;
  String? get error => _error;
  TaskFilter get filter => _filter;
  int get pendingCount => _tasks.where((task) => !task.done).length;

  /// Valor derivado: se calcula a partir del estado.
  List<Task> get visibleTasks => switch (_filter) {
    TaskFilter.all => _tasks,
    TaskFilter.pending => _tasks.where((task) => !task.done).toList(),
    TaskFilter.done => _tasks.where((task) => task.done).toList(),
  };

  Future<void> load() async {
    _loading = true;
    _error = null;
    notifyListeners();
    try {
      _tasks = await _api.list();
    } catch (error) {
      _error = error.toString();
    } finally {
      _loading = false;
      notifyListeners();
    }
  }

  void setFilter(TaskFilter filter) {
    _filter = filter;
    notifyListeners();
  }

  Future<void> add(String title) async {
    final created = await _api.create(title);
    _tasks = [..._tasks, created];
    notifyListeners();
  }

  Future<void> toggle(Task task) async {
    final updated = await _api.update(task.id, done: !task.done);
    _tasks = [for (final item in _tasks) item.id == task.id ? updated : item];
    notifyListeners();
  }

  Future<void> remove(String id) async {
    await _api.remove(id);
    _tasks = _tasks.where((task) => task.id != id).toList();
    notifyListeners();
  }
}
```

`ChangeNotifier` + `notifyListeners()` es el mecanismo de estado incorporado en Flutter: la interfaz se suscribe y se redibuja al cambiar. Para aplicaciones grandes se usan librerías como Provider, Riverpod o Bloc, con la misma idea.

```dart
import 'package:flutter/material.dart';

import '../models/task.dart';
import '../state/tasks_controller.dart';

class TasksScreen extends StatefulWidget {
  const TasksScreen({super.key, required this.controller});

  final TasksController controller;

  @override
  State<TasksScreen> createState() => _TasksScreenState();
}

class _TasksScreenState extends State<TasksScreen> {
  final _titleController = TextEditingController();
  final _formKey = GlobalKey<FormState>();
  bool _saving = false;

  @override
  void initState() {
    super.initState();
    widget.controller.load();
  }

  @override
  void dispose() {
    _titleController.dispose(); // los controladores se liberan siempre
    super.dispose();
  }

  Future<void> _submit() async {
    if (!_formKey.currentState!.validate()) {
      return;
    }
    setState(() => _saving = true);
    try {
      await widget.controller.add(_titleController.text.trim());
      _titleController.clear();
    } catch (error) {
      if (mounted) {
        ScaffoldMessenger.of(context)
            .showSnackBar(SnackBar(content: Text('$error')));
      }
    } finally {
      if (mounted) {
        setState(() => _saving = false);
      }
    }
  }

  Future<bool> _confirmDelete(Task task) async {
    final accepted = await showDialog<bool>(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('¿Eliminar tarea?'),
        content: Text(task.title),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context, false),
            child: const Text('No'),
          ),
          FilledButton(
            onPressed: () => Navigator.pop(context, true),
            child: const Text('Sí'),
          ),
        ],
      ),
    );
    return accepted ?? false;
  }

  @override
  Widget build(BuildContext context) {
    final controller = widget.controller;
    return Scaffold(
      appBar: AppBar(title: const Text('Tareas')),
      body: Padding(
        padding: const EdgeInsets.all(16),
        // ListenableBuilder reconstruye este trozo cada vez que el controlador notifica
        child: ListenableBuilder(
          listenable: controller,
          builder: (context, _) => Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              Form(
                key: _formKey,
                child: Row(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Expanded(
                      child: TextFormField(
                        controller: _titleController,
                        decoration: const InputDecoration(
                          labelText: 'Nueva tarea',
                        ),
                        validator: (value) => (value ?? '').trim().length < 2
                            ? 'Escribe al menos 2 caracteres'
                            : null,
                        onFieldSubmitted: (_) => _submit(),
                      ),
                    ),
                    const SizedBox(width: 8),
                    FilledButton(
                      onPressed: _saving ? null : _submit,
                      child: Text(_saving ? 'Guardando…' : 'Añadir'),
                    ),
                  ],
                ),
              ),
              const SizedBox(height: 12),
              SegmentedButton<TaskFilter>(
                segments: const [
                  ButtonSegment(value: TaskFilter.all, label: Text('Todas')),
                  ButtonSegment(
                    value: TaskFilter.pending,
                    label: Text('Pendientes'),
                  ),
                  ButtonSegment(value: TaskFilter.done, label: Text('Hechas')),
                ],
                selected: {controller.filter},
                onSelectionChanged: (selection) =>
                    controller.setFilter(selection.first),
              ),
              const SizedBox(height: 12),
              Expanded(child: _body(controller)),
            ],
          ),
        ),
      ),
    );
  }

  /// Los cuatro estados de la pantalla: cargando, error, vacío y datos.
  Widget _body(TasksController controller) {
    if (controller.loading) {
      return const Center(child: CircularProgressIndicator());
    }
    if (controller.error != null) {
      return Center(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Text(controller.error!),
            TextButton(
              onPressed: controller.load,
              child: const Text('Reintentar'),
            ),
          ],
        ),
      );
    }
    final tasks = controller.visibleTasks;
    if (tasks.isEmpty) {
      return const Center(child: Text('No hay tareas. ¡Añade la primera!'));
    }
    return Column(
      children: [
        Align(
          alignment: Alignment.centerLeft,
          child: Text('${controller.pendingCount} pendientes'),
        ),
        Expanded(
          child: ListView.builder(
            itemCount: tasks.length,
            itemBuilder: (context, index) {
              final task = tasks[index];
              return Dismissible(
                key: ValueKey(task.id),
                direction: DismissDirection.endToStart,
                confirmDismiss: (_) => _confirmDelete(task),
                onDismissed: (_) => controller.remove(task.id),
                background: Container(
                  color: Colors.red,
                  alignment: Alignment.centerRight,
                  padding: const EdgeInsets.only(right: 16),
                  child: const Icon(Icons.delete, color: Colors.white),
                ),
                child: CheckboxListTile(
                  value: task.done,
                  onChanged: (_) => controller.toggle(task),
                  title: Text(
                    task.title,
                    style: TextStyle(
                      decoration: task.done ? TextDecoration.lineThrough : null,
                    ),
                  ),
                ),
              );
            },
          ),
        ),
      ],
    );
  }
}
```

| Elemento | Significado |
|---|---|
| `ListenableBuilder` | Reconstruye su hijo cuando el `ChangeNotifier` avisa |
| `Form` + `TextFormField(validator: …)` | Validación de formularios; `_formKey.currentState!.validate()` |
| `TextEditingController` | Lee y limpia el texto; se libera en `dispose` |
| `SegmentedButton` | Control de filtro (Todas / Pendientes / Hechas) |
| `_body(...)` | Los cuatro estados: cargando, error con reintento, vacío y datos |
| `Dismissible` + `confirmDismiss` | Deslizar para borrar, con diálogo de confirmación |
| `CheckboxListTile` | Fila con casilla |
| `mounted` | Comprueba que el widget sigue en pantalla tras un `await` |

```dart
import 'package:flutter/material.dart';

import 'api/tasks_api.dart';
import 'screens/tasks_screen.dart';
import 'state/tasks_controller.dart';

void main() {
  runApp(TasksApp(controller: TasksController(TasksApi())));
}

class TasksApp extends StatelessWidget {
  const TasksApp({super.key, required this.controller});

  final TasksController controller;

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Tareas',
      theme: ThemeData(colorSchemeSeed: Colors.teal, useMaterial3: true),
      home: TasksScreen(controller: controller),
    );
  }
}
```

### Ejecutar contra tu API

```bash
# Linux de escritorio o web (la API en la misma máquina)
fvm flutter run -d linux

# Emulador de Android: el anfitrión es 10.0.2.2, no localhost
fvm flutter run -d emulator-5554 --dart-define=API_URL=http://10.0.2.2:3000/api

# Teléfono físico: la IP de tu máquina en la red local
fvm flutter run -d <id> --dart-define=API_URL=http://192.168.1.50:3000/api
```

| Escenario | URL de la API |
|---|---|
| Escritorio / navegador | `http://localhost:3000/api` |
| Emulador Android | `http://10.0.2.2:3000/api` |
| Teléfono por Wi-Fi | `http://<IP-de-tu-equipo>:3000/api` (la API debe escuchar en `0.0.0.0`) |
| Navegador (Flutter web) | Requiere **CORS** en la API para el origen de la web |

Android bloquea por defecto el tráfico HTTP sin cifrar en versiones recientes: en desarrollo hay que declararlo (`android:usesCleartextTraffic="true"` en `AndroidManifest.xml`); en producción usa HTTPS.

## 12. Pruebas, depuración y calidad

```dart
import 'dart:convert';

import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:http/http.dart' as http;
import 'package:http/testing.dart';
import 'package:tasks_app/api/tasks_api.dart';
import 'package:tasks_app/main.dart';
import 'package:tasks_app/state/tasks_controller.dart';

/// Cliente HTTP falso: responde sin red, como un servidor en memoria.
MockClient fakeServer() {
  final tasks = <Map<String, dynamic>>[];
  var lastId = 0;
  return MockClient((request) async {
    final headers = {'content-type': 'application/json'};
    if (request.method == 'GET') {
      return http.Response(jsonEncode(tasks), 200, headers: headers);
    }
    if (request.method == 'POST') {
      final body = jsonDecode(request.body) as Map<String, dynamic>;
      final task = {'id': '${++lastId}', 'title': body['title'], 'done': false};
      tasks.add(task);
      return http.Response(jsonEncode(task), 201, headers: headers);
    }
    return http.Response('{"error":"no soportado"}', 400, headers: headers);
  });
}

void main() {
  testWidgets('valida el formulario y añade una tarea', (tester) async {
    final controller = TasksController(
      TasksApi(client: fakeServer(), baseUrl: 'http://test/api'),
    );
    await tester.pumpWidget(TasksApp(controller: controller));
    await tester.pumpAndSettle();

    expect(find.text('No hay tareas. ¡Añade la primera!'), findsOneWidget);

    // Título demasiado corto: aparece el error de validación
    await tester.enterText(find.byType(TextFormField), 'x');
    await tester.tap(find.text('Añadir'));
    await tester.pump();
    expect(find.text('Escribe al menos 2 caracteres'), findsOneWidget);

    // Título válido: se crea y aparece en la lista
    await tester.enterText(find.byType(TextFormField), 'Estudiar Flutter');
    await tester.tap(find.text('Añadir'));
    await tester.pumpAndSettle();
    expect(find.text('Estudiar Flutter'), findsOneWidget);
    expect(find.text('1 pendientes'), findsOneWidget);
  });

  testWidgets('muestra el error y permite reintentar', (tester) async {
    final failing = MockClient(
      (request) async => http.Response('{"error":"Servidor caído"}', 500),
    );
    final controller = TasksController(
      TasksApi(client: failing, baseUrl: 'http://test/api'),
    );
    await tester.pumpWidget(TasksApp(controller: controller));
    await tester.pumpAndSettle();

    expect(find.text('Servidor caído'), findsOneWidget);
    expect(find.text('Reintentar'), findsOneWidget);
  });
}
```

```bash
fvm flutter analyze
fvm flutter test
fvm flutter test --plain-name "valida el formulario"
dart format lib test
```

| Comando | Significado |
|---|---|
| `flutter analyze` | Análisis estático (errores y reglas de `flutter_lints`) |
| `flutter test` | Ejecuta las pruebas de `test/` |
| `--plain-name "texto"` | Solo las pruebas cuyo nombre contenga ese texto |
| `dart format lib test` | Formatea el código |
| `tester.pumpWidget`, `pump`, `pumpAndSettle` | Dibujar, avanzar un fotograma, esperar a que acaben animaciones |
| `find.text`, `find.byType` | Localizar widgets |
| `MockClient` (de `package:http/testing.dart`) | Servidor falso para probar sin red |

Depuración: **hot reload** (`r` en la terminal o guardar en el IDE), **hot restart** (`R`), **Flutter DevTools** (inspector de widgets, rendimiento, red), `debugPrint`, y puntos de interrupción en VS Code.

## 13. Acceder a las APIs del dispositivo

Los **plugins** envuelven las APIs nativas con una interfaz Dart.

```bash
fvm flutter pub add sensors_plus geolocator shared_preferences
```

| Necesidad | Paquete habitual |
|---|---|
| Acelerómetro, giroscopio, magnetómetro | `sensors_plus` |
| Ubicación (GPS) | `geolocator` |
| Cámara y galería | `camera`, `image_picker` |
| Almacenamiento clave-valor | `shared_preferences` |
| Archivos y rutas | `path_provider` |
| Bluetooth de bajo consumo | `flutter_blue_plus` |
| Permisos | `permission_handler` |
| Biometría | `local_auth` |
| Notificaciones | `flutter_local_notifications` |
| Batería, red | `battery_plus`, `connectivity_plus` |

### Sensores: un Stream

```dart
import 'dart:async';

import 'package:flutter/material.dart';
import 'package:sensors_plus/sensors_plus.dart';

/// Muestra en vivo los valores del acelerómetro del dispositivo.
class AccelerometerView extends StatefulWidget {
  const AccelerometerView({super.key});

  @override
  State<AccelerometerView> createState() => _AccelerometerViewState();
}

class _AccelerometerViewState extends State<AccelerometerView> {
  StreamSubscription<AccelerometerEvent>? _subscription;
  AccelerometerEvent? _event;

  @override
  void initState() {
    super.initState();
    // Un Stream emite valores a lo largo del tiempo
    _subscription = accelerometerEventStream().listen(
      (event) => setState(() => _event = event),
      onError: (Object error) => debugPrint('Sensor no disponible: $error'),
    );
  }

  @override
  void dispose() {
    _subscription?.cancel(); // cancela SIEMPRE las suscripciones
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final event = _event;
    if (event == null) {
      return const Text('Esperando datos del sensor…');
    }
    return Text(
      'x: ${event.x.toStringAsFixed(2)}  y: ${event.y.toStringAsFixed(2)}  z: ${event.z.toStringAsFixed(2)}',
    );
  }
}
```

### Ubicación: con permisos

```dart
import 'package:geolocator/geolocator.dart';

/// Obtiene la posición actual pidiendo antes los permisos necesarios.
Future<Position> currentPosition() async {
  if (!await Geolocator.isLocationServiceEnabled()) {
    throw Exception('Activa la ubicación del dispositivo');
  }
  var permission = await Geolocator.checkPermission();
  if (permission == LocationPermission.denied) {
    permission =
        await Geolocator.requestPermission(); // muestra el diálogo del sistema
  }
  if (permission == LocationPermission.denied ||
      permission == LocationPermission.deniedForever) {
    throw Exception('Permiso de ubicación denegado');
  }
  return Geolocator.getCurrentPosition();
}
```

Los permisos se declaran en el sistema nativo. En `android/app/src/main/AndroidManifest.xml` (dentro de `<manifest>`):

```xml
<uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" />
<uses-permission android:name="android.permission.ACCESS_COARSE_LOCATION" />
<uses-permission android:name="android.permission.INTERNET" />
<uses-permission android:name="android.permission.CAMERA" />
```

Declararlos **no basta**: los permisos peligrosos (ubicación, cámara) se piden en ejecución y el usuario puede denegarlos. Maneja siempre el caso «denegado».

### Almacenamiento local

```dart
import 'package:shared_preferences/shared_preferences.dart';

/// Guarda y lee pequeños valores en el almacenamiento local del dispositivo.
class Preferences {
  static const _lastFilterKey = 'last_filter';

  static Future<void> saveFilter(String filter) async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.setString(_lastFilterKey, filter);
  }

  static Future<String> loadFilter() async {
    final prefs = await SharedPreferences.getInstance();
    return prefs.getString(_lastFilterKey) ?? 'all';
  }
}
```

### Cuando no hay plugin: canal de plataforma

Dart llama a código nativo por un canal con nombre.

```dart
import 'package:flutter/services.dart';

/// Canal de plataforma: Dart llama a código nativo (Kotlin en Android) por nombre.
class BatteryChannel {
  static const _channel = MethodChannel('com.example.tasks/battery');

  static Future<int> level() async {
    final value = await _channel.invokeMethod<int>('getBatteryLevel');
    return value ?? -1;
  }
}
```

Lado Android (Kotlin), en `android/app/src/main/kotlin/.../MainActivity.kt`. *Este fragmento no se pudo compilar aquí; revísalo con Android Studio:*

```kotlin
package com.example.tasks_app

import android.content.Context
import android.os.BatteryManager
import io.flutter.embedding.android.FlutterActivity
import io.flutter.embedding.engine.FlutterEngine
import io.flutter.plugin.common.MethodChannel

class MainActivity : FlutterActivity() {
    override fun configureFlutterEngine(flutterEngine: FlutterEngine) {
        super.configureFlutterEngine(flutterEngine)
        MethodChannel(flutterEngine.dartExecutor.binaryMessenger, "com.example.tasks/battery")
            .setMethodCallHandler { call, result ->
                if (call.method == "getBatteryLevel") {
                    val manager = getSystemService(Context.BATTERY_SERVICE) as BatteryManager
                    result.success(manager.getIntProperty(BatteryManager.BATTERY_PROPERTY_CAPACITY))
                } else {
                    result.notImplemented()
                }
            }
    }
}
```

Este patrón es la puerta de entrada a **cualquier** API de Android que ningún plugin cubra. Se profundiza en [Android nativo](#android).

## 14. Compilar y empaquetar

```bash
fvm flutter build apk --release
fvm flutter build appbundle --release
fvm flutter build web
fvm flutter build linux
```

| Comando | Resultado |
|---|---|
| `build apk --release` | `build/app/outputs/flutter-apk/app-release.apk` para instalar directamente |
| `build appbundle --release` | `.aab` para Google Play |
| `build web` | `build/web/` (archivos estáticos); `--no-web-resources-cdn` empaqueta los recursos en lugar de pedirlos a un CDN |
| `build linux` | Aplicación de escritorio |
| `--dart-define=CLAVE=valor` | Variables de compilación |

La firma de la aplicación para Play Store exige una clave de firma: **guárdala fuera del repositorio** y con copia de seguridad.

## 15. Errores habituales

| Mensaje | Causa | Solución |
|---|---|---|
| `flutter: command not found` | Usas `flutter` sin FVM | `fvm flutter …` o `fvm use` y el SDK del IDE |
| `Unable to locate Android SDK` | No está configurado | `flutter config --android-sdk RUTA`; `flutter doctor` |
| `Android license status unknown` | Faltan licencias | `flutter doctor --android-licenses` |
| `RenderFlex overflowed by N pixels` | El contenido no cabe en una `Row`/`Column` | `Expanded`, `Flexible`, `SingleChildScrollView` o `ListView` |
| `setState() called after dispose()` | `setState` tras un `await` con el widget ya destruido | Comprueba `if (mounted)` |
| `Null check operator used on a null value` | Un `!` sobre un valor nulo | Comprueba antes |
| `Unhandled Exception: SocketException` | Sin conexión o URL incorrecta (`localhost` en el emulador) | `10.0.2.2` o la IP de la red |
| `Cleartext HTTP traffic not permitted` | Android bloquea HTTP sin cifrar | Usa HTTPS o `usesCleartextTraffic` solo en desarrollo |
| `MissingPluginException` | Plugin nuevo sin reiniciar la app | Detén y vuelve a ejecutar (no basta el hot reload) |
| `Gradle task assembleDebug failed` | Problema de Java/Gradle/SDK | Lee el error de Gradle; alinea JDK y versión del plugin |
| CORS en Flutter web | La API no permite el origen | Configura CORS en el servidor |

## 16. Ejercicios

> **Ejercicio:** resuelve los ejercicios 1–15 de [lógica y programación](#ejercicios-basicos) en Dart con `dart test`.

> **Ejercicio:** añade edición del título (pulsación larga → diálogo con `TextField`) y escribe una prueba de widget.

> **Ejercicio:** muestra el acelerómetro en una pantalla nueva con `Navigator.push` y añade un botón para pausar la suscripción.

> **Ejercicio:** guarda el último filtro elegido con `shared_preferences` y restáuralo al abrir la app.

> **Ejercicio:** implementa el canal de plataforma de batería de extremo a extremo (Dart + Kotlin) y muéstralo en pantalla; compara con el plugin `battery_plus`.

## 17. Documentación oficial

- [Dart: tour del lenguaje](https://dart.dev/language)
- [Flutter: primeros pasos](https://docs.flutter.dev/get-started)
- [Catálogo de widgets](https://docs.flutter.dev/ui/widgets)
- [Cookbook de Flutter](https://docs.flutter.dev/cookbook)
- [Platform channels](https://docs.flutter.dev/platform-integration/platform-channels)
- [pub.dev: paquetes](https://pub.dev/)
- [FVM](https://fvm.app/)
