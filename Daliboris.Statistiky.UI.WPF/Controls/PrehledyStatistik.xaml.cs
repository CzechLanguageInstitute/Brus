using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;
using Daliboris.Pomucky.Xml;
using Daliboris.Statistiky.Core.Services;
using Daliboris.Statistiky.UI.WPF.ViewModels;

namespace Daliboris.Statistiky.UI.WPF.Controls
{
    /// <summary>
    /// Interaction logic for PrehledyStatistik.xaml
    /// </summary>
    public partial class PrehledyStatistik : UserControl
    {
        private BackgroundWorker mbgwNacitaniJevu;
        private BackgroundWorker mbgwFiltrovani;
        private Dictionary<string, Ukazy> mgdcUkazy;
        private Dictionary<string, DataGrid> mgdcTabulky;
        private UkazFilter mukfAktualniFiltr;
        private PrehledyStatistikViewModel _viewModel;

        public PrehledyStatistik()
        {
 
            InitializeComponent();
            _viewModel = new PrehledyStatistikViewModel(this);
            DataContext = _viewModel;
            IdentifikujPromenne();
        }


        //private bool mblnRozlisovatVelikostPismen;
        //private bool mblnFiltrovatAutomaticky;
        //private bool mblnSlovaZacinajiciMalymPismenem;
        
        //
        //     FrameworkPropertyMetadata mdht = new FrameworkPropertyMetadata(String.Empty, HledanyTextPropertyChanged);
        //     HledanyTextProperty =
        //         DependencyProperty.Register("HledanyText", typeof(string), typeof(PrehledyStatistik), mdht);


        /*
static void SouborStatistikChangedCallBack(DependencyObject property,
    DependencyPropertyChangedEventArgs args) {
    SearchTextBox searchTextBox = (SearchTextBox)property;
    searchTextBox.textSearch.Text = (string)args.NewValue;
}
    

        */

        #region SouborStatistik dependency property

        /// <summary>
        /// A property wrapper for the <see cref="SouborStatistikProperty"/>
        /// dependency property:<br/>
        /// Description
        /// </summary>
        /// <summary>
        /// Handles changes on the <see cref="SouborStatistikProperty"/> dependency property. As
        /// WPF internally uses the dependency property system and bypasses the
        /// <see cref="SouborStatistik"/> property wrapper, updates should be handled here.
        /// </summary>
        /// <param name="d">The currently processed owner of the property.</param>
        /// <param name="e">Provides information about the updated property.</param>

        #endregion

        public void ZrusitFiltry(bool isChecked)
        {
            if (!isChecked)
            {
                PriraditUsekyDataGridum();
            }
        }

        
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
        public string HledanyText
        {
            get { return (string) GetValue(HledanyTextProperty); }
            set { SetValue(HledanyTextProperty, value); }
        }


        /// <summary>
        /// Handles changes on the <see cref="HledanyTextProperty"/> dependency property. As
        /// WPF internally uses the dependency property system and bypasses the
        /// <see cref="HledanyText"/> property wrapper, updates should be handled here.
        /// </summary>
        /// <param name="d">The currently processed owner of the property.</param>
        /// <param name="e">Provides information about the updated property.</param>
        private static void HledanyTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PrehledyStatistik owner = (PrehledyStatistik) d;
            string newValue = (string) e.NewValue;
            //TODO provide implementation
            //throw new NotImplementedException("Change event handler for dependency property HledanyText not implemented.");
            // if (owner.FiltrovatAutomaticky)
            // {
            //     owner.NastavAktualniFiltr(newValue, owner.RozlisovatVelikostPismen);
            //     owner.PriraditFiltryDataGridum(1);
            // }
        }

        #endregion
        

        private void IdentifikujPromenne()
        {
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


        // private void VycistitDatoveMrizky()
        // {
        //     foreach (KeyValuePair<string, DataGrid> kvp in mgdcTabulky)
        //     {
        //         kvp.Value.ItemsSource = null;
        //     }
        // }

        public void NastavitVelikostTextu(double velikostTextu)
        {
            foreach (KeyValuePair<string, DataGrid> kvp in mgdcTabulky)
            {
                kvp.Value.FontSize = velikostTextu;
            }
        }

        private async void tlvStatistiky_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var o = e.NewValue;
            var xn = o as XmlNode;
            if (xn == null)
                return;
            var xa = xn.Attributes["id"];
            if (xa == null)
                return;


            await NactiObsah(xa.Value);
            PriraditUsekyDataGridum();
        }


        private async Task NactiObsah(string idRelace)
        {
            var file = ((PrehledyStatistikViewModel) DataContext).FilePath;
            var jvs = StatisticsService.NactiObsah(file, idRelace);

            mgdcUkazy["1"] = PrevestJevyNaUkazy(jvs, 4);
            mgdcUkazy["2"] = PrevestJevyNaUkazy(jvs, 1);
            mgdcUkazy["3"] = PrevestJevyNaUkazy(jvs, 2);
            mgdcUkazy["4"] = PrevestJevyNaUkazy(jvs, 3);
            mgdcUkazy["5"] = PrevestJevyNaUkazy(jvs, 0);
        }

        private void PriraditUsekyDataGridum()
        {
            PriraditUsekyDataGridum(1);
        }

        private void PriraditUsekyDataGridum(int iPocatek)
        {
            for (int i = iPocatek; i < 6; i++)
            {
                mgdcTabulky[i.ToString()].ItemsSource = mgdcUkazy[i.ToString()];
            }
        }


        private Ukazy PrevestJevyNaUkazy(List<Jevy> jvs, int i)
        {
            Ukazy us = new Ukazy();
            foreach (Jev jv in jvs[i])
            {
                us.Add(new Ukaz(jv.Nazev, jv.Pocet));
            }

            return us;
        }

        private void Dtg_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProvestFiltrovaniPodleVyberu(sender as DataGrid, e);
        }

        private void ProvestFiltrovaniPodleVyberu(DataGrid dgAktualni, SelectionChangedEventArgs e)
        {
            if (!_viewModel.IsFiltrovatAutomaticky)
                return;

            if (dgAktualni == null)
                return;

            if (e.AddedItems.Count == 0)
                return;
            Ukaz uk = e.AddedItems[0] as Ukaz;
            if (uk == null)
                return;
            NastavAktualniFiltr(uk.Nazev, ((PrehledyStatistikViewModel)DataContext).IsRozlisovatVelikostPismen);

            int iPoradi;
            if (!Int32.TryParse(dgAktualni.Tag.ToString(), out iPoradi))
                return;

            //PriraditUsekyDataGridum(iPoradi);
            PriraditFiltryDataGridum(iPoradi + 1);
        }

        private void PriraditFiltryDataGridum(int iPocatek)
        {
            for (int i = iPocatek; i < 6; i++)
            {
                mgdcTabulky[i.ToString()].ItemsSource =
                    new Ukazy(mgdcUkazy[i.ToString()].FindAll(mukfAktualniFiltr.ObsahujeText));
            }
        }

        private void NastavAktualniFiltr(string strText, bool blnRozlizovatVelikostPismen)
        {
            mukfAktualniFiltr = new UkazFilter(!blnRozlizovatVelikostPismen, strText);
        }

        /*
                private void FiltrovatDataGrid(DataGrid dgDataGrid) {
                    //DS.Ukazy ukzf = new DS.Ukazy(ukUkazy.FindAll(DS.UkazFilter.ZacinaMalymPismenem));
                    Ukazy ukzf = new Ukazy(mgdcUkazy[dgDataGrid.Tag.ToString()].FindAll(mukfAktualniFiltr.ObsahujeText));
                    dgDataGrid.ItemsSource = ukzf;
                }
        */

        public void FiltrujText(string sText, bool? nullable)
        {
            bool bRozlisovat = nullable.GetValueOrDefault(false);
            NastavAktualniFiltr(sText, bRozlisovat);
            PriraditFiltryDataGridum(1);
        }

        public void ZrusFiltry()
        {
            ZrusitFiltry(false);
        }
    }
}