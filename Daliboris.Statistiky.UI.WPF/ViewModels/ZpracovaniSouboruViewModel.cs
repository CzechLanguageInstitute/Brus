using System.Windows;
using System.Windows.Input;
using Daliboris.Statistiky.Word;
using Microsoft.Xaml.Behaviors.Core;

namespace Daliboris.Statistiky.UI.WPF.ViewModels
{
    public class ZpracovaniSouboruViewModel : BaseViewModel
    {

        public Window ActualWindow { get; set; }
        
        public void LoadSettings(WordSettings wordSettings)
        {
            IsPonechatInterpunkci = wordSettings.OdstranitPocatecniAKoncoveMezery;
            IsOdstranitTeckuUSlov = wordSettings.OdstranitTeckuUSlov;
        }


        private bool _isOdstranitTeckuUSlov = true;

        public bool IsOdstranitTeckuUSlov
        {
            get { return _isOdstranitTeckuUSlov; }
            set
            {
                if (value != _isOdstranitTeckuUSlov)
                {
                    _isOdstranitTeckuUSlov = value;
                    OnPropertyChanged();
                }
            }
        }
        
        private bool _isOdstranitPocatecniAKoncoveMezery = true;

        public bool IsOdstranitPocatecniAKoncoveMezery
        {
            get { return _isOdstranitPocatecniAKoncoveMezery; }
            set
            {
                if (value != _isOdstranitPocatecniAKoncoveMezery)
                {
                    _isOdstranitPocatecniAKoncoveMezery = value;
                    OnPropertyChanged();
                }
            }
        }


        private bool _isPonechatInterpunkci = true;

        public bool IsPonechatInterpunkci
        {
            get { return _isPonechatInterpunkci; }
            set
            {
                if (value != _isPonechatInterpunkci)
                {
                    _isPonechatInterpunkci = value;
                    OnPropertyChanged();
                }
            }
        }


        private bool _isZahrnoutTextOdstavce = true;

        public bool IsZahrnoutTextOdstavce
        {
            get { return _isZahrnoutTextOdstavce; }
            set
            {
                if (value != _isZahrnoutTextOdstavce)
                {
                    _isZahrnoutTextOdstavce = value;
                    OnPropertyChanged();
                }
            }
        }
        
        private bool _isZahrnoutTextZnakovychStylu = true;

        public bool IsZahrnoutTextZnakovychStylu
        {
            get { return _isZahrnoutTextZnakovychStylu; }
            set
            {
                if (value != _isZahrnoutTextZnakovychStylu)
                {
                    _isZahrnoutTextZnakovychStylu = value;
                    OnPropertyChanged();
                }
            }
        }
        
        public ActionCommand SaveSettingsCommand { get; set; }


        public ZpracovaniSouboruViewModel()
        {
            SaveSettingsCommand = new ActionCommand(SaveSettings);
        }


        private void SaveSettings()
        {
            var wordParserSettings = new WordSettings();
            
            wordParserSettings.OdstranitTeckuUSlov = _isOdstranitTeckuUSlov;
            wordParserSettings.OdstranitPocatecniAKoncoveMezery = _isOdstranitPocatecniAKoncoveMezery;
            wordParserSettings.ZahrnoutTextOdstavce = _isZahrnoutTextOdstavce;
            wordParserSettings.ZahrnoutTextZnakovychStylu = _isZahrnoutTextZnakovychStylu;
            var mainWindow = (MainWindow) Application.Current.MainWindow;
            ((MainWindowsViewModel)mainWindow.DataContext).UpdateWordSettings(wordParserSettings);
            ActualWindow?.Close();

        }
    }
}