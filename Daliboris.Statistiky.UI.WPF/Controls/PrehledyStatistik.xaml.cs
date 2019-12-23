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
using System.ComponentModel;
using System.Xml;
using System.IO;
using Daliboris.Statistiky;

namespace Daliboris.Statistiky.UI.WPF {
	class NacitaniObsahu {
		public NacitaniObsahu() { }

		public NacitaniObsahu(string strSoubor, string strIdentifikatorRelace) {
			Soubor = strSoubor;
			IdentifikatorRelace = strIdentifikatorRelace;
		}
		public string Soubor { get; set; }
		public string IdentifikatorRelace { get; set; }
	}

	/// <summary>
	/// Interaction logic for PrehledyStatistik.xaml
	/// </summary>
	public partial class PrehledyStatistik : UserControl {

		private BackgroundWorker mbgwNacitaniJevu;
		private BackgroundWorker mbgwFiltrovani;
		private Dictionary<string, Ukazy> mgdcUkazy;
		private Dictionary<string, DataGrid> mgdcTabulky;
		private UkazFilter mukfAktualniFiltr;

		//private bool mblnRozlisovatVelikostPismen;
		//private bool mblnFiltrovatAutomaticky;
		//private bool mblnSlovaZacinajiciMalymPismenem;

		static PrehledyStatistik() {
			FrameworkPropertyMetadata md = new FrameworkPropertyMetadata(String.Empty, SouborStatistikPropertyChanged);
			SouborStatistikProperty = DependencyProperty.Register("SouborStatistik", typeof(string), typeof(PrehledyStatistik), md);

			FrameworkPropertyMetadata mdfa = new FrameworkPropertyMetadata(false, FiltrovatAutomatickyPropertyChanged);
			FiltrovatAutomatickyProperty = DependencyProperty.Register("FiltrovatAutomaticky", typeof(bool), typeof(PrehledyStatistik), mdfa);

			FrameworkPropertyMetadata mdrvp = new FrameworkPropertyMetadata(true, RozlisovatVelikostPismenPropertyChanged);
			RozlisovatVelikostPismenProperty = DependencyProperty.Register("RozlisovatVelikostPismen", typeof(bool), typeof(PrehledyStatistik), mdrvp);

			FrameworkPropertyMetadata mdht = new FrameworkPropertyMetadata(String.Empty, HledanyTextPropertyChanged);
			HledanyTextProperty = DependencyProperty.Register("HledanyText", typeof(string), typeof(PrehledyStatistik), mdht);

		FrameworkPropertyMetadata mdvt = new FrameworkPropertyMetadata(13d, VelikostTextuPropertyChanged);
		VelikostTextuProperty = DependencyProperty.Register("VelikostTextu", typeof (double), typeof (PrehledyStatistik), mdvt);


		}



		public PrehledyStatistik() {
			InitializeComponent();
			IdentifikujPromenne();
			NactiSouborStatistik();
		}

		/*
static void SouborStatistikChangedCallBack(DependencyObject property,
	DependencyPropertyChangedEventArgs args) {
	SearchTextBox searchTextBox = (SearchTextBox)property;
	searchTextBox.textSearch.Text = (string)args.NewValue;
}
	


public string SouborStatistik {
	get { return (string)GetValue(SouborStatistikProperty); }
	set { SetValue(SouborStatistikProperty, value); }
}

// Using a DependencyProperty as the backing store for SouborStatistik.  This enables animation, styling, binding, etc...
public static readonly DependencyProperty SouborStatistikProperty =
				DependencyProperty.Register("SouborStatistik", typeof(string), typeof(PrehledyStatistik),
				new UIPropertyMetadata(string.Empty, new PropertyChangedCallback(SouborStatistikChangedCallBack)));
*/

		/*
		private string mstrSouborStatistik;
		public string SouborStatistik {
			get {
				return mstrSouborStatistik;
			}
			set {
				mstrSouborStatistik = value;
				NactiSouborStatistik();
			}
		}
		*/

		#region SouborStatistik dependency property

		/// <summary>
		/// Description
		/// </summary>
		public static readonly DependencyProperty SouborStatistikProperty;

		//TODO: copy to static constructor
		//register dependency property


		/// <summary>
		/// A property wrapper for the <see cref="SouborStatistikProperty"/>
		/// dependency property:<br/>
		/// Description
		/// </summary>
		public string SouborStatistik {
			get { return (string)GetValue(SouborStatistikProperty); }
			set { SetValue(SouborStatistikProperty, value); }
		}


		/// <summary>
		/// Handles changes on the <see cref="SouborStatistikProperty"/> dependency property. As
		/// WPF internally uses the dependency property system and bypasses the
		/// <see cref="SouborStatistik"/> property wrapper, updates should be handled here.
		/// </summary>
		/// <param name="d">The currently processed owner of the property.</param>
		/// <param name="e">Provides information about the updated property.</param>
		private static void SouborStatistikPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PrehledyStatistik owner = (PrehledyStatistik)d;
			string newValue = (string)e.NewValue;

			//TODO provide implementation
			//throw new NotImplementedException("Change event handler for dependency property SouborStatistik not implemented.");
			owner.SouborStatistik = newValue;
			owner.NactiSouborStatistik();
		}

		#endregion

		#region FiltrovatAutomaticky dependency property

		/// <summary>
		/// Description
		/// </summary>
		public static readonly DependencyProperty FiltrovatAutomatickyProperty;


		/// <summary>
		/// A property wrapper for the <see cref="FiltrovatAutomatickyProperty"/>
		/// dependency property:<br/>
		/// Description
		/// </summary>
		public bool FiltrovatAutomaticky {
			get { return (bool)GetValue(FiltrovatAutomatickyProperty); }
			set { SetValue(FiltrovatAutomatickyProperty, value); }
		}


		/// <summary>
		/// Handles changes on the <see cref="FiltrovatAutomatickyProperty"/> dependency property. As
		/// WPF internally uses the dependency property system and bypasses the
		/// <see cref="FiltrovatAutomaticky"/> property wrapper, updates should be handled here.
		/// </summary>
		/// <param name="d">The currently processed owner of the property.</param>
		/// <param name="e">Provides information about the updated property.</param>
		private static void FiltrovatAutomatickyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PrehledyStatistik owner = (PrehledyStatistik)d;
			bool newValue = (bool)e.NewValue;

			//TODO provide implementation
			//throw new NotImplementedException("Change event handler for dependency property FiltrovatAutomaticky not implemented.");
			//owner.ProvestFiltrovaniPodleVyberu();
			owner.FiltrovatAutomaticky = newValue;
			if (!newValue) {
				owner.ZrusitFiltry();
			}
		}

		private void ZrusitFiltry() {
			PriraditUsekyDataGridum();
		}

		#endregion

		#region RozlisovatVelikostPismen dependency property

		/// <summary>
		/// Rozlišování velikosti písmen při filtrování a hledání
		/// </summary>
		public static readonly DependencyProperty RozlisovatVelikostPismenProperty;

		//TODO: copy to static constructor
		//register dependency property


		/// <summary>
		/// A property wrapper for the <see cref="RozlisovatVelikostPismenProperty"/>
		/// dependency property:<br/>
		/// Rozlišování velikosti písmen při filtrování a hledání
		/// </summary>
		public bool RozlisovatVelikostPismen {
			get { return (bool)GetValue(RozlisovatVelikostPismenProperty); }
			set { SetValue(RozlisovatVelikostPismenProperty, value); }
		}


		/// <summary>
		/// Handles changes on the <see cref="RozlisovatVelikostPismenProperty"/> dependency property. As
		/// WPF internally uses the dependency property system and bypasses the
		/// <see cref="RozlisovatVelikostPismen"/> property wrapper, updates should be handled here.
		/// </summary>
		/// <param name="d">The currently processed owner of the property.</param>
		/// <param name="e">Provides information about the updated property.</param>
		private static void RozlisovatVelikostPismenPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PrehledyStatistik owner = (PrehledyStatistik)d;
			bool newValue = (bool)e.NewValue;

			//TODO provide implementation
			//throw new NotImplementedException("Change event handler for dependency property RozlisovatVelikostPismen not implemented.");
		}

		#endregion

		#region HledanyText dependency property

		/// <summary>
		/// Hledaný text ve statistikách
		/// </summary>
		public static readonly DependencyProperty HledanyTextProperty;

		//TODO: copy to static constructor
		//register dependency property


		/// <summary>
		/// A property wrapper for the <see cref="HledanyTextProperty"/>
		/// dependency property:<br/>
		/// Hledaný text ve statistikách
		/// </summary>
		public string HledanyText {
			get { return (string)GetValue(HledanyTextProperty); }
			set { SetValue(HledanyTextProperty, value); }
		}


		/// <summary>
		/// Handles changes on the <see cref="HledanyTextProperty"/> dependency property. As
		/// WPF internally uses the dependency property system and bypasses the
		/// <see cref="HledanyText"/> property wrapper, updates should be handled here.
		/// </summary>
		/// <param name="d">The currently processed owner of the property.</param>
		/// <param name="e">Provides information about the updated property.</param>
		private static void HledanyTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PrehledyStatistik owner = (PrehledyStatistik)d;
			string newValue = (string)e.NewValue;
			//TODO provide implementation
			//throw new NotImplementedException("Change event handler for dependency property HledanyText not implemented.");
			if (owner.FiltrovatAutomaticky)
			{
				owner.NastavAktualniFiltr(newValue, owner.RozlisovatVelikostPismen);
				owner.PriraditFiltryDataGridum(1);
			}

		}

		#endregion

		#region VelikostTextu dependency property

		/// <summary>
		/// Nastavuje nebo vrací velikost textu v tabulkách
		/// </summary>
		public static readonly DependencyProperty VelikostTextuProperty;

//TODO: copy to static constructor
//register dependency property


		/// <summary>
		/// A property wrapper for the <see cref="VelikostTextuProperty"/>
		/// dependency property:<br/>
		/// Nastavuje nebo vrací velikost textu v tabulkách
		/// </summary>
		public double VelikostTextu
		{
			get { return (double) GetValue(VelikostTextuProperty); }
			set { SetValue(VelikostTextuProperty, value); }
		}


		/// <summary>
		/// Handles changes on the <see cref="VelikostTextuProperty"/> dependency property. As
		/// WPF internally uses the dependency property system and bypasses the
		/// <see cref="VelikostTextu"/> property wrapper, updates should be handled here.
		/// </summary>
		/// <param name="d">The currently processed owner of the property.</param>
		/// <param name="e">Provides information about the updated property.</param>
		private static void VelikostTextuPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			PrehledyStatistik owner = (PrehledyStatistik) d;
			double newValue = (double) e.NewValue;

			//TODO provide implementation
			//throw new NotImplementedException("Change event handler for dependency property VelikostTextu not implemented.");
			owner.NastavitVelikostTextu();
		}

		#endregion


		private void IdentifikujPromenne() {
			mgdcTabulky = new Dictionary<string, DataGrid>(5);
			mgdcTabulky.Add(dtgZnaky.Tag.ToString(), dtgZnaky);
			mgdcTabulky.Add(dtgSlova.Tag.ToString(), dtgSlova);
			mgdcTabulky.Add(dtgTrigramy.Tag.ToString(), dtgTrigramy);
			mgdcTabulky.Add(dtgDigramy.Tag.ToString(), dtgDigramy);
			mgdcTabulky.Add(dtgUseky.Tag.ToString(), dtgUseky);

			mgdcUkazy = new Dictionary<string, Ukazy>(5);
			mgdcUkazy.Add(dtgZnaky.Tag.ToString(), new Ukazy("Znaky"));
			mgdcUkazy.Add(dtgDigramy.Tag.ToString(), new Ukazy("Digramy"));
			mgdcUkazy.Add(dtgTrigramy.Tag.ToString(), new Ukazy("Trigramy"));
			mgdcUkazy.Add(dtgSlova.Tag.ToString(), new Ukazy("Slova"));
			mgdcUkazy.Add(dtgUseky.Tag.ToString(), new Ukazy("Useky"));

		}
		private void NactiSouborStatistik() {
			//@"V:\Projekty\Daliboris\Statistiky\Data\SlovKlem.docx.pjv";
			/* @"D:\Slovniky\ESSC\Text\Word\Hotovo\Pismena\ESSC_Ž.docx.pjv";
				@"D:\Slovniky\ESSC\Text\Word\Hotovo\Pismena\ESSC_Z.docx.pjv";
				@"V:\Projekty\Daliboris\Statistiky\Data\ŠtítSvátA kontrola.docx.pjv";
			*/
			string sSoubor = SouborStatistik;
			if (String.IsNullOrEmpty(sSoubor))
				return;
			XmlDocument xd = null;
			if (File.Exists(sSoubor)) {
				xd = Statistik.NactiDataStatistiky(sSoubor);
			}
			//if (xd != null)
			VycistitDatoveMrizky();
			tlvStatistiky.DataContext = xd;
		}

		private void VycistitDatoveMrizky() {
			foreach (KeyValuePair<string, DataGrid> kvp in mgdcTabulky)
			{
				kvp.Value.ItemsSource = null;
			}
		}

		private  void NastavitVelikostTextu()
		{
			tlvStatistiky.FontSize = VelikostTextu;
			foreach (KeyValuePair<string, DataGrid> kvp in mgdcTabulky)
			{
				kvp.Value.FontSize = VelikostTextu;
			}
		}
		private void tlvStatistiky_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e) {
			object o = e.NewValue;
			XmlNode xn = o as XmlNode;
			if (xn == null)
				return;
			XmlAttribute xa = xn.Attributes["id"];
			if (xa == null)
				return;

			mbgwNacitaniJevu = new BackgroundWorker();
			mbgwNacitaniJevu.DoWork += new DoWorkEventHandler(bgwNacitaniJevu_DoWork);
			mbgwNacitaniJevu.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgwNacitaniJevu_RunWorkerCompleted);
			Mouse.OverrideCursor = Cursors.Wait;
			NacitaniObsahu np = new NacitaniObsahu(SouborStatistik, xa.Value);
			mbgwNacitaniJevu.RunWorkerAsync(np);

		}

		void bgwNacitaniJevu_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
			Mouse.OverrideCursor = null;
			if (mgdcUkazy.Count <= 0)
				return;

			PriraditUsekyDataGridum();
			/*
			PriraditUsekyDataGridu(0, dtgUseky);
			PriraditUsekyDataGridu(1, dtgDigramy);
			PriraditUsekyDataGridu(2, dtgTrigramy);
			PriraditUsekyDataGridu(3, dtgSlova);
			PriraditUsekyDataGridu(4, dtgZnaky);
			*/
		}

		void bgwNacitaniJevu_DoWork(object sender, DoWorkEventArgs e) {
			NacitaniObsahu np = e.Argument as NacitaniObsahu;
			if (np == null)
				return;
			NactiObsah(np);
		}

		private void NactiObsah(NacitaniObsahu np) {
			List<Jevy> jvs = Statistik.NactiObsah(np.Soubor, np.IdentifikatorRelace);

			mgdcUkazy["1"] = PrevestJevyNaUkazy(jvs, 4);
			mgdcUkazy["2"] = PrevestJevyNaUkazy(jvs, 1);
			mgdcUkazy["3"] = PrevestJevyNaUkazy(jvs, 2);
			mgdcUkazy["4"] = PrevestJevyNaUkazy(jvs, 3);
			mgdcUkazy["5"] = PrevestJevyNaUkazy(jvs, 0);

			/*
			for (int i = 0; i < jvs.Count; i++) {
				mgdcUkazy[(i + 1).ToString()] = PrevestJevyNaUkazy(jvs, i);
			}
			 * */
		}

		/*
		private void PriraditUsekyDataGridu(List<Jevy> jvs, int i, DataGrid dg)
		{
			Ukazy uk;
			uk = PrevestJevyNaUkazy(jvs, i);
			mglstUkazy.Add(uk);
			dg.ItemsSource = uk;
		}
		*/

		private void PriraditUsekyDataGridum() {
			PriraditUsekyDataGridum(1);
		}

		private void PriraditUsekyDataGridum(int iPocatek) {
			for (int i = iPocatek; i < 6; i++) {
				mgdcTabulky[i.ToString()].ItemsSource = mgdcUkazy[i.ToString()];
			}
		}


		private Ukazy PrevestJevyNaUkazy(List<Jevy> jvs, int i) {
			Ukazy us = new Ukazy();
			foreach (Jev jv in jvs[i]) {
				us.Add(new Ukaz(jv.Nazev, jv.Pocet));
			}
			return us;
		}

		private void Dtg_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			ProvestFiltrovaniPodleVyberu(sender as DataGrid, e);
		}

		private void ProvestFiltrovaniPodleVyberu(DataGrid dgAktualni, SelectionChangedEventArgs e) {

			if (!FiltrovatAutomaticky)
				return;

			if (dgAktualni == null)
				return;

			if (e.AddedItems.Count == 0)
				return;
			Ukaz uk = e.AddedItems[0] as Ukaz;
			if (uk == null)
				return;
			NastavAktualniFiltr(uk.Nazev, RozlisovatVelikostPismen);

			int iPoradi;
			if (!Int32.TryParse(dgAktualni.Tag.ToString(), out iPoradi))
				return;

			//PriraditUsekyDataGridum(iPoradi);
			PriraditFiltryDataGridum(iPoradi + 1);
		}

		private void PriraditFiltryDataGridum(int iPocatek) {
			for (int i = iPocatek; i < 6; i++) {
				mgdcTabulky[i.ToString()].ItemsSource = new Ukazy(mgdcUkazy[i.ToString()].FindAll(mukfAktualniFiltr.ObsahujeText));
			}
		}

		private void NastavAktualniFiltr(string strText, bool blnRozlizovatVelikostPismen) {
			mukfAktualniFiltr = new UkazFilter(!blnRozlizovatVelikostPismen, strText);
		}

		/*
				private void FiltrovatDataGrid(DataGrid dgDataGrid) {
					//DS.Ukazy ukzf = new DS.Ukazy(ukUkazy.FindAll(DS.UkazFilter.ZacinaMalymPismenem));
					Ukazy ukzf = new Ukazy(mgdcUkazy[dgDataGrid.Tag.ToString()].FindAll(mukfAktualniFiltr.ObsahujeText));
					dgDataGrid.ItemsSource = ukzf;
				}
		*/

		public void FiltrujText(string sText, bool? nullable) {
			bool bRozlisovat = nullable.GetValueOrDefault(false);
			NastavAktualniFiltr(sText, bRozlisovat);
			PriraditFiltryDataGridum(1);
		}

		public void ZrusFiltry() {
			ZrusitFiltry();
		}
	}
}
