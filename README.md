# ChemBioOffice Enterprise

## Overview
This project aims to modernize the applications in ChemBioOffice Enterprise suite.

## Getting Started
  * Run `gradle build` to build all projects
  * Run `gradle bootRun` to build all projects and start the main application
  * Run `gradle clean` to clean up the build output
  * Run the following command to run the application in docker environment
  ```
  $ gradle buildDocker
  $ docker run -P -d --name mongodb mongo
  $ docker run -P -d --name cboe-reg --link mongodb pkiinformatics/registration
  $ echo To open application, browse to http://$(docker-machine ip):$(docker inspect --format '{{ (index (index .NetworkSettings.Ports "18088/tcp") 0).HostPort }}' cboe-reg)
  $ echo To connect to MongoDB, use: mongo --host $(docker-machine ip) --port $(docker inspect --format '{{ (index (index .NetworkSettings.Ports "27017/tcp") 0).HostPort }}' mongodb)
  ```

## Links
  * [Project Setup Details](docs/project-setup.md)