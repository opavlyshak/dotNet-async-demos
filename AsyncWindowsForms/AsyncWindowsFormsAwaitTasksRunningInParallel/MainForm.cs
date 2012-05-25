using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AsyncWindowsFormsAwaitTasksRunningInParallel
{
    public partial class MainForm : Form
    {
        private readonly IDictionary<TextBox, string> inputTextBoxToFileNameMap;

        private IEnumerable<TextBox> InputTextBoxes
        {
            get { return inputTextBoxToFileNameMap.Keys; }
        } 

        public MainForm()
        {
            InitializeComponent();
            inputTextBoxToFileNameMap = new Dictionary<TextBox, string>
                {
                    { txtInput1, "savedInput1.txt" },
                    { txtInput2, "savedInput2.txt" },
                    { txtInput3, "savedInput3.txt" },
                    { txtInput4, "savedInput4.txt" }
                };
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                DisplaySavingInProgressNotification();
                IEnumerable<Task> tasks = 
                    from textBox in InputTextBoxes
                    select SaveInputAsync(textBox.Text, GetFileName(textBox));
                await Task.WhenAll(tasks);
            }
            catch
            {
                MessageBox.Show("Error!");
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
                string[] restoredInputs = await Task.WhenAll(
                    from textBox in InputTextBoxes
                    select RestoreInput(GetFileName(textBox))
                    );
                for (int i = 0; i < restoredInputs.Length; i++)
                {
                    InputTextBoxes.ElementAt(i).Text = restoredInputs[i];
                }
            }
            catch
            {
                MessageBox.Show("Error!");
            }
            finally
            {
                DisplayLoadCompletedNotification();
            }
        }

        private async Task SaveInputAsync(string input, string fileName)
        {
            using (var writer = new StreamWriter(fileName, append: false))
            {
                await Task.Run(() => Thread.Sleep(TimeSpan.FromSeconds(0.5)));
                await writer.WriteAsync(input);
            }
        }

        private async Task<string> RestoreInput(string fileName)
        {
            if (File.Exists(fileName))
                return await Task.Run(() =>
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(2));
                        return File.ReadAllText(fileName);   // if file doesn't exist exception is thrown
                    });
            else
            {
                return String.Empty;
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

        private string GetFileName(TextBox textBox)
        {
            return inputTextBoxToFileNameMap[textBox];
        }
    }
}
