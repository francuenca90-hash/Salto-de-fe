module Rules

open Types

let clamp (v: int) = max 0 (min 100 v)

/// Máx = 50 + Físico × 0,4 + Disciplina × 0,1
let resistenciaMax (e: Estado) =
    50.0 + float e.Fisico * 0.4 + float e.Disciplina * 0.1
    |> round
    |> int

/// Ganancia = base × (1 − Disciplina/200); solo reduce las subidas de estrés.
let estresEscalado (e: Estado) (baseValor: int) =
    if baseValor > 0 then
        float baseValor * (1.0 - float e.Disciplina / 200.0) |> round |> int
    else
        baseValor

let estadoInicial : Estado =
    { Fisico = 50
      Disciplina = 50
      Paracaidista = 40
      Resistencia = 75
      Estres = 20
      Humanidad = 15
      Mandos = 40
      Companeros = 30
      Pareja = 60
      Banderas = Set.empty
      NodoActual = "prologo_guitarra"
      GuitarraUsada = false }

let aplicarEfecto (e: Estado) (ef: Efecto) : Estado =
    match ef with
    | Estres v -> { e with Estres = clamp (e.Estres + estresEscalado e v) }
    | Humanidad v -> { e with Humanidad = clamp (e.Humanidad + v) }
    | Resistencia v -> { e with Resistencia = max 0 (min (resistenciaMax e) (e.Resistencia + v)) }
    | Fisico v -> { e with Fisico = clamp (e.Fisico + v) }
    | Disciplina v -> { e with Disciplina = clamp (e.Disciplina + v) }
    | Paracaidista v -> { e with Paracaidista = clamp (e.Paracaidista + v) }
    | Mandos v -> { e with Mandos = clamp (e.Mandos + v) }
    | Companeros v -> { e with Companeros = clamp (e.Companeros + v) }
    | Pareja v -> { e with Pareja = clamp (e.Pareja + v) }
    | PonerBandera b -> { e with Banderas = Set.add b e.Banderas }

let aplicarEfectos (e: Estado) (efectos: Efecto list) = List.fold aplicarEfecto e efectos

let cumple (e: Estado) (c: Condicion) =
    match c with
    | Siempre -> true
    | HumanidadMin n -> e.Humanidad >= n
    | EstresMax n -> e.Estres <= n
    | DisciplinaMin n -> e.Disciplina >= n
    | Bandera b -> Set.contains b e.Banderas

/// Umbrales de estrés: <40 todo disponible, 40-69 respuestas más bruscas, >=70 impulsivas.
let opcionVisible (e: Estado) (o: Opcion) =
    cumple e o.Condicion
    && (match o.Tono with
        | Impulsivo -> e.Estres >= 70
        | Empatico -> e.Estres < 70
        | _ -> true)
