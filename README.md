# ConsulOnContainers

## Introduction
ConsulOnContainers is a consept library around Consul, and it shows how to use Consul as a service registration orchestrator.  
In summary, each service registers with Consul during bootup with information on which IP address to locate the service, and how to do health checks on the respective service.

The solution includes an API service which registers itself with Consul, and a client service which locates the API service using Consul.
- [APIService](/src/Services/APIService/)
- [ClientService](/src/Services/ClientService/)

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
2. Open a new cmd window and build solution as container images:
    - python DockerBuild.py build
3. Test solution as containers:
    - python DockerBuild.py test
4. Run solution as containers:
    - python DockerBuild.py run
5. Stop development when you feel like it:
    - python DockerBuild.py stop-dev

## Nice To Know
Consul is deployed in a cluster of Consul services, and there should only be one Consul service on a each node in the cluster, thus it is not possible to run this concept in Swarm mode. Each Consul service will be in conflict if they are running on a single node if the cluster is running in Swarm mode. The concept is therefore deployed with bridge networks running on a single node.
- Wanna know more about consul? 
    - https://www.consul.io/docs/internals/architecture.html