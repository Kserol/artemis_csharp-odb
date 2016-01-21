using Artemis.Blackboard;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Artemis
{
    /// <summary>
    ///  Most basic system.
    ///  Upon calling world.process(), your systems are processed in sequence.
    /// </summary>
    public abstract class BaseSystem: INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return false;
            }

            storage = value;
            this.OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        ///     Notifies listeners that a property value has changed.
        /// </summary>
        /// <param name="propertyName">
        ///     Name of the property used to notify listeners.  This
        ///     value is optional and can be provided automatically when invoked from compilers
        ///     that support <see cref="CallerMemberNameAttribute" />.
        /// </param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        private World world;
        private bool enabled;

        public BlackBoard BlackBoard
        {
            get
            {
                return world.BlackBoard;
            }
        }

        public BaseSystem() {
            enabled = true;
        }

        /// <summary>
        /// Called before system processing begins
        /// NB : Any entiies created in this method won't become avtive until the next system starts processing or when a new processing rounds begins
        /// </summary>
        protected virtual void Begin()
        {

        }

        /// <summary>
        /// Process system
        /// Does nothing if CheckProcessing is false ore system disabled
        /// </summary>
        public void Process()
        {
            if (enabled && CheckProcessing())
            {
                Begin();
                ProcessSystem();
                End();
            }
        }

        /// <summary>
        /// Process system
        /// </summary>
        protected abstract void ProcessSystem();

        /// <summary>
        /// Called after System ha finished Processing
        /// </summary>
        protected virtual void End()
        {

        }

        /// <summary>
        /// Useful for system occasionally needs to process
        /// </summary>
        /// <returns>return true if the system should be processed</returns>
        protected virtual bool CheckProcessing()
        {
            return true;
        }

        public virtual void Initialize()
        {

        }

        public bool IsEnabled
        {
            get
            {
                return enabled;
            }
            set
            {
                enabled = value;
            }
        }

        public virtual  World World
        {
            get
            {
                return world;
            }

            set
            {
                world = value;
            }
        }
    }
}