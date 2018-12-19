#!/usr/bin/env bash

# Update the google-services.json for release builds
if [ "$APPCENTER_XAMARIN_CONFIGURATION" == "Release" ];
then
    echo "Updating google-services.json"
	echo $GOOGLE_SERVICES_JSON | sed 's/\\"/"/g' > $APPCENTER_SOURCE_DIRECTORY/App/Avalanche/Avalanche.Android/google-services.json
else
    echo "Current connfiguration is $APPCENTER_XAMARIN_CONFIGURATION"
fi
echo "Contents of google-services.json"
cat $APPCENTER_SOURCE_DIRECTORY/App/Avalanche/Avalanche.Android/google-services.json