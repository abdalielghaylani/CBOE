#!/bin/bash

eval "$(docker-machine env $1)"
if [[ -n $(docker ps -q -f name=mongodb) ]]; then
    echo Stopping mongodb...
    docker stop mongodb  >/dev/null 2>/dev/null
fi
if [[ -n $(docker ps -a -q -f name=mongodb) ]]; then
    echo Deleting mongodb container...
    docker rm mongodb  >/dev/null 2>/dev/null
fi
if [[ -n $(docker ps -q -f name=cboe-registration) ]]; then
    echo Stopping registration application...
    docker stop cboe-registration  >/dev/null 2>/dev/null
fi
if [[ -n $(docker ps -a -q -f name=cboe-registration) ]]; then
    echo Removing registration container...
    docker rm cboe-registration  >/dev/null 2>/dev/null
fi
if [[ -n $(docker images -q pkiinformatics/registration) ]]; then
    echo Removing registration image...
    docker rmi pkiinformatics/registration  >/dev/null 2>/dev/null
fi
exit 0
