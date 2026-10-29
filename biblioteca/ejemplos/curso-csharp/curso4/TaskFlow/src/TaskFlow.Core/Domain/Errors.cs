namespace TaskFlow.Domain;

/// <summary>Los datos no cumplen las reglas del negocio (se muestra al usuario tal cual).</summary>
public sealed class ValidationException(string message) : Exception(message);

/// <summary>Se pidió algo que no existe.</summary>
public sealed class NotFoundException(string message) : Exception(message);
