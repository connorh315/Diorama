using Diorama.Editor;
using Diorama.Editor.ShaderSystem;
using Diorama.UI.Progress;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Diorama.UI.ViewModels
{
    public class GenerateFingerprintCacheViewModel : EditableItem
    {
        private GenerateFingerprintsWindow window;

        private int progress;
        public int Progress
        {
            get => progress;
            set => Set(ref progress, value);
        }

        private string status = "";
        public string Status
        {
            get => status;
            set => Set(ref status, value);
        }

        private bool cancelled = false;
        public bool Cancelled
        {
            get => cancelled;
            set => Set(ref cancelled, value);
        }

        private readonly CancellationTokenSource cts = new();

        public ICommand CancelCommand { get; }

        public GenerateFingerprintCacheViewModel(GenerateFingerprintsWindow window)
        {
            this.window = window;
            CancelCommand = new RelayCommand<object?>((_) => Cancel());
        }

        public void Cancel()
        {
            if (Cancelled) return;

            Cancelled = true;
            cts.Cancel(); 
            window.Close();
        }

        public async Task RunAsync()
        {
            var progress = new Progress<FingerprintCacheProgress>(p =>
            {
                Progress = p.Current;
                Status = p.Status;
            });

#if DEBUG
            try
            {
                await EditorShaderSystem.CreateAsync(progress, cts.Token);
            }
            catch (OperationCanceledException _)
            {

            }
#else
            try
            {
                await EditorShaderSystem.CreateAsync(progress, cts.Token);
            }
            catch (Exception e)
            {
                Status = $"Failed: {e.Message}";

            }
#endif
        }
    }
}
