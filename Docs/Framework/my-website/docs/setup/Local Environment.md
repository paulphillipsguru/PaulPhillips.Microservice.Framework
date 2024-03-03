---
sidebar_label: 'Local Environment'
sidebar_position: 1
---



## Getting Started

You will require to have Docker running locally.

The following will be required:

1. NET 8
2. Redis
3. Prometheus
4. Blackbox
5. Grafana
6. Rabbit MQ

To save time, I have a docker compose file that will setup these services, which can be found at: 

https://github.com/paulphillipsguru/PaulPhillips.Microservice.Framework/tree/master/Docker

(ensure that you download all folders as well as the docker compose file)

Below are the urls to access the relevant platforms:

Prometheus: http://localhost:9090/

Blackbox: http://localhost:9115/

RabbitMQ: http://localhost:15672/

Grafana: http://localhost:3400/
