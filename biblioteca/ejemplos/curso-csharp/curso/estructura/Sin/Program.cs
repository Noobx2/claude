var list = new List<int> { 3, 1, 2 };
Console.WriteLine(string.Join(",", list.OrderBy(n => n)));
