using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Guebot
{
    public partial class Form1 : Form
    {
        private SerialPort port;
        private string portNameActual;

        public Form1()
        {
            InitializeComponent();

            // Muestra puertos disponibles
            ShowPorts();
        }

        private void ShowPorts()
        {
            try
            {
                cmbPorts.DataSource = SerialPort.GetPortNames();
                txtLog.AppendText(string.Format("Encontrados {0} puertos\n", SerialPort.GetPortNames().Length));
            }
            catch (Exception ex)
            {
                txtLog.AppendText(string.Format("Serial Port Error: {0}\n", ex.Message));
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                // abre
                portNameActual = cmbPorts.Text;
                port = new SerialPort(portNameActual);
                port.Open();
                port.DataReceived += port_DataReceived;

                if (port.IsOpen)
                {
                    //log
                    txtLog.AppendText(string.Format("{0}: Puerto abierto exitosamente.\n", portNameActual));

                    //cambia estado de los botones
                    btnStart.Enabled = false;
                    btnStop.Enabled = true;
                    btnCloseHand.Enabled = true;
                    btnMoveDown.Enabled = true;
                    btnMoveUp.Enabled = true;
                    btnOpenHand.Enabled = true;
                    cmbPorts.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                txtLog.AppendText(string.Format("Error al abrir el puerto. {0}\n", ex.Message));
            }
        }

        void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                //log
                txtLog.AppendText(string.Format("{0}: Recibiendo datos [{1}]\n", portNameActual, port.ReadExisting()));
            }
            catch (Exception ex)
            {
                txtLog.AppendText(string.Format("Serial Port Error: {0}\n", ex.Message));
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            try
            {
                if (port.IsOpen)
                {
                    // cierra
                    port.Close();

                    //log
                    txtLog.AppendText(string.Format("{0}: Puerto cerrado exitosamente.\n", portNameActual));
                    portNameActual = string.Empty;

                    //cambia estado de los botones
                    btnStart.Enabled = true;
                    btnStop.Enabled = false;
                    btnCloseHand.Enabled = false;
                    btnMoveDown.Enabled = false;
                    btnMoveUp.Enabled = false;
                    btnOpenHand.Enabled = false;
                    cmbPorts.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                txtLog.AppendText(string.Format("Serial Port Error: {0}\n", ex.Message));
            }
        }

        private void btnMoveUp_Click(object sender, EventArgs e)
        {
            try
            {
                if (port.IsOpen)
                {
                    // envia data
                    port.Write("UP");

                    //log
                    txtLog.AppendText(string.Format("{0}: Enviando comando para subir brazo\n", portNameActual));
                }
            }
            catch (Exception ex)
            {
                txtLog.AppendText(string.Format("Serial Port Error: {0}\n", ex.Message));
            }
        }

        private void btnMoveDown_Click(object sender, EventArgs e)
        {
            try
            {
                if (port.IsOpen)
                {
                    // envia data
                    port.Write("DOWN");

                    //log
                    txtLog.AppendText(string.Format("{0}: Enviando comando para bajar brazo\n", portNameActual));
                }
            }
            catch (Exception ex)
            {
                txtLog.AppendText(string.Format("Serial Port Error: {0}\n", ex.Message));
            }
        }

        private void btnOpenHand_Click(object sender, EventArgs e)
        {
            try
            {
                if (port.IsOpen)
                {
                    // envia data
                    port.Write("OPENH");

                    //log
                    txtLog.AppendText(string.Format("{0}: Enviando comando para abrir mano\n", portNameActual));
                }
            }
            catch (Exception ex)
            {
                txtLog.AppendText(string.Format("Serial Port Error: {0}\n", ex.Message));
            }
        }

        private void btnCloseHand_Click(object sender, EventArgs e)
        {
            try
            {
                if (port.IsOpen)
                {
                    // envia data
                    port.Write("CLOSEH");

                    //log
                    txtLog.AppendText(string.Format("{0}: Enviando comando para cerrar mano\n", portNameActual));
                }
            }
            catch (Exception ex)
            {
                txtLog.AppendText(string.Format("Serial Port Error: {0}\n", ex.Message));
            }
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (port != null && port.IsOpen)
            {
                port.Close();
            }
        }
    }
}
