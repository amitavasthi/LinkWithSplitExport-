using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ApplicationUtilities
{
    public class TaskCollection
    {
        #region Properties

        public bool Synchronously { get; set; }

        public List<Task2> Tasks { get; set; }

        public CancellationTokenSource CancellationToken { get; set; }

        #endregion


        #region Constructor

        public TaskCollection(bool synchronously = false)
        {
            this.Synchronously = synchronously;
            //this.Synchronously = false;

            this.CancellationToken = new CancellationTokenSource();

            this.Tasks = new List<Task2>();
        }

        #endregion


        #region Methods

        public void Add(Action action)
        {
            this.Tasks.Add(new Task2(action, this.CancellationToken.Token, !this.Synchronously));
        }

        public void WaitAll(int timeout = 60000)
        {
            try
            {
                if (!this.Synchronously)
                {
                    System.Timers.Timer timer = new System.Timers.Timer(timeout);
                    timer.Elapsed += Timer_Elapsed;
                    timer.Start();

                    Task.WaitAll(this.Tasks.ToArray(), this.CancellationToken.Token);

                    foreach (Task task in this.Tasks)
                    {
                        try {
                            task.Dispose();
                        }
                        catch { }
                    }
                }
                else
                {
                    foreach (Task2 task in this.Tasks)
                    {
                        task.RunSynchronously();
                    }
                }
            }
            catch
            {
            }
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            System.Timers.Timer timer = (System.Timers.Timer)sender;
            timer.Stop();

            bool corrupt = false;
            foreach (Task task in this.Tasks)
            {
                if(task.Status != TaskStatus.RanToCompletion)
                {
                    corrupt = true;
                }

                break;
            }

            if (!corrupt)
                return;

            this.CancellationToken.Cancel();
        }

        #endregion
    }

    public class Task2 : Task
    {
        #region Properties

        public Action Action { get; set; }

        public int Tries { get; set; }

        #endregion


        #region Constructor

        public Task2(Action action, CancellationToken cancellationToken, bool start = true)
            : base(action, cancellationToken)
        {
            this.Action = action;

            if (start)
                base.Start();
        }

        #endregion
    }
}
