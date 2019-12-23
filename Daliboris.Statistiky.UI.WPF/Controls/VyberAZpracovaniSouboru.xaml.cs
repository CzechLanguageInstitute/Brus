using System;
using System.Windows;
using System.IO;
using System.Windows.Input;
using Daliboris.Statistiky.Word;

namespace Daliboris.Statistiky.UI.WPF {
	/// <summary>
	/// Interaction logic for VyberAZpracovaniSouboru.xaml
	/// </summary>
	public partial class VyberAZpracovaniSouboru : Window {
		public VyberAZpracovaniSouboru() {
			InitializeComponent();
		}


		private void btnZpracovat_Click(object sender, RoutedEventArgs e) {
			if (openFileSelector.FileName.Length > 0 || saveFileSelector.FileName.Length == 0) {
				//vybrat zpracovatele podle přípony
				FileInfo fi = new FileInfo(openFileSelector.FileName);


				if (fi.Exists) {

					Mouse.OverrideCursor = Cursors.Wait;

					try {
						DocxRotorNastaveni nst = new DocxRotorNastaveni();
						if (ChbOdstranitTecku.IsChecked != null) nst.OdstranitTeckuUSlov = (bool) ChbOdstranitTecku.IsChecked;
						if (ChbPonechatInterpunkci.IsChecked != null)
							nst.OdstranitPocatecniAKoncoveMezery = (bool) ChbPonechatInterpunkci.IsChecked;

						DocxRotor dxr = new DocxRotor(fi.FullName, nst);
						dxr.ZpracujDocx();
						//dxr.UlozPrehledy();
						Statistik st = new Statistik();
						st.SkupinaJevu = dxr.SkupinaJevu;
						st.VystupniSoubor = saveFileSelector.FileName;
						st.SloucitDetaily = false;
					    Statistik.OdstranitTecku = nst.OdstranitTeckuUSlov;
						Statistik.UlozStatistiky(dxr.SkupinaJevu, saveFileSelector.FileName, FormatUlozeniSeznamu.Text);

					}
					catch (Exception ex) {
						MessageBox.Show("Při zpracovní přehledů došlo k chybě.\r\n" + ex.Message);
					}
					finally {
						MessageBox.Show("Soubor " + fi.FullName +  " byl zpracován.");
						Mouse.OverrideCursor = null;
					}
				}
			}
		}

		private void openFileSelector_FileNameChanged(object sender, RoutedEventArgs e) {
			string sCesta = openFileSelector.FileName;
			if (string.IsNullOrEmpty(sCesta))
				return;
			if (!String.IsNullOrEmpty(saveFileSelector.FileName))
				return;
			FileInfo fi = new FileInfo(sCesta);
			saveFileSelector.FileName = fi.FullName.Replace(fi.Extension, ".pjv");
		}
	}
}
