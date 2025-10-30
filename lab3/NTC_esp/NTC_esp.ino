const int PIN_VIN = 32;

void setup() {
  Serial.begin(115200);
  while (!Serial)

  analogReadResolution(12);
  analogSetPinAttenuation(PIN_VIN, ADC_11db);
  Serial.println("Reading voltage on GPIO25...");
}

void loop() {
  int raw = analogRead(32);
  float volts = (raw / 4095.0f) * 3.3f;
  Serial.println(volts, 3);      // one number per line, no units
  delay(50);                     // ~20 Hz update
}