using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Butler.Annotations;

namespace Butler {

    public class Logger: INotifyPropertyChanged {

        // SINGLETON
        private Logger() { }
        private static Logger _instance;
        public static Logger Instance => (_instance = _instance ?? new Logger());

        public HashSet<Log> Logs { get; private set; } = new HashSet<Log>();

        public static HashSet<Log> LogSet => Instance.Logs;

        public static void Log(Log log) { Instance.Logs.Add(log); Instance.OnPropertyChanged(nameof(Logs)); }
        public static void Log(string description, Exception exc = null) {
            Instance.Logs.Add(new Log(description, exc));
            Instance.OnPropertyChanged(nameof(Logs));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class Log {

        public DateTime DateTime { get; private set; }
        public Exception Exception { get; private set; }
        public string Description { get; private set; }

        public Log(string description, Exception exc = null) {
            DateTime = DateTime.Now;
            Description = description;
            Exception = exc;
        }

        public override string ToString() {
            return $"[{DateTime:T}] {Description}{(Exception?.InnerException?.ToString() ?? Exception?.Message?.ToString() ?? "")}";
            //return $"[{DateTime:T}] {Description}{(Exception == null ? "" : $" [{Exception.Message}]")}";
        }
    }

}
