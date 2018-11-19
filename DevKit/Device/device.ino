#include "Arduino.h"
#include "AZ3166WiFi.h"
#include "DevKitMQTTClient.h"
#include "SystemVersion.h"
#include "Sensor.h"
#include "parson.h"

RGB_LED rgbLed;

static bool hasWifi = false;
static int rgbLEDR = 0;
static int rgbLEDG = 0;
static int rgbLEDB = 0;

static void InitWifi()
{
  Screen.print(2, "Connecting...");

  if (WiFi.begin() == WL_CONNECTED)
  {
    IPAddress ip = WiFi.localIP();
    Screen.print(1, ip.get_address());
    hasWifi = true;
    Screen.print(2, "Running... \r\n");
  }
  else
  {
    hasWifi = false;
    Screen.print(1, "No Wi-Fi\r\n ");
  }
}

void parseTwinMessage(DEVICE_TWIN_UPDATE_STATE updateState, const char *message)
{
    JSON_Value *root_value;
    root_value = json_parse_string(message);
    if (json_value_get_type(root_value) != JSONObject)
    {
        if (root_value != NULL)
        {
            json_value_free(root_value);
        }
        LogError("parse %s failed", message);
        return;
    }
    JSON_Object *root_object = json_value_get_object(root_value);

    if (updateState == DEVICE_TWIN_UPDATE_COMPLETE)
    {
        JSON_Object *desired_object = json_object_get_object(root_object, "desired");
        if (desired_object != NULL)
        {
          if (json_object_has_value(desired_object, "rgbLEDR"))
          {
            rgbLEDR = json_object_get_number(desired_object, "rgbLEDR");
          }
          if (json_object_has_value(desired_object, "rgbLEDG"))
          {
            rgbLEDG = json_object_get_number(desired_object, "rgbLEDG");
          }
          if (json_object_has_value(desired_object, "rgbLEDB"))
          {
            rgbLEDB = json_object_get_number(desired_object, "rgbLEDB");
          }
        }
    }
    else
    {
      if (json_object_has_value(root_object, "rgbLEDR"))
      {
        rgbLEDR = json_object_get_number(root_object, "rgbLEDR");
      }
      if (json_object_has_value(root_object, "rgbLEDG"))
      {
        rgbLEDG = json_object_get_number(root_object, "rgbLEDG");
      }
      if (json_object_has_value(root_object, "rgbLEDB"))
      {
        rgbLEDB = json_object_get_number(root_object, "rgbLEDB");
      }
    }

    rgbLed.setColor(rgbLEDR, rgbLEDG, rgbLEDB);
    
    json_value_free(root_value);
    
    Screen.print(3, " > Updating...");
}

static void DeviceTwinCallback(DEVICE_TWIN_UPDATE_STATE updateState, const unsigned char *payLoad, int size)
{
  char *temp = (char *)malloc(size + 1);
  if (temp == NULL)
  {
    return;
  }
  memcpy(temp, payLoad, size);
  temp[size] = '\0';
  parseTwinMessage(updateState, temp);
  free(temp);
}

void setup()
{
  rgbLed.turnOff();
  Screen.init();
  Screen.print(0, "IoT DevKit");
  Screen.print(2, "Initializing...");
  Screen.print(3, " > WiFi");
  hasWifi = false;
  InitWifi();
  if (!hasWifi)
  {
    return;
  }

  Screen.print(3, " > IoT Hub");
  DevKitMQTTClient_Init(true);
  DevKitMQTTClient_SetDeviceTwinCallback(DeviceTwinCallback);
}

void loop()
{
  DevKitMQTTClient_Check();
  rgbLed.setColor(rgbLEDR, rgbLEDG, rgbLEDB);
  Screen.print(3, " > Waiting");

  delay(100);
}
