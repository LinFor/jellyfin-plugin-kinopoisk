#!/bin/bash

jprm repo add -u https://raw.githubusercontent.com/LinFor/jellyfin-plugin-kinopoisk/master/dist/ ./dist ./artifacts/*.zip
rm ./artifacts/*
