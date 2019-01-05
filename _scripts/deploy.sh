#!/bin/bash

set -x
if [ $TRAVIS_BRANCH == 'master' ] ; then
    # Initialize a new git repo in _deploy, and push it to our server.
        
    git remote add deploy "mnemosyne@165.227.132.89:/home/mnemosyne/authenticatedLadder"
    git config user.name "Travis CI"
    git config user.email "nomadster+authenticatedLadderTravisCI@gmail.com"
    
    git push --force deploy master
else
    echo "Not deploying, since this branch isn't master."
fi