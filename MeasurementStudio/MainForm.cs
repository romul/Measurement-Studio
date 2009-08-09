using System;
using System.Windows.Forms;
using Common;
using Microsoft.VisualBasic.Devices;
using System.IO;

namespace MeasurementStudio
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();       
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Text = "Measurement Studio v." + Application.ProductVersion;
            PluginsManager.LoadRulesFromDlls(Application.StartupPath + @"\Plugins");
            if (AppSettings.Mode != null && AppSettings.Mode.IsSettingsLoaded)
            {
                PrepareToStart();
            } 
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (new AboutBox()).ShowDialog();
        }

        private void choiceOfConfigurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (new ChoiceOfConfiguration()).ShowDialog();
            if (AppSettings.Mode != null && AppSettings.Mode.IsSettingsLoaded)
            {
                PrepareToStart();
            } 
            else
            {
                ErrorLogProvider.ShowInformationMessage("Вы не выбрали режим измерений, либо файл настроек не полон");
            }
        }

        private void PrepareToStart()
        {
            tbMode.Clear();
            tbMode.AppendText("Текущий режим:");
            tbMode.AppendText("\n");
            tbMode.AppendText(AppSettings.Mode.ToString());
            tbMode.AppendText("\n");
            tbMode.AppendText(Path.GetFileNameWithoutExtension(Properties.Settings.Default.SettingsFileName));
            tbIntro.Clear();
            tbIntro.AppendText(AppSettings.Mode.SettingsToString());
            AppSettings.Mode.InitChart(new ZedGraphProxy(zedGraphControl1));
            AppSettings.Mode.Connect();
            EnableStartActions();
        }

        private void tsbStart_Click(object sender, EventArgs e)
        {
            try
            {
                AppSettings.Mode.StartMeasurement();
            }
            catch (MeasurementException ex)
            {
                ErrorLogProvider.WriteToEventLogAndShow(ex.Message);
            }
            EnableStopActions();            
        }

        private void tsbStop_Click(object sender, EventArgs e)
        {
            AppSettings.Mode.StopMeasurement();
            EnableStartActions();           
            if (AppSettings.Mode.ShouldBeSaved) tsbSave_Click(sender, e);
        }

        private void tsbSave_Click(object sender, EventArgs e)
        {
            var sfd = new SaveFileDialog();
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                AppSettings.Mode.SaveAllDataToFile(sfd.FileName);
            }
        }

        private void EnableStartActions()
        {
            EnableActions(false);
        }

        private void EnableStopActions()
        {
            EnableActions(true);
        }

        private void EnableActions(bool isStarted)
        {
            tsbStart.Enabled = !isStarted;
            tsbStop.Enabled = isStarted;
            tsbSave.Enabled = AppSettings.Mode.ShouldBeSaved;
            tsbTimeMachine.Enabled = !AppSettings.Mode.Data.IsNullOrEmpty();
        }

        private void zedGraphControl1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Scroll)
            {
                var keyBoard = new Keyboard();
                var measurementEvent = keyBoard.ScrollLock ? MeasurementEvents.Start : MeasurementEvents.Stop;
                AppSettings.Mode.AddMeasurementPoint(measurementEvent);
            }
        }

        private void tsbTimeMachine_Click(object sender, EventArgs e)
        {
            (new TimeMachineForm()).ShowDialog();
        }
       
    }
}
