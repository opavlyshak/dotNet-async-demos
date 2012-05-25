using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TPLWindowsForms
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            DisplaySavingInProgressNotification();
            Task.Factory.StartNew(() => SaveInput(Input))
                .ContinueWith(task =>
                    {
                        if (task.IsFaulted)
                        {
                            Input = "Error!!!";
                        }
                        DisplaySaveCompletedNotification();
                    },
                    TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            DisplayLoadingInProgressNotification();
            RestoreInput()
                .ContinueWith(restoreInputTask =>
                    {
                        try
                        {
                            Input = restoreInputTask.Result;
                        }
                        catch
                        {
                            Input = "Error!!!";
                        }
                        finally
                        {
                            DisplayLoadCompletedNotification();
                        }
                    }, 
                    TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void SaveInput(string input)
        {
            using (var writer = new StreamWriter("savedInput.txt", append: false))
            {
                var trimmedInput = input.Trim();
                Thread.Sleep(TimeSpan.FromSeconds(2));
                writer.Write(trimmedInput);
                Debug.WriteLine("Input saved.");
            }
        }

        private Task<string> RestoreInput()
        {
            return Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(TimeSpan.FromSeconds(2));
                    return File.ReadAllText("savedInput.txt");   // if file doesn't exist exception is thrown
                });
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
