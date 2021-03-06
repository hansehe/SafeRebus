# SafeRebus

## Introduction
SafeRebus is a concept solution introducing how [Rebus](https://github.com/rebus-org/Rebus) may be extended with security options to preserve safe message transactions in a distributed microservice system, and this solution implements the [outbox](https://docs.particular.net/nservicebus/outbox/) pattern.

The solution spins up two replicas of the SafeRebus service and both of them continuously sends a set of requests which they expects to find a corresponding unique response with, during a short period of time. Unfortunately, the second replica seems to contain some severe bugs, thus it tries to break the message transactions by throwing random exceptions when handling the message requests. Additionally, multiple other spamming services tries to spam the queue with dummy requests with the intention of breaking any of the other services. The solution will stop if any of the services doesn't find the corresponding response in the database before timeout, so it is critical that none of the messages are lost.

## Get Started
1. Install [Docker](https://www.docker.com/)
2. Install [Python](https://www.python.org/) and [pip](https://pypi.org/project/pip/)
    - Windows:  https://matthewhorne.me/how-to-install-python-and-pip-on-windows-10/
    - Ubuntu: Python is installed by default
        - Install pip: sudo apt-get install python-pip
3. Install python dependencies:
    - pip install -r requirements.txt
4. See available commands:
    - python DockerBuild.py help

## Build & Run
1. Start domain development by deploying service dependencies:
    - python DockerBuild.py start-dev
2. Build solution as container images:
    - python DockerBuild.py build
3. Test solution in containers:
    - python DockerBuild.py test
4. Run solution in containers:
    - python DockerBuild.py run
5. Open solution and continue development:
    - [SafeRebus](src/SafeRebus)
6. Publish new nuget version:
    - Bump version in [CHANGELOG.md](CHANGELOG.md)
    - Expose your api key by exposing the environment variable `API_KEY`
        - Linux:
            - export API_KEY=<YOUR-API_KEY>
        - Powershell:
            - $env:API_KEY = "<YOUR-API_KEY>"
    - python DockerBuild.py publish
7. Stop development when you feel like it:
    - python DockerBuild.py stop-dev

## Outbox Pattern & Duplication Filter
The outbox pattern involves storing all outgoing messages before invoking the distributed processes to send the outgoing messages. An accompaning task or service continuously resends outgoing messages which should have been sent, but failed due to a failure during the sending process, thus all outgoing messages will eventually be sent. The duplication filter ignores all duplicated message handle sessions, which may occur if a message handle process was commited, but some other occurence raised an exception before the message was acknowledged.

It should be noted the the outbox pattern is unecessary if all services are idempotent, meaning all messages may be handled multiple times with the same outcome.

## Standard Adapters
SafeRebus implements a proposed standard adapter to make different amqp libraries cope with another.
The standard adapter follows the [robustness principle](https://en.wikipedia.org/wiki/Robustness_principle), which states the following:

- `Be conservative in what you do, be liberal in what you accept from others`.

The adapter will allow a standard set of headers with body types from other libraries implementing the amqp protocol with RabbitMq, which is understood by every party. Outgoing messages will contain serialized bodies specific to the respective library (Json is default by Rebus) with headers specified by the [SafeStandard](src/SafeRebus/Standards) proposed specification.

As an example, the safe standard is implemented with [NServiceBus](https://particular.net/nservicebus), which communicates with the SafeRebus implementation using the SafeStandard headers.

## Additional Info
The solution spins up a [RabbitMq](https://www.rabbitmq.com/) message broker, and a [PostgreSQL](https://www.postgresql.org/) database. Additionally, the [Portainer](https://portainer.io/) container management service is included to manage the container services.
- Locate Portainer at: http://localhost:9000
- The PostgreSQL database exposes port 5433 on the localhost for development outside of containers.
    - Access the database with the the PgAdmin4 container service on port [8080](http://localhost:8080).
        - Username/password: admin/admin
    - Username/password to access the database: saferebus/saferebus
- The RabbitMq service exposes port 5672 (AMQP) and [15672](http://localhost:15672) (UI management) on the localhost, also for development outside of containers.
    - Username/password to access RabbitMq management UI: rabbituser/rabbitpassword

## Buildsystem
- [DockerBuildSystem](https://github.com/DIPSAS/DockerBuildSystem)
- [SwarmManagement](https://github.com/DIPSAS/SwarmManagement)
