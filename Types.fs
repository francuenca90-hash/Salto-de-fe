module Types

/// Tono de una respuesta: sirve para bloquear o desbloquear opciones según el estrés.
type Tono =
    | Neutro
    | Seco
    | Empatico
    | Disciplinado
    | Impulsivo

type Condicion =
    | Siempre
    | HumanidadMin of int
    | EstresMax of int
    | DisciplinaMin of int
    | Bandera of string

type Efecto =
    | Estres of int
    | Humanidad of int
    | Resistencia of int
    | Fisico of int
    | Disciplina of int
    | Paracaidista of int
    | Mandos of int        // confianza
    | Companeros of int    // cohesión
    | Pareja of int        // vínculo
    | PonerBandera of string

type Opcion =
    { Texto: string
      Tono: Tono
      Condicion: Condicion
      Efectos: Efecto list
      Siguiente: string }

type Nodo =
    { Id: string
      Personaje: string
      Texto: string
      Opciones: Opcion list }

type Estado =
    { // Atributos base (0-100)
      Fisico: int
      Disciplina: int
      Paracaidista: int
      // Variables derivadas
      Resistencia: int
      Estres: int
      Humanidad: int
      // Relaciones
      Mandos: int
      Companeros: int
      Pareja: int
      // Progreso
      Banderas: Set<string>
      NodoActual: string
      GuitarraUsada: bool }
