# IoT LED color changer

This is a demo app showing how to update the LED on an MXChip board using Azure Device Twins. This code accompanies a blog post:

https://jimbobbennett.io/controlling-an-iot-device-from-your-phone

#### What you will need:

* An MXChip board - you can find places to buy one [here](http://mxchip.com/az3166O)
* An Azure account - you can sign up for free [here](https://aka.ms/FreeAz) and get loads of free stuff including US$200 of credit for the first month, lots of free services for a year, and some stuff which is free forever!

#### To run this code

This repo contains 3 parts:

* DevKit - This is the code for the IoT device. One of the files has been sanitized to not include my connection details. You will need to add your own connection strings to the `.azurecomponent/azureconfig.json` file.
* Backend - This is the code for the Azure functions backend. You will need to update the `local.settings.json` file with your connection string and device name.
* MobileApp - This is the code for a [Fabulous] cross-platform mobile app. You will need to add your Azure functions app URL and function key to the `ApiKeys.fs1 file.
