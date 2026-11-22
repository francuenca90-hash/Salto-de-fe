module Engine

open Types
open Rules

let nodoActual (e: Estado) : Nodo = Map.find e.NodoActual Content.nodos

let opcionesDisponibles (e: Estado) (n: Nodo) : Opcion list =
    n.Opciones |> List.filter (opcionVisible e)

/// Aplica los efectos de la opción y avanza al siguiente nodo.
let elegir (e: Estado) (o: Opcion) : Estado =
    let e2 = aplicarEfectos e o.Efectos
    { e2 with NodoActual = o.Siguiente; GuitarraUsada = false }

/// La guitarra reduce el estrés una vez por jornada/evento.
let tocarGuitarra (e: Estado) : Estado =
    if e.GuitarraUsada then
        e
    else
        { aplicarEfectos e [ Estres -10; Humanidad 1 ] with GuitarraUsada = true }

/// Descripción del nivel de estrés para la interfaz.
let nivelEstres (e: Estado) =
    if e.Estres < 40 then "sereno"
    elif e.Estres < 70 then "tenso"
    else "al límite"
