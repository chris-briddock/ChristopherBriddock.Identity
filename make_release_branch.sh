#!/bin/bash

if [ $# -eq 0 ]; then
    echo "Usage: $0 <version>"
    exit 1
fi

release_version="$1"
release_branch="release/$release_version"

# Ensure we are on the main branch
git checkout main

# Create a release branch
git checkout -b "$release_branch"

# Apply tags
git tag -a "$release_version" -m "Release version $release_version"

# Push changes and tags to the remote repository
git push origin "$release_branch"
git push --tags