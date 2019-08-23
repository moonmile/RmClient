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
using RmBulkup.ViewModels;

namespace RmBulkup.ViewModels
{
    class MainViewModel : ObservableObject
    {
        private string _Filename = "";
        public string Filename { get => _Filename; set => SetProperty(ref _Filename, value, nameof(Filename)); }

        private List<IssueEx> _Items;
        public List<IssueEx> Items {
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

        private Project _Project;
        public Project Project
        {
            get { return _Project; }
            set
            {
                SetProperty(ref _Project, value, nameof(Project));
                if (Project != null)
                {
                    this.GetTickets(Project.Id);
                    _sv.GetUsers(Project.Id).ContinueWith(x => {
                        this.Users = x.Result;
                    });

                }
            }
        }
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
        /// チケット一覧を取得
        /// </summary>
        /// <param name="pid"></param>
        public async void GetTickets(int pid)
        {
            var items = await _sv.Issue.GetListAsync(pid);
            // TODO: プロジェクト内の membership のみ取得する
            this.Users = await _sv.User.GetListAsync(pid);
            // this.Users.AddRange(await _sv.User.GetListAsync());
            this.Items = items.ToIsseuExList();
        }



        /// <summary>
        /// Excelから読み込み
        /// </summary>
        /// <param name="path"></param>
        public void LoadExcel(string path)
        {
            using (var wb = new XLWorkbook(path))
            {
                /// 最初のシートからチケット一覧を読み込み
                var items = new List<IssueEx>();
                var sh = wb.Worksheets.FirstOrDefault();
                int r = 1;

                /// 先頭のIDを探す
                for (; r < 100; r++)
                {
                    if (sh.Cell(r, 1).GetString() == "ID") break;
                }
                if (r == 100) return;   // 見つからなかった場合
                /// r行の列名を探す
                int colID = 1, colTracker = 0, colStatus = 0, colPriority = 0,
                    colSubject = 0, colDesc = 0, colAuthor = 0;
                int colUpdate = 0;

                for (int col = 1; col < 100; col++)
                {
                    switch (sh.Cell(r, col).GetString())
                    {
                        case "ID": colID = col; break;
                        case "Tracker":
                        case "トラッカー":
                            colTracker = col; break;
                        case "Status":
                        case "ステータス":
                        case "状態":
                            colStatus = col; break;
                        case "Priority":
                        case "優先度":
                            colPriority = col; break;
                        case "Subject":
                        case "題名":
                        case "タイトル":
                            colSubject = col; break;
                        case "Description":
                        case "Desc":
                        case "内容":
                        case "詳細":
                            colDesc = col; break;
                        case "Author":
                        case "担当者":
                            colAuthor = col; break;
                        case "更新":
                        case "Update":
                            colUpdate = col; break;
                    }
                }
                r++;

                while (sh.Cell(r, colSubject).GetString() != "")
                {
                    var id = 0;
                    if ( sh.Cell(r, colID).GetString() != "" )
                        id = sh.Cell(r, colID).GetValue<int>();

                    string getcell(int c) => (c == 0) ? "" : sh.Cell(r, c).GetString();

                    var TrackerName = getcell(colTracker);
                    var PriorityName = getcell(colPriority);
                    var StatusName = getcell(colStatus);
                    var Subject = getcell(colSubject);
                    var Description = getcell(colDesc);
                    var AuthorName = getcell(colAuthor);
                    var updateMark = getcell(colUpdate);

                    var it = new IssueEx();
                    items.Add(it);
                    it.Id = 0;
                    it.Project = this.Project;
                    it.Tracker = this.Trackers.FirstOrDefault(t => t.Name == TrackerName);
                    it.Status = this.Statuses.FirstOrDefault(t => t.Name == StatusName);
                    it.Priority = this.Priorities.FirstOrDefault(t => t.Name == PriorityName);
                    // 名 姓の順のため姓で一致させる
                    it.Author = this.Users.FirstOrDefault(t => t.Name.EndsWith(AuthorName));
                    it.Subject = Subject;
                    it.Description = Description;
                    r++;
                    if (r > 1000) break;    // 念のため1000行まで
                }
                this.Items = items;
            }
        }

        /// <summary>
        /// Excel ファイルに保存する
        /// </summary>
        public void Save( string path ) 
        {
            /// 指定ファイルがない場合は、新規に保存する
            /// 指定ファイルがある場合は、一度

        }
        /// <summary>
        /// アップロードする
        /// </summary>
        public async Task Upload()
        {
            int i = 0;
            foreach (var issue in this.Items)
            {
                await _sv.Issue.CreateAsync(issue);
                await Task.Delay(500);   // ちょっとだけ待つ
                i++;
            }
        }
    }

    public class IssueEx : Issue
    {
        public bool IsUpdate { get; set; }
    }
    public static class IssueExExtensions
    {
        public static IssueEx ToIssueEx(this Issue t)
        {
            var ix = new IssueEx();
            RmBulkup.ViewModels.PropertyCopier.CopyTo<IssueEx>(t, ix);
            return ix;

        }
        public static List<IssueEx>ToIsseuExList( this List<Issue> items ) 
        {
            var lst = new List<IssueEx>();
            foreach ( var it in items )
            {
                lst.Add(it.ToIssueEx());
            }
            return lst;
        }
    }

    /*
    public class Ticket
    {
        public Project Project { get; set; }
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

        public Issue ToIssue()
        {
            var it = new Issue();
            it.Project = this.Project;
            it.Subject = this.題名;
            it.Description = this.説明;
            it.Tracker = new Tracker() { Id = this.トラッカーID };
            it.Status = new Status() { Id = this.ステータスID };
            it.Priority = new Priority() { Id = this.優先度ID };
            it.AssignedTo =
                this.担当者ID == null ? null : new AssignedTo() { Id = this.担当者ID ?? 0 };

            return it;
        }
    }
    */

}
