using Moonmile.Redmine;
using Moonmile.Redmine.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RmClient
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

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _vm = new ViewModel();
            this.DataContext = _vm;
        }
        ViewModel _vm;
        /// <summary>
        /// プロジェクト一覧を取得
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clickProjects(object sender, RoutedEventArgs e)
        {
        }
        /// <summary>
        /// 選択中のプロジェクトのチケット一覧を取得
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clickIssues(object sender, RoutedEventArgs e)
        {
            if (this.lv.SelectedItem == null)
                return;

            var pid = (this.lv.SelectedItem as Project).Id;
            _vm.GetTickets(pid);
        }

        /// <summary>
        /// 単独チケットを取得
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clickOneIssue(object sender, RoutedEventArgs e)
        {
            if (this.lv2.SelectedItem == null)
                return;
            var id = (this.lv2.SelectedItem as Issue).Id;
            _vm.GetTicket(id);
        }

        /// <summary>
        /// チケットを更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clickIssueUpdate(object sender, RoutedEventArgs e)
        {
            _vm.UpdateTicket();
        }
        /// <summary>
        /// チケットを新規作成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clickIssueNew(object sender, RoutedEventArgs e)
        {
            _vm.CreateTicket();
        }

        /// <summary>
        /// CSV形式でクリップボードにコピーする
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clickIssueCopy(object sender, RoutedEventArgs e)
        {
            _vm.CopyTicket();
        }
    }

 }

