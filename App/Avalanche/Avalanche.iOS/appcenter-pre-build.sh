#!/usr/bin/env bash

# Update the GoogleService-Info.plist for all builds
echo "Updating GoogleService-Info.plist"
echo $GOOGLE_SERVICE_INFO_PLIST | sed 's/\\"/"/g' > $APPCENTER_SOURCE_DIRECTORY/App/Avalanche/Avalanche.iOS/GoogleService-Info.plist

echo "Contents of GoogleService-Info.plist"
cat $APPCENTER_SOURCE_DIRECTORY/App/Avalanche/Avalanche.iOS/GoogleService-Info.plist