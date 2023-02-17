using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Sockets;
using LitJSON;
using QLTK.Models;

namespace QLTK.Models
{
    public class Account : INotifyPropertyChanged
    {

        int _id = 0;
        string _username = string.Empty;
        string _password = string.Empty;
        int _indexServer = 0;
        int _gem = -1;

        [JsonSkip]
        string _status = "-";

        [JsonSkip]
        public Process process;

        [JsonSkip]
        public Socket socket = null;

        [JsonSkip]
        public Server server => Server.servers[_indexServer];

        public string proxy = null;

        #region
        public int gem
        {
            get => _gem;
            set { _gem = value; OnPropertyChanged("gem"); }
        }
        public int id
        {
            get => _id;
            set { _id = value; OnPropertyChanged("id"); }
        }
        public string username
        {
            get => _username;
            set { _username = value; OnPropertyChanged("username"); }
        }
        public string password
        {
            get => _password;
            set { _password = value; OnPropertyChanged("password"); }
        }
        public int index_server
        {
            get
            {
                return _indexServer;
            }
            set
            {
                _indexServer = value;
                OnPropertyChanged("indexServer");
                OnPropertyChanged("server");
            }
        }
        public string status
        {
            get => _status;
            set { _status = value; OnPropertyChanged("status"); }
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
