# Salto de fe – esqueleto inicial (F# + Fable)

Requisitos: .NET SDK 8 y Node.js.

```bash
dotnet new tool-manifest
dotnet tool install fable      # compilador Fable (F# -> JavaScript)
dotnet restore
dotnet fable --outDir build    # genera build/App.js
python3 -m http.server 8000    # o: npx serve .
```

Abre http://localhost:8000. El prólogo está en `src/Content.fs`.

Orden de compilación (importa en F#): Types, Rules, Content, Engine, Save, App.
