import { defineConfig } from 'vite';

export default defineConfig({
  server: {
    port: 5173,
    // En desarrollo, Vite reenvía las peticiones /api al backend de C#.
    // Así el navegador cree que todo viene del mismo sitio y no hace falta CORS.
    proxy: { '/api': 'http://localhost:5080' },
  },
  build: {
    // "vite build" deja el resultado dentro de wwwroot, que ASP.NET Core sirve como archivos estáticos
    outDir: '../src/Inventory.Api/wwwroot',
    emptyOutDir: true,
  },
});
