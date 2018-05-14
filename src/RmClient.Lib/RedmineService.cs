using Moonmile.Redmine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Moonmile.Redmine
{
    /// <summary>
    /// Redmine REST API 呼び出しクラス
    /// </summary>
    public class RedmineService
    {
        public ProjectService Project { get; set; }
        public IssueService Issue { get; set; }
        public TrackerService Tracker { get; set; }
        public StatusService Status { get; set; }
        public UserService User { get; set; }
        public PriorityService Priority { get; set; }

        string apikey = "";
        string baseurl = "";
        HttpClient _cl = new HttpClient();

        public string ApiKey { get { return apikey; } set { apikey = value; } }
        public string BaseUrl { get { return baseurl; } set { baseurl = value; } }

        public RedmineService()
        {
            this.Project = new ProjectService(this);
            this.Issue = new IssueService(this);
            this.Tracker = new TrackerService(this);
            this.Status = new StatusService(this);
            this.User = new UserService(this);
            this.Priority = new PriorityService(this);
        }

        /// <summary>
        /// ファイルから設定を読み込む
        /// </summary>
        /// <param name="path"></param>
        public bool SetConfig( string path )
        {
            try
            {
                var xml = System.IO.File.ReadAllText(path);
                var doc = XDocument.Load(new System.IO.StringReader(xml));
                this.ApiKey = doc.Root.Element("ApiKey").Value;
                this.BaseUrl = doc.Root.Element("BaseUrl").Value;
                return true;
            }
            catch
            {
                return false;
            }
        }

        #region // ショートカットメソッド
        public async Task<bool> CreateObjectAsync( Issue item )
        {
            return await this.Issue.CreateAsync(item);
        }
        public async Task<bool> UpdateObjectAsync(Issue item)
        {
            return await this.Issue.UpdateAsync(item);
        }
        public async Task<List<Project>> GetProjects()
        {
            return await this.Project.GetListAsync();
        }
        public async Task<List<Issue>> GetIssues( int project_id = 0 )
        {
            if (project_id == 0)
                return await this.Issue.GetListAsync();
            else
                return await this.Issue.GetListAsync(project_id);
        }
        public async Task<Project> GetProject( int id )
        {
            return await this.Project.GetAsync(id);
        }
        public async Task<Issue> GetIssue(int id)
        {
            return await this.Issue.GetAsync(id);
        }
        public async Task<List<Tracker>> GetTrackers()
        {
            return await this.Tracker.GetListAsync();
        }
        public async Task<List<Status>> GetStatuses()
        {
            return await this.Status.GetListAsync();
        }
        public async Task<List<Priority>> GetPriorities()
        {
            return await this.Priority.GetListAsync();
        }
        public async Task<List<User>> GetUsers( int project_id = 0)
        {
            if (project_id == 0)
                return await this.User.GetListAsync();
            else
                return await this.User.GetListAsync(project_id);
        }
        #endregion


        /// <summary>
        /// 各サービスクラスのテンプレート
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class Service<T> where T : new() 
        {
            protected RedmineService _rs;

            protected HttpClient _cl {  get { return _rs._cl;  } }
            protected string _apikey { get { return _rs.ApiKey; } }
            protected string _baseurl { get { return _rs.BaseUrl; } }
            protected string _tablename;

            public Service(RedmineService rs, string tablename )
            {
                _rs = rs;
                _tablename = tablename;
            }

            /// <summary>
            /// new T(el) の代わりに、CreateItem を用意してオーバーライドさせる
            /// </summary>
            /// <param name="el"></param>
            /// <returns></returns>
            internal Func<XElement, T> CreateItem = (el) => { return new T(); };

            /// <summary>
            /// 全体リストを取得
            /// </summary>
            /// <returns></returns>
            public virtual async Task<List<T>> GetListAsync()
            {
                var url = $"{_baseurl}{_tablename}.xml?key={_apikey}";
                var xml = await _cl.GetStringAsync(url);
                var doc = XDocument.Load(new System.IO.StringReader(xml));
                var items = new List<T>();
                foreach (var it in doc.Root.Elements())
                {
                    items.Add(CreateItem(it));
                }
                return items;
            }

            /// <summary>
            /// 親IDを指定してリストを取得
            /// </summary>
            /// <param name="pname"></param>
            /// <param name="pid"></param>
            /// <returns></returns>
            public async Task<List<T>> GetListAsync(string pname, int pid)
            {
                var url = $"{_baseurl}{_tablename}.xml?{pname}={pid}&key={_apikey}";
                var xml = await _cl.GetStringAsync(url);
                var doc = XDocument.Load(new System.IO.StringReader(xml));
                var items = new List<T>();
                foreach (var it in doc.Root.Elements())
                {
                    items.Add(CreateItem(it));
                }
                return items;
            }
            /// <summary>
            /// IDを指定して単独のアイテムを取得
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            public virtual async Task<T> GetAsync( int id)
            {
                var url = $"{_baseurl}{_tablename}/{id}.xml?key={_apikey}";
                var xml = await _cl.GetStringAsync(url);
                var doc = XDocument.Load(new System.IO.StringReader(xml));
                var item = CreateItem(doc.Root);
                return item;
            }
            /*
            public virtual async void CreateAsync(T item)
            {

            }
            public virtual async void UpdateAsync(T item)
            {

            }
            public virtual async void RemoveAsync(int id)
            {

            }
            */
        }

        /// <summary>
        /// プロジェクト情報へアクセス
        /// </summary>
        public class ProjectService : Service<Project>
        {
            public ProjectService(RedmineService rs) : base( rs, "projects" )
            {
                this.CreateItem = el => new Model.Project(el);
            }
        }
        /// <summary>
        /// チケット情報へのアクセス
        /// </summary>
        public class IssueService : Service<Issue>
        {
            public IssueService(RedmineService rs) : base( rs, "issues" )
            {
                this.CreateItem = el => new Model.Issue(el);
            }
            /// <summary>
            /// プロジェクトIDを指定して、チケット一覧を取得
            /// </summary>
            /// <param name="pid"></param>
            /// <returns></returns>
            public Task<List<Issue>> GetListAsync( int pid )
            {
                return base.GetListAsync("project_id", pid);
            }
            /// <summary>
            /// 既存チケットを更新
            /// </summary>
            /// <param name="item"></param>
            public async Task<bool> UpdateAsync(Issue item)
            {
                var url = $"{_baseurl}{_tablename}/{item.Id}.xml?key={_apikey}";
                var xml = item.ToXml();
                var contnet = new StringContent(xml, Encoding.UTF8, "application/xml");
                var res = await _cl.PutAsync(url, contnet);
                return true;
            }
            /// <summary>
            /// 新規チケットを更新
            /// </summary>
            /// <param name="item"></param>
            public async Task<bool> CreateAsync(Issue item)
            {
                var url = $"{_baseurl}{_tablename}.xml?key={_apikey}";
                var xml = item.ToXml();
                var contnet = new StringContent(xml, Encoding.UTF8, "application/xml");
                var res = await _cl.PostAsync(url, contnet);
                return true;
            }
        }
        /// <summary>
        /// トラッカー一覧へのアクセス
        /// </summary>
        public class TrackerService : Service<Tracker>
        {
            public TrackerService(RedmineService rs) : base(rs, "trackers")
            {
                this.CreateItem = el => new Model.Tracker(el);
            }
        }
        /// <summary>
        /// ステータス一覧へのアクセス
        /// </summary>
        public class StatusService : Service<Status>
        {
            public StatusService(RedmineService rs) 
                : base( rs, "issue_statuses")
            { 
                this.CreateItem = el => new Model.Status(el);
            }
        }
        /// <summary>
        /// 優先度一覧へのアクセス
        /// </summary>
        public class PriorityService : Service<Priority>
        {
            public PriorityService(RedmineService rs)
                : base(rs, "enumerations/issue_priorities")
            {
                this.CreateItem = el => new Model.Priority(el);
            }
        }
        /// <summary>
        /// ユーザー一覧へのアクセス
        /// </summary>
        public class UserService : Service<User>
        {
            public UserService(RedmineService rs)
                : base(rs, "users")
            {
                this.CreateItem = el => new Model.User(el);
            }
            /// <summary>
            /// プロジェクトIDを指定してリストを取得
            /// </summary>
            /// <param name="pname"></param>
            /// <param name="pid"></param>
            /// <returns></returns>
            public async Task<List<User>> GetListAsync(int pid)
            {
                var url = $"{_baseurl}projects/{pid}/memberships.xml?key={_apikey}";
                var xml = await _cl.GetStringAsync(url);
                var doc = XDocument.Load(new System.IO.StringReader(xml));
                var items = new List<User>();
                foreach (var it in doc.Root.Elements())
                {
                    items.Add(new User(it));
                }
                return items;
            }
        }
    }
}
