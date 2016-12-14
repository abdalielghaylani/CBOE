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
* Create `src\docker\Dockerfile` in projects that should be containerized
* Modify `build.gradle` file of those projects to include `buildDocker` task

