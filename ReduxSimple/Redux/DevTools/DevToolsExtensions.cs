namespace ReduxSimple.Redux.DevTools
{
    public static class DevToolsExtensions
    {
        /// <summary>
        /// Open Store DevTools in a separate window.
        /// </summary>
        /// <typeparam name="TState">Type of the state.</typeparam>
        /// <param name="store">Store to display information about.</param>
        /// <returns>True if the Store DevTools has been shown.</returns>
        public static bool OpenDevTools<TState>(this ReduxStore<TState> store, DevToolsConfiguration devToolsConfiguration)
            where TState : class, new()
        {
            if (store == null || !store.TimeTravelEnabled)
            {
                return false;
            }

            var devToolsComponent = new DevToolsComponent();
            var window = new MainWindow
            {
                Content = devToolsComponent
            };

            window.Show();
            devToolsComponent?.Initialize(store, devToolsConfiguration);

            return true;
        }
    }
}
