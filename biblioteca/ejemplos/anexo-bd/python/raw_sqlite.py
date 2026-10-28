"""Acceso a datos SIN ORM: módulo sqlite3 de la biblioteca estándar de Python."""
import sqlite3

conn = sqlite3.connect(":memory:")          # base en memoria: desaparece al cerrar
conn.row_factory = sqlite3.Row              # permite leer columnas por nombre
conn.execute("PRAGMA foreign_keys = ON")    # SQLite no comprueba claves foráneas si no se pide

conn.executescript("""
CREATE TABLE users (id INTEGER PRIMARY KEY, name TEXT NOT NULL, role TEXT NOT NULL);
INSERT INTO users (name, role) VALUES ('ana', 'admin'), ('luis', 'user'), ('marta', 'user');
""")

# MAL: construir SQL pegando texto del usuario -> inyección SQL
def find_user_unsafe(name):
    query = f"SELECT id, name, role FROM users WHERE name = '{name}'"
    return conn.execute(query).fetchall()

# BIEN: consulta parametrizada; el dato viaja aparte y nunca se interpreta como SQL
def find_user_safe(name):
    return conn.execute("SELECT id, name, role FROM users WHERE name = ?", (name,)).fetchall()

attack = "x' OR '1'='1"                     # texto que un atacante escribiría en un formulario
print("inseguro con 'luis':", [tuple(r) for r in find_user_unsafe("luis")])
print("inseguro con ataque:", [tuple(r) for r in find_user_unsafe(attack)])   # devuelve a TODOS
print("seguro   con ataque:", [tuple(r) for r in find_user_safe(attack)])     # no devuelve nada

# Transacción con "with": confirma si todo va bien, deshace si hay excepción
try:
    with conn:
        conn.execute("INSERT INTO users (name, role) VALUES (?, ?)", ("pedro", "user"))
        conn.execute("INSERT INTO users (name, role) VALUES (?, ?)", (None, "user"))   # viola NOT NULL
except sqlite3.IntegrityError as error:
    print("deshecho:", error)

print("usuarios:", [r["name"] for r in conn.execute("SELECT name FROM users ORDER BY id")])
