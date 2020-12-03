---
layout: page
title: Contributor's Guide
permalink: /contributor/
order: 4
---

# Overview
Contributions to "Tales of Jord" are always welcome. There are two types of contributions: code contributions and content contributions.
This guide describes how to do both.

# Code Contributions
The project source code is hosted at github: <https://github.com/rds1983/Jord>

The code contribution process is the standard one: fork the repo and make a PR.

# Content Contributions
If you open "data" folder of the game, you'll find the whole game data in the JSON format. If you make any changes to it and restart the game, 
the changes would be applied. See next section("Content Description") for the detailed description of the game data content.

It's important to note, that there's no difference on how JSON files are named. Only their content matter. The game parses the "data" folder recursively
on startup, loading every ".json" file. So if you want to add a new TileInfo, you can either modify TileInfo.json or create new TileInfo2.json, the result 
will be similar.

Now, let's say, you've finished working upon the content and would like to contribute it.
There are two ways to do it:
1. If you're familiar with how git and github works, then the process is similar to Code Contributions in the previous section. As the whole game data is 
available in the github repo.

2. Zip all relevant JSON files and email the archive to netframeworker at gmail.com.

# Content Description