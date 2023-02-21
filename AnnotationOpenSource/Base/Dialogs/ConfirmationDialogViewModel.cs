using Prism.Commands;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS.Dialogs
{
    public class ConfirmationDialogViewModel : NotifyPropertyChangedBase, IDialogAware
    {
        public event Action<IDialogResult> RequestClose;
        public DelegateCommand AckCommand { get; }
        public ConfirmationDialogViewModel()
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

        #region Properties
        public string Title => "Messages";
        private string _message = "Confirmation Message!";

        public string Messages
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        #endregion
    }
}
