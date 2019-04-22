using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Reflection;

namespace Moonmile.Redmine.Model
{
    public class Project
    {
        [XmlElement("id")]
        public int Id { get; set; }
        [XmlElement("name")]
        public string Name { get; set; }
        [XmlElement("identifier")]
        public string Identifier { get; set; }
        [XmlElement("description")]
        public string Description { get; set; }
        [XmlElement("status")]
        public int Status { get; set; }
        [XmlElement("is_public")]
        public bool IsPublic { get; set; }
        [XmlElement("created_on")]
        public DateTime CreatedOn { get; set; }
        [XmlElement("updated_on")]
        public DateTime UpdatedOn { get; set; }

        public Project() { }
        public Project(XElement el) { el.ToObject(this); }

    }
    public class Issue
    {
        [XmlElement("id")]
        public int Id { get; set; }
        public Project Project { get; set; }
        public Tracker Tracker { get; set; }
        public Status Status { get; set; }
        public Priority Priority { get; set; }
        public User Author { get; set; }
        [XmlElement("subject")]
        public string Subject { get; set; }
        [XmlElement("description")]
        public string Description { get; set; }
        [XmlElement("start_date")]
        public DateTime? StartDate { get; set; }
        [XmlElement("due_date")]
        public DateTime? DueDate { get; set; }
        [XmlElement("done_ratio")]
        public int DoneRatio { get; set; }
        [XmlElement("is_private")]
        public bool IsPrivate { get; set; }
        [XmlElement("estimated_hours")]
        public int? EstimatedHours { get; set; }
        [XmlElement("created_on")]
        public DateTime CreatedOn { get; set; }
        [XmlElement("updated_on")]
        public DateTime UpdatedOn { get; set; }
        [XmlElement("closed_on")]
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
            this.Project = new Project();
            this.Tracker = new Tracker();
            this.Status = new Status();
            this.Priority = new Priority();
            this.Author = new User();
            it.Element("project").ToObject(this.Project);
            it.Element("tracker").ToObject(this.Tracker);
            it.Element("status").ToObject(this.Status);
            it.Element("priority").ToObject(this.Priority);
            it.Element("author").ToObject(this.Author);
            it.ToObject(this);
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
        [XmlElement("id")]
        public int Id { get; set; }
        [XmlElement("name")]
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
        [XmlElement("id")]
        public int Id { get; set; }
        [XmlElement("name")]
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
        [XmlElement("id")]
        public int Id { get; set; }
        [XmlElement("name")]
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
                    else if ( attr != null )
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
