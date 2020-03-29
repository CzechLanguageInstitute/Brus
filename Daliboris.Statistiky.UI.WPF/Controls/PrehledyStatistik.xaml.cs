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
        

        public void ZrusitFiltry(bool isChecked)
        {
            if (!isChecked)
            {
                PriraditUsekyDataGridum();
            }
        }
        
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

        public void FiltrujText(string sText)
        {
            ZrusFiltry();
            bool bRozlisovat = _viewModel.IsRozlisovatVelikostPismen;
            NastavAktualniFiltr(sText, bRozlisovat);
            PriraditFiltryDataGridum(1);
        }

        public void ZrusFiltry()
        {
            ZrusitFiltry(false);
        }

        private void DtgZnaky_OnLoadingRowDetails(object sender, DataGridRowDetailsEventArgs e)
        {

            string text1 = "";
            string text2 = "";
            string text3 = "";

            foreach (var child in ((StackPanel)e.DetailsElement).Children)
            {
                if (child.GetType() == typeof(TextBlock))
                {
                    text1 = ((TextBlock) child).Text;
                }

                if (child.GetType() == typeof(StackPanel))
                {
                    int i = 0;
                    foreach (var stackChild in ((StackPanel) child).Children)
                    {
                        if (i == 0)
                        {
                            text2 = ((TextBlock) stackChild).Text;
                        }
                        else
                        {
                            text3 = ((TextBlock) stackChild).Text;
                        }

                        i++;
                    }

                }

            }

            tbxNadpis.Text = text1;
            tbxPodnadpis1.Text = text2;
            tbxPodnadpis2.Text = text3;
            

        }
    }
}