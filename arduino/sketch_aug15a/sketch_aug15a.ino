#include <Adafruit_NeoPixel.h>
#include <BluetoothSerial.h>
#include <ArduinoJson.h>

Adafruit_NeoPixel pixelStrips[] = {
  Adafruit_NeoPixel(1, 14, NEO_RGBW + NEO_KHZ800),
  Adafruit_NeoPixel(1, 32, NEO_RGBW + NEO_KHZ800),
  Adafruit_NeoPixel(1, 15, NEO_RGBW + NEO_KHZ800),
  Adafruit_NeoPixel(1, 33, NEO_RGBW + NEO_KHZ800),
  Adafruit_NeoPixel(1, 27, NEO_RGBW + NEO_KHZ800)
};
#define NUMSTRIPS (sizeof(pixelStrips)/sizeof(pixelStrips[0]))


String mode = "whiteglow";
DynamicJsonBuffer jsonBuffer;
JsonObject& modeData = jsonBuffer.parseObject("{'test':'test'}");
;
BluetoothSerial SerialBT;

void setup() {

  Serial.begin(115200);
  SerialBT.begin("TestWOW");
  for(int i=0; i<NUMSTRIPS; i++){
    pixelStrips[i].begin();
    pixelStrips[i].show(); // Initialize all pixels to 'off'
  }
}



int BT_PERIOD = 5;
long lastBT = 0;
int light_PERIOD = 100;
long lastLight = 0;
void loop() {

  long t = millis();
  if( t - lastBT > BT_PERIOD ){
    lastBT = t;
    readBT();
  }

  if( t - lastLight > light_PERIOD ){
    lastLight = t;
    nextStep();
  }
  
  
  
  delay(5);
}





String buffer = "";


void readBT(){
  
  if(SerialBT.available()){
    char c = SerialBT.read();
    if(c != '\n'){
      buffer = buffer + c;
    }else{
      Serial.println(buffer);
      JsonObject& root = jsonBuffer.parseObject(buffer);
      mode = root["modeType"].as<String>();
      modeData = root["modeData"];
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



  showAllStrips();
}







//Modes

void setColorMode(){
  //setAllPixelsTo( modeData["r"], modeData["g"] modeData["b"], modeData["w"]);
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




