using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using Daliboris.Statistiky.Core.Models.Jevy.XML;
using Daliboris.Statistiky.Core.Services;
using Daliboris.Statistiky.UI.WPF.Controls;
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

        public string _fullPath;


        public string FileName { get; set; }
        private PrehledyStatistik _prehledyStatistikUserControl;
        private WordSettings _wordParserSettings;
        public ICommand OpenFileCommand { get; set; }
        public ICommand OpenSettingsCommand { get; set; }

        public ICommand SaveCommand { get; set; }

        
        public ICommand FilterAutomaticCommand { get; set; }

        public ICommand FilterZnakyCommand { get; set; }

        public ICommand FilterSlovaCommand { get; set; }

        public ICommand FilterUsekyCommand { get; set; }

        public ICommand FilterDigramyCommand { get; set; }

        public ICommand FilterTrigramyCommand { get; set; }

        public ICommand RozlisovatVelikostPismenCommand { get; set; }

        public ICommand VelikostTextuCommand { get; set; }

        public ICommand FilterTextCommand { get; set; }
        
        
        // Path to actual load file
        
        public bool IsZpracovaníOtevreno
        {
            get => _isZpracovaníOtevreno;
            set
            {
                if (value == _isZpracovaníOtevreno) return;
                _isZpracovaníOtevreno = value;
                OnPropertyChanged();
            }
        }

        public bool IsSaving
        {
            get => _isSaving;
            set
            {
                if (value == _isSaving) return;
                _isSaving = value;
                OnPropertyChanged();
            }
        }
        
        
        public MainWindowsViewModel(PrehledyStatistik prehledyStatistikUserControl)
        {
            OpenFileCommand = new ActionCommand(OpenFile);
            OpenSettingsCommand = new ActionCommand(OpenSettings);
            SaveCommand = new ActionCommand(SaveSettings);
            FilterAutomaticCommand = new ActionCommand(new Action<object>(FilterAutomaticly));
            FilterZnakyCommand = new ActionCommand(new Action<object>(FilterZnaky));
            FilterSlovaCommand = new ActionCommand(new Action<object>(FilterSlova));
            FilterUsekyCommand = new ActionCommand(new Action<object>(FilterUseky));
            FilterDigramyCommand = new ActionCommand(new Action<object>(FilterDigramy));
            FilterTrigramyCommand = new ActionCommand(new Action<object>(FilterTrigramy));
            RozlisovatVelikostPismenCommand = new ActionCommand(new Action<object>(RozlisovatVelikostPismen));
            VelikostTextuCommand = new ActionCommand(new Action<object>(ZmenaVelikostiTextu)); 
            FilterTextCommand = new ActionCommand(new Action<object>(FilterText));
            _prehledyStatistikUserControl = prehledyStatistikUserControl;
        }


        private void FilterText(object text)
        { 
            ((PrehledyStatistikViewModel) _prehledyStatistikUserControl?.DataContext)?.TextFilter((string)text);

        }

        private void ZmenaVelikostiTextu(object velikostTextu)
        {

            var velikost = Convert.ToDouble((string) velikostTextu);
            
            ((PrehledyStatistikViewModel) _prehledyStatistikUserControl?.DataContext)?.NastavitVelikostTextu(velikost);

        }

        private void RozlisovatVelikostPismen(object isChecked)
        { 
            ((PrehledyStatistikViewModel) _prehledyStatistikUserControl.DataContext).IsRozlisovatVelikostPismen = ((bool)isChecked);

        }
        
        private void FilterZnaky(object isChecked)
        {
            ((PrehledyStatistikViewModel) _prehledyStatistikUserControl.DataContext).IsZnakyVisible = ((bool)isChecked);
        }

        
        private void FilterSlova(object isChecked)
        {
            ((PrehledyStatistikViewModel) _prehledyStatistikUserControl.DataContext).IsSlovaVisible = ((bool)isChecked);
        }
        
        private void FilterUseky(object isChecked)
        {
            ((PrehledyStatistikViewModel) _prehledyStatistikUserControl.DataContext).IsUsekyVisible = ((bool)isChecked);
        }
        
        private void FilterDigramy(object isChecked)
        {
            ((PrehledyStatistikViewModel) _prehledyStatistikUserControl.DataContext).IsDigramyVisible = ((bool)isChecked);
        }
        
        private void FilterTrigramy(object isChecked)
        {
            ((PrehledyStatistikViewModel) _prehledyStatistikUserControl.DataContext).IsTrigramyVisible = ((bool)isChecked);
        }
        
        
        private void FilterAutomaticly(object isChecked)
        {
            ((PrehledyStatistikViewModel) _prehledyStatistikUserControl.DataContext).SetAutomaticFiltering((bool)isChecked);
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
            if (string.IsNullOrEmpty(filepath))
                return;
            
            var dxr = new WordService(filepath, wordParserSettings);
            var skupinaJevu = dxr.ZpracujDocx();
            _skupinaJevu = skupinaJevu;
            
            var statisticsService = new StatisticsService(); 
            statisticsService.SkupinaJevu = _skupinaJevu;
            // statisticsService.VystupniSoubor = 
            //statisticsService.SloucitDetaily = false;
            
            var tempPath = Path.GetTempPath();
            var filename = Path.GetFileNameWithoutExtension(filepath);
            var extension = ".pjv";
            string fullPath = $"{tempPath}{filename}{extension}";
            
            if(File.Exists(fullPath))
            {
                File.SetAttributes(fullPath, FileAttributes.Normal);
                File.Delete(fullPath); 
            }
            
            StatisticsService.UlozStatistiky(_skupinaJevu,fullPath , FormatUlozeniSeznamu.Text);
            LoadPjvParser(fullPath);
            
        }

        private void LoadPjvParser(string filepath)
        {
            if (String.IsNullOrEmpty(filepath))
                return;

            XmlDocument xd = null;
            if (File.Exists(filepath))
            {
                // xd = StatisticsService.NactiDataStatistiky(sSoubor);
                // Data = StatisticsService.NactiDataStatistiky(filepath);
                
                ((PrehledyStatistikViewModel)_prehledyStatistikUserControl.DataContext).Data = StatisticsService.NactiDataStatistiky(filepath);
                ((PrehledyStatistikViewModel) _prehledyStatistikUserControl.DataContext).FilePath = filepath;

                Debug.WriteLine("Načtení");
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