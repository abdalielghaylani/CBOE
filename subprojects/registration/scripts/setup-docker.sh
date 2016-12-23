#!/bin/bash

eval "$(docker-machine env $1)"
echo Starting mongodb...
docker run -P -d --name mongodb mongo
echo Starting registration...
docker run -P -d --name cboe-registration --link mongodb pkiinformatics/registration
echo To open application, browse to:
echo http://$(docker-machine ip):$(docker inspect --format '{{ (index (index .NetworkSettings.Ports "18088/tcp") 0).HostPort }}' cboe-registration)
echo To connect to MongoDB, use:
echo mongo --host $(docker-machine ip) --port $(docker inspect --format '{{ (index (index .NetworkSettings.Ports "27017/tcp") 0).HostPort }}' mongodb)
exit 0
