' Visual Basic .NET: otra sintaxis sobre la MISMA plataforma .NET.
' Consume el Core escrito en C# sin cambiarlo.
Imports Inventory

Module Program
    Sub Main()
        Dim repo As New ProductRepository()
        repo.Add("Teclado mecánico", "Periféricos", 59.9D, 25)
        repo.Add("Ratón inalámbrico", "Periféricos", 24.5D, 60)
        repo.Add("Monitor 24""", "Monitores", 139D, 10)

        ' LINQ funciona igual que en C#
        Dim expensive = repo.FindAll().Where(Function(p) p.Price > 20D).OrderByDescending(Function(p) p.Price)
        For Each p In expensive
            Console.WriteLine($"{p.Name,-20} {p.Price,7:F2}")
        Next

        Try
            Dim invalid As New Product(99, "Cable", "Cables", -5D, 3)
        Catch ex As ArgumentException
            Console.WriteLine("rechazado: " & ex.Message)
        End Try

        Console.WriteLine(Report.Summary(repo.FindAll()))
    End Sub
End Module
