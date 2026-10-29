// ╔═══════════════════════════════════════════════════════════════════════════════════════════╗
// ║ ESTO NO ES UNITY. Es un «molde» mínimo, escrito para este curso, con la forma que tiene la ║
// ║ API de Unity que se usa en los ejemplos. Sirve SOLO para que el compilador de C# compruebe ║
// ║ que los scripts no tienen errores de sintaxis ni de tipos. NO comprueba que la API real de ║
// ║ Unity se llame así, ni cómo se comporta: eso solo se verifica abriendo el proyecto en Unity.║
// ╚═══════════════════════════════════════════════════════════════════════════════════════════╝
using System;
using System.Collections;
using System.Threading;

namespace UnityEngine
{
    public class Object { public string name; public static void Destroy(Object o) { } public static T Instantiate<T>(T original) where T : Object => original; public static void DontDestroyOnLoad(Object o) { } public static T FindFirstObjectByType<T>() where T : Object => null; public static implicit operator bool(Object o) => o != null; }
    public class Component : Object { public GameObject gameObject; public Transform transform; public T GetComponent<T>() => default; }
    public class Transform : Component { public Vector3 position; }
    public class GameObject : Object { public void SetActive(bool v) { } public bool activeSelf; public T GetComponent<T>() => default; public T AddComponent<T>() where T : Component => null; }
    public class Behaviour : Component { public bool enabled; }
    public class MonoBehaviour : Behaviour
    {
        public Coroutine StartCoroutine(IEnumerator rutina) => null;
        public void StopCoroutine(Coroutine c) { }
        public CancellationToken destroyCancellationToken => default;
    }
    public class ScriptableObject : Object { public static T CreateInstance<T>() where T : ScriptableObject, new() => new T(); }
    public sealed class Coroutine { }
    public struct Vector3 { public float x, y, z; public Vector3(float x, float y, float z) { this.x = x; this.y = y; this.z = z; } public static Vector3 zero => default; }
    public static class Time { public static float deltaTime => 0.016f; public static float time => 0f; }
    public static class Debug { public static void Log(object m) { } public static void LogWarning(object m) { } public static void LogError(object m) { } }
    public static class Mathf { public static int Max(int a, int b) => a > b ? a : b; public static int Clamp(int v, int min, int max) => v < min ? min : v > max ? max : v; }
    public class WaitForSeconds { public WaitForSeconds(float s) { } }
    public class WaitForEndOfFrame { }
    public sealed class Awaitable { public static System.Threading.Tasks.Task NextFrameAsync(CancellationToken ct = default) => System.Threading.Tasks.Task.CompletedTask; public static System.Threading.Tasks.Task WaitForSecondsAsync(float s, CancellationToken ct = default) => System.Threading.Tasks.Task.CompletedTask; }

    [AttributeUsage(AttributeTargets.Field)] public sealed class SerializeField : Attribute { }
    [AttributeUsage(AttributeTargets.Field)] public sealed class HeaderAttribute : Attribute { public HeaderAttribute(string h) { } }
    [AttributeUsage(AttributeTargets.Field)] public sealed class TooltipAttribute : Attribute { public TooltipAttribute(string t) { } }
    [AttributeUsage(AttributeTargets.Field)] public sealed class RangeAttribute : Attribute { public RangeAttribute(float min, float max) { } }
    [AttributeUsage(AttributeTargets.Class)] public sealed class RequireComponent : Attribute { public RequireComponent(Type t) { } }
    [AttributeUsage(AttributeTargets.Class)] public sealed class CreateAssetMenuAttribute : Attribute { public string fileName, menuName; }
    public enum RuntimeInitializeLoadType { SubsystemRegistration, AfterAssembliesLoaded, BeforeSceneLoad, AfterSceneLoad }
    [AttributeUsage(AttributeTargets.Method)] public sealed class RuntimeInitializeOnLoadMethodAttribute : Attribute { public RuntimeInitializeOnLoadMethodAttribute(RuntimeInitializeLoadType t) { } }
}

namespace UnityEngine.TestTools { [AttributeUsage(AttributeTargets.Method)] public sealed class UnityTestAttribute : Attribute { } }
