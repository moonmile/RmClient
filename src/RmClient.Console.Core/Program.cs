using System;
using System.Threading.Tasks;
using System.Linq;

namespace Moonmile.RmClient
{
    class Program
    {
        static void Main(string[] args)
        {
            // args = new string[] { "GET", "60" };

            if (args.Length == 0)
            {
                Console.WriteLine(@"
RmClient コンソールツール
  Get <issue_id>
  Post <filename>
");
                return;
            }

            var app = new Program();
            if (args.Length == 2 && args[0].ToUpper() == "GET")
            {
                var b = app.GetTicket(int.Parse(args[1])).Result;
                return;
            }
            if (args.Length == 2 && args[0].ToUpper() == "POST")
            {
                var b = app.PostTicket(args[1]).Result;
                return;
            }
        }

        public Program()
        {
            _sv = new Moonmile.Redmine.RedmineService();
            _sv.SetConfig(AppDomain.CurrentDomain.BaseDirectory + "RmClient.config");
        }
        Moonmile.Redmine.RedmineService _sv;

        /*フォーマット
～～～
プロジェクトID: 	    1	
チケットID:			2
トラッカー:			機能
ステータス:			新規
優先度:				通常
期日:				
進捗率:				0
題名: コンソールから投稿				

コンソールから投稿できるようにする
～～～
         *  
         * 
         * 
         * 
         * 
         */

        public async Task<bool> PostTicket(string path)
        {
            // 1. ファイルから Issue オブジェクトの作成
            // var sr = new System.IO.StreamReader(path, System.Text.Encoding.GetEncoding("shift_jis"));
            var sr = new System.IO.StreamReader(path, System.Text.Encoding.Default);
            var issue = new Moonmile.Redmine.Model.Issue();


            while (!sr.EndOfStream)
            {
                var line = sr.ReadLine();
                if (line == "" || line.StartsWith("-")) break;
                var s = getv(line);
                if (line.StartsWith("プロジェクト") || line.StartsWith("Project"))
                {
                    // 数値であれば Project.Id にいれる
                    // 文字列であれば Project.Identifier に入れる
                    int i = 0;
                    if (int.TryParse(s, out i) == true)
                    {
                        issue.Project.Id = i;
                    }
                    else
                    {
                        issue.Project.Identifier = s;
                    }
                }
                else if (line.StartsWith("チケット") || line.StartsWith("Ticket") || line.StartsWith("Issue"))
                {
                    issue.Id = int.Parse(s);
                }
                else if (line.StartsWith("トラッカー") || line.StartsWith("Tracker"))
                {
                    int i = 0;
                    if (int.TryParse(s, out i) == true)
                    {
                        issue.Tracker = new Redmine.Model.Tracker() { Id = i };
                    }
                    else
                    {
                        var items = await _sv.Tracker.GetListAsync();
                        var tr = items.FirstOrDefault(t => t.Name == s);
                        if (tr != null)
                            issue.Tracker = tr;
                    }
                }
                else if (line.StartsWith("ステータス") || line.StartsWith("Status"))
                {
                    int i = 0;
                    if (int.TryParse(s, out i) == true)
                    {
                        issue.Status = new Redmine.Model.Status() { Id = i };
                    }
                    else
                    {
                        var items = await _sv.Status.GetListAsync();
                        if (s == "完了") s = "終了";
                        var st = items.FirstOrDefault(t => t.Name == s);
                        if (st != null)
                            issue.Status = st;
                    }

                }
                else if (line.StartsWith("優先度") || line.StartsWith("Priority"))
                {
                    int i = 0;
                    if (int.TryParse(s, out i) == true)
                    {
                        issue.Priority = new Redmine.Model.Priority() { Id = i };
                    }
                    else
                    {
                        var items = await _sv.Priority.GetListAsync();
                        var pr = items.FirstOrDefault(t => t.Name == s);
                        if (pr != null)
                            issue.Priority = pr;
                    }
                }
                else if (line.StartsWith("進捗率") || line.StartsWith("DoneRatio"))
                {
                    int i = 0;
                    if (int.TryParse(s, out i) == true)
                    {
                        issue.DoneRatio = i;
                    }
                }
                else if (line.StartsWith("題名") || line.StartsWith("Subject"))
                {
                    issue.Subject = s;
                }
            }
            // 説明を読み込み
            while ( !sr.EndOfStream)
            {
                var line = sr.ReadLine();
                issue.Description += line + "\n";
            }
            sr.Close();
            bool res = false;
            if (issue.Id == 0)
            {
                // 2.1 ID が空白の場合は新規チケット
                res = await _sv.Issue.CreateAsync(issue);

            }
            else
            {
                // 2.2 ID が指定されている場合は更新チケット
                res = await _sv.Issue.UpdateAsync(issue);
            }
            return res;
        }
        private string getv( string line )
        {
            if (line.IndexOf(' ') > 0) line = line.Substring(line.IndexOf(' ') + 1).Trim();
            if (line.IndexOf('\t') > 0) line = line.Substring(line.IndexOf('\t') + 1).Trim();
            return line;
        }

        /// <summary>
        /// チケットIDを指定して、既存のチケットを取得
        /// </summary>
        /// <param name="id"></param>
        public async Task<bool> GetTicket(int id)
        {
            // 既存のチケットを取得
            var t = await _sv.Issue.GetAsync(id);
            // コンソールに出力
            Console.WriteLine($"プロジェクトID: {t.Project.Id}");
            Console.WriteLine($"チケットID: {t.Id}");
            Console.WriteLine($"トラッカー: {t.Tracker.Name}");
            Console.WriteLine($"ステータス: {t.Status.Name}");
            Console.WriteLine($"優先度: {t.Priority.Name}");
            if (t.DueDate.HasValue)
                Console.WriteLine($"期日: " + t.DueDate.Value.ToString("yyyy/MM/dd"));
            Console.WriteLine($"進捗率: {t.DoneRatio}");
            Console.WriteLine($"更新日時: " + t.UpdatedOn.ToString("yyyy/MM/dd HH:mm"));
            Console.WriteLine($"題名: {t.Subject}");
            Console.WriteLine($"");
            Console.WriteLine(t.Description);

            return true;
        }
    }
}
