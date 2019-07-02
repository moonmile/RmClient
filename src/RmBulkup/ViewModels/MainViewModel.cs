using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using ClosedXML.Excel;
using ClosedXML.Utils;
using Moonmile.Redmine;
using Moonmile.Redmine.Model;

namespace RmBulkup.ViewModels
{
    class MainViewModel : ObservableObject
    {
        private string _Filename = "";
        public string Filename { get => _Filename; set => SetProperty(ref _Filename, value, nameof(Filename)); }

        private List<Ticket> _Items;
        public List<Ticket> Items {
            get => _Items;
            set => SetProperty(ref _Items, value, nameof(Items));
        }

        RedmineService _sv;

        public MainViewModel()
        {
            _sv = new RedmineService();
            _sv.SetConfig(AppDomain.CurrentDomain.BaseDirectory + "RmBulkup.config");
            Init();

        }
        public async void Init()
        {
            this.Projects = await _sv.Project.GetListAsync();
            this.Trackers = await _sv.Tracker.GetListAsync();
            this.Statuses = await _sv.Status.GetListAsync();
            this.Priorities = await _sv.Priority.GetListAsync();

        }
        // コンボボックス用のリストを取得
        private List<Project> _Projects;
        public Project Project; // 選択用
        public List<Project> Projects { get => _Projects; set => SetProperty(ref _Projects, value, nameof(Projects)); }

        private List<Tracker> _Trackers;
        public List<Tracker> Trackers { get { return _Trackers; } set { SetProperty(ref _Trackers, value, nameof(Trackers)); } }
        private List<Status> _Statuses;
        public List<Status> Statuses { get { return _Statuses; } set { SetProperty(ref _Statuses, value, nameof(Statuses)); } }
        private List<Priority> _Priorities;
        public List<Priority> Priorities { get { return _Priorities; } set { SetProperty(ref _Priorities, value, nameof(Priorities)); } }
        private List<User> _Users;
        public List<User> Users { get { return _Users; } set { SetProperty(ref _Users, value, nameof(Users)); } }

        /// <summary>
        /// Excelから読み込み
        /// </summary>
        /// <param name="path"></param>
        public void LoadExcel( string path )
        {
            using (var wb = new XLWorkbook(path))
            {
                /// 最初のシートからチケット一覧を読み込み
                var items = new List<Ticket>();
                var sh = wb.Worksheets.FirstOrDefault();
                int r = 5;
                while ( sh.Cell(r, 1).GetString() != "" )
                {
                    var it = new Ticket();
                    items.Add(it);
                    it.ID = 0;
                    it.題名 = sh.Cell(r, 5).GetValue<string>();
                    it.説明 = sh.Cell(r, 6).GetValue<string>();
                    it.トラッカー = "機能";
                    it.ステータス = "新規";
                    it.優先度 = "通常";
                    it.担当者 = sh.Cell(r, 7).GetValue<string>();
                    r++;
                    if (r > 1000) break;
                }
                this.Items = items;
            }
        }
    }
    public class Ticket
    {
        public int ID { get; set; }
        public string 題名 { get; set; }
        public string 説明 { get; set; }
        public string トラッカー { get; set; }
        public int トラッカーID { get; set; }
        public string ステータス { get; set; }
        public int ステータスID { get; set; }
        public string 優先度 { get; set; }
        public int 優先度ID { get; set; }
        public string 担当者 { get; set; }
        public int? 担当者ID { get; set; }
    }
}
