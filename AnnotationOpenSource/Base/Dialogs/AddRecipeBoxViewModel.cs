using Prism.Commands;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS.Dialogs
{
    public class AddRecipeBoxViewModel : NotifyPropertyChangedBase, IDialogAware
    {
        public DelegateCommand AckCommand { get; }
        public DelegateCommand NoCommand { get; }
        public AddRecipeBoxViewModel()
        {
            AckCommand = new DelegateCommand(OnAckCommand);
            NoCommand = new DelegateCommand(OnNoCommand);
        }

        private void OnNoCommand()
        {
            var result = ButtonResult.Cancel;
            var p = new DialogParameters();
            p.Add("CreateState", false);

            RequestClose?.Invoke(new DialogResult(result, p));
        }

        private void OnAckCommand()
        {
            var result = ButtonResult.OK;
            var p = new DialogParameters();
            p.Add("CreateState", true);
            p.Add("RecipeName", RecipeName);

            RequestClose?.Invoke(new DialogResult(result, p));
        }
        private string _recipeName;

        public string RecipeName
        {
            get { return _recipeName; }
            set { SetProperty( ref this._recipeName , value); }
        }

        public string Title => "Messages";

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            
        }
    }
}

