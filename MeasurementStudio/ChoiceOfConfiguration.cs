using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Common;

namespace MeasurementStudio
{
    public partial class ChoiceOfConfiguration : Form
    {
        public ChoiceOfConfiguration()
        {
            InitializeComponent();
        }

        private void ChoiceOfConfiguration_Load(object sender, EventArgs e)
        {
            panel1.BringToFront();            
            cbModes.Items.Clear();            
            cbModes.Items.AddRange(PluginsManager.ExperimentModes.ToArray());
        }

        private void bNext_Click(object sender, EventArgs e)
        {
            var mode = cbModes.SelectedItem as AbstractExperiment;
            if (mode != null)
            {
                AppSettings.Mode = mode;
                Properties.Settings.Default.PluginCaption = mode.ToString(); 
                LoadProfiles();
                panel2.BringToFront();                          
            }
            else
            {
                ErrorLogProvider.ShowInformationMessage("Вы должны выбрать одну из доступных конфигураций!");                
            }
        }

        private void LoadProfiles()
        {
            var dir = new DirectoryInfo(AppSettings.Mode.GetSettingsPath());
            if (!dir.Exists) dir.Create();
            var files = dir.GetFiles("*.xml");
            cbProfiles.Items.Clear();
            cbProfiles.Items.AddRange(files);
        }

        private FileInfo profile;
        private void bOk_Click(object sender, EventArgs e)
        {
            if (profile != null)
            {                
                AppSettings.Mode.LoadSettings(profile.FullName);
                Properties.Settings.Default.SettingsFileName = profile.FullName;
                Properties.Settings.Default.Save();
                
                this.Close();
            }
        }

        private void bEdit_Click(object sender, EventArgs e)
        {
            if (profile != null) AppSettings.Mode.EditSettingsByUser(profile.FullName);              
            else AppSettings.Mode.SetSettingsByUser();

            LoadProfiles();
        }

        private void cbProfiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            profile = cbProfiles.SelectedItem as FileInfo;
        }

        private void ChoiceOfConfiguration_FormClosing(object sender, FormClosingEventArgs e)
        {
            /*
            if (profile == null)
            {
                e.Cancel = true;
                MessageBox.Show("Сделайте выбор!");
            }
            else
            {
                AppSettings.Mode.LoadSettings(profile.FullName);
            }
             */
        }
    }
}
