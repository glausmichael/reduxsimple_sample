using ReduxSimple.Redux;

namespace ReduxSimple.Notes
{
    static class NotesSelectors
    {
        public static ISelectorWithoutProps<RootState, NotesState> SelectNotesFeatureState = Selectors.CreateSelector(
            (RootState state) => state.GetStateByType<NotesState>());
    }
}
