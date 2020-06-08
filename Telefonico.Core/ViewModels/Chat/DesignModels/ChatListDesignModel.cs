using System.Collections.Generic;

namespace Telefonico.Core
{

    /// <summary>
    /// Design-time Daten für <see cref="ChatListViewModel"/>
    /// </summary>
    public class ChatListDesignModel : ChatListViewModel
    {
        #region Singleton

        /// <summary>
        /// Eine einzige Instanz des Design Models
        /// </summary>
        public static ChatListDesignModel Instance => new ChatListDesignModel();
        // Dasselbe wie: public static ChatListItemDesignModel Instance {get {return new ChatListItemDesignModel();}} C#7


        #endregion

        #region Konstruktor

        public ChatListDesignModel()
        {
            Items = new List<ChatListItemViewModel>
            {
                new ChatListItemViewModel
                {
                    Name = "Cooper",
                    Status = "Online",
                    NewMessages = true
                },
                new ChatListItemViewModel
                {
                    Name = "Morpheus",
                    Status = "Offline",
                    NewMessages = false
                },
                new ChatListItemViewModel
                {
                    Name = "Mark",
                    Status = "Offline",
                    NewMessages = true,
                    IsSelected = true
                },
                new ChatListItemViewModel
                {
                    Name = "Salvadore",
                    Status = "Offline",
                    NewMessages = false
                }


            };

        }

        #endregion
    }
}
