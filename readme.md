# PaulPhillips.Framework.Feature (Microservice Framework)

> [!CAUTION]
>
> This documentation is work in progress, I will be updating this on a daily basis.

Introducing Microservice Framework, the compact solution designed to enhance Microservice development with Vertical Slice Architecture in mind.

Key Features:

1. **Vertical Slice Architecture Support:** Seamlessly integrate Vertical Slice Architecture into your Microservices, simplifying development and maintenance.
2. **Built-in Health Check:** Ensure the reliability of your Microservices with a built-in health check feature, enabling real-time monitoring of service status.
3. **Idempotency:** Guarantee consistency and reliability across operations with native idempotency support, minimizing unintended side effects.
4. **Messaging (Rabbit MQ):** Facilitate efficient communication and coordination between Microservices components with integrated messaging support via Rabbit MQ.
5. **Security (JWT):** Safeguard your Microservices with robust security measures, including JWT-based authentication, ensuring data integrity and confidentiality.
6. **Structured Logging (Prometheus, Grafana, Blackbox), Error Handling, and Security:** Maintain a resilient application ecosystem with structured logging through Prometheus, Grafana, and Blackbox, comprehensive error handling mechanisms, and fortified security protocols.

### Terminology:

1. **Feature:** Represents a distinct vertical slice of functionality within the framework, encapsulating core logic.
2. **Command:** Execute server-side actions in alignment with the CQRS pattern, enabling efficient handling of write operations.
3. **Query:** Perform read-only actions integral to the CQRS pattern, facilitating data retrieval without altering the system state.

(Note: A feature can only be designated as either a Command OR a Query, not both.)

Elevate your Microservices architecture with Microservice Framework, empowering developers to build scalable, resilient, and secure applications with unparalleled ease and efficiency. 

Requirements

1. NET 8
2. Redis
3. Prometheus
4. Blackbox
5. Grafana
6. Rabbit MQ

[Documents]: https://docs.paulphillips.guru

