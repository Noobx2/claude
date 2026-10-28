using Inventory;

// Windows Forms necesita ejecutarse en un hilo «STA» y con esta inicialización
ApplicationConfiguration.Initialize();

string file = args.Length > 0 ? args[0] : "products.json";
var repository = new ProductRepository();
repository.Load(file);
Application.Run(new MainForm(repository, file));
