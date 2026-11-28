# Salto de fe – novela interactiva (F# + Fable → APK Android y web)

Juego completo: prólogo, tres actos (instrucción, despliegue en Afganistán, rescate), la condecoración y **tres finales** (redención plena, parcial y cinismo persistente) según tu Humanidad. 70 escenas de diálogo, sistema de atributos (condición física, disciplina, cualificación paracaidista), resistencia, estrés, relaciones y eventos con tiempo.

El código en F# lo compila Fable a JavaScript, esbuild lo empaqueta en `dist/` (~100 KB) y Capacitor lo mete en un APK.

---

## Camino A (el más fácil): obtener el APK sin instalar nada, con GitHub

1. Crea una cuenta gratuita en https://github.com y un repositorio nuevo (por ejemplo `salto-de-fe`; puede ser privado).
2. Sube esta carpeta al repositorio. Desde una terminal, dentro de la carpeta del proyecto:
   ```bash
   git init
   git add .
   git commit -m "Salto de fe"
   git branch -M main
   git remote add origin https://github.com/TU_USUARIO/salto-de-fe.git
   git push -u origin main
   ```
3. En GitHub entra en la pestaña **Actions**. Verás el flujo **Compilar APK** ejecutándose (tarda unos 5-10 minutos). Si no arranca solo, púlsalo y usa **Run workflow**.
4. Cuando termine con ✔ verde, abre la ejecución y descarga **salto-de-fe-apk** en la sección *Artifacts*. Es un ZIP: descomprímelo y dentro está `app-debug.apk`.
5. Pásalo al móvil (cable USB, Drive, Telegram, correo…) y ábrelo.
6. Si Android lo pide, autoriza **"Instalar apps desconocidas"** para la app desde la que lo abres (Ajustes → Aplicaciones → Acceso especial, o el aviso que aparece al abrir el archivo) y pulsa **Instalar**.

> Es un APK de depuración, firmado con la clave de depuración de Android: se instala sin problemas para jugar y compartir. Para subirlo a Google Play hace falta un paquete firmado con tu propia clave (ver más abajo).

Si el flujo falla, abre el paso en rojo y copia el error: lo más probable es un ajuste menor de sintaxis de F# o de versiones, porque este proyecto no se ha podido compilar donde se escribió.

---

## Camino B: compilar en tu ordenador

Requisitos: .NET SDK 8 o superior, Node.js 22 o superior, JDK 21 y Android Studio (para el SDK de Android).

```bash
npm install
npm run restore        # Fable (dotnet tool) y paquetes NuGet
npm run build          # F# → JS → dist/
npm run android:add    # solo la primera vez: crea la carpeta android/
npm run android:apk    # compila y genera el APK de depuración
```

El APK queda en `android/app/build/outputs/apk/debug/app-debug.apk`. En Windows, si falla `./gradlew`, entra en `android/` y ejecuta `gradlew.bat assembleDebug`.

Para instalarlo directamente con el móvil conectado por USB (depuración USB activada): `adb install -r android/app/build/outputs/apk/debug/app-debug.apk`.

Para abrir el proyecto en Android Studio: `npm run android:open`.

### Probar sin APK (en el navegador)

```bash
npm run start          # compila y sirve en http://localhost:8000
```
En el móvil por USB: `adb reverse tcp:8000 tcp:8000` y abre `http://localhost:8000` en Chrome (se puede instalar como PWA).

---

## Versiones y compatibilidad

| Para | Requisito |
|---|---|
| Ejecutar el APK | Android 7.0 (API 24) o superior (mínimo de Capacitor 8) |
| Ejecutar la PWA | Chrome o navegador Chromium 58 o superior |
| Compilar | .NET SDK 8+, Node 22+, JDK 21, Android Studio reciente |

- Fable está fijado a la **4.29.0** (última estable verificada) en `.config/dotnet-tools.json`. Para probar Fable 5, cambia ahí la versión y ejecuta `dotnet tool restore`.
- Un solo archivo JS (ES2017) sin dependencias en tiempo de ejecución, sin canvas ni animaciones: pensado para gama media-baja.
- Guardado automático en `localStorage` tras cada decisión. El botón **Reiniciar** borra la partida.
- Las cuentas atrás de los eventos con tiempo se congelan si la app pasa a segundo plano.

## Publicar en Google Play (opcional)

Necesitas una cuenta de desarrollador, un identificador propio (`appId` en `capacitor.config.json`, ahora `es.saltodefe.juego`) y un paquete firmado: `cd android && ./gradlew bundleRelease`, firmado con tu propia clave. Sigue la guía oficial de Capacitor "Deploying to Google Play".

## Estructura

```
src/          F#: Types, Rules, Dsl, Prologo, Acto1, Acto2, Acto3, Content, Engine, Save, App
public/       index.html, manifest, service worker, iconos
scripts/      build.mjs (empaquetado), serve.mjs (pruebas), make-icons.py
.github/      flujo que compila el APK en la nube
```

Añadir o cambiar diálogos: edita los nodos en `src/Acto1.fs`, `Acto2.fs` o `Acto3.fs`. Al arrancar, la consola del navegador avisa de enlaces rotos entre nodos.

## Mapa de la historia

- **Prólogo**: la rutina, la guitarra, Lucía, Marcos, el sargento.
- **Acto I – Instrucción**: marcha, noche de guitarra, salto con viento, lesión de Marcos, bronca del teniente, convocatoria y conflicto con Lucía.
- **Acto II – Afganistán**: ayuda humanitaria en Deh Sabz, primera emboscada, escuela, ruptura con Lucía, segundo ataque y rescate de Samir.
- **Acto III – Rescate**: aproximación (salto o marcha), ruta con Farid, emboscada, refugio con la patrulla de Marcos, dilema del helicóptero, extracción.
- **Cierre**: ceremonia de la Cruz del Mérito Militar con distintivo rojo y epílogo, con tres finales:
  - Redención plena (Humanidad ≥ 70).
  - Redención parcial (40-69).
  - Cinismo persistente (< 40).

Los personajes, la aldea y los hechos son ficticios. La condecoración existe en el Reglamento general de recompensas militares español; la ceremonia es una licencia narrativa.
