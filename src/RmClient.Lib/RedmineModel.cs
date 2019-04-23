using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Moonmile.Redmine.Model
{
    public class Project
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("identifier")]
        public string Identifier { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("status")]
        public int Status { get; set; }
        [JsonProperty("created_on")]
        public DateTime CreatedOn { get; set; }
        [JsonProperty("updated_on")]
        public DateTime UpdatedOn { get; set; }
        [JsonProperty("is_public")]
        public bool IsPublic { get; set; }
    }

    public class Issue
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("project")]
        public Project Project { get; set; }
        [JsonProperty("tracker")]
        public Tracker Tracker { get; set; }
        [JsonProperty("status")]
        public Status Status { get; set; }
        [JsonProperty("priority")]
        public Priority Priority { get; set; }
        [JsonProperty("author")]
        public User Author { get; set; }
        [JsonProperty("subject")]
        public string Subject { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("start_date")]
        public DateTime? StartDate { get; set; }
        [JsonProperty("done_ratio")]
        public int DoneRatio { get; set; }
        [JsonProperty("created_on")]
        public DateTime CreatedOn { get; set; }
        [JsonProperty("updated_on")]
        public DateTime UpdatedOn { get; set; }
        [JsonProperty("assigned_to")]
        public AssignedTo AssignedTo { get; set; }
        [JsonProperty("category")]
        public Category Category { get; set; }
        [JsonProperty("due_date")]
        public DateTime? DueDate { get; set; }
        [JsonProperty("attachments")]
        public Attachment[] Attachments { get; set; }
    }

    class IssueUpdate
    {
        public IssueUpdate(Issue it)
        {
            this.subject = it.Subject;
            this.description = it.Description;
            this.project_id = it.Project?.Id ?? 0;
            this.priority_id = it.Priority?.Id ?? 0;
            this.tracker_id = it.Tracker?.Id ?? 0;
            this.status_id = it.Status?.Id ?? 0;
            this.assigned_to_id = it.AssignedTo?.Id ?? 0;
            this.start_date = it.StartDate == null ? "" : it.StartDate.Value.ToString("yyyy-MM-dd");
            this.done_ratio = it.DoneRatio;
            this.due_date = it.DueDate == null ? "" : it.DueDate.Value.ToString("yyyy-MM-dd");
            /*
            this.author_id = it.Author?.Id ?? 0;
            this.created_on = it.CreatedOn;
            this.updated_on = it.UpdatedOn;
            this.category_id = it.Category?.Id ?? 0;
            */
        }
        public string subject { get; }
        public string description { get; }
        public int project_id { get; }
        public int tracker_id { get; }
        public int status_id { get; }
        public int priority_id { get; }
        public int assigned_to_id { get; }
        public int done_ratio { get; }
        public string start_date { get; }
        public string due_date { get; }
        /*
        public int author_id { get; }
        public int category_id { get; }
        */

        public string ToJson()
        {
            var json = JsonConvert.SerializeObject(this);
            json = $"{{ \"issue\": {json} }}";
            return json;
        }
    }
    class IssueUpload
    {
        public string token { get; }
        public string filename { get; }
        public string content_type { get; }

        public IssueUpload( string token, string filename, string content_type )
        {
            this.token = token;
            this.filename = filename;
            this.content_type = content_type;
        }
        public string ToJson()
        {
            var json = JsonConvert.SerializeObject(this);
            json = @"{ ""issue"": { ""uploads"" : [ " + json + " ] }}";
            return json;
        }
    }


    public class Tracker
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class Status
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class Priority
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class User
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }




    public class Membership
    {
        public int id { get; set; }
        public Project project { get; set; }
        public User user { get; set; }
        public Role[] roles { get; set; }
    }


    public class Role
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }



    public class AssignedTo
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class Category
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }



    public class Attachment
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("filename")]
        public string Filename { get; set; }
        [JsonProperty("filesize")]
        public int Filesize { get; set; }
        [JsonProperty("content_type")]
        public string ContentType { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("content_url")]
        public string ContentUrl { get; set; }
        [JsonProperty("thumbnail_url")]
        public string ThumbnailUrl { get; set; }
        [JsonProperty("author")]
        public User Author { get; set; }
        [JsonProperty("created_on")]
        public DateTime CreatedOn { get; set; }
    }



    #region JSON形式受信のルートクラス
    public interface IRootItems<T>
    {
        List<T> Items { get; }
    }

    public class RootProject : IRootItems<Project>
    {
        public Project[] projects { get; set; }
        public int total_count { get; set; }
        public int offset { get; set; }
        public int limit { get; set; }
        public List<Project> Items { get => this.projects.ToList(); }
    }
    public class RootIssue : IRootItems<Issue>
    {
        public Issue[] issues { get; set; }
        public int total_count { get; set; }
        public int offset { get; set; }
        public int limit { get; set; }
        public List<Issue> Items { get => this.issues.ToList(); }
    }
    public class RootUser : IRootItems<User>
    {
        [JsonProperty("memberships")]
        public User[] users { get; set; }
        public int total_count { get; set; }
        public int offset { get; set; }
        public int limit { get; set; }
        public List<User> Items { get => this.users.ToList(); }
    }
    public class RootTracker : IRootItems<Tracker>
    {
        public Tracker[] trackers { get; set; }
        public int total_count { get; set; }
        public int offset { get; set; }
        public int limit { get; set; }
        public List<Tracker> Items { get => this.trackers.ToList(); }
    }

    public class RootStatus : IRootItems<Status>
    {
        [JsonProperty("issue_statuses")]
        public Status[] statuses { get; set; }
        public int total_count { get; set; }
        public int offset { get; set; }
        public int limit { get; set; }
        public List<Status> Items { get => this.statuses.ToList(); }
    }

    public class RootPriorty : IRootItems<Priority>
    {
        [JsonProperty("issue_priorities")]
        public Priority[] priorities { get; set; }
        public int total_count { get; set; }
        public int offset { get; set; }
        public int limit { get; set; }
        public List<Priority> Items { get => this.priorities.ToList(); }
    }
    public class RootMembership : IRootItems<Membership>
    {
        public Membership[] memberships { get; set; }
        public int total_count { get; set; }
        public int offset { get; set; }
        public int limit { get; set; }
        public List<Membership> Items => this.memberships.ToList();
    }
    #endregion

}
