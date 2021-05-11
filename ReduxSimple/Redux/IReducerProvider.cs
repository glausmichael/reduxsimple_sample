using System.Collections.Generic;

namespace ReduxSimple.Redux
{
    internal interface IReducerProvider
    {
        IEnumerable<On<RootState>> GetReducers();
    }
}
