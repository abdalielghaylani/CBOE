# Project Setup

## Gradle Project Setup
### Getting Started
* Follow instructions on [getting started with Gradle](https://spring.io/guides/gs/gradle/)
* Create `build.gradle` in the root directory
* Use `gradle wrapper` to add wrapper goods to the project

### Sub-Projects
* Refer to [this documentation](https://docs.gradle.org/current/userguide/multi_project_builds.html) on how to set up multi-project builds with Gradle
* Create a new directory, `subprojects`
* Add projects to `subprojects` directory
* Remove Gradle wrapper from sub-projects if any
* Add `settings.gradle` in the root directory to include sub-projects

### Docker
* Refer to [this documentation](https://github.com/Transmode/gradle-docker) on Gradle Docker plugin
* Create `src/docker/Dockerfile` in projects that should be containerized based on [this documentation](https://spring.io/guides/gs/spring-boot-docker/)
* Modify `build.gradle` file of those projects to include `buildDocker` task

### MongoDB
* Refer to [this documentation](http://docs.spring.io/spring-data/mongodb/docs/current/reference/html/) on how to use Spring Data MongoDB
* [Install MongoDB](https://docs.mongodb.com/manual/administration/install-community/) or run dockerized MongoDB using `docker run -P -d --name mongodb mongo`
* Update `src/main/resources/application.properties` to include `spring.data.mongodb.uri`

### Client
* Refer to [this documentation](https://semaphoreci.com/community/tutorials/setting-up-angular-2-with-webpack) on how to set up Angular 2 project with Webpack
* Create a directory for the client project
* Use `npm init -f` to create `package.json`
* Install Angular 2 using the following command:
    ```
    $ npm install -save @angular/common @angular/compiler @angular/core @angular/platform-browser @angular/platform-browser-dynamic \
      es6-shim reflect-metadata rxjs@5.0.0-rc.4 zone.js
    
    ```
* Install TypeScript dependencies using:
    ```
    $ npm install --save-dev typescript tslint typings
    ```
* Install Webpack dependencies using:
    ```
    $ npm install --save-dev webpack webpack-dev-server html-webpack-plugin raw-loader ts-loader tslint-loader
    ```
* Install unit-testing dependencies using:
    ```
    $ npm install --save-dev karma karma-jasmine jasmine-core karma-chrome-launcher karma-phantomjs-launcher \
      phantomjs-prebuilt karma-sourcemap-loader karma-webpack
    ```
* Configure Typescript (by generating typings.json) using `./node_modules/.bin/typings init`