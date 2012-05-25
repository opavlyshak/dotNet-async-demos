using System;
using System.IO;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Windows.Forms;

namespace RxUIEvents
{
    public partial class Form1 : Form
    {
        private readonly SynchronizationContextScheduler UIThreadScheduler;

        public Form1()
        {
            InitializeComponent();
            ConsoleUtils.AllocConsole();
            UIThreadScheduler = new SynchronizationContextScheduler(SynchronizationContext.Current);

            RestoreInput();

            StartObservingTextChanged();
        }

        private void StartObservingTextChanged_1()
        {
            var inputChangedEvents = Observable
                .FromEventPattern<EventArgs>(txtInput, "TextChanged");
            inputChangedEvents
                .Subscribe(e => Console.WriteLine("changed"));
        }

        private void StartObservingTextChanged_2()
        {
            var inputChangedEvents = Observable
                .FromEventPattern<EventArgs>(txtInput, "TextChanged")
                .Select(e => ((TextBox)e.Sender).Text);
            inputChangedEvents
                .Subscribe(s => Console.WriteLine(s));
        }

        private void StartObservingTextChanged_3()
        {
            var inputChangedEvents = Observable
                .FromEventPattern<EventArgs>(txtInput, "TextChanged")
                .Select(e => ((TextBox)e.Sender).Text)
                .Select(input => input.Trim());
            inputChangedEvents
                .Subscribe(s => Console.WriteLine("'{0}'", s));
        }

        private void StartObservingTextChanged_4()
        {
            var inputChangedEvents = Observable
                .FromEventPattern<EventArgs>(txtInput, "TextChanged")
                .Select(e => ((TextBox)e.Sender).Text)
                .Select(input => input.Trim())
                .Throttle(TimeSpan.FromMilliseconds(500));
            inputChangedEvents
                .Subscribe(s => Console.WriteLine(s));
        }

        private void StartObservingTextChanged_5()
        {
            var inputChangedEvents = Observable
                .FromEventPattern<EventArgs>(txtInput, "TextChanged")
                .Select(e => ((TextBox)e.Sender).Text)
                .Select(input => input.Trim())
                .Throttle(TimeSpan.FromMilliseconds(500))
                .DistinctUntilChanged();
            inputChangedEvents
                .Subscribe(s => Console.WriteLine(s));
        }

        private void StartObservingTextChanged_6()
        {
            var inputChangedEvents = Observable
                .FromEventPattern<EventArgs>(txtInput, "TextChanged")
                .Select(e => ((TextBox)e.Sender).Text)
                .Select(input => input.Trim())
                .Throttle(TimeSpan.FromMilliseconds(500))
                .DistinctUntilChanged();
            inputChangedEvents
                .Subscribe(s => Console.WriteLine(s));

            inputChangedEvents
                .Throttle(TimeSpan.FromSeconds(2))
                .Select(input => SaveInput(input))
                .Subscribe(_ => DisplaySavedNotification());
        }

        private void StartObservingTextChanged_7()
        {
            var inputChangedEvents = Observable
                .FromEventPattern<EventArgs>(txtInput, "TextChanged")
                .Select(e => ((TextBox)e.Sender).Text)
                .Select(input => input.Trim())
                .Throttle(TimeSpan.FromMilliseconds(500))
                .DistinctUntilChanged();
            inputChangedEvents
                .Subscribe(s => Console.WriteLine(s));

            inputChangedEvents
                .Throttle(TimeSpan.FromSeconds(2))
                .ObserveOn(Scheduler.TaskPool)
                .Select(input => SaveInput(input))
                .ObserveOn(UIThreadScheduler)
                .Subscribe(_ => DisplaySavedNotification());
//            inputChangedEvents
//                .ObserveOn(UIThreadScheduler)
//                .Subscribe(_ => DisplayInputNotSavedNotification());
        }

        private void StartObservingTextChanged_8()
        {
            var inputChangedEvents = Observable
                .FromEventPattern<EventArgs>(txtInput, "TextChanged")
                .Select(e => ((TextBox)e.Sender).Text)
                .Select(input => input.Trim())
                .Throttle(TimeSpan.FromMilliseconds(500))
                .DistinctUntilChanged();
            inputChangedEvents
                .Subscribe(s => Console.WriteLine(s));

            var saveClickEvents = Observable
                .FromEventPattern<EventArgs>(btnSave, "Click");
            saveClickEvents
                .Subscribe(_ => Console.WriteLine(">> Save clicked."));

            inputChangedEvents
                .Throttle(TimeSpan.FromSeconds(2))
                .ObserveOn(Scheduler.TaskPool)
                .Select(input => SaveInput(input))
                .ObserveOn(UIThreadScheduler)
                .Subscribe(_ => DisplaySavedNotification());
            inputChangedEvents
                .ObserveOn(UIThreadScheduler)
                .Subscribe(_ => DisplayInputNotSavedNotification());
        }

        private void StartObservingTextChanged_9()
        {
            var inputChangedEvents = Observable
                .FromEventPattern<EventArgs>(txtInput, "TextChanged")
                .Select(e => ((TextBox)e.Sender).Text)
                .Select(input => input.Trim())
                .Throttle(TimeSpan.FromMilliseconds(500))
                .DistinctUntilChanged();
            inputChangedEvents
                .Subscribe(s => Console.WriteLine(s));

            var saveClickEvents = Observable
                .FromEventPattern<EventArgs>(btnSave, "Click")
                .Select(_ => txtInput.Text);

            Observable.Merge(saveClickEvents, inputChangedEvents)
                .Throttle(TimeSpan.FromSeconds(2))
                .ObserveOn(Scheduler.TaskPool)
                .Select(input => SaveInput(input))
                .ObserveOn(UIThreadScheduler)
                .Subscribe(_ => DisplaySavedNotification());
            inputChangedEvents
                .ObserveOn(UIThreadScheduler)
                .Subscribe(_ => DisplayInputNotSavedNotification());
        }

        private void StartObservingTextChanged()
        {
            var inputChangedEvents = Observable.FromEventPattern<EventArgs>(txtInput, "TextChanged")
                .Select(e => ((TextBox)e.Sender).Text)
                .Select(input => input.Trim())
                .Throttle(TimeSpan.FromMilliseconds(500))
                .DistinctUntilChanged();
            inputChangedEvents.Subscribe(input => Console.WriteLine(input));

            var saveClickEvents = Observable.FromEventPattern<EventArgs>(btnSave, "Click")
                .Do(_ => DisplaySavingInProgressNotification())
                .Select(_ => txtInput.Text);
            Observable.Merge(saveClickEvents, inputChangedEvents)
                .Throttle(TimeSpan.FromSeconds(2))
                .ObserveOn(Scheduler.TaskPool)
                .Select(input => SaveInput(input))
                .ObserveOn(UIThreadScheduler)
                .Subscribe(_ => DisplaySavedNotification());
            inputChangedEvents
                .ObserveOn(UIThreadScheduler)
                .Subscribe(_ => DisplayInputNotSavedNotification());
        }

        private Unit SaveInput(string input)
        {
            Console.WriteLine(">>> saving...");
            Thread.Sleep(TimeSpan.FromSeconds(3));
            File.WriteAllText("savedInput.txt", input);
            Console.WriteLine(">>> saved >>> {0}", input);
            return Unit.Default;
        }

        private void RestoreInput()
        {
            if (File.Exists("savedInput.txt"))
            {
                txtInput.Text = File.ReadAllText("savedInput.txt");
            }
        }

        private void DisplaySavingInProgressNotification()
        {
            btnSave.Enabled = false;
            btnSave.Text = "Saving...";
        }

        private void DisplaySavedNotification()
        {
            btnSave.Text = "Saved";
        }

        private void DisplayInputNotSavedNotification()
        {
            btnSave.Enabled = true;
            btnSave.Text = "Save";
        }
    }
}
