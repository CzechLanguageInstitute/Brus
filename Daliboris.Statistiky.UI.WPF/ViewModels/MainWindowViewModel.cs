using System;
using System.IO;
using System.Windows.Input;
using System.Xml;
using Daliboris.Statistiky.Core.Models.Jevy.XML;
using Daliboris.Statistiky.Core.Services;
using Daliboris.Statistiky.Word;
using Microsoft.Win32;
using Microsoft.Xaml.Behaviors.Core;

namespace Daliboris.Statistiky.UI.WPF.ViewModels
{
    public class MainWindowsViewModel : BaseViewModel
    {
        private bool _isSaving = false;
        private bool _isZpracovaníOtevreno = false;
        private SkupinaJevu _skupinaJevu;
        private WordSettings _wordParserSettings;
        private XmlDocument _data; 
        
        public XmlDocument Data
        {
            get
            {
                return _data;
            }
            set
            {
                if (value != _data)
                {
                    _data = value;
                    OnPropertyChanged();
                }
            }
        }
        
        
        
        
        public MainWindowsViewModel()
        {
            OpenFileCommand = new ActionCommand(OpenFile);
            OpenSettingsCommand = new ActionCommand(OpenSettings);
            SaveCommand = new ActionCommand(SaveSettings);
        }

        public ICommand OpenFileCommand { get; set; }
        public ICommand OpenSettingsCommand { get; set; }

        public ICommand SaveCommand { get; set; }

        // Path to actual load file
        public string FileName { get; set; }

        public bool IsZpracovaníOtevreno
        {
            get { return _isZpracovaníOtevreno; }
            set
            {
                if (value != _isZpracovaníOtevreno)
                {
                    _isZpracovaníOtevreno = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsSaving
        {
            get { return _isSaving; }
            set
            {
                if (value != _isSaving)
                {
                    _isSaving = value;
                    OnPropertyChanged();
                }
            }
        }

        private void OpenSettings()
        {
            var zpracovaniSouboru = new ZpracovaniSouboru(_wordParserSettings);
            bool? dlgOK = zpracovaniSouboru.ShowDialog();
        }

        private void SaveSettings()
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExt = ".pjv";
            saveFileDialog.Filter = "Přehled jevů (*.pjv)|*.pjv";
            if (saveFileDialog.ShowDialog() == true)
            {
                //var statisticsService = new StatisticsService();
                //statisticsService.SkupinaJevu = _skupinaJevu;
                //statisticsService.VystupniSoubor = saveFileDialog.FileName;
                //statisticsService.SloucitDetaily = false;
                StatisticsService.OdstranitTecku = _wordParserSettings.OdstranitTeckuUSlov;
                StatisticsService.UlozStatistiky(_skupinaJevu, saveFileDialog.FileName, FormatUlozeniSeznamu.Text);
            }
        }

        public void UpdateWordSettings(WordSettings wordSettings)
        {
            _wordParserSettings = wordSettings;
            LoadWordParser(FileName, wordSettings);
        }

        private void LoadWordParser(string filepath, WordSettings wordParserSettings)
        {
            if (String.IsNullOrEmpty(filepath))
                return;


            var dxr = new WordService(filepath, wordParserSettings);
            var skupinaJevu = dxr.ZpracujDocx();
            _skupinaJevu = skupinaJevu;
            
            var statisticsService = new StatisticsService(); 
            statisticsService.SkupinaJevu = _skupinaJevu;
            // statisticsService.VystupniSoubor = 
            //statisticsService.SloucitDetaily = false;
            
            var tempPath = Path.GetTempPath();
            var filename = Guid.NewGuid().ToString();
            var extension = ".pjv";
            string fullPath = Path.Combine(tempPath, filename, extension);
            
            StatisticsService.UlozStatistiky(_skupinaJevu,fullPath , FormatUlozeniSeznamu.Text);
            LoadPjvParser(fullPath);
            
            File.SetAttributes(fullPath, FileAttributes.Normal);
            File.Delete(fullPath);

            //                 //dxr.UlozPrehledy();
            //                 
        }

        private void LoadPjvParser(string filepath)
        {
            if (String.IsNullOrEmpty(filepath))
                return;

            XmlDocument xd = null;
            if (File.Exists(filepath))
            {
                // xd = StatisticsService.NactiDataStatistiky(sSoubor);
                Data = StatisticsService.NactiDataStatistiky(filepath);
            }
        }

        private void OpenFile()
        {
            var fileDialog = new OpenFileDialog();
            fileDialog.CheckFileExists = true;
            fileDialog.Filter =
                "Dokument na zpracování (*.docx)|*.docx|Přehled jevů (*.pjv)|*.pjv"; // Filter files by extension
            fileDialog.DefaultExt = ".docx"; // Default file extension

            // Show open file dialog box
            bool? result = fileDialog.ShowDialog();
            var index = fileDialog.FilterIndex;

            // Process open file dialog box results
            if (result == true)
            {
                FileName = fileDialog.FileName;

                if (index == 1)
                {
                    IsSaving = true;
                    IsZpracovaníOtevreno = true;
                    _wordParserSettings = new WordSettings();
                    LoadWordParser(FileName, _wordParserSettings);
                }
                else if (index == 2)
                {
                    IsSaving = false;
                    IsZpracovaníOtevreno = false;
                    LoadPjvParser(FileName);
                }
                else
                {
                    throw new Exception();
                }
            }
        }
    }
}