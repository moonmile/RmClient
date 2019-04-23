using Microsoft.Win32;
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

        /// <summary>
        /// ファイルアップロード
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void clickFileUpload(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            // ファイルの種類を設定
            dialog.Filter = 
                "テキストファイル (*.txt)|*.txt|" + 
                "画像ファイル (*.jpg, *.jpeg, *.bmp, *.png)|*.jpg;*.jpeg;*.bmp,*.png|" +
                "PDFファイル (*.pdf)|*.pdf|" +
                "Excelファイル (*.xls,*.xlsx)|*.xls;*.xlsx|" +
                "Wordファイル (*.doc,*.docx)|*.doc;*.docx|" +
                "圧縮ファイル (*.zip)|*.zip|" +
                "全てのファイル (*.*)|*.*";
            // ダイアログを表示する
            if (dialog.ShowDialog() == true)
            {
                var path = dialog.FileName;
                if ( await _vm.FileUpload(path) == true )
                {
                    MessageBox.Show("ファイルを添付しました", "RmClient", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("ファイルの添付に失敗しました", "RmClient", MessageBoxButton.OK, MessageBoxImage.Error );
                }
            }
        }

        /// <summary>
        /// ブラウザで表示する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clickShowIssue(object sender, RoutedEventArgs e)
        {
            var url = _vm.TicketUrl;
            System.Diagnostics.Process.Start(url);
        }
    }
 }

