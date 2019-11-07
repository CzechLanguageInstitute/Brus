using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Windows.Controls.Ribbon;

namespace Daliboris.Statistiky.UI.WPF {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : RibbonWindow {
		public MainWindow() {
			InitializeComponent();

			// Insert code required on object creation below this point.
		}

		private void rbnPasKaret_SelectionChanged(object sender, SelectionChangedEventArgs e) {

		}

		private void BtnOtevrit_Click(object sender, RoutedEventArgs e) {
			Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
			//dlg.FileName = "Přehled jevů"; // Default file name
			dlg.CheckFileExists = true;
			dlg.Filter = "Přehled jevů (*.pjv)|*.pjv|Vše (*.*)|*.*"; // Filter files by extension
			dlg.DefaultExt = ".pjv"; // Default file extension

			// Show open file dialog box
			bool? result = dlg.ShowDialog();

			// Process open file dialog box results
			if (result == true) {
				// Open document
				string filename = dlg.FileName;
				PsPrehledy.SouborStatistik = filename;
			}


		}

		private void BtnTabPrehledyVytvorit_Click(object sender, RoutedEventArgs e) {
			VyberAZpracovaniSouboru vzsVybert = new VyberAZpracovaniSouboru();
			bool? dlgOK = vzsVybert.ShowDialog();
		}

		private void BtnHledat_Click(object sender, RoutedEventArgs e) {
			string sText = TxtHledanyText.Text;
			if(String.IsNullOrEmpty(sText))
				PsPrehledy.ZrusFiltry();
			PsPrehledy.FiltrujText(sText, ChbRozlisovatVelikostPismen.IsChecked);
		}

		private void BtnVaidovatAplikovat_Click(object sender, RoutedEventArgs e) {

		}
	}
}
