# Yape.AntiFraud.Challenge
Desafío técnico Anti-Fraud para Yape
Diagrama de secuencia
sequenceDiagram
    participant Plataforma
    participant BaseDeDatos
    participant Kafka as Kafka (transactions-validation)
    participant AntiFraud
    participant Kafka2 as Kafka (validated-transactions-topic)
    participant ServicioTransacciones

    Plataforma->>BaseDeDatos: Crear transacción
    BaseDeDatos-->>Plataforma: Confirmación de creación
    Plataforma->>Kafka: Publicar mensaje en "transactions-validation"
    
    AntiFraud->>Kafka: Leer mensaje de "transactions-validation"
    Kafka-->>AntiFraud: Mensaje de transacción
    AntiFraud->>AntiFraud: Validar transacción
    alt Transacción aprobada
        AntiFraud->>Kafka2: Publicar mensaje en "validated-transactions-topic" (Aprobada)
    else Transacción rechazada
        AntiFraud->>Kafka2: Publicar mensaje en "validated-transactions-topic" (Rechazada)
    end
    
    ServicioTransacciones->>Kafka2: Leer mensaje de "validated-transactions-topic"
    Kafka2-->>ServicioTransacciones: Mensaje validado
    ServicioTransacciones->>ServicioTransacciones: Actualizar estado de la transacción
