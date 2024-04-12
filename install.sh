#!/bin/bash

if [ "x$0" = "xsh" ]; then
  curl -f -L -s https://raw.githubusercontent.com/pieza/cpm/main/install.sh > cpm-install-$$.sh
  ret=$?
  if [ $ret -eq 0 ]; then
    (exit 0)
  else
    rm cpm-install-$$.sh
    echo "failed to download script" >&2
    exit $ret
  fi
  sh cpm-install-$$.sh
  ret=$?
  rm cpm-install-$$.sh
  exit $ret
fi

DEPENDENCIES="curl unzip"
DOTNET_VERSION="8.0.3"
BASE_URL="https://api.github.com/repos/pieza/cpm"
APP_NAME="cpm"
INSTALL_DIR="$HOME/.$APP_NAME/bin"
PLATFORM=$(uname | tr '[:upper:]' '[:lower:]')
ARCHITECTURE=$(uname -m)

for dep in $DEPENDENCIES; do
    if ! command -v "$dep" &>/dev/null; then
        echo "Error: $dep is not available. Please install $dep to continue."
        exit 1
    fi
done

# Perform a clean install
rm -rf "$HOME/.$APP_NAME"

mkdir -p "$INSTALL_DIR"

if [ $# -eq 1 ]; then
    VERSION="$1"
else
    VERSION="latest"
fi

if [ "$VERSION" == "latest" ]; then
    RELEASE_TAG=$(curl -s "$BASE_URL/releases/latest" | grep -o '"tag_name": "[^"]*"' | awk -F ': ' '{print $2}' | tr -d '"')
else
    RELEASE_TAG="v$VERSION"
fi

case "$PLATFORM" in
    "linux")
        case "$ARCHITECTURE" in
            "x86_64")
                ASSET_NAME="$APP_NAME-$RELEASE_TAG-linux-x64"
                ;;
            "i386" | "i686")
                ASSET_NAME="$APP_NAME-$RELEASE_TAG-linux-x86"
                ;;
            "armv7l")
                ASSET_NAME="$APP_NAME-$RELEASE_TAG-linux-arm"
                ;;
            "aarch64")
                ASSET_NAME="$APP_NAME-$RELEASE_TAG-linux-arm64"
                ;;
            *)
                echo "Unsupported architecture: $ARCHITECTURE"
                exit 1
                ;;
        esac
        ;;
    "darwin")
        case "$ARCHITECTURE" in
            "x86_64")
                ASSET_NAME="$APP_NAME-$RELEASE_TAG-osx-x64"
                ;;
            "arm64")
                ASSET_NAME="$APP_NAME-$RELEASE_TAG-osx-arm64"
                ;;
            *)
                echo "Unsupported architecture: $ARCHITECTURE"
                exit 1
                ;;
        esac
        ;;
    *)
        echo "Unsupported platform: $PLATFORM"
        exit 1
        ;;
esac;

RELEASE_ID=$(curl -s "$BASE_URL/releases/tags/$RELEASE_TAG" | grep -o '"id": [0-9]*' | awk '{print $2}' | head -n 1)
DOWNLOAD_URL=$(curl -s "$BASE_URL/releases/$RELEASE_ID/assets" | grep -o '"browser_download_url": "[^"]*"' | grep "$ASSET_NAME" | awk -F ': ' '{print $2}' | tr -d '"')

if [ -z "$DOWNLOAD_URL" ]; then
    echo "Error: Asset $ASSET_NAME not found in release $RELEASE_TAG."
    exit 1
fi

curl -L "$DOWNLOAD_URL" -o "$INSTALL_DIR/$ASSET_NAME.zip"

if [ ! -f "$INSTALL_DIR/$ASSET_NAME.zip" ]; then
    echo "Error: Failed to download $ASSET_NAME."
    exit 1
fi

unzip -q "$INSTALL_DIR/$ASSET_NAME.zip" -d "$INSTALL_DIR"
rm -f "$INSTALL_DIR/$ASSET_NAME.zip"

if [ ! -f "$INSTALL_DIR/$APP_NAME" ]; then
    echo "Error: Failed to extract $ASSET_NAME."
    exit 1
fi

export PATH="$PATH:$HOME/.cpm/bin"

echo "¡$APP_NAME successfully installed!"
echo "make sure to update your shell configuration path to add the executable to the PATH"
echo "export PATH=\"\$PATH:\$HOME/.cpm/bin\""