using Prism.Commands;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS.Dialogs
{
    public class ErrorMessageBoxViewModel : NotifyPropertyChangedBase, IDialogAware
    {
        public event Action<IDialogResult> RequestClose;
        public DelegateCommand AckCommand { get; }

        public ErrorMessageBoxViewModel()
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
            ErrMessages = parameters.GetValue<string>("message");
            ErrTitle = parameters.GetValue<string>("title");
        }

        #region 
        public string Title => "Alert!";
        private string _errTitle= "Confirmation Message!";

        public string ErrTitle
        {
            get { return _errTitle; }
            set { SetProperty(ref _errTitle, value); }
        }
        private string _errMessage = "Confirmation Message!";

        public string ErrMessages
        {
            get { return _errMessage; }
            set { SetProperty(ref _errMessage, value); }
        }
        #endregion
    }
}
