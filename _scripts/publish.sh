#!/bin/bash

set -x
if [ $TRAVIS_BRANCH == 'master' ] ; then
    docker login -u "$DOCKER_USERNAME" -p "$DOCKER_PASSWORD"

    REPO=spasmodicsoftware/authenticated-ladder
    docker build -t $REPO:$COMMIT .
	dokcer tag $REPO:$COMMIT $REPO:latest    
    dokcer tag $REPO:$COMMIT $REPO:travis-$TRAVIS_BUILD_NUMBER
	docker push $REPO
else
    echo "Not deploying, since this branch isn't master."
fi
