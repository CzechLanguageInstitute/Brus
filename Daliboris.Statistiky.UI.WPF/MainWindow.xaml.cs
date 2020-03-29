using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Daliboris.Statistiky.UI.WPF.ViewModels;
using Microsoft.Win32;

namespace Daliboris.Statistiky.UI.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private MainWindowsViewModel _viewModel;
        public MainWindow()
        {
   
            InitializeComponent();
                     
            _viewModel = new MainWindowsViewModel(PrehledyStatistik);
            DataContext = _viewModel;
           

            // Insert code required on object creation below this point.
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _viewModel?.VelikostTextuCommand.Execute(e.AddedItems[0]);
        }
    }
}