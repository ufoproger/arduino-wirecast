class Led
{
  private: 
    int _pin;

  public:
    Led (int pin)
    {
      _pin = pin;
      
      pinMode(_pin, OUTPUT);
      digitalWrite(_pin, LOW);
    }
  
    void on()
    {
      digitalWrite(_pin, HIGH);
    }
  
    void off()
    {
      digitalWrite(_pin, LOW);
    }

    bool isOn()
    {
      return (digitalRead(_pin) == HIGH);
    }

    bool isOff()
    {
      return !isOn();
    }

    void inverse()
    {
      if (isOn())
        off();
      else
        on();
    }
};

Led led1(13), led2(12);
unsigned long long lastSerialActive = 0;
const int serialUnactiveDelay = 5 * 1000; // 5 секунд

void setup()
{
  Serial.begin(9600);
}

void loop()
{
  if (Serial.available() > 0)
  {
    lastSerialActive = millis();
    
    int t = Serial.read();

    switch (t)
    {
      case (int)'1':
        led1.on();
        led2.off();
        break;

      case (int)'2':
        led1.off();
        led2.on();
        break;

      case (int)'3':
        led1.on();
        led2.on();
        break;

      default:
        led1.off();
        led2.off();
    }
  }
  else
    if (millis() - lastSerialActive > serialUnactiveDelay)
    {
      led1.on();
      led2.off();
      delay(100);

      led1.off();
      led2.on();
      delay(100);
      
      led1.off();
      led2.off();    
    }
}
