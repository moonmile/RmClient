﻿using Moonmile.Redmine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

        public string ApiKey
        {
            get { return apikey; }
            set
            {
                apikey = value;
                // APIキーをヘッダに追加
                _cl.DefaultRequestHeaders.Add("X-Redmine-API-Key", apikey);
            }
        }

        public string BaseUrl { get { return baseurl; } set { baseurl = value; } }
        public string AuthBasicUsername { get; set; }
        public string authBasicPassword { get; set; }



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
                // APIキーをヘッダに追加
                // _cl.DefaultRequestHeaders.Add("X-Redmine-API-Key", this.ApiKey);

                if ( doc.Root.Element("Authorization") != null )
                {
                    // BASIC認証を設定
                    this.AuthBasicUsername = doc.Root.Element("Authorization").Element("username").Value;
                    this.authBasicPassword = doc.Root.Element("Authorization").Element("password").Value;
                    var byteArray = Encoding.ASCII.GetBytes(string.Format("{0}:{1}", AuthBasicUsername, authBasicPassword));
                    _cl.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        #region // ショートカットメソッド
        public async Task<bool> AddAsync( Issue item )
        {
            return await this.Issue.CreateAsync(item);
        }
        public async Task<bool> UpdateAsync(Issue item)
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
        public class Service<RootT, T> where RootT : IRootItems<T> 
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
            /// 全体リストを取得
            /// </summary>
            /// <returns></returns>
            public virtual async Task<List<T>> GetListAsync()
            {
                // var url = $"{_baseurl}{_tablename}.json?key={_apikey}";
                var url = $"{_baseurl}{_tablename}.json";
                var json = await _cl.GetStringAsync(url);
                var root = JsonConvert.DeserializeObject<RootT>(json);
                return root.Items;
            }

            /// <summary>
            /// 親IDを指定してリストを取得
            /// </summary>
            /// <param name="pname"></param>
            /// <param name="pid"></param>
            /// <returns></returns>
            public async Task<List<T>> GetListAsync(string pname, int pid)
            {
                var url = $"{_baseurl}{_tablename}.json?{pname}={pid}";
                var json = await _cl.GetStringAsync(url);
                var root = JsonConvert.DeserializeObject<RootT>(json);
                return root.Items;
            }
            /// <summary>
            /// IDを指定して単独のアイテムを取得
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            public virtual async Task<T> GetAsync( int id)
            {
                var url = $"{_baseurl}{_tablename}/{id}.json";
                var json = await _cl.GetStringAsync(url);
                var item = JsonConvert.DeserializeObject<T>(json);
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
        public class ProjectService : Service<RootProject, Project>
        {
            public ProjectService(RedmineService rs) : base( rs, "projects" )
            {
            }
        }
        /// <summary>
        /// チケット情報へのアクセス
        /// </summary>
        public class IssueService : Service<RootIssue, Issue>
        {
            public IssueService(RedmineService rs) : base(rs, "issues")
            {
            }
            /// <summary>
            /// プロジェクトIDを指定して、チケット一覧を取得
            /// </summary>
            /// <param name="pid"></param>
            /// <returns></returns>
            public Task<List<Issue>> GetListAsync(int pid)
            {
                return base.GetListAsync("project_id", pid);
            }
            /// <summary>
            /// 既存チケットを更新
            /// </summary>
            /// <param name="item"></param>
            public async Task<bool> UpdateAsync(Issue item)
            {
                var url = $"{_baseurl}{_tablename}/{item.Id}.json";
                var json = new IssueUpdate(item).ToJson();
                var contnet = new StringContent(json, Encoding.UTF8, "application/json");
                var res = await _cl.PutAsync(url, contnet);

                return true;
            }
            /// <summary>
            /// 新規チケットを更新
            /// </summary>
            /// <param name="item"></param>
            public async Task<bool> CreateAsync(Issue item)
            {
                var url = $"{_baseurl}{_tablename}.json";
                var json = new IssueUpdate(item).ToJson();
                var contnet = new StringContent(json, Encoding.UTF8, "application/json");
                var res = await _cl.PostAsync(url, contnet);
                return true;
            }

            /// <summary>
            /// 添付ファイルを追加
            /// </summary>
            /// <param name="path"></param>
            /// <returns></returns>
            public async Task<bool> UploadFile(int id, string path)
            {
                var filename = System.IO.Path.GetFileName(path);
                var url = $"{_baseurl}uploads.json?filename={filename}";
                var contnet = new System.Net.Http.StreamContent(System.IO.File.OpenRead(path));
                contnet.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                var res = await _cl.PostAsync(url, contnet);
                var json = await res.Content.ReadAsStringAsync();
                JObject o = JsonConvert.DeserializeObject(json) as JObject;
                // トークンを取得
                string token = o.Value<JObject>("upload").Value<string>("token");

                string contentType = "text/plain";
                switch (System.IO.Path.GetExtension(path).ToLower())
                {
                    case ".txt": contentType = "text/plain"; break;
                    case ".bmp": contentType = "image/bmp"; break;
                    case ".jpg": contentType = "image/jpeg"; break;
                    case ".png": contentType = "image/png"; break;
                    case ".pdf": contentType = "application/pdf"; break;
                    case ".doc": contentType = "application/word"; break;
                    case ".docx": contentType = "application/word"; break;
                    case ".xls": contentType = "application/excel"; break;
                    case ".xlsx": contentType = "application/excel"; break;
                    case ".zip": contentType = "application/zip"; break;
                    default: contentType = "text/plain"; break;
                }
                url = $"{_baseurl}{_tablename}/{id}.json";
                json = new IssueUpload(token, filename, contentType).ToJson();
                var contnet2 = new StringContent(json, Encoding.UTF8, "application/json");
                res = await _cl.PutAsync(url, contnet2);

                System.Diagnostics.Debug.WriteLine(json);
                return true;
            }
        }
        /// <summary>
        /// トラッカー一覧へのアクセス
        /// </summary>
        public class TrackerService : Service<RootTracker, Tracker>
        {
            public TrackerService(RedmineService rs) : base(rs, "trackers")
            {
            }
        }
        /// <summary>
        /// ステータス一覧へのアクセス
        /// </summary>
        public class StatusService : Service<RootStatus, Status>
        {
            public StatusService(RedmineService rs) 
                : base( rs, "issue_statuses")
            { 
            }
        }
        /// <summary>
        /// 優先度一覧へのアクセス
        /// </summary>
        public class PriorityService : Service<RootPriorty,Priority>
        {
            public PriorityService(RedmineService rs)
                : base(rs, "enumerations/issue_priorities")
            {
            }
        }
        /// <summary>
        /// ユーザー一覧へのアクセス
        /// </summary>
        public class UserService : Service<RootUser, User>
        {
            public UserService(RedmineService rs)
                : base(rs, "users")
            {
            }
            /// <summary>
            /// プロジェクトIDを指定してリストを取得
            /// </summary>
            /// <param name="pname"></param>
            /// <param name="pid"></param>
            /// <returns></returns>
            public async Task<List<User>> GetListAsync(int pid)
            {
                var url = $"{_baseurl}projects/{pid}/memberships.json";
                var json = await _cl.GetStringAsync(url);
                var root = JsonConvert.DeserializeObject<RootMembership>(json);
                var items = new List<User>();
                foreach( var it in root.memberships )
                {
                    items.Add(it.user);
                }
                return items;
            }
        }
    }
}
