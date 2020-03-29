using System.Xml;
using Daliboris.Statistiky.UI.WPF.Controls;

namespace Daliboris.Statistiky.UI.WPF.ViewModels
{
    public class PrehledyStatistikViewModel : BaseViewModel
    {

          
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
        
        
        private bool _isZnakyVisible = true; 
        
        
        public bool IsZnakyVisible
        {
            get
            {
                return _isZnakyVisible;
            }
            set
            {
                if (value != _isZnakyVisible)
                {
                    _isZnakyVisible = value;
                    OnPropertyChanged();
                }
            }
        }
        
        private double _fontSize = 10; 

        
        public double FontSize
        {
            get
            {
                return _fontSize;
            }
            set
            {
                if (value != _fontSize)
                {
                    _fontSize = value;
                    OnPropertyChanged();
                }
            }
        }

        
         
        private bool _isSlovaVisible = true; 
        
        
        public bool IsSlovaVisible
        {
            get
            {
                return _isSlovaVisible;
            }
            set
            {
                if (value != _isSlovaVisible)
                {
                    _isSlovaVisible = value;
                    OnPropertyChanged();
                }
            }
        }

        
        private bool _isUsekyVisible = true; 
        
        
        public bool IsUsekyVisible
        {
            get
            {
                return _isUsekyVisible;
            }
            set
            {
                if (value != _isUsekyVisible)
                {
                    _isUsekyVisible = value;
                    OnPropertyChanged();
                }
            }
        }
        
        
        private bool _isDigramyVisible = true; 
        
        
        public bool IsDigramyVisible
        {
            get
            {
                return _isDigramyVisible;
            }
            set
            {
                if (value != _isDigramyVisible)
                {
                    _isDigramyVisible = value;
                    OnPropertyChanged();
                }
            }
        }
        
        
        
        private bool _isTrigramyVisible = true; 
        
        
        public bool IsTrigramyVisible
        {
            get
            {
                return _isTrigramyVisible;
            }
            set
            {
                if (value != _isTrigramyVisible)
                {
                    _isTrigramyVisible = value;
                    OnPropertyChanged();
                }
            }
        }
        
        
        
          
        private bool _isRozlisovatVelikostPismen = true; 
        
        
        public bool IsRozlisovatVelikostPismen
        {
            get
            {
                return _isRozlisovatVelikostPismen;
            }
            set
            {
                if (value != _isRozlisovatVelikostPismen)
                {
                    _isRozlisovatVelikostPismen = value;
                    OnPropertyChanged();
                }
            }
        }
        
        
        public string FilePath { get; set; }
        public bool IsFiltrovatAutomaticky { get; set; }

        
        private PrehledyStatistik _prehledyStatistik;
        
        public PrehledyStatistikViewModel(PrehledyStatistik userControl)
        {
            _prehledyStatistik = userControl;
        }


        public void SetAutomaticFiltering(bool isChecked)
        {
            IsFiltrovatAutomaticky = isChecked;
            _prehledyStatistik.ZrusitFiltry(isChecked);
            
        }
        
        public void TextFilter(string text)
        {
            _prehledyStatistik.FiltrujText(text);
            
        }
        
        public void NastavitVelikostTextu(double velikost)
        {
            _prehledyStatistik.NastavitVelikostTextu(velikost);
            // FontSize = velikost;
            _prehledyStatistik.tlvStatistiky.FontSize = velikost;
        }

    }
}