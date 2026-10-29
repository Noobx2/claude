// La carpeta se llama "Weird" pero el namespace es otro: el compilador NO exige que coincidan.
// Es solo una convención; romperla confunde a todo el mundo.
namespace Totally.Different.Name;

public class Thing { public override string ToString() => "Soy Thing, de otro namespace"; }
