#!/bin/bash
VERSION="10.9.0.0"
CHANGELOG="Fix bugs"

check_command() {
    if ! command -v $1 &> /dev/null
    then
        echo "Error: $1 could not be found. Please install it."
        exit 1
    fi
}

# Check for required commands
check_command gsed

brew link --overwrite dotnet@8
export PATH="/usr/local/opt/dotnet@8/bin:$PATH"

find . -name project.assets.json -delete

gsed -i'' "s/version: .*/version: \"$VERSION\"/" src/Jellyfin.Plugin.Kinopoisk/build.yaml
BUILDYAML=`head -$(grep -n "changelog: >" src/Jellyfin.Plugin.Kinopoisk/build.yaml | head -1 | cut -d: -f1) src/Jellyfin.Plugin.Kinopoisk/build.yaml`
echo -e "$BUILDYAML\n  $CHANGELOG" > src/Jellyfin.Plugin.Kinopoisk/build.yaml

dotnet restore ./src/Jellyfin.Plugin.Kinopoisk/
dotnet build --configuration Release ./src/Jellyfin.Plugin.Kinopoisk/

RELEASEDIR="$(pwd)/dist/kinopoisk/kinopoisk_$VERSION"
rm -rf "$RELEASEDIR" "$RELEASEDIR.zip"
mkdir -p "$RELEASEDIR"
cp "$(pwd)/src/Jellyfin.Plugin.Kinopoisk/bin/Release/net8.0/Jellyfin.Plugin.Kinopoisk.dll" "$RELEASEDIR/"
cp "$(pwd)/src/KinopoiskUnofficialInfo.ApiClient/bin/Release/net8.0/KinopoiskUnofficialInfo.ApiClient.dll" "$RELEASEDIR/"
cat << EOF > "dist/kinopoisk/kinopoisk_$VERSION/meta.json"
{
    "category": "Metadata",
    "changelog": "$CHANGELOG",
    "description": "\u0417\u0430\u0433\u0440\u0443\u0436\u0430\u0435\u0442 \u0440\u0435\u0439\u0442\u0438\u043d\u0433, \u043e\u043f\u0438\u0441\u0430\u043d\u0438\u044f, \u0430\u043a\u0442\u0451\u0440\u043e\u0432, \u0442\u0440\u0435\u0439\u043b\u0435\u0440\u044b \u0438 \u0442.\u0434. \u0441 \u0441\u0430\u0439\u0442\u0430 \u041a\u0438\u043d\u043e\u041f\u043e\u0438\u0441\u043a. \u041c\u043e\u0436\u0435\u0442 \u043f\u043e\u0442\u0440\u0435\u0431\u043e\u0432\u0430\u0442\u044c\u0441\u044f \u0437\u0430\u0440\u0435\u0433\u0438\u0441\u0442\u0440\u0438\u0440\u043e\u0432\u0430\u0442\u044c \u0441\u0432\u043e\u0439 ApiToken, \u0441\u043c. \u0438\u043d\u0444\u043e\u0440\u043c\u0430\u0446\u0438\u044e \u0432 \u043f\u0430\u0440\u0430\u043c\u0435\u0442\u0440\u0430\u0445 \u043f\u043b\u0430\u0433\u0438\u043d\u0430. \u0414\u043b\u044f \u0442\u043e\u0447\u043d\u043e\u0433\u043e \u0440\u0430\u0441\u043f\u043e\u0437\u043d\u0430\u0432\u0430\u043d\u0438\u044f \u0440\u0435\u043a\u043e\u043c\u0435\u043d\u0434\u0443\u0435\u0442\u0441\u044f \u0443\u043a\u0430\u0437\u044b\u0432\u0430\u0442\u044c id \u0444\u0438\u043b\u044c\u043c\u0430 \u0441 \u0441\u0430\u0439\u0442\u0430 \u041a\u0438\u043d\u043e\u041f\u043e\u0438\u0441\u043a \u0432 \u0438\u043c\u0435\u043d\u0438 \u0444\u0430\u0439\u043b\u0430 \u0432 \u0444\u043e\u0440\u043c\u0430\u0442\u0435 kp-12345 \u0438\u043b\u0438 kp12345. \u041f\u043e\u0434\u0440\u043e\u0431\u043d\u0435\u0435 \u0441\u043c. https://github.com/LinFor/jellyfin-plugin-kinopoisk/blob/master/README.md\n",
    "guid": "0c136f8a-ff77-4f2b-ade5-13462cae6216",
    "imageUrl": "https://kinopoisk.userecho.com/s/attachments/28876/0/1/25f8c0315e6ccb2aa6c2642e48f2c9e9.png",
    "name": "\u041a\u0438\u043d\u043e\u041f\u043e\u0438\u0441\u043a",
    "overview": "\u0418\u043d\u0444\u043e\u0440\u043c\u0430\u0446\u0438\u044f \u043e \u0444\u0438\u043b\u044c\u043c\u0430\u0445 \u0438 \u0441\u0435\u0440\u0438\u0430\u043b\u0430\u0445 \u0441 \u041a\u0438\u043d\u043e\u041f\u043e\u0438\u0441\u043a\u0430",
    "owner": "svk",
    "targetAbi": "10.9.0",
    "timestamp": "$(date -u "+%Y-%m-%dT%H:%M:%SZ")",
    "version": "$VERSION"
}
EOF
echo $( cd $RELEASEDIR; zip -j "../kinopoisk_$VERSION.zip" *)
rm -rf "$RELEASEDIR" 
HASH=$(md5sum "$RELEASEDIR.zip" | cut -d' ' -f1)

jq --arg HASH "$HASH" --arg URL "https://raw.githubusercontent.com/skrashevich/jellyfin-plugin-kinopoisk/master/dist/kinopoisk/kinopoisk_$VERSION.zip" \
    --arg TIMESTAMP "$(date -u "+%Y-%m-%dT%H:%M:%SZ")" \
    --arg VERSION "$VERSION" \
    '.[0].versions |= [{"version": $VERSION, "checksum": $HASH, "changelog": "new release", "name": "\u041a\u0438\u043d\u043e\u041f\u043e\u0438\u0441\u043a", "targetAbi": "10.9.0", "sourceUrl": $URL, "timestamp": $TIMESTAMP}] + .' \
    "$(pwd)/dist/manifest.json" > "$(pwd)/dist/manifest.json.tmp" && \
    mv "$(pwd)/dist/manifest.json.tmp" "$(pwd)/dist/manifest.json"
exit
#jprm repo add -u https://raw.githubusercontent.com/skrashevich/jellyfin-plugin-kinopoisk/master/dist/ ./dist ./artifacts/*.zip
rm -rf ./artifacts/*
git add "$RELEASEDIR.zip" "dist/manifest.json" "publish.sh" "src/Jellyfin.Plugin.Kinopoisk/build.yaml" && \
git commit -m "version $VERSION" && \
git tag -f "v$VERSION" #&& \
git push --force && git push --tags --force
