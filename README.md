# DontMergeMeYet

A GitHub app that adds an "in progress" status to your pull requests to avoid merging them prematurely.

DontMergeMeYet marks the pull request as pending if one of these conditions is true:

- the title contains "WIP" or "DO NOT MERGE"
- a commit message contains "contains "WIP" or "DO NOT MERGE"
- a commit message starts with "fixup!" or "squash!", indicating the branch should be [autosquashed](http://fle.github.io/git-tip-keep-your-branch-clean-with-fixup-and-autosquash.html)

Otherwise, the pull request is marked as ready to merge.

## Installation

Head to the application's [public page](https://github.com/apps/dontmergemeyet), and install the app into one or more repos.

## Privacy

DontMergeMeYet doesn't store any information on users or repositories. By using DontMergeMeYet, you grant it the following permissions:

❌ No access to code  
✔️ Read access to metadata and pull requests  
✔️ Read and write access to commit statuses
