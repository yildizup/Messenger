namespace Client
{
    /// <summary>
    /// ein View Model für jeden individuellen "ChatListItem"
    /// </summary>
    public class ChatListItemViewModel : ViewModelBase
    {

        /// <summary>
        /// Der Name des Benutzers
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// Aktivitätstatus des Benutzers
        /// </summary>
        public string Status { get; set; }

    }
}
