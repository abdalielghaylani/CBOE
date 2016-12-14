FROM frolvlad/alpine-oraclejdk8:slim
VOLUME /tmp
ADD registration-0.0.1-SNAPSHOT.jar app.jar
EXPOSE 18088
RUN sh -c 'touch /app.jar'
ENV JAVA_OPTS=""
ENTRYPOINT [ "sh", "-c", "java $JAVA_OPTS -Dspring.data.mongodb.uri=mongodb://mongodb/cboe-registration -Djava.security.egd=file:/dev/./urandom -jar /app.jar" ]
