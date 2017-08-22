using System;
using RaspberryPi;

namespace CanReader
{
    class Program
    {
        private static GPIO GpioOut, GpioIn;
        private static GPIOWatcher watcher;
        private static bool exiting = false;
        private static string state;

        static void GPIOChange()
        {
            state = (GpioIn.value == GPIO.Value.Off) ? "Off" : "On";
        }

        static void Main(string[] args)
        {
            GpioOut = new GPIO(26, GPIO.Direction.Out, GPIO.Value.On);
            GpioOut.Open();

            GpioIn = new GPIO(20, GPIO.Direction.In);
            GpioIn.Open();

            Console.Write("\nLaser Level Detector");
            state = (GpioIn.value == GPIO.Value.Off) ? "Off" : "On";

            watcher = new GPIOWatcher(GpioIn);
            watcher.SetOnChange(GPIOChange);
            watcher.Start();

            while (!exiting)
            {
                Console.Write("\rCurrent State " + state);
                string input = Console.ReadLine();
                if (input == "exit")
                {
                    watcher.Stop();
                    exiting = true;
                }
            }

            GpioIn.Close();
            GpioOut.Close();
        }
    }
}
