using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace APMWindowsForms
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
            try
            {
                DisplaySavingInProgressNotification();
                SaveInput(Input); // will block
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

        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                DisplayLoadingInProgressNotification();
                RestoreInput();
            }
            catch
            {
                Input = "Error!!!";
            }
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

        private void RestoreInput()  // cannot return String from this method
        {
            var stream = new FileStream(
                "savedInput.txt", 
                FileMode.OpenOrCreate, 
                FileAccess.Read, 
                FileShare.Read, 
                bufferSize: 8192,
                useAsync: true);
            var buffer = new byte[stream.Length];
            var context = SynchronizationContext.Current;
            stream.BeginRead(buffer, 0, buffer.Length, asyncResult =>
                {
                    Thread.Sleep(TimeSpan.FromSeconds(2));
                    context.Post(_ =>
                        {
                            try
                            {
                                stream.EndRead(asyncResult);
                                stream.Dispose();
                                Input = Encoding.ASCII.GetString(buffer);
                            }
                            catch
                            {
                                Input = "Error!!!";
                            }
                            finally
                            {
                                DisplayLoadCompletedNotification();
                            }
                        }, null);
                }, 
                null);
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
