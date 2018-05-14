using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Moonmile.Redmine.Model
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Identifier { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
        public bool IsPublic { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }

        public Project() { }
        public Project(XElement it)
        {
            this.Id = it.Element("id").ToInt();
            this.Name = it.Element("name").Value;
            this.Identifier = it.Element("identifier").Value;
            this.Description = it.Element("description").Value;
            this.Status = it.Element("status").ToInt();
            this.IsPublic = it.Element("is_public").ToBool();
            this.CreatedOn = it.Element("created_on").ToDatetime();
            this.UpdatedOn = it.Element("updated_on").ToDatetime();
        }
    }
    public class Issue
    {
        public int Id { get; set; }
        public Project Project { get; set; }
        public Tracker Tracker { get; set; }
        public Status Status { get; set; }
        public Priority Priority { get; set; }
        public User Author { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? DueDate { get; set; }
        public int DoneRatio { get; set; }
        public bool IsPrivate { get; set; }
        public int? EstimatedHours { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public DateTime? ClosedOn { get; set; }

        public Issue() {
            this.Project = new Project();
            this.Tracker = new Tracker();
            this.Status = new Status();
            this.Priority = new Priority();
            this.Author = new User();

        }
        public Issue(XElement it)
        {
            this.Id = it.Element("id").ToInt();
            this.Project = new Project() { Id = it.Element("project", "id").ToInt(), Name = it.Element("project", "name").Value };
            this.Tracker = new Tracker() { Id = it.Element("tracker", "id").ToInt(), Name = it.Element("tracker", "name").Value };
            this.Status = new Status() { Id = it.Element("status", "id").ToInt(), Name = it.Element("status", "name").Value };
            this.Priority = new Priority() { Id = it.Element("priority", "id").ToInt(), Name = it.Element("priority", "name").Value };
            this.Author = new User() { Id = it.Element("author", "id").ToInt(), Name = it.Element("author", "name").Value };
            this.Subject = it.Element("subject").Value;
            this.Description = it.Element("description").Value;
            this.StartDate = it.Element("start_date").ToDatetime();
            this.DueDate = it.Element("due_date").ToDatetimeOrNull();
            this.DoneRatio = it.Element("done_ratio").ToInt();
            this.IsPrivate = it.Element("is_private").ToBool();
            this.EstimatedHours = it.Element("estimated_hours").ToInt();
            this.CreatedOn = it.Element("created_on").ToDatetime();
            this.UpdatedOn = it.Element("updated_on").ToDatetime();
            this.ClosedOn = it.Element("closed_on").ToDatetimeOrNull();
        }
        public string ToXml()
        {
            var root = new XElement(XName.Get("issue"));
            root.Add(new XElement(XName.Get("project_id"), this.Project.Id));
            root.Add(new XElement(XName.Get("tracker_id"), this.Tracker.Id));
            root.Add(new XElement(XName.Get("status_id"), this.Status.Id));
            root.Add(new XElement(XName.Get("priority_id"), this.Priority.Id));
            root.Add(new XElement(XName.Get("subject"), this.Subject));
            root.Add(new XElement(XName.Get("description"), this.Description));
            return root.ToString();
        }
    }

    public class Tracker
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Tracker() { }
        public Tracker(XElement it)
        {
            this.Id = it.Element("id").ToInt();
            this.Name = it.Element("name").Value;
        }
    }
    public class Status
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Status() { }
        public Status(XElement it)
        {
            this.Id = it.Element("id").ToInt();
            this.Name = it.Element("name").Value;
        }
    }
    public class Priority
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Priority() { }
        public Priority(XElement it)
        {
            this.Id = it.Element("id").ToInt();
            this.Name = it.Element("name").Value;
        }
    }
    public class User
    {
        public int Id { get; set; }
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

    }
}
