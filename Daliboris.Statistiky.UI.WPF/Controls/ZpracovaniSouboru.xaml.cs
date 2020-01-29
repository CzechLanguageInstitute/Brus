using System;
using System.Windows;
using System.IO;
using System.Windows.Input;
using Daliboris.Statistiky.Core.Services;
using Daliboris.Statistiky.UI.WPF.ViewModels;
using Daliboris.Statistiky.Word;

namespace Daliboris.Statistiky.UI.WPF
{
    /// <summary>
    /// Interaction logic for VyberAZpracovaniSouboru.xaml
    /// </summary>
    public partial class ZpracovaniSouboru : Window
    {
        public ZpracovaniSouboru(WordSettings wordSettings)
        {
            InitializeComponent();
            ((ZpracovaniSouboruViewModel) DataContext).LoadSettings(wordSettings);
            ((ZpracovaniSouboruViewModel) DataContext).ActualWindow = Window.GetWindow(this);
        }
    }
}