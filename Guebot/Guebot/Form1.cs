using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GuebotLib;
using System.Configuration;
using System.Threading.Tasks;

namespace Guebot
{
    public partial class Form1 : Form
    {
        private string portNameActual;
        private Bot robot;
        private string WebSocketUri = string.Empty;
        private string EggProfile = string.Empty;

        public Form1()
        {
            InitializeComponent();

            if (SerialPort.GetPortNames().Count() > 0)   
                cmbPorts.Items.AddRange(SerialPort.GetPortNames());

            Log.WriteToLog(txtLog, "Encontrados {0} puertos", SerialPort.GetPortNames().Length);

            if (ProfilesGuebot.GetProfiles().Count() > 0)
                cmbProfile.Items.AddRange(ProfilesGuebot.GetProfiles().Select(x => x.Name).ToArray());

            Log.WriteToLog(txtLog, "Encontrados {0} perfiles", ProfilesGuebot.GetProfiles().Count);

            #if !DEBUG
            this.WebSocketUri = ConfigurationManager.AppSettings["WebSocketUri"].ToString();
            #endif

            // Carga perfiles
            //LoadProfiles();
            // Muestra puertos disponibles
            //ShowPorts();
        }

        private void LoadProfiles()
        {
            try
            {
                foreach (ProfileEntity p in ProfilesGuebot.GetProfiles())
                {
                    cmbProfile.Items.Add(p.Name);
                }
                if (cmbProfile.Items.Count > 0)
                {
                    cmbProfile.SelectedIndex = 0;
                }
                Log.WriteToLog(txtLog, "Encontrados {0} perfiles", ProfilesGuebot.GetProfiles().Count);
            }
            catch (Exception ex)
            {
                Log.WriteToLog(txtLog, "Guebot Error: {0}", ex.Message);
            }
        }

        private void ShowPorts()
        {
            try
            {
                for (int i = 0; i < SerialPort.GetPortNames().Length; i++)
                {
                    cmbPorts.Items.Add(SerialPort.GetPortNames()[i]);
                }
                if (cmbPorts.Items.Count > 0)
                {
                    cmbPorts.SelectedIndex = 0;
                }
                Log.WriteToLog("Encontrados {0} puertos", SerialPort.GetPortNames().Length);
            }
            catch (Exception ex)
            {
                txtLog.AppendText(string.Format("Serial Port Error: {0}\n", ex.Message));
            }
        }

        private void IniciarRobot()
        {
            try
            {
                // abre
#if DEBUG
                portNameActual = cmbPorts.Text;
                EggProfile = cmbProfile.Text;
#else
                portNameActual = ConfigurationManager.AppSettings["DefaultPort"].ToString();
                EggProfile = ConfigurationManager.AppSettings["DefaultEgg"].ToString();
#endif

                robot = new Bot(portNameActual, ProfilesGuebot.GetProfiles().FirstOrDefault(f => f.Name.Equals(EggProfile)).Arm, ProfilesGuebot.GetProfiles().FirstOrDefault(f => f.Name.Equals(EggProfile)).Hand, this.WebSocketUri);
                robot.OpenPort();

                if (robot.IsPortOpen())
                {
                    Log.WriteToLog("{0}: Puerto abierto exitosamente.", portNameActual);

                    //// reinicia brazo                    
                    string rCmd = string.Empty;
                    if (robot.ResetSystem(out rCmd))
                        Log.WriteToLog("{0}: Reinicio del brazo exitoso.", portNameActual);
                    else
                        Log.WriteToLog("{0}: Error al reiniciar el brazo. [{1}]", portNameActual, rCmd);

                    //cambia estado de los botones
                    btnStart.Enabled = false;
                    btnStop.Enabled = true;
                    btnCloseHand.Enabled = true;
                    btnMoveDown.Enabled = true;
                    btnMoveUp.Enabled = true;
                    btnOpenHand.Enabled = true;
                    cmbPorts.Enabled = false;
                    txtCommand.Enabled = true;
                    btnCommand.Enabled = true;
                    cmbProfile.Enabled = false;

                }
            }
            catch (Exception ex)
            {
                Log.WriteToLog("Error al abrir el puerto. {0}", ex.Message);
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
#if DEBUG
            IniciarRobot();
#endif
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            try
            {

                // cierra
                //port.Close();
                robot.ClosePort();

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
                txtCommand.Enabled = false;
                btnCommand.Enabled = false;
                cmbProfile.Enabled = true;
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
                //log
                txtLog.AppendText(string.Format("{0}: Enviando comando para subir el brazo\n", portNameActual));
                string respBot = string.Empty;
                if (robot.MoveUpArm(out respBot))
                {
                    //log
                    txtLog.AppendText(string.Format("{0}: Brazo arriba\n", portNameActual));
                }
                else
                {
                    //log
                    txtLog.AppendText(string.Format("{0}: Error al subir el brazo [{1}]\n", portNameActual, respBot));
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

                //log
                txtLog.AppendText(string.Format("{0}: Enviando comando para bajar el brazo\n", portNameActual));
                string respBot = string.Empty;
                if (robot.MoveDownArm(out respBot))
                {
                    //log
                    txtLog.AppendText(string.Format("{0}: Brazo abajo\n", portNameActual));
                }
                else
                {
                    //log
                    txtLog.AppendText(string.Format("{0}: Error al bajar el brazo [{1}]\n", portNameActual, respBot));
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
                //log
                txtLog.AppendText(string.Format("{0}: Enviando comando para abrir la mano\n", portNameActual));
                string respBot = string.Empty;
                if (robot.MoveOpenHand(out respBot))
                {
                    //log
                    txtLog.AppendText(string.Format("{0}: Mano abierta\n", portNameActual));
                }
                else
                {
                    //log
                    txtLog.AppendText(string.Format("{0}: Error al abrir la mano [{1}]\n", portNameActual, respBot));
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
                //log
                txtLog.AppendText(string.Format("{0}: Enviando comando para cerrar la mano\n", portNameActual));
                string respBot = string.Empty;
                if (robot.MoveCloseHand(out respBot))
                {
                    //log
                    txtLog.AppendText(string.Format("{0}: Mano cerrada\n", portNameActual));
                }
                else
                {
                    //log
                    txtLog.AppendText(string.Format("{0}: Error al cerrar la mano [{1}]\n", portNameActual, respBot));
                }
            }
            catch (Exception ex)
            {
                txtLog.AppendText(string.Format("Serial Port Error: {0}\n", ex.Message));
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (robot != null)
            {
                robot.ClosePort();
            }
        }

        private void btnCommand_Click(object sender, EventArgs e)
        {
            try
            {
                string rCmd = string.Empty;
                bool resCmd = robot.TextCommand(txtCommand.Text, out rCmd);
                txtLog.AppendText(string.Format("{0}: Enviando comando: {1}. Resultado: {2}. Respuesta: {3}.\n", portNameActual, txtCommand.Text, resCmd ? "OK" : "ERROR", rCmd));

            }
            catch (Exception ex)
            {
                txtLog.AppendText(string.Format("Serial Port Error: {0}\n", ex.Message));
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
#if !DEBUG
            IniciarRobot();
#endif 
        }

    }
}
