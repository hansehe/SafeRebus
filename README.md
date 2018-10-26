# SafeRebus

## Introduction
SafeRebus is a concept solution showing how [Rebus](https://github.com/rebus-org/Rebus) may be extended with security options to preserve safe message transactions in a distributed microservice system. This solution implements a simple version of the [outbox](https://docs.particular.net/nservicebus/outbox/) pattern to preserve safe message transactions.

The solution spins up to replicas of the same service and both of them continuously sends a set of requests which they expect to find a corresponding unique response with, during a short period of time. Unfortunately, the second replica seems to contain some severe bugs, thus it tries to break the message transactions by throwing random exceptions and continuously spamming with non-usable dummy messages. The solution will stop if one of the services doesn't find the corresponding response in the database before timeout, so it is critical that none of the messages are lost.

## Get Started
1. Install Docker
2. Install python and pip
    - Windows:  https://matthewhorne.me/how-to-install-python-and-pip-on-windows-10/
    - Ubuntu: Python is installed by default
        - Install pip: sudo apt-get install python-pip
3. Install python dependencies:
    - -> pip install -r requirements.txt
4. See available commands:
    - -> python DockerBuild.py help

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
    - [SafeRebus.sln](src/SafeRebus/SafeRebus.sln)
5. Stop development when you feel like it:
    - python DockerBuild.py stop-dev

## Additional Info
The solution spins up a [RabbitMq](https://www.rabbitmq.com/) message broker, and a [PostgreSQL](https://www.postgresql.org/) database. Additionally, the [Portainer](https://portainer.io/) container management service is included to manage the container services.
- Locate Portainer at: http://localhost:9000
- The PostgreSQL database exposes port 5433 on the localhost for development outside of containers.
    - Access the database with the the PgAdmin4 container service on port [8080](http://localhost:8080).
        - Username/password: admin/admin
    - Username/password to access the database: saferebus/saferebus
- The RabbitMq service exposes port 5672 (AMQP) and [15672](http://localhost:15672) (UI management) on the localhost, also for development outside of containers.
    - Username/password to access RabbitMq management UI: rabbituser/rabbitpassword
