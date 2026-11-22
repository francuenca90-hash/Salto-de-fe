module Save

open Fable.Core
open Fable.Core.JsInterop
open Browser.Dom
open Types

let private clave = "salto-de-fe-v1"

let guardar (e: Estado) =
    try
        let o =
            createObj [ "fisico" ==> e.Fisico
                        "disciplina" ==> e.Disciplina
                        "paracaidista" ==> e.Paracaidista
                        "resistencia" ==> e.Resistencia
                        "estres" ==> e.Estres
                        "humanidad" ==> e.Humanidad
                        "mandos" ==> e.Mandos
                        "companeros" ==> e.Companeros
                        "pareja" ==> e.Pareja
                        "banderas" ==> (e.Banderas |> Set.toArray)
                        "nodo" ==> e.NodoActual
                        "guitarra" ==> e.GuitarraUsada ]
        window.localStorage.setItem(clave, JS.JSON.stringify o)
    with _ -> ()

let cargar () : Estado option =
    try
        match window.localStorage.getItem clave with
        | null -> None
        | s ->
            let o = JS.JSON.parse s
            let banderas: string[] = o?banderas
            Some
                { Fisico = o?fisico
                  Disciplina = o?disciplina
                  Paracaidista = o?paracaidista
                  Resistencia = o?resistencia
                  Estres = o?estres
                  Humanidad = o?humanidad
                  Mandos = o?mandos
                  Companeros = o?companeros
                  Pareja = o?pareja
                  Banderas = Set.ofArray banderas
                  NodoActual = o?nodo
                  GuitarraUsada = o?guitarra }
    with _ -> None

let borrar () = window.localStorage.removeItem clave
