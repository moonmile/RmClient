using Moonmile.Redmine;
using Moonmile.Redmine.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RmClient
{
    public class ViewModel : ObservableObject
    {
        private List<Project> _ItemsProject;
        private ObservableCollection<Issue> _ItemsTicket;

        public List<Project> ItemsProject { get { return _ItemsProject; } set { SetProperty(ref _ItemsProject, value, nameof(ItemsProject)); } }
        public ObservableCollection <Issue> ItemsTicket { get { return _ItemsTicket; } set { SetProperty(ref _ItemsTicket, value, nameof(ItemsTicket)); } }
        private Issue _Ticket;
        public Issue Ticket
        {
            get { return _Ticket; }
            set
            {
                SetProperty(ref _Ticket, value, nameof(Ticket));
            }
        }
        private Project _Project;
        public Project Project
        {
            get { return _Project; }
            set
            {
                SetProperty(ref _Project, value, nameof(Project));
                if (Project != null)
                    this.GetTickets(Project.Id);
            }
        }

        RedmineService _sv;

        public ViewModel()
        {
            _sv = new RedmineService();
            _sv.SetConfig(AppDomain.CurrentDomain.BaseDirectory + "RmClient.config");
            Init();

        }
        public async void Init()
        {
            GetProjects();
            this.Trackers = await _sv.Tracker.GetListAsync();
            this.Statuses = await _sv.Status.GetListAsync();
            this.Priorities = await _sv.Priority.GetListAsync();

        }
        public async void GetProjects()
        {
            var items = await _sv.Project.GetListAsync();
            this.ItemsProject = items;
        }
        public async void GetTickets(int pid)
        {
            var items = await _sv.Issue.GetListAsync(pid);
            // TODO: プロジェクト内の membership のみ取得する
            this.Users = await _sv.User.GetListAsync(pid);
            // this.Users.AddRange(await _sv.User.GetListAsync());
            this.Ticket = null;
            this.ItemsTicket = new ObservableCollection<Issue>( items );
        }
        public async void GetTicket(int id)
        {
            var item = await _sv.Issue.GetAsync(id);
            this.Ticket = item;
        }

        /// <summary>
        /// チケットを更新
        /// </summary>
        public async void UpdateTicket()
        {
            if (this.Ticket.Id == 0)
            {
                await _sv.Issue.CreateAsync(this.Ticket);
                // チケットIDを再取得する
                var items = await _sv.Issue.GetListAsync(this.Project.Id);
                // 多分、最初のひとつが更新したもの
                if ( items.Count > 0 )
                {
                    this.Ticket.Id = items[0].Id;
                    this.Ticket.UpdatedOn = items[0].UpdatedOn;
                }
                this.ItemsTicket.Add(this.Ticket);
            }
            else
            {
                await _sv.Issue.UpdateAsync(this.Ticket);
            }
        }

        /// <summary>
        /// プロジェクトを指定してチケットを新規作成
        /// </summary>
        /// <returns></returns>
        public Issue CreateTicket()
        {
            var item = new Issue();
            item.Project = new Project { Id = this.Project.Id };
            item.Tracker = this.Trackers.First(t => t.Name == "機能");
            item.Status = this.Statuses.First(t => t.Name == "新規");
            item.Priority = this.Priorities.First(t => t.Name == "通常");
            this.Ticket = item;
            return item;
        }

        // コンボボックス用のリストを取得
        private List<Tracker> _Trackers;
        public List<Tracker> Trackers { get { return _Trackers; } set { SetProperty(ref _Trackers, value, nameof(Trackers)); } }
        private List<Status> _Statuses;
        public List<Status> Statuses { get { return _Statuses; } set { SetProperty(ref _Statuses, value, nameof(Statuses)); } }
        private List<Priority> _Priorities;
        public List<Priority> Priorities { get { return _Priorities; } set { SetProperty(ref _Priorities, value, nameof(Priorities)); } }
        private List<User> _Users;
        public List<User> Users { get { return _Users; } set { SetProperty(ref _Users, value, nameof(Users)); } }


    }
    public class ObservableObject : INotifyPropertyChanged
    {
        protected bool SetProperty<T>(
            ref T backingStore, T value,
            [CallerMemberName]string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
