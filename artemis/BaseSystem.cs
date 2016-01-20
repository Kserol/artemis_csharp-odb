namespace Artemis
{
    /// <summary>
    ///  Most basic system.
    ///  Upon calling world.process(), your systems are processed in sequence.
    /// </summary>
    public abstract class BaseSystem
    {
        private World world;
        private bool enabled;

        public BaseSystem() { }

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
        public virtual bool CheckProcessing()
        {
            return true;
        }

        public virtual void Initialize()
        {

        }

        public bool Enabled
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

        public  World World
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