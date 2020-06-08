using System.Collections.Generic;

namespace Telefonico.Core
{
    /// <summary>
    /// ein View Model für jeden individuellen "ChatListItem"
    /// </summary>
    public class ChatListViewModel : BaseViewModel
    {

        /// <summary>
        /// Die Chat list items für die Liste
        /// </summary>
        public List<ChatListItemViewModel> Items { get; set; }



    }
}
