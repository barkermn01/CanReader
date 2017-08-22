using System.IO;

namespace RaspberryPi
{
    /// <summary>
    /// GPIO - this class is designed to utilize the Raspberry Pi GPIO Filesystem
    /// it allow for reading or writing a 0 or 1 to a GPIO pin 
    /// </summary>
    public class GPIO
    {
        /// <summary>
        /// Excpetion for when developer trys to Set/Write a value on an Input GPIO pin
        /// </summary>
        public class CantSetValueOnInputException : System.Exception { }
        /// <summary>
        /// Exception for when a developer try to Get/Read a value on a Outpu GPIO pin
        /// </summary>
        public class CantGetValueOnOutputException : System.Exception { }
        /// <summary>
        /// Exception for when a developer trys to use a GPIO pin that is already in use.
        /// </summary>
        public class AlreadyInUseException : System.Exception { }

        // enum of options for the Direction In for Input and Out for Output
        public enum Direction
        {
            /// <summary>
            /// Input
            /// </summary>
            In,
            /// <summary>
            /// Output
            /// </summary>
            Out
        }
        // enum of options for the value On for a 1 and Off for a 0
        public enum Value
        {
            /// <summary>
            /// binary 0
            /// </summary>
            Off,
            /// <summary>
            /// binary 1
            /// </summary>
            On
        }

        // holds the direction of this GPIO pin
        public Direction direction {
            get;
            private set;
        }
        // holds the value as set by developer for output or read from file system on input
        public Value value
        {
            get;
            private set;
        }

        /// <summary>
        /// holds the path for the GPIO filesystem in RaspberryPi
        /// </summary>
        private string Path = "/sys/class/gpio";
        /// <summary>
        /// Holds the GPIO number in use
        /// </summary>
        private int Id;

        /// <summary>
        /// Generates the GPIO pin Directory name
        /// </summary>
        public string PathName
        {
            get {
                return "gpio" + this.Id.ToString();
            }
        }

        /// <summary>
        /// Generates the full path for the Filesystem mappings for this GPIO pin
        /// </summary>
        public string FullPath
        {
            get
            {
                return this.Path + "/" + this.FullPath;
            }
        }

        /// <summary>
        /// Constructor for creating a new Application binding to a Pin.
        /// </summary>
        /// <param name="gpioId">this GPIO number of the pin</param>
        /// <param name="direction">is the Pin used for Input or Output</param>
        /// <param name="defaultValue">If for Output what is the default value</param>
        public GPIO(int gpioId, Direction direction = Direction.In, Value defaultValue = Value.Off)
        {
            this.Id = gpioId;
            this.direction = direction;
            this.value = defaultValue;
        }

        /// <summary>
        /// Check if this pin is in use by another application
        /// </summary>
        /// <returns></returns>
        private bool CheckIfInUse()
        {
            return Directory.Exists(this.FullPath);
        }

        /// <summary>
        /// Open the GPIO driver for talking to the pin of this instance
        /// </summary>
        public void Open()
        {
            if (this.CheckIfInUse())
            {
                throw new AlreadyInUseException();
            }

            FileStream fs = new FileStream(this.Path + "/export", FileMode.Append);
            using(StreamWriter write = new StreamWriter(fs))
            {
                write.Write(this.Id.ToString());
            }
            fs.Close();
        }

        /// <summary>
        /// Close the GPIO driver for talking to the pin of this instance
        /// </summary>
        public void Close()
        {
            FileStream fs = new FileStream(this.Path + "/unexport", FileMode.Append);
            using (StreamWriter write = new StreamWriter(fs))
            {
                write.Write(this.Id.ToString());
            }
            fs.Close();
        }

        /// <summary>
        /// Write the direction of the communication for this pin instance
        /// </summary>
        private void SetDirection()
        {
            FileStream fs = new FileStream(this.FullPath + "/direction", FileMode.Append);
            using (StreamWriter write = new StreamWriter(fs))
            {
                write.Write((this.direction == Direction.In) ? "In" : "Out");
            }
        }

        /// <summary>
        /// If we're allowed to write the value to send to this pin
        /// </summary>
        private void WriteValue()
        {
            if (this.direction == Direction.In)
            {
                throw new GPIO.CantSetValueOnInputException();
            }
            FileStream fs = new FileStream(this.FullPath + "/value", FileMode.Append);
            using (StreamWriter write = new StreamWriter(fs))
            {
                write.Write((this.value == Value.Off) ? "0" : "1");
            }
        }
        /// <summary>
        /// if we're allowed to read the value from this pin 
        /// </summary>
        /// <returns>the value read from the pin </returns>
        public Value ReadValue()
        {
            if (this.direction == Direction.Out)
            {
                throw new GPIO.CantGetValueOnOutputException();
            }
            string text = System.IO.File.ReadAllText(this.FullPath + "/value");
            if(text.Trim() == "1")
            {
                this.value = Value.On;
            }
            else
            {
                this.value = Value.Off;
            }
            return this.value;
        }
        /// <summary>
        /// If this i write pin set the default value
        /// </summary>
        private void SetDefaultValue()
        {
            if (this.direction == Direction.Out)
            {
                this.WriteValue();
            }
        }
        /// <summary>
        /// Set the value of the pin to the given value
        /// </summary>
        /// <param name="val">the value to set this pin to</param>
        public void SetValue(Value val)
        {
            this.value = val;
            this.WriteValue();
        }
    }
}
