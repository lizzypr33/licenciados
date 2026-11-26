# Diagrama de clases – Entidades (SIGEPOSTA, módulo de Admisión)

Namespace: `SigepostaAdmision.Entidades`

```mermaid
classDiagram
    direction TB

    class Persona {
        <<abstract>>
        -string dni
        -string nombres
        -string apellidos
        -DateTime fechaNacimiento
        -string telefono
        +string Dni
        +string Nombres
        +string Apellidos
        +DateTime FechaNacimiento
        +string Telefono
        #Persona()
        #Persona(dni, nombres, apellidos, fechaNacimiento, telefono)
        +ObtenerEdad() int
        +MostrarDatos() string
    }

    class Paciente {
        -string numeroHistoriaClinica
        -string direccion
        -string postaAsignada
        +string NumeroHistoriaClinica
        +string Direccion
        +string PostaAsignada
        +Paciente()
        +Paciente(dni, nombres, apellidos, fechaNacimiento, telefono, numeroHistoriaClinica, direccion, postaAsignada)
        +HistoriaClinica Historia
        +RegistrarAtencion(Cita c) Atencion
        +MostrarDatos() string
    }

    class PersonalSalud {
        <<abstract>>
        -string codigoColegiatura
        -string postaAsignada
        +string CodigoColegiatura
        +string PostaAsignada
        #PersonalSalud()
        #PersonalSalud(..., codigoColegiatura, postaAsignada)
        +Atender(Paciente p)* string
        +MostrarDatos() string
    }

    class Medico {
        -string especialidad
        +string Especialidad
        +Medico()
        +Medico(..., especialidad)
        +Atender(Paciente p) string
        +MostrarDatos() string
    }

    class Enfermero {
        -string areaAsignada
        +string AreaAsignada
        +Enfermero()
        +Enfermero(..., areaAsignada)
        +Atender(Paciente p) string
        +MostrarDatos() string
    }

    class EstadoCita {
        <<enumeration>>
        Pendiente
        EnCola
        Atendida
        Cancelada
    }

    class Cita {
        -int idCita
        -Paciente paciente
        -Medico medico
        -DateTime fecha
        -TimeSpan hora
        -EstadoCita estado
        -string consultorio
        -int numeroTurno
        +int IdCita
        +Paciente Paciente
        +Medico Medico
        +DateTime Fecha
        +TimeSpan Hora
        +EstadoCita Estado
        +string Consultorio
        +int NumeroTurno
        +Cita()
        +Cita(idCita, paciente, medico, fecha, hora, consultorio, numeroTurno)
        +Reprogramar(DateTime nuevaFecha) void
        +Reprogramar(string nuevoConsultorio) void
        +Cancelar() void
        +PonerEnCola() void
        +MarcarAtendida() void
    }

    class HistoriaClinica {
        -string numeroHistoria
        -Paciente paciente
        -DateTime fechaApertura
        -List~Atencion~ listaAtenciones
        -string diagnosticoCifrado
        +string NumeroHistoria
        +Paciente Paciente
        +DateTime FechaApertura
        +List~Atencion~ ListaAtenciones
        +string DiagnosticoCifrado
        +HistoriaClinica()
        +HistoriaClinica(numeroHistoria, paciente, fechaApertura)
    }

    class Atencion {
        -int idAtencion
        -int idCita
        -int idHistoria
        -int idUsuario
        -DateTime fechaHora
        -string observaciones
        +Atencion()
        +Atencion(idAtencion, idCita, idHistoria, idUsuario, fechaHora, observaciones)
    }

    class Posta {
        -int idPosta
        -string nombre
        -string distrito
        -string categoria
        -List~PersonalSalud~ listaPersonal
        -List~Paciente~ listaPacientes
        +int IdPosta
        +string Nombre
        +string Distrito
        +string Categoria
        +List~PersonalSalud~ ListaPersonal
        +List~Paciente~ ListaPacientes
        +Posta()
        +Posta(idPosta, nombre, distrito, categoria)
    }

    %% Herencia
    Persona <|-- Paciente
    Persona <|-- PersonalSalud
    PersonalSalud <|-- Medico
    PersonalSalud <|-- Enfermero

    %% Asociaciones
    Cita "*" --> "1" Paciente : paciente
    Cita "*" --> "1" Medico : medico
    Cita --> EstadoCita : estado
    HistoriaClinica "1" --> "1" Paciente : pertenece a
    HistoriaClinica "1" *-- "*" Atencion : listaAtenciones
    Posta "1" o-- "*" PersonalSalud : listaPersonal
    Posta "1" o-- "*" Paciente : listaPacientes
    Paciente ..> Cita : RegistrarAtencion(Cita)
    Paciente ..> Atencion : crea
    Atencion --> Cita : idCita
```

**Leyenda:** `-` privado, `+` público, `#` protegido, `~` interno, `*` método abstracto,
`<|--` herencia, `-->` asociación, `o--` agregación, `..>` dependencia.
La clase auxiliar interna `Validacion` (validaciones comunes de los `set`) no se incluye porque no es una entidad del dominio.
