namespace Telefonico.Core
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



        /// <summary>
        /// true, wenn neue Nachrichten vorhanden sind
        /// </summary>
        public bool NewMessages { get; set; }



        /// <summary>
        /// True, wenn Kontakt ausgewählt ist
        /// </summary>
        public bool IsSelected { get; set; }

    }
}
