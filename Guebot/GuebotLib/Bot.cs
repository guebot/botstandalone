using GuebotEntities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GuebotLib
{
    public class Bot
    {
        private SerialPort port;

        private bool waitingResponse;

        public GuebotComponentEntity Hand { get; set; }

        public GuebotComponentEntity Arm { get; set; }

        public string LastResponse { get; set; }

        protected static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        protected bool SendCommand(string cmd, out string response)
        {
            bool result = false;
            waitingResponse = true;
            LastResponse = string.Empty;

            if (!cmd.Length.Equals(10))
            {
                response = "Invalid Command";
                return false;
            }

            if (!port.IsOpen)
            {
                response = "Port Disconnected";
                return false;
            }

            // write command
            //byte[] write = new byte[] { 0x55, 0x03, 0x0, 0x0, 0x0 };
            byte[] write = StringToByteArray(cmd.Trim());
            port.Write(write, 0, write.Length);

            // Espera respuesta
            Stopwatch timeout = new Stopwatch();
            timeout.Start();
            do
            {
                if (timeout.ElapsedMilliseconds > 1000)
                {
                    LastResponse = "Timeout Command";
                    waitingResponse = false;
                }
                response = LastResponse;

            } while (waitingResponse);
            timeout.Stop();

            // Verifica respuesta
            if (!string.IsNullOrEmpty(LastResponse) && LastResponse.Length >= 4)
            {
                result = LastResponse.Substring(0, 4).Equals("5501");
            }

            return result;
        }

        protected void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //espera respuesta del sistema
            if (waitingResponse)
            {
                if (port.BytesToRead >= 5)
                {
                    byte[] data = new byte[port.BytesToRead];
                    port.Read(data, 0, data.Length);
                    LastResponse = BitConverter.ToString(data).Replace("-", string.Empty);
                    waitingResponse = false;
                }
            }
            else
            {
                LastResponse = port.ReadExisting();
            }
        }

        public async Task<JSONStatusEntity> StatusAsync()
        {
            JSONStatusEntity obj = new JSONStatusEntity();

            return obj;
        }

        public void Movement(JSONMovementEntity move)
        {

        }

        protected bool UpdateComponentPos(GuebotComponentEntity comp, out string response)
        {
            string respCommand = string.Empty;
            bool result = SendCommand(string.Format("5502{0}0000", comp.Id.ToString("X2")), out respCommand);
            if (result)
            {
                if (respCommand.Length >= 10)
                {
                    comp.ActualValue = int.Parse(respCommand.Substring(6, 4), System.Globalization.NumberStyles.HexNumber);
                }
                else
                {
                    respCommand = "Error actual value";
                    result = false;
                }
            }
            response = respCommand;
            return result;
        }

        protected bool MovePositive(GuebotComponentEntity comp, out string response)
        {
            string respCommand = string.Empty;
            bool result = false;

            // posicion actual
            if (!UpdateComponentPos(comp, out response))
            {
                return result;
            }

            int actual = comp.ActualValue;
            for (int i = actual; i <= comp.MaxValue; i += comp.StepMovement)
            {
                result = SendCommand(string.Format("5501{0}{1}", comp.Id.ToString("X2"), i.ToString("X4")), out respCommand);

                if (result)
                {
                    comp.ActualValue = i;
                }
                else
                {
                    break;
                }
                Thread.Sleep(10);
            }

            response = respCommand;
            return result;
        }

        protected bool MoveNegative(GuebotComponentEntity comp, out string response)
        {
            string respCommand = string.Empty;
            bool result = false;

            // posicion actual
            if (!UpdateComponentPos(comp, out response))
            {
                return result;
            }

            int actual = comp.ActualValue;
            for (int i = actual; i >= comp.MinValue; i -= comp.StepMovement)
            {
                result = SendCommand(string.Format("5501{0}{1}", comp.Id.ToString("X2"), i.ToString("X4")), out respCommand);

                if (result)
                {
                    comp.ActualValue = i;
                }
                else
                {
                    break;
                }
                Thread.Sleep(10);
            }

            response = respCommand;
            return result;
        }

        public Bot(string serialPortName)
        {
            port = new SerialPort(serialPortName);
            port.ReadTimeout = 100;
            port.WriteTimeout = 100;
            port.BaudRate = 9600;
            port.DataReceived += port_DataReceived;
            waitingResponse = false;

            // valores por defecto para el brazo
            Arm = new GuebotComponentEntity() { ActualValue = 0, Id = 1, MaxValue = 1000, MinValue = 0, StepMovement = 50 };

            //valores por defecto para la mano
            Hand = new GuebotComponentEntity() { ActualValue = 0, Id = 2, MaxValue = 1000, MinValue = 0, StepMovement = 50 };
        }

        public Bot(string serialPortName, GuebotComponentEntity arm, GuebotComponentEntity hand)
        {
            port = new SerialPort(serialPortName);
            port.ReadTimeout = 100;
            port.WriteTimeout = 100;
            port.BaudRate = 9600;
            port.DataReceived += port_DataReceived;
            waitingResponse = false;

            // valores para los componentes
            Arm = arm;
            Hand = hand;
        }

        public void OpenPort()
        {
            port.Open();
        }

        public void ClosePort()
        {
            if (port.IsOpen)
            {
                port.Close();
                port.Dispose();
            }
        }

        public bool IsPortOpen()
        {
            return port.IsOpen;
        }

        public bool ResetSystem(out string response)
        {
            string respCommand = string.Empty;
            bool result = SendCommand(string.Format("5503000000"), out respCommand);
            if (result)
            {
                Arm.ActualValue = 0;
                Hand.ActualValue = 0;
            }
            response = respCommand;
            return result;
        }

        public bool TextCommand(string command, out string response)
        {
            bool result = SendCommand(string.Format(command.Trim()), out response);
            return result;
        }

        public bool CalibrateArm(int maxValue, out string response)
        {
            bool result = SendCommand(string.Format("5504{0}{1}", Arm.Id.ToString("X2"), maxValue.ToString("X4")), out response);
            if (result)
            {
                Arm.MaxValue = maxValue;
                Arm.ActualValue = 0;
            }
            return result;
        }

        public bool CalibrateHand(int maxValue, out string response)
        {
            bool result = SendCommand(string.Format("5504{0}{1}", Hand.Id.ToString("X2"), maxValue.ToString("X4")), out response);
            if (result)
            {
                Hand.MaxValue = maxValue;
                Hand.ActualValue = 0;
            }
            return result;
        }

        public bool MoveDownArm(out string response)
        {
            return MovePositive(Arm, out response);
        }

        public bool MoveUpArm(out string response)
        {
            return MoveNegative(Arm, out response);
        }

        public bool MoveCloseHand(out string response)
        {
            return MovePositive(Hand, out response);
        }

        public bool MoveOpenHand(out string response)
        {
            return MoveNegative(Hand, out response);
        }
    }
}
