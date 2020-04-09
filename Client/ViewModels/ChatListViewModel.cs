using System.Collections.Generic;

namespace Client
{
    /// <summary>
    /// ein View Model für jeden individuellen "ChatListItem"
    /// </summary>
    public class ChatListViewModel : ViewModelBase
    {

        /// <summary>
        /// Die Chat list items für die Liste
        /// </summary>
        public List<ChatListItemViewModel> Items { get; set; }



    }
}
