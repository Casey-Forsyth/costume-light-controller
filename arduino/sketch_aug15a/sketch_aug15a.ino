#include <Adafruit_NeoPixel.h>
#include <ArduinoJson.h>



#if defined(ARDUINO_FEATHER_ESP32)

  #include <BluetoothSerial.h>

  Adafruit_NeoPixel pixelStrips[] = {
  Adafruit_NeoPixel(2, 14, NEO_RGBW + NEO_KHZ800),
  Adafruit_NeoPixel(2, 32, NEO_RGBW + NEO_KHZ800),
  Adafruit_NeoPixel(2, 15, NEO_RGBW + NEO_KHZ800),
  Adafruit_NeoPixel(2, 33, NEO_RGBW + NEO_KHZ800),
  Adafruit_NeoPixel(2, 27, NEO_RGBW + NEO_KHZ800)
};
#elif defined(ARDUINO_AVR_FEATHER32U4 )
  Adafruit_NeoPixel pixelStrips[] = {
  Adafruit_NeoPixel(2, 6, NEO_RGBW + NEO_KHZ800),
  Adafruit_NeoPixel(2, 9, NEO_RGBW + NEO_KHZ800),
  Adafruit_NeoPixel(2, 10, NEO_RGBW + NEO_KHZ800),
  Adafruit_NeoPixel(2, 11, NEO_RGBW + NEO_KHZ800)
  };


    #define FACTORYRESET_ENABLE      1


  // COMMON SETTINGS
  // ----------------------------------------------------------------------------------------------
  // These settings are used in both SW UART, HW UART and SPI mode
  // ----------------------------------------------------------------------------------------------
  #define BUFSIZE                        160   // Size of the read buffer for incoming data
  #define VERBOSE_MODE                   true  // If set to 'true' enables debug output
  
  
  // SOFTWARE UART SETTINGS
  // ----------------------------------------------------------------------------------------------
  // The following macros declare the pins that will be used for 'SW' serial.
  // You should use this option if you are connecting the UART Friend to an UNO
  // ----------------------------------------------------------------------------------------------
  #define BLUEFRUIT_SWUART_RXD_PIN       9    // Required for software serial!
  #define BLUEFRUIT_SWUART_TXD_PIN       10   // Required for software serial!
  #define BLUEFRUIT_UART_CTS_PIN         11   // Required for software serial!
  #define BLUEFRUIT_UART_RTS_PIN         -1   // Optional, set to -1 if unused
  
  
  // HARDWARE UART SETTINGS
  // ----------------------------------------------------------------------------------------------
  // The following macros declare the HW serial port you are using. Uncomment
  // this line if you are connecting the BLE to Leonardo/Micro or Flora
  // ----------------------------------------------------------------------------------------------
  #ifdef Serial1    // this makes it not complain on compilation if there's no Serial1
    #define BLUEFRUIT_HWSERIAL_NAME      Serial1
  #endif
  
  
  // SHARED UART SETTINGS
  // ----------------------------------------------------------------------------------------------
  // The following sets the optional Mode pin, its recommended but not required
  // ----------------------------------------------------------------------------------------------
  #define BLUEFRUIT_UART_MODE_PIN        12    // Set to -1 if unused
  
  
  // SHARED SPI SETTINGS
  // ----------------------------------------------------------------------------------------------
  // The following macros declare the pins to use for HW and SW SPI communication.
  // SCK, MISO and MOSI should be connected to the HW SPI pins on the Uno when
  // using HW SPI.  This should be used with nRF51822 based Bluefruit LE modules
  // that use SPI (Bluefruit LE SPI Friend).
  // ----------------------------------------------------------------------------------------------
  #define BLUEFRUIT_SPI_CS               8
  #define BLUEFRUIT_SPI_IRQ              7
  #define BLUEFRUIT_SPI_RST              4    // Optional but recommended, set to -1 if unused
  
  // SOFTWARE SPI SETTINGS
  // ----------------------------------------------------------------------------------------------
  // The following macros declare the pins to use for SW SPI communication.
  // This should be used with nRF51822 based Bluefruit LE modules that use SPI
  // (Bluefruit LE SPI Friend).
  // ----------------------------------------------------------------------------------------------
  #define BLUEFRUIT_SPI_SCK              13
  #define BLUEFRUIT_SPI_MISO             12
  #define BLUEFRUIT_SPI_MOSI             11


  #include <Arduino.h>
  #include <SPI.h>
  #include "Adafruit_BLE.h"
  #include "Adafruit_BluefruitLE_SPI.h"
  #include "Adafruit_BluefruitLE_UART.h"


  
  /* ...or hardware serial, which does not need the RTS/CTS pins. Uncomment this line */
  // Adafruit_BluefruitLE_UART ble(BLUEFRUIT_HWSERIAL_NAME, BLUEFRUIT_UART_MODE_PIN);
  
  /* ...hardware SPI, using SCK/MOSI/MISO hardware SPI pins and then user selected CS/IRQ/RST */
  Adafruit_BluefruitLE_SPI ble(BLUEFRUIT_SPI_CS, BLUEFRUIT_SPI_IRQ, BLUEFRUIT_SPI_RST);
  
  /* ...software SPI, using SCK/MOSI/MISO user-defined SPI pins and then user selected CS/IRQ/RST */
  //Adafruit_BluefruitLE_SPI ble(BLUEFRUIT_SPI_SCK, BLUEFRUIT_SPI_MISO,
  //                             BLUEFRUIT_SPI_MOSI, BLUEFRUIT_SPI_CS,
  //                             BLUEFRUIT_SPI_IRQ, BLUEFRUIT_SPI_RST);
  
  
  // A small helper
  void error(const __FlashStringHelper*err) {
    Serial.println(err);
    while (1);
  }




#else
  #error Unsupported board selection.
#endif



#define NUMSTRIPS (sizeof(pixelStrips)/sizeof(pixelStrips[0]))



// mode 1 char
// period 3 char s/10
// custom for mode
String modeData = "G040000000000000250000255000";

// off "0"
// white "w"
// white glow "g"
// set Color "s250000000000"
// glowUpAndDown "G250000000000000000000000255"

//BluetoothSerial SerialBT;

void setup() {

  Serial.begin(115200);
  Serial.println("Start");
  //SerialBT.begin("KomomoPack");
  for(int i=0; i<NUMSTRIPS; i++){
    pixelStrips[i].begin();
    pixelStrips[i].show(); // Initialize all pixels to 'off'
  }


  //Blue Fruit

  Serial.print(F("Initialising the Bluefruit LE module: "));

  if ( !ble.begin(VERBOSE_MODE) )
  {
    error(F("Couldn't find Bluefruit, make sure it's in CoMmanD mode & check wiring?"));
  }
  Serial.println( F("OK!") );

  if ( FACTORYRESET_ENABLE )
  {
    /* Perform a factory reset to make sure everything is in a known state */
    Serial.println(F("Performing a factory reset: "));
    if ( ! ble.factoryReset() ){
      error(F("Couldn't factory reset"));
    }
  }

  /* Disable command echo from Bluefruit */
  ble.echo(false);

  Serial.println("Requesting Bluefruit info:");
  /* Print Bluefruit information */
  ble.info();

  if (! ble.sendCommandCheckOK(F("AT+GAPDEVNAME=Komomo Sword")) ) {
    error(F("Could not set device name?"));
  }
  ble.setMode(BLUEFRUIT_MODE_DATA);







}



int BT_PERIOD = 1;
long lastBT = 0;
int light_PERIOD = 50;
long lastLight = 0;
void loop() {

  long t = millis();
  readBT();



  if( t - lastLight > light_PERIOD ){
    lastLight = t;
    nextStep();
  }
  
  delay(1);
}





String buffer = "";


void readBT(){
  
//  if(SerialBT.available()){
//    char c = SerialBT.read();
//    if(c != '\n'){
//      buffer = buffer + c;
//    }else{
//      Serial.println(buffer);
//      modeData = buffer;
//      buffer = "";
//    }
//  }


  if( ble.available() ){
    int c = ble.read();
    if(c != '\n'){
      buffer = buffer + (char)c;
    }else{
      Serial.println(buffer);
      modeData = buffer;
      buffer = "";
    }
  }





}


void nextStep() {

  long t = millis();
  char mode = modeData[0];
  
  if(t<5000){
    flashing(0,255,0,0,500,0.50);
  } else {
    if( mode == '0' ){
      setAllPixelsTo( 0, 0, 0, 0);
    }
    
    if( mode == 'w' ){
      setAllPixelsTo( 0, 0, 0, 255);
    }
  
    if( mode == 'g' ){
      int w = ((millis()%4000) / 4000.0) * 255;
      setAllPixelsTo( 0, 0, 0, w);
    }
  
    if( mode == 's' ){
      setColorMode();
    }
  
    if( mode == 'G' ){
      setGlowUpAndDownMode();
    }

  }
  showAllStrips();
}




//Data Reading

int getIntFromModeData(int s, int e){
  if(modeData.length()>s &&
  modeData.length()>e){
    return modeData.substring(s,e+1).toInt();
  } else{
    return 0;
  }
}




//Modes

void setColorMode(){

  int r = getIntFromModeData(1,3);
  int g = getIntFromModeData(4,6);
  int b = getIntFromModeData(7,9);
  int w = getIntFromModeData(10,12);
  
  setAllPixelsTo( r,g,b,w );
}

void setGlowUpAndDownMode(){

  long t = millis();
  int p = getIntFromModeData(1,3)*100;

  float per = (1.0 - 2*(t%p)/((float)p));
  if(per <0){
    per = 0-per;
  }

  int rd = getIntFromModeData(4,6);;
  int gd = getIntFromModeData(7,9);;
  int bd = getIntFromModeData(10,12);;
  int wd = getIntFromModeData(13,15);;
 
  int ru = getIntFromModeData(16,18);;
  int gu = getIntFromModeData(19,21);;
  int bu = getIntFromModeData(22,24);;
  int wu = getIntFromModeData(25,27);;

  int r = rd + (ru - rd) * per;
  int g = gd + (gu - gd) * per;
  int b = bd + (bu - bd) * per;
  int w = wd + (wu - wd) * per;
  
  
  setAllPixelsTo( r, g, b, w );
}


void flashing(int r, int g, int b, int w, int p, float duty){

  long t = millis();
  float per = abs(1.0 - 2*(t%p)/((float)p));

  if(per < duty){
    setAllPixelsTo( r, g, b, w );
  }else{
    setAllPixelsTo( 0, 0, 0, 0 );
  }
  
  
  
}























//Light Utils

void showAllStrips(){
  for(int i=0; i<NUMSTRIPS; i++){
    pixelStrips[i].show(); // Initialize all pixels to 'off'
  }

}



void setAllPixelsTo( int r, int g, int b, int w){

  for(int i=0; i<NUMSTRIPS; i++){
    setAllPixelsOnStringTo(i,r,g,b,w);
  }

}

void setAllPixelsOnStringTo(int s, int r, int g, int b, int w){

    for(int i=0; i<pixelStrips[s].numPixels(); i++){
      setpixelToRGBW(s,i,r,g,b,w);
    }

}

void setpixelToRGBW(int s, int p, int r, int g, int b, int w){
    pixelStrips[s].setPixelColor(p, g , r, b, w);
    
}
