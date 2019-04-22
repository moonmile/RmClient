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
    public interface IItems<T>
    {
        List<T> Items { get; }
    }

    public class Projects : IItems<Project>
    {
        public Project[] projects { get; set; }
        public int total_count { get; set; }
        public int offset { get; set; }
        public int limit { get; set; }
        public List<Project> Items { get => this.projects.ToList(); }
    }

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

    public class Issues : IItems<Issue>
    {
        public Issue[] issues { get; set; }
        public int total_count { get; set; }
        public int offset { get; set; }
        public int limit { get; set; }
        public List<Issue> Items { get => this.issues.ToList(); }
    }

    public class Users : IItems<User>
    {
        [JsonProperty("memberships")]
        public User[] users { get; set; }
        public int total_count { get; set; }
        public int offset { get; set; }
        public int limit { get; set; }
        public List<User> Items { get => this.users.ToList(); }
    }

    public class Trackers : IItems<Tracker>
    {
        public Tracker[] trackers { get; set; }
        public int total_count { get; set; }
        public int offset { get; set; }
        public int limit { get; set; }
        public List<Tracker> Items { get => this.trackers.ToList(); }
    }

    public class Statuses : IItems<Status>
    {
        [JsonProperty("issue_statuses")]
        public Status[] statuses { get; set; }
        public int total_count { get; set; }
        public int offset { get; set; }
        public int limit { get; set; }
        public List<Status> Items { get => this.statuses.ToList(); }
    }

    public class Priorities : IItems<Priority>
    {
        [JsonProperty("issue_priorities")]
        public Priority[] priorities { get; set; }
        public int total_count { get; set; }
        public int offset { get; set; }
        public int limit { get; set; }
        public List<Priority> Items { get => this.priorities.ToList(); }
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
    }

    public class IssueUpdate
    {
        public IssueUpdate( Issue it )
        {
            this.subject = it.Subject;
            this.description = it.Description;
            this.priority_id = it.Project?.Id ?? 0;
            this.tracker_id = it.Tracker?.Id ?? 0;
            this.status_id = it.Status?.Id ?? 0;

            this.assigned_to_id = it.AssignedTo?.Id ?? 0;
            this.start_date = it.StartDate;
            this.done_ratio = it.DoneRatio;
            this.due_date = it.DueDate;

            /*
            this.id = it.Id;
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
        public DateTime? start_date { get; }
        public DateTime? due_date { get; }
        /*
        public int id { get; }
        public int author_id { get; }
        public DateTime created_on { get; }
        public DateTime updated_on { get; }
        public int category_id { get; }
        */
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



    public class Memberships : IItems<Membership>
    {
        public Membership[] memberships { get; set; }
        public int total_count { get; set; }
        public int offset { get; set; }
        public int limit { get; set; }
        public List<Membership> Items => this.memberships.ToList();
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



#if false
    public class User
    {
        [XmlElement("id")]
        public int Id { get; set; }
        [XmlElement("name")]
        public string Name { get; set; }
        public User() { }
        public User(XElement it)
        {
            this.Id = it.Element("id").ToInt();
            if (it.Element("name") != null)
            {
                this.Name = it.Element("name").Value;
            }
            else if (it.Element("firstname") != null)
            {
                this.Name = it.Element("firstname").Value + " " + it.Element("lastname").Value;
            }
            else if (it.Element("user") != null)
            {
                this.Id = it.Element("user", "id").ToInt();
                this.Name = it.Element("user", "name").Value;
            }
        }
    }
#endif
    /// <summary>
    /// XElementアクセス用の拡張メソッド
    /// </summary>
    internal static class XElementExtentions
    {
        public static XElement Element(this XElement el, string name)
        {
            return el.Element(XName.Get(name));
        }
        public static XAttribute Element(this XElement el, string name, string attr)
        {
            return el.Element(XName.Get(name)).Attribute(XName.Get(attr));
        }
        public static int ToInt(this XElement el)
        {
            if (string.IsNullOrEmpty(el.Value))
                return 0;

            return int.Parse(el.Value);
        }
        public static bool ToBool(this XElement el)
        {
            return bool.Parse(el.Value);
        }
        public static DateTime ToDatetime(this XElement el)
        {
            return DateTime.Parse(el.Value);
        }
        public static DateTime? ToDatetimeOrNull(this XElement el)
        {
            if (el.Value == "")
                return null;
            return DateTime.Parse(el.Value);
        }


        public static int ToInt(this XAttribute el)
        {
            return int.Parse(el.Value);
        }
        public static bool ToBool(this XAttribute el)
        {
            return bool.Parse(el.Value);
        }
        public static DateTime ToDatetime(this XAttribute el)
        {
            return DateTime.Parse(el.Value);
        }
        public static DateTime? ToDatetimeOrNull(this XAttribute el)
        {
            if (el.Value == "")
                return null;
            return DateTime.Parse(el.Value);
        }


        public static void ToObject(this XElement root, object o)
        {
            var pis = o.GetType().GetProperties();
            foreach (var pi in pis)
            {
                var at = pi.GetCustomAttribute(typeof(XmlElementAttribute)) as XmlElementAttribute;
                if (at != null)
                {
                    var elName = at.ElementName;
                    var el = root.Element(elName);
                    var attr = root.Attribute(elName);
                    if (el != null)
                    {
                        try
                        {
                            pi.SetValue(o,
                            pi.PropertyType == typeof(int) ? (object)el.ToInt() :
                            pi.PropertyType == typeof(string) ? (object)el.Value :
                            pi.PropertyType == typeof(bool) ? (object)el.ToBool() :
                            pi.PropertyType == typeof(DateTime) ? (object)el.ToDatetime() :
                            pi.PropertyType == typeof(DateTime?) ? (object)el.ToDatetimeOrNull() :
                            null);
                        }
                        catch { }
                    }
                    else if (attr != null)
                    {
                        try
                        {
                            pi.SetValue(o,
                            pi.PropertyType == typeof(int) ? (object)attr.ToInt() :
                            pi.PropertyType == typeof(string) ? (object)attr.Value :
                            pi.PropertyType == typeof(bool) ? (object)attr.ToBool() :
                            pi.PropertyType == typeof(DateTime) ? (object)attr.ToDatetime() :
                            pi.PropertyType == typeof(DateTime?) ? (object)attr.ToDatetimeOrNull() :
                            null);
                        }
                        catch { }
                    }
                }
            }
        }
    }
}
