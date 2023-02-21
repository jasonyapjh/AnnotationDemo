using Prism.Commands;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS.Dialogs
{
    public class MessageBoxViewModel : NotifyPropertyChangedBase, IDialogAware
    {
        public event Action<IDialogResult> RequestClose;
        public DelegateCommand CloseDialogCommand { get; }
        public DelegateCommand OkDialogCommand { get; }
        public MessageBoxViewModel()
        {
            CloseDialogCommand = new DelegateCommand(CloseDialog);
            OkDialogCommand = new DelegateCommand(OkDialog);
        }

        private void OkDialog()
        {
            var result = ButtonResult.OK;
            var p = new DialogParameters();
            p.Add("my param", "Closing this dialog");

            RequestClose?.Invoke(new DialogResult(result, p));
        }

        private void CloseDialog()
        {
            var result = ButtonResult.Cancel;
            var p = new DialogParameters();
            p.Add("my param", "Closing this dialog");

            RequestClose?.Invoke(new DialogResult(result, p));
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

            CloseNoButtonString = "Close";

            if (parameters.ContainsKey("responseMessage"))
                ResponseMessage = parameters.GetValue<string>("responseMessage");

            if (parameters.ContainsKey("recoveryMessage"))
                RecoveryMessage = parameters.GetValue<string>("recoveryMessage");
            // RecoveryMessage = "Please Follow Below Recovery Action: \n" + parameters.GetValue<string>("recoveryMessage");

        }


        #region Properties
        public string Title => "Messages";
        private string _message;
        private string _responseMessage;
        private string _recoveryMessage;
        private string _closeNoButtonString;
        public string Messages
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }
        public string ResponseMessage
        {
            get { return _responseMessage; }
            set { SetProperty(ref _responseMessage, value); }
        }
        public string RecoveryMessage
        {
            get { return _recoveryMessage; }
            set { SetProperty(ref _recoveryMessage, value); }
        }
        public string CloseNoButtonString
        {
            get { return _closeNoButtonString; }
            set { SetProperty(ref _closeNoButtonString, value); }
        }

        #endregion
    }
}
