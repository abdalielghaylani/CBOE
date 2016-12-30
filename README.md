# ChemBioOffice Enterprise

## Overview
This project aims to modernize the applications in ChemBioOffice Enterprise suite.

## Prerequisites
  * Install [yarn](https://yarnpkg.com/en/docs/install)
  
## Getting Started
  * Run `gradle build` to build all projects
  * Run `gradle bootRun` to build all projects and start the main application
  * Run `gradle clean` to clean up the build output
  * Run `gradle buildDocker` to run the application in docker environment
  
## Client Development
  * Run `gradle bootRun` to start the server
  * Run `npm run dev` in `subprojects/registration.client` directory to debug
  * Run `npm test` in `subprojects/registration.client` directory for unit-testing

## Links
  * [Project Setup Details](docs/project-setup.md)