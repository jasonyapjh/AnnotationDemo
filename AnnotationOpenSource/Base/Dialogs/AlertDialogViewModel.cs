using Prism.Commands;
using Prism.Services.Dialogs;
using System;

namespace IS.Dialogs
{
    public class AlertDialogViewModel : NotifyPropertyChangedBase, IDialogAware
    {
        public event Action<IDialogResult> RequestClose;
        public DelegateCommand AckCommand { get; }

        public AlertDialogViewModel()
        {
            AckCommand = new DelegateCommand(OnAckCommand);
        }

        private void OnAckCommand()
        {
            var result = ButtonResult.OK;
            RequestClose?.Invoke(new DialogResult(result));
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {

        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            Messages = parameters.GetValue<string>("message");
        }

        #region 
        public string Title => "Alert!";
        private string _message = "Confirmation Message!";

        public string Messages
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        #endregion
    }
}
