using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IS.Dialogs
{
    public static class IDialogServiceExtensions
    {
        public static void ShowMessageDialog(this IDialogService dialogService, string message, string responseMessage, Action<IDialogResult> callBack)
        {
            var p = new DialogParameters();

            p.Add("message", message);
            p.Add("responseMessage", responseMessage);

            dialogService.ShowDialog("MessageBox", p, callBack);
        }
        public static void ShowAlertDialog(this IDialogService dialogService, string message, Action<IDialogResult> callBack)
        {
            var p = new DialogParameters();

            p.Add("message", message);
            
            dialogService.ShowDialog("AlertDialog", p, callBack);
        }
        public static void ShowErrorMessageDialog(this IDialogService dialogService, string title, string message, Action<IDialogResult> callBack)
        {
            var p = new DialogParameters();

            p.Add("message", message);
            p.Add("title", title);

            dialogService.ShowDialog("ErrorMessageBox", p, callBack);
        }
        public static void ShowAddRecipeDialog(this IDialogService dialogService, Action<IDialogResult> callBack)
        {
            var p = new DialogParameters();

            dialogService.ShowDialog("AddRecipeBox", p, callBack);
        }
        public static void ShowConfirmationDialog(this IDialogService dialogService,string message, Action<IDialogResult> callBack)
        {
            var p = new DialogParameters();
            p.Add("message", message);
            dialogService.ShowDialog("ConfirmationDialog", p, callBack);
        }
        /*public static void ShowMessageDialog(this IDialogService dialogService, InfoIcon m_infoIcon, string message, string responseMessage, string recoveryMessage, Action<IDialogResult> callBack)
       {
           var p = new DialogParameters();

           p.Add("InfoIcon", m_infoIcon);
           p.Add("message", message);
           p.Add("responseMessage", responseMessage);
           p.Add("recoveryMessage", recoveryMessage);

           dialogService.ShowDialog("MessageBox", p, callBack);
       }
       public static void ShowAddShapeDialog(this IDialogService dialogService, Action<IDialogResult> callBack)
       {
           var p = new DialogParameters();

           dialogService.ShowDialog("AddShapeBox", p, callBack);
       }
       */
        //public static readonly string[] supportedFileExt = new string[] { ".jpg", ".hobj", ".ima", ".tif", ".tiff", ".gif", ".bmp", ".jpeg", ".jp2", ".jxr", ".png", ".pcx", ".ras", ".xwd", ".pbm", ".pnm", ".pgm", ".ppm" };
        public static void ShowAboutBoxDialog(this IDialogService dialogService, Action<IDialogResult> callBack)
        {
            var p = new DialogParameters();

            dialogService.ShowDialog("AboutAppBox", p, callBack);
        }
        public static string OpenImageFile(this IDialogService dialogService, string directory)
        {
            string filename = "";
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Bitmap (*.bmp)|*.bmp|TIFF (*.tiff)|*.tiff|Jpeg (*.jpeg)|*jpeg|Jpg (*.jpg)|*jpg|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = Directory.Exists(directory) ? Path.GetFullPath(directory) : string.Empty;
            if (openFileDialog.ShowDialog() == true)
            {
                filename = openFileDialog.FileName;
            }

            return filename;
        }
        public static string OpenImageFolder(this IDialogService dialogService)
        {
            string filename = "";
            using (var dialog = new FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                filename = dialog.SelectedPath;
            }

            return filename;
        }
    }
}
