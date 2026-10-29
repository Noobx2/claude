// F#: lenguaje "funcional primero" que compila a la MISMA plataforma .NET que C#.
// Aquí consume el Core escrito en C#, sin cambiar una sola línea de él.
open System
open Inventory

let repo = ProductRepository()

[ "Teclado mecánico", "Periféricos", 59.90m, 25
  "Ratón inalámbrico", "Periféricos", 24.50m, 60
  "Monitor 24\"", "Monitores", 139.00m, 10
  "Alfombrilla XL", "Periféricos", 14.00m, 0 ]
|> List.iter (fun (name, category, price, stock) -> repo.Add(name, category, price, stock) |> ignore)

// Unión discriminada: un tipo que es "una de varias formas"; el compilador avisa si olvidas un caso
type StockLevel =
    | OutOfStock
    | LowStock of int
    | InStock of int

let level (p: Product) =
    match p.Stock with
    | 0 -> OutOfStock
    | s when s < 10 -> LowStock s
    | s -> InStock s

// Tubería (|>): el resultado de la izquierda pasa como último argumento a la derecha
repo.FindAll()
|> Seq.filter (fun p -> p.Price > 20m)
|> Seq.sortByDescending (fun p -> p.Price)
|> Seq.iter (fun p -> printfn "%-20s %7.2f  %A" p.Name p.Price (level p))

// Result en lugar de excepciones: el fallo es un valor, no un salto
let tryProduct id name category price stock =
    try Ok(Product(id, name, category, price, stock))
    with :? ArgumentException as e -> Error e.Message

match tryProduct 99 "Cable" "Cables" -5m 3 with
| Ok p -> printfn "creado %s" p.Name
| Error message -> printfn "rechazado: %s" message

printfn "%s" (Report.Summary(repo.FindAll()))
