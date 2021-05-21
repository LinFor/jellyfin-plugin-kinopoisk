#!/bin/bash

JELLYFIN_WEB_URL=$(curl -sSL https://api.github.com/repos/jellyfin/jellyfin-web/releases | grep -E 'tarball_url.*jellyfin-web.*$'  | head -n 1 | cut -d '"' -f 4)
mkdir /tmp/jellyfin-web
curl -sSL $JELLYFIN_WEB_URL | tar xzC /tmp/jellyfin-web
mkdir /jellyfin-web
mv /tmp/jellyfin-web/jellyfin-*/{,.[^.]}* /jellyfin-web
rm -rf /tmp/jellyfin-web
cd /jellyfin-web
npm install
npm run build:development
