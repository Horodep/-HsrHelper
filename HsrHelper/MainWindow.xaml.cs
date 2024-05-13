using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace HsrHelper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataParser.Initialize();

            BindUi();
        }

        private void BindUi()
        {
            relicListUi.ItemsSource = DataParser.RelicListToCharacters;
            planarListUi.ItemsSource = DataParser.PlanarListToCharacters;
            RelicListTitles.ItemsSource = DataParser.RelicListTitles;
            PlanarListTitles.ItemsSource = DataParser.PlanarListTitles;
            characterList.ItemsSource = DataParser.CharList;

            RelicListMode.IsChecked = true;
            PlanarListMode.IsChecked = true;

            CollectionView view1 = (CollectionView)CollectionViewSource.GetDefaultView(relicListUi.ItemsSource);
            view1.GroupDescriptions.Add(new PropertyGroupDescription("character"));

            CollectionView view2 = (CollectionView)CollectionViewSource.GetDefaultView(planarListUi.ItemsSource);
            view2.GroupDescriptions.Add(new PropertyGroupDescription("character"));
        }
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.ToString()) { UseShellExecute = true });
        }

        #region Relic Set Filters

        private List<string> relicFilterListByCharacter = new List<string>();

        private void RefreshRelicTable(object sender, SelectionChangedEventArgs e)
        {
            relicFilterListByCharacter.Clear();

            RelicSet selectedRelic = (RelicSet)RelicListTitles.SelectedValue;

            if (selectedRelic != null) 
            {
                foreach (var rtc in DataParser.RelicListToCharacters)
                {
                    if (rtc.relic.name == selectedRelic.name || rtc.relic_2?.name == selectedRelic.name)
                        relicFilterListByCharacter.Add(rtc.character.name);
                }
            }

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(relicListUi.ItemsSource);
            view.Filter = RelicSetFilter;
        }
        private void RelicListMode_Checked(object sender, RoutedEventArgs e) => RefreshRelicTable(sender, null);

        private bool RelicSetFilter(object item)
        {
            if (!(item is RelicSetToCharacter rstc))
                throw new Exception("Wrong data in filter");

            RelicSet selectedRelic = (RelicSet)RelicListTitles.SelectedValue;

            if (selectedRelic == null || selectedRelic.name == "")
            {
                rstc.flag = false;
                return rstc.character.enabled;
            }

            rstc.flag = rstc.relic.name == selectedRelic.name || rstc.relic_2?.name == selectedRelic.name;

            if (!RelicListMode.IsChecked ?? false) 
                return  (rstc.relic.name == selectedRelic.name || rstc.relic_2?.name == selectedRelic.name) && rstc.character.enabled;
            else
                return relicFilterListByCharacter.Contains(rstc.character.name) && rstc.character.enabled;
        }

        #endregion

        #region Planar Set Filters

        private List<string> planarFilterListByCharacter = new List<string>();

        private void RefreshPlanarTable(object sender, SelectionChangedEventArgs e)
        {
            planarFilterListByCharacter.Clear();

            PlanarSet selectedPlanar = (PlanarSet)PlanarListTitles.SelectedValue;

            if (selectedPlanar != null)
            {
                foreach (var planar in DataParser.PlanarListToCharacters)
                {
                    if (planar.planar.name == selectedPlanar.name)
                        planarFilterListByCharacter.Add(planar.character.name);
                }
            }

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(planarListUi.ItemsSource);
            view.Filter = PlanarSetFilter;
        }

        private void PlanarListMode_Checked(object sender, RoutedEventArgs e) => RefreshPlanarTable(sender, null);

        private bool PlanarSetFilter(object item)
        {
            if (!(item is PlanarSetToCharacter pstc))
                throw new Exception("Wrong data in filter");

            PlanarSet selectedPlanar = (PlanarSet)PlanarListTitles.SelectedValue;

            if (selectedPlanar == null || selectedPlanar.name == "")
            {
                pstc.flag = false;
                return pstc.character.enabled;
            }

            pstc.flag = pstc.planar.name == selectedPlanar.name;

            if (!PlanarListMode.IsChecked ?? false)
                return pstc.planar.name == selectedPlanar.name && pstc.character.enabled;
            else
                return planarFilterListByCharacter.Contains(pstc.character.name) && pstc.character.enabled;
        }

        #endregion

        #region Data Reload

        public void ReloadDataButtonClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Please, don't refresh data unless it is required.\nThis may take some time (approx. ~10 seconds).");
            (sender as Button).IsEnabled = false;

            Task.Run(() =>
            {
                this.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate ()
                {
                    progressBar.Visibility = Visibility.Visible;
                }));

                WebLoader.LoadData(UpdateProgressBar);
                DataParser.Initialize();

                this.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate ()
                {
                    progressBar.Visibility = Visibility.Hidden;
                }));
            });
        }

        public void UpdateProgressBar(int value)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                this.progressBar.Value = value;
            }));
        }

        #endregion

        #region Settings

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var name = (sender as CheckBox).Content;
            var data = (sender as CheckBox).IsChecked;
            Config.WriteINI("Characters", name.ToString(), data.ToString());
        }

        #endregion
    }
}