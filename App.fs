module App

open Browser.Dom
open Types

let mutable estado : Estado =
    match Save.cargar () with
    | Some e when Map.containsKey e.NodoActual Content.nodos -> e
    | _ -> Rules.estadoInicial

let el (id: string) = document.getElementById id

let pintarEstado () =
    (el "estado").textContent <-
        sprintf
            "Resistencia %d/%d · Estrés %d (%s) · Humanidad %d · Mandos %d · Compañeros %d · Pareja %d"
            estado.Resistencia
            (Rules.resistenciaMax estado)
            estado.Estres
            (Engine.nivelEstres estado)
            estado.Humanidad
            estado.Mandos
            estado.Companeros
            estado.Pareja

let rec render () =
    let nodo = Engine.nodoActual estado
    (el "personaje").textContent <- nodo.Personaje
    (el "texto").textContent <- nodo.Texto
    pintarEstado ()

    let cont = el "opciones"
    cont.innerHTML <- ""

    for op in Engine.opcionesDisponibles estado nodo do
        let b = document.createElement "button"
        b.textContent <- op.Texto
        b.addEventListener ("click", fun _ ->
            estado <- Engine.elegir estado op
            Save.guardar estado
            render ())
        cont.appendChild b |> ignore

    (el "guitarra").toggleAttribute ("disabled", estado.GuitarraUsada) |> ignore

(el "guitarra").addEventListener ("click", fun _ ->
    estado <- Engine.tocarGuitarra estado
    Save.guardar estado
    render ())

(el "reiniciar").addEventListener ("click", fun _ ->
    Save.borrar ()
    estado <- Rules.estadoInicial
    render ())

render ()
