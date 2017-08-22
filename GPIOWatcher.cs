using RaspberryPi;
using System.Threading;

namespace CanReader
{
    public class GPIOWatcher
    {
        private GPIO Gpio;
        private Thread Thread;
        private GPIO.Value OldValue;

        public delegate void OnChange();
        OnChange Delegate;

        public GPIOWatcher(GPIO gpio)
        {
            this.Gpio = gpio;
        }

        public void Start()
        {
            Thread = new Thread(new ThreadStart(watch));
            OldValue = Gpio.value;
            Thread.Start();
        }

        public void SetOnChange(OnChange del)
        {
            this.Delegate = del;
        }

        public void watch()
        {
            GPIO.Value val = Gpio.ReadValue();
            while (val != OldValue)
            {
                val = Gpio.ReadValue();
                if (Delegate != null)
                {
                    Delegate.DynamicInvoke();
                }
                Thread.Sleep(10);
            }
        }

        public void Stop()
        {
            Thread.Abort();
        }

    }
}
