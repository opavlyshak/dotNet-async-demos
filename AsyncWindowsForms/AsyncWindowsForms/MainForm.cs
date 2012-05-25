using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AsyncWindowsForms
{
    public partial class MainForm : Form
    {
        private string Input
        {
            get { return txtInput.Text; }
            set { txtInput.Text = value; }
        }

        public MainForm()
        {
            InitializeComponent();
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                DisplaySavingInProgressNotification();
                await SaveInputAsync(Input);
            }
            catch
            {
                Input = "Error!!!";
            }
            finally
            {
                DisplaySaveCompletedNotification();
            }
        }

        private async void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                DisplayLoadingInProgressNotification();
                Input = await RestoreInput();
            }
            catch
            {
                Input = "Error!!!";
            }
            finally
            {
                DisplayLoadCompletedNotification();
            }
        }

        private async Task SaveInputAsync(string input)
        {
            using (var writer = new StreamWriter("savedInput.txt", append: false))
            {
                var trimmedInput = input.Trim();
                await Task.Run(() => Thread.Sleep(TimeSpan.FromSeconds(2)));
                await writer.WriteAsync(trimmedInput);
                Debug.WriteLine("Input saved.");
            }
        }

        private async Task<string> RestoreInput()
        {
            using (var reader = File.OpenText("savedInput.txt"))
            {
                await Task.Delay(TimeSpan.FromSeconds(2));
                var result = await reader.ReadToEndAsync();
                Debug.WriteLine("Input restored from file.");
                return result;
            }
        }

        private void DisplaySavingInProgressNotification()
        {
            btnSave.Enabled = false;
            btnSave.Text = "Saving...";
            progressBar.Visible = true;
        }

        private void DisplaySaveCompletedNotification()
        {
            btnSave.Enabled = true;
            btnSave.Text = "Save";
            progressBar.Visible = false;
        }

        private void DisplayLoadingInProgressNotification()
        {
            btnLoad.Enabled = false;
            progressBar.Visible = true;
        }

        private void DisplayLoadCompletedNotification()
        {
            btnLoad.Enabled = true;
            progressBar.Visible = false;
        }
    }
}
