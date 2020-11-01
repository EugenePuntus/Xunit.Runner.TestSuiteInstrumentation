#!/usr/bin/env bash

APK_LOCATION=$1

echo "Install application to emulator"

echo "Install $APK_LOCATION"

$ANDROID_HOME/platform-tools/adb -s emulator-5554 install -r $APK_LOCATION

echo "Application installed"