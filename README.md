# Yape.AntiFraud.Challenge
Desafío técnico Anti-Fraud para Yape
Diagrama de secuencia

```mermaid
sequenceDiagram
    participant Transacciones
    participant DbContext
    participant Kafka as Kafka (transactions-validation)
    participant AntiFraud
    participant Kafka2 as Kafka (validated-transactions-topic)

    Transacciones->>DbContext: Crear transacción
    DbContext-->>Transacciones: Confirmación de creación
    Transacciones->>Kafka: Publicar mensaje en "transactions-validation"
    
    AntiFraud->>Kafka: Leer mensaje de "transactions-validation"
    Kafka-->>AntiFraud: Mensaje de transacción
    AntiFraud->>AntiFraud: Validar transacción
    alt Transacción aprobada
        AntiFraud->>Kafka2: Publicar mensaje en "validated-transactions-topic" (Aprobada)
    else Transacción rechazada
        AntiFraud->>Kafka2: Publicar mensaje en "validated-transactions-topic" (Rechazada)
    end
    
    Transacciones->>Kafka2: Leer mensaje de "validated-transactions-topic"
    Kafka2-->>Transacciones: Mensaje validado
    Transacciones->>ServicioTransacciones: Actualizar estado de la transacción
