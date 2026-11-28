module Acto3

open Types
open Dsl

// Resumen del acto:
//  A. Preparación y aproximación (salto o marcha, según la cualificación)
//  B. Terreno hostil: ruta, emboscada y refugio con la patrulla de Marcos
//  C. Extracción: dilema del helicóptero y último tramo
//  D. Regreso, condecoración (Cruz del Mérito Militar con distintivo rojo) y tres finales según la Humanidad
//
// Final de redención plena: Humanidad >= 70 · parcial: 40-69 · cinismo persistente: < 40

let private tiene (b: string) (e: Estado) = Set.contains b e.Banderas

// ---------- Texto dinámico del epílogo (depende de las relaciones y de las banderas) ----------
let private lineaSamir (e: Estado) =
    if tiene "samir_conoce_guitarra" e then
        "Farid te hace llegar una carta de Samir: ya sabe tocar tres acordes en una guitarra de segunda mano, y la escuela de Deh Sabz tiene techo nuevo."
    else
        "Farid te hace llegar noticias de Deh Sabz: Samir va cada mañana a la escuela que ayudaste a levantar."

let private lineaMarcos (e: Estado) =
    if e.Companeros >= 50 then
        "Marcos camina ya sin muletas a tu lado y se ríe de lo mal que afinas."
    elif e.Companeros >= 30 then
        "Marcos y tú os cruzáis a menudo en el cuartel; os debéis una conversación larga, pero ya no os esquiváis."
    else
        "Marcos te saluda con cortesía. Os salvasteis la vida, pero no habéis sabido hablar de ello."

let private lineaMando (e: Estado) =
    if e.Mandos >= 50 then
        "El teniente Vidal te propone para el curso de jefe de salto."
    elif e.Mandos >= 25 then
        "El teniente Vidal te estrecha la mano con un asentimiento correcto."
    else
        "El teniente Vidal cumple el protocolo y poco más."

let private lineaLucia (e: Estado) =
    if tiene "despedida_sincera" e then
        "En tu correo hay una nota breve de Lucía: «Me alegro de que hayas vuelto entero». No pide respuesta."
    elif tiene "despedida_confusa" e then
        "Lucía no ha escrito. A veces el silencio también es una respuesta, y has aprendido a no leerlo como un castigo."
    else
        "No hay noticias de Lucía, y empiezas a aceptar que no las habrá."

let private epilogo (cabecera: string) (cierre: string) (e: Estado) =
    String.concat "\n\n" [ cabecera; lineaSamir e; lineaMarcos e; lineaMando e; lineaLucia e; cierre ]

// ---------- Texto dinámico de Marcos en el refugio ----------
let private textoMarcos (e: Estado) =
    if e.Companeros >= 50 then
        "Marcos levanta la cabeza, pálido y con el hombro vendado. «Sabía que vendrías, ermitaño. Siempre vienes, aunque te quejes.»"
    elif e.Companeros >= 30 then
        "Marcos te mira con sorpresa. «No esperaba verte aquí, Dani.» Intenta sonreír y le sale una mueca."
    else
        "Marcos tarda en reconocerte. «Has venido... No pensé que vendrías.» No hay reproche en su voz, y eso te duele más."

let nodos : Map<string, Nodo> =
    aMapa
        [ // ---------- A. Preparación y aproximación ----------
          nodo "acto3_amanecer" "Teniente Vidal"
              "«Marcos salió ayer con un equipo médico y tres soldados hacia una aldea de la sierra. Han perdido contacto hace treinta horas. Hay civiles con ellos: una doctora, dos enfermeros y varios niños. Solo hay un punto de extracción posible, y está a kilómetro y medio de donde creemos que se han refugiado.»"
              [ op "Voy a buscarle. Es mi compañero." Empatico Siempre [ Humanidad 2; Companeros 3 ] "acto3_marcos_recuerdo"
                op "Cuente conmigo, mi teniente." Disciplinado Siempre [ Mandos 3 ] "acto3_marcos_recuerdo"
                op "Si es una trampa, mejor ir con cabeza." Seco Siempre [ Estres 2 ] "acto3_marcos_recuerdo" ]

          nodo "acto3_marcos_recuerdo" "Narrador"
              "Mientras revisas el equipo, te viene a la cabeza Marcos: su risa, sus bromas pesadas, su manera de llamarte ermitaño."
              [ op "Recordar el circuito de obstáculos en que lo dejaste atrás, y apretar los dientes." Neutro (Bandera "dejo_a_marcos") [ Estres 6; Humanidad 2 ] "acto3_metodo"
                op "Recordar la marcha en que le ayudaste con la mochila." Neutro (No (Bandera "dejo_a_marcos")) [ Estres -3; Humanidad 1 ] "acto3_metodo" ]

          nodo "acto3_metodo" "Teniente Vidal"
              "«Dos opciones: el helicóptero os deja a baja cota al borde de la zona y saltáis, o vais a pie por la cresta. Más rápido por aire, más discreto por tierra. Elegid.»"
              [ prueba "Saltar desde el helicóptero a baja cota." Neutro Siempre [] AParacaidista 52 20
                    ([ Mandos 2; PonerBandera "llegada_limpia" ], "acto3_llegada_ok")
                    ([ Estres 8; Resistencia -10 ], "acto3_llegada_dura")
                prueba "Aproximarte a pie por la cresta." Disciplinado Siempre [] AFisico 55 35
                    ([ Companeros 2; PonerBandera "llegada_limpia" ], "acto3_llegada_ok")
                    ([ Estres 6 ], "acto3_llegada_dura") ]

          nodo "acto3_llegada_ok" "Narrador"
              "Llegas a tiempo y con el aliento justo. El terreno es áspero, pero conoces cada paso que has entrenado."
              [ op "Continuar." Neutro Siempre [] "acto3_ruta" ]

          nodo "acto3_llegada_dura" "Narrador"
              "Llegas tarde y magullado, con las piernas flojas. Te recompones mirando a Farid, que ha insistido en venir de guía y te espera con una cantimplora."
              [ op "Continuar." Neutro Siempre [] "acto3_ruta" ]

          // ---------- B. Terreno hostil ----------
          nodo "acto3_ruta" "Farid"
              "Farid señala dos caminos: la pista abierta, rápida pero muy visible, o un cauce seco entre rocas, más largo. «Mi gente usa el cauce. Si alguien os espera, no mirará hacia allí.»"
              [ op "Seguir el cauce que propone Farid: confías en él." Neutro (Alguna [ Bandera "habla_farid"; Bandera "escucho_al_anciano" ]) [ Humanidad 1; Companeros 1; PonerBandera "ruta_segura" ] "acto3_contacto"
                op "Confiar en Farid, aunque apenas le conoces." Empatico (No (Alguna [ Bandera "habla_farid"; Bandera "escucho_al_anciano" ])) [ Humanidad 2; Estres 3; Resistencia -10; PonerBandera "ruta_segura" ] "acto3_contacto"
                op "Tomar la pista abierta: no hay tiempo." Seco Siempre [ Estres 3; Resistencia 5 ] "acto3_contacto" ]

          nodoT "acto3_contacto" "Voz por radio" "Un chasquido seco desde la loma. Después otro. Polvo saltando entre las rocas, a unos metros de Farid. Los insurgentes han abierto fuego sobre el camino." 7
              (op "Te echas sobre Farid para cubrirle." Neutro Siempre [ Humanidad 3; Estres 8; Resistencia -10 ] "acto3_refugio")
              [ prueba "Aplicar el procedimiento de reacción y replegarte ordenadamente." Disciplinado Siempre [] ADisciplina 52 20
                    ([ Mandos 3 ], "acto3_refugio")
                    ([ Estres 8 ], "acto3_refugio")
                op "Proteger a Farid y replegarte con él." Neutro Siempre [ Humanidad 3; Companeros 2; Resistencia -15 ] "acto3_refugio"
                op "Aprovechar el cauce para rodear la emboscada." Neutro (Bandera "ruta_segura") [ Estres 2; Resistencia -10; PonerBandera "evito_emboscada" ] "acto3_refugio" ]

          nodo "acto3_refugio" "Narrador"
              "Ruinas de un antiguo puesto agrícola. Marcos, con el hombro vendado y la cara gris. Los soldados Reyes e Iván. La doctora Laila, dos enfermeros y tres niños pegados a la pared. Todos te miran como si fueras una puerta que se abre."
              [ op "Ir directo hacia Marcos." Empatico Siempre [ Companeros 4; Humanidad 2 ] "acto3_marcos"
                op "Evaluar la situación con la soldado Reyes." Disciplinado Siempre [ Mandos 2 ] "acto3_marcos"
                op "Pedir un parte rápido y revisar las salidas." Neutro Siempre [ Estres 1 ] "acto3_marcos" ]

          nodoD "acto3_marcos" "Marcos" textoMarcos
              [ op "Prometerle que saldréis todos." Empatico Siempre [ Companeros 4; Humanidad 2; Estres 3 ] "acto3_plan"
                op "Decirle que guarde fuerzas." Neutro Siempre [ Companeros 1 ] "acto3_plan"
                op "Decirle que es una orden." Disciplinado Siempre [ Mandos 1; Disciplina 1 ] "acto3_plan" ]

          // ---------- C. Extracción ----------
          nodo "acto3_plan" "Teniente Vidal"
              "«Punto de extracción a kilómetro y medio. El helicóptero puede hacer una primera pasada para heridos y civiles; la segunda será veinte minutos después. Alguien tiene que cubrir ese margen. Decidid.»"
              [ prueba "Organizar el traslado por relevos entre todos." Disciplinado Siempre [] ADisciplina 55 20
                    ([ Mandos 3; Companeros 3 ], "acto3_dilema")
                    ([ Estres 6; Companeros 1 ], "acto3_dilema")
                prueba "Cargar tú con Marcos todo el camino." Neutro Siempre [ Companeros 4 ] AFisico 58 40
                    ([ Humanidad 2 ], "acto3_dilema")
                    ([ Resistencia -10; Estres 8 ], "acto3_dilema")
                op "Dividir el grupo: tú con los civiles, Reyes con Marcos." Neutro Siempre [ Humanidad 1; Companeros 1; Resistencia -10 ] "acto3_dilema" ]

          nodoT "acto3_dilema" "Teniente Vidal" "El helicóptero se posa entre polvo y gritos. Caben los heridos y los civiles. Los demás tendréis que esperar la segunda pasada, veinte minutos en un lugar donde ya os han localizado. Vidal te mira. Marcos está herido y tú también estás agotado: podrías subir con él ahora." 10
              (op "Dudas, y el helicóptero se lleva a Marcos y a los civiles. Te quedas con Reyes e Iván." Neutro Siempre [ Humanidad 3; Estres 10; PonerBandera "ultimo_en_salir" ] "acto3_espera")
              [ op "Subir a Marcos y quedarte con Reyes e Iván a esperar la segunda pasada." Neutro Siempre [ Humanidad 6; Resistencia -20; Estres 6; PonerBandera "ultimo_en_salir" ] "acto3_espera"
                op "Subir con Marcos: te lo has ganado." Seco Siempre [ Humanidad -2; Estres -4; Companeros -3; Mandos -2; PonerBandera "subio_primero" ] "acto3_espera"
                op "Dejar que Vidal asigne los asientos." Disciplinado Siempre [ Mandos 3; Disciplina 1 ] "acto3_espera" ]

          nodoT "acto3_espera" "Narrador" "Veinte minutos eternos. Desde la loma vuelven los disparos, y una ráfaga levanta polvo en el único camino hacia el punto de extracción. Iván se queda bloqueado, con la espalda pegada a una roca." 7
              (op "Te quedas un segundo mirando el camino sin decidir." Seco Siempre [ Estres 10; Companeros -2 ] "acto3_extraccion")
              [ prueba "Dirigir la retirada: tú abres camino y ellos te siguen." Disciplinado Siempre [ Mandos 2 ] ADisciplina 52 20
                    ([ Companeros 3 ], "acto3_extraccion")
                    ([ Estres 8; Resistencia -10 ], "acto3_extraccion")
                prueba "Sacar a Iván de la roca cargando con él." Neutro Siempre [ Humanidad 2; Companeros 3 ] AFisico 54 30
                    ([ Humanidad 1 ], "acto3_extraccion")
                    ([ Resistencia -10; Estres 8 ], "acto3_extraccion")
                op "Pedir al helicóptero que adelante la pasada." Neutro Siempre [ Estres 3; Mandos 1 ] "acto3_extraccion" ]

          nodo "acto3_extraccion" "Narrador"
              "El rotor aplasta la hierba seca. Subís de uno en uno; la mano de Reyes te agarra del chaleco para ayudarte a entrar. Abajo, la sierra se encoge hasta ser un dibujo de arrugas y sombras."
              [ op "Mirar atrás hasta que desaparezca." Neutro Siempre [ Humanidad 1; Estres -5 ] "acto3_hospital"
                op "Cerrar los ojos." Seco Siempre [ Estres -3 ] "acto3_hospital" ]

          nodo "acto3_hospital" "Narrador"
              "Hospital de campaña. Marcos está estable, con el hombro inmovilizado. Los niños duermen en una sala contigua. Tú no has soltado todavía el chaleco."
              [ op "Sentarte junto a Marcos sin hablar." Neutro Siempre [ Companeros 4; Humanidad 2; Estres -4 ] "acto3_guitarra"
                op "Contarle lo de Samir." Empatico (Bandera "salvo_a_samir") [ Humanidad 3; Companeros 3 ] "acto3_guitarra"
                op "Decirle que ya estáis en paz." Seco Siempre [ Companeros -1 ] "acto3_guitarra" ]

          nodo "acto3_guitarra" "Narrador"
              "Esa noche, la guitarra sigue donde la dejaste. Alguien en la sala de al lado tararea bajito."
              [ op "Tocar para Marcos y los demás." Neutro Siempre [ Humanidad 4; Estres -10; Companeros 4; PonerBandera "toca_en_hospital" ] "acto3_regreso"
                op "Tocar solo, en voz baja." Neutro Siempre [ Humanidad 2; Estres -6 ] "acto3_regreso"
                op "No tocar esta noche." Seco Siempre [ Estres 2 ] "acto3_regreso" ]

          // ---------- D. Regreso, condecoración y finales ----------
          nodo "acto3_regreso" "Narrador"
              "Semanas después, de vuelta en España. Hay un acto en el patio de armas del acuartelamiento. Formación completa, banderas movidas por el viento. Te han avisado de que te van a condecorar."
              [ op "Ir hacia la formación." Neutro Siempre [] "acto3_veredicto" ]

          // Hay exactamente una opción visible según la Humanidad: da lugar a los tres finales.
          nodo "acto3_veredicto" "Narrador"
              "Pronuncian tu nombre. Das un paso al frente."
              [ op "Avanzar." Neutro (HumanidadMin 70) [] "acto3_ceremonia_plena"
                op "Avanzar." Neutro (HumanidadEntre (40, 69)) [] "acto3_ceremonia_parcial"
                op "Avanzar." Neutro (HumanidadEntre (0, 39)) [] "acto3_ceremonia_cinica" ]

          // --- Final 1: redención plena ---
          nodo "acto3_ceremonia_plena" "Jefe de la Brigada"
              "Se lee la concesión: Cruz del Mérito Militar con distintivo rojo, por haber actuado con valor durante las operaciones en Afganistán y haber puesto la vida de otros por delante de la propia. Cuando te prenden la cruz en el pecho, ya no piensas en el metal. Piensas en Samir, en Marcos, en la escuela."
              [ op "Saludar con la voz firme." Disciplinado Siempre [ Mandos 2 ] "acto3_epilogo_plena"
                op "Buscar a Marcos con la mirada entre la formación." Empatico Siempre [ Companeros 2; Humanidad 2 ] "acto3_epilogo_plena"
                op "Dedicar la cruz en silencio a Samir." Neutro Siempre [ Humanidad 2 ] "acto3_epilogo_plena" ]

          nodoD "acto3_epilogo_plena" "Epílogo"
              (epilogo
                  "Semanas después, la cruz descansa en un cajón y ya no necesitas mirarla para recordar lo que significa."
                  "Esa noche, en la cantina, sacas la guitarra y tocas ante toda la sección. La guitarra, que durante años fue un refugio para esconderte, es ahora una forma de volver. El cabo cínico que aterrizó en Afganistán ha quedado atrás: no por la cruz, sino por un niño, un puñado de compañeros y una decisión que no calculaste.")
              [ op "Fin." Neutro Siempre [] "acto3_fin" ]

          // --- Final 2: redención parcial ---
          nodo "acto3_ceremonia_parcial" "Jefe de la Brigada"
              "Se lee la concesión: Cruz del Mérito Militar con distintivo rojo, por acciones realizadas con valor en operaciones militares. Aplauden. Notas el peso de la cruz y también el de las dudas: no todo lo que hiciste te enorgullece, pero ya no te escondes de ello."
              [ op "Saludar con corrección." Disciplinado Siempre [ Mandos 1 ] "acto3_epilogo_parcial"
                op "Agradecer en voz baja a quien tengas al lado." Neutro Siempre [ Companeros 2; Humanidad 1 ] "acto3_epilogo_parcial"
                op "Mirar al frente y aguantar el momento." Seco Siempre [ Estres 2 ] "acto3_epilogo_parcial" ]

          nodoD "acto3_epilogo_parcial" "Epílogo"
              (epilogo
                  "Llevas la cruz con un orgullo que todavía no sabes explicar."
                  "Cambiaste de verdad, pero con costes: hay conversaciones pendientes y noches en que el ruido de los generadores vuelve. Tocas la guitarra a ratos, en voz baja, con la puerta cerrada. Es un comienzo.")
              [ op "Fin." Neutro Siempre [] "acto3_fin" ]

          // --- Final 3: cinismo persistente ---
          nodo "acto3_ceremonia_cinica" "Jefe de la Brigada"
              "Se lee la concesión: Cruz del Mérito Militar con distintivo rojo, por acciones realizadas con valor en operaciones militares. Aplauden todos. La cruz pesa menos de lo que debería; tú miras el reloj del patio y esperas a que termine."
              [ op "Saludar con la fórmula de rigor." Disciplinado Siempre [ Mandos 1 ] "acto3_epilogo_cinica"
                op "Encogerte de hombros ante las felicitaciones." Seco Siempre [ Companeros -2 ] "acto3_epilogo_cinica"
                op "Pensar en el niño y no decirlo a nadie." Neutro Siempre [ Humanidad 1 ] "acto3_epilogo_cinica" ]

          nodoD "acto3_epilogo_cinica" "Epílogo"
              (epilogo
                  "Guardas la cruz en su estuche sin mirarla."
                  "Todo el mundo dice que la mereces; tú no consigues sentirlo. La guitarra sigue en su funda. Quizá otro día, otra misión, otro niño. Por ahora el muro sigue en pie, aunque ya tiene una grieta.")
              [ op "Fin." Neutro Siempre [] "acto3_fin" ]

          nodo "acto3_fin" "Fin"
              "Gracias por jugar a «Salto de fe». Pulsa «Reiniciar» para empezar una nueva partida y probar otro camino."
              [] ]
