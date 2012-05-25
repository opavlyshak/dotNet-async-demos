using System;
using System.Diagnostics;
using System.IO;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace RxWindowsForms
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
            var savedInput = SaveInput();
            savedInput
                .ObserveOn(SynchronizationContext.Current)
                .Finally(() => DisplaySaveCompletedNotification())
                .Subscribe(
                    onNext: _ => { },
                    onError: _ => { Input = "Error!!!"; });
        }

        private IObservable<Unit> SaveInput()
        {
            return Observable
                .Return(Input)
                .ObserveOn(Scheduler.ThreadPool)
                .Select(input =>
                    {
                        SaveInputToFile(input);
                        return Unit.Default;
                    });
        }

        private void SaveInputToFile(string input)
        {
            using (var writer = new StreamWriter("savedInput.txt", append: false))
            {
                var trimmedInput = input.Trim();
                Thread.Sleep(TimeSpan.FromSeconds(2));
                writer.Write(trimmedInput);
                Debug.WriteLine("Input saved.");
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            DisplayLoadingInProgressNotification();
            var restoredInput = RestoreInput()
                .ObserveOn(SynchronizationContext.Current)
                .Finally(() => DisplayLoadCompletedNotification());
            restoredInput
                .Subscribe(
                    onNext: value => { Input = value; },
                    onError: _ => { Input = "Error!!!"; }
                );
        }

        private IObservable<string> RestoreInput()
        {
            var stream = new FileStream(
                "savedInput.txt",
                FileMode.OpenOrCreate,
                FileAccess.Read,
                FileShare.Read,
                bufferSize: 8192,
                useAsync: true);
            var buffer = new byte[stream.Length];

            var readAsynchronously = Observable.FromAsyncPattern<byte[], int, int, int>(
                stream.BeginRead, 
                stream.EndRead);
            var result = readAsynchronously(buffer, 0, buffer.Length)  // build an observable that will return EndRead() result when finishes (single event)
                .Finally(() => stream.Dispose())
                .Delay(TimeSpan.FromSeconds(2)) // simulate long running operation
                .Select(_ => buffer)            // EndRead() returns int, but what we need is buffer
                .Select(bytes => Encoding.ASCII.GetString(bytes));  // convert buffer to string
            return result;
        }

        private IObservable<string> RestoreInputWithCorrectDispose()
        {
            var result = Observable.Using(
                () => new FileStream(
                    "savedInput.txt",
                    FileMode.OpenOrCreate,
                    FileAccess.Read,
                    FileShare.Read,
                    bufferSize: 8192,
                    useAsync: true),
                stream =>
                    {
                        var buffer = new byte[stream.Length];
                        return Observable.FromAsyncPattern<byte[], int, int, int>(
                                stream.BeginRead, 
                                stream.EndRead)
                            (buffer, 0, buffer.Length)
                            .Delay(TimeSpan.FromSeconds(2))
                            .Select(_ => buffer);
                    })
                .Select(bytes => Encoding.ASCII.GetString(bytes));
            
            return result;
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
