#include <Adafruit_NeoPixel.h>
#include <BluetoothSerial.h>
#include <ArduinoJson.h>

Adafruit_NeoPixel pixelStrips[] = {
  Adafruit_NeoPixel(2, 14, NEO_RGBW + NEO_KHZ800),
  Adafruit_NeoPixel(2, 32, NEO_RGBW + NEO_KHZ800),
  Adafruit_NeoPixel(2, 15, NEO_RGBW + NEO_KHZ800),
  Adafruit_NeoPixel(2, 33, NEO_RGBW + NEO_KHZ800),
  Adafruit_NeoPixel(2, 27, NEO_RGBW + NEO_KHZ800)
};
#define NUMSTRIPS (sizeof(pixelStrips)/sizeof(pixelStrips[0]))


String mode = "whiteglow";
DynamicJsonDocument jsonDoc;
JsonObject modeData;

BluetoothSerial SerialBT;

void setup() {

  Serial.begin(115200);
  Serial.println("Start");
  SerialBT.begin("TestWOW");
  for(int i=0; i<NUMSTRIPS; i++){
    pixelStrips[i].begin();
    pixelStrips[i].show(); // Initialize all pixels to 'off'
  }
}



int BT_PERIOD = 1;
long lastBT = 0;
int light_PERIOD = 100;
long lastLight = 0;
void loop() {

  long t = millis();
  readBT();
  
  if( t - lastLight > light_PERIOD ){
    //Serial.println("lIGHT UDATE");
    lastLight = t;
    nextStep();
  }
  
  
  
  delay(1);
}





String buffer = "";


void readBT(){
  
  if(SerialBT.available()){
    char c = SerialBT.read();
    if(c != '\n'){
      buffer = buffer + c;
    }else{
      Serial.println(buffer);
      deserializeJson(jsonDoc, buffer);
      JsonObject obj = jsonDoc.as<JsonObject>();

      mode = obj["modeType"].as<String>();
      if( obj.containsKey("modeData")){
        modeData = obj["modeData"];  
      }
      buffer = "";
    }
  }
}


void nextStep() {

  if( mode.equals("off") ){
    setAllPixelsTo( 0, 0, 0, 0);
  }
  
  if( mode.equals("white") ){
    setAllPixelsTo( 0, 0, 0, 255);
  }

  if( mode.equals("whiteglow") ){
    int w = ((millis()%4000) / 4000.0) * 255;
    setAllPixelsTo( 0, 0, 0, w);
  }

  if( mode.equals("setColor") ){
    setColorMode();
  }

  if( mode.equals("glowUpAndDown") ){
    setGlowUpAndDownMode();
  }



  showAllStrips();
}







//Modes

void setColorMode(){
  setAllPixelsTo( modeData["r"], modeData["g"], modeData["b"], modeData["w"] );
}

void setGlowUpAndDownMode(){

  long t = millis();
  int p = modeData["p"];
  float per = abs(1.0 - 2*(t%p)/((float)p));

  int rd = modeData["rd"];
  int gd = modeData["gd"];
  int bd = modeData["bd"];
  int wd = modeData["wd"];
 
  int ru = modeData["ru"];
  int gu = modeData["gu"];
  int bu = modeData["bu"];
  int wu = modeData["wu"];

  int r = rd + (ru - rd) * per;
  int g = gd + (gu - gd) * per;
  int b = bd + (bu - bd) * per;
  int w = wd + (wu - wd) * per;
  
  
  setAllPixelsTo( r, g, b, w );
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




