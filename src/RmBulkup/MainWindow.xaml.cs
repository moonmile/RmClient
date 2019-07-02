using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RmBulkup
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        ViewModels.MainViewModel _vm;

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _vm = new ViewModels.MainViewModel();
            this.DataContext = _vm;
        }

        private void clickUpload(object sender, RoutedEventArgs e)
        {

        }

        private void clickSelectExcel(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.Filter = "Excelファイル(*.xlsx)|*.xlsx";
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _vm.Filename = dlg.FileName;
                _vm.LoadExcel(_vm.Filename);
            }
        }

        /// <summary>
        /// プロジェクト情報取得
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clickGetRedmine(object sender, RoutedEventArgs e)
        {

        }
    }
}
