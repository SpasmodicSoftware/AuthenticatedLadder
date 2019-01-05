#!/bin/bash

set -x
if [ $TRAVIS_BRANCH == 'master' ] ; then
    # Initialize a new git repo in _deploy, and push it to our server.
    mkdir _deploy
    cd _deploy
    git init
        
    git remote add deploy "mnemosyne@"
    git config user.name "Travis CI"
    git config user.email "nomadster+authenticatedLadderTravisCI@gmail.com"
    
    git add .
    git commit -m "Deploy"
    git push --force deploy master
else
    echo "Not deploying, since this branch isn't master."
fi