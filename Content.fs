module Content

open Types

let private op texto tono cond efectos sig =
    { Texto = texto; Tono = tono; Condicion = cond; Efectos = efectos; Siguiente = sig }

let nodos : Map<string, Nodo> =
    [ { Id = "prologo_guitarra"
        Personaje = "Narrador"
        Texto = "Sábado por la mañana. El apartamento huele a café frío. La guitarra sigue apoyada junto a la ventana, con una cuerda aflojada desde hace semanas."
        Opciones =
          [ op "Cogerla y afinar la cuerda." Neutro Siempre [ Estres -5; PonerBandera "toco_prologo" ] "prologo_lucia"
            op "Ignorarla. No hay tiempo para tonterías." Seco Siempre [ Estres 3 ] "prologo_lucia" ] }

      { Id = "prologo_lucia"
        Personaje = "Lucía"
        Texto = "Casi no hablas últimamente, Dani. ¿Vas a pasarte otro fin de semana encerrado entre tu cuartel y tu cabeza?"
        Opciones =
          [ op "Estoy cansado, nada más." Seco Siempre [ Pareja -3 ] "prologo_marcos"
            op "Tienes razón. Salgamos a comer." Empatico Siempre [ Pareja 5; Humanidad 2 ] "prologo_marcos"
            op "Es mi trabajo, Lucía." Disciplinado Siempre [ Pareja -2; Disciplina 1 ] "prologo_marcos"
            op "¡Déjame en paz!" Impulsivo Siempre [ Pareja -10; Estres 5 ] "prologo_marcos" ] }

      { Id = "prologo_marcos"
        Personaje = "Marcos"
        Texto = "Lunes, cuartel. Marcos te da una palmada en la espalda. «Han abierto plazas para Afganistán. ¿Te apuntas o sigues de ermitaño?»"
        Opciones =
          [ op "Ya veremos." Seco Siempre [ Companeros -2 ] "prologo_sargento"
            op "Si vas tú, voy yo." Empatico Siempre [ Companeros 6; Humanidad 2 ] "prologo_sargento"
            op "Depende de lo que diga el sargento." Disciplinado Siempre [ Mandos 3 ] "prologo_sargento" ] }

      { Id = "prologo_sargento"
        Personaje = "Sargento Rubio"
        Texto = "Ibáñez, he visto tu último salto: técnica correcta, cabeza en otra parte. Aquí no vale con saltar bien, hay que confiar en el de al lado."
        Opciones =
          [ op "A sus órdenes, sargento." Disciplinado Siempre [ Mandos 4; Disciplina 2 ] "prologo_fin"
            op "Yo salto solo, sargento." Seco Siempre [ Mandos -4; Companeros -2 ] "prologo_fin"
            op "Tiene razón. Lo intentaré." Empatico (HumanidadMin 15) [ Mandos 3; Humanidad 3 ] "prologo_fin" ] }

      { Id = "prologo_fin"
        Personaje = "Narrador"
        Texto = "Termina el prólogo. El Acto I (instrucción) todavía no está escrito: aquí empezará la fase de pruebas físicas y cursos de salto."
        Opciones = [] } ]
    |> List.map (fun n -> n.Id, n)
    |> Map.ofList
