#!/bin/bash

jprm repo add -u https://raw.githubusercontent.com/skrashevich/jellyfin-plugin-kinopoisk/master/dist/ ./dist ./artifacts/*.zip
rm ./artifacts/*
