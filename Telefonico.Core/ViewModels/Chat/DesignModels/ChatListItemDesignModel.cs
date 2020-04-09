namespace Telefonico.Core
{

    /// <summary>
    /// Design-time Daten für <see cref="ChatListItemViewModel"/>
    /// </summary>
    public class ChatListItemDesignModel : ChatListItemViewModel
    {
        #region Singleton

        /// <summary>
        /// Eine einzige Instanz des Design Models
        /// </summary>
        public static ChatListItemDesignModel Instance => new ChatListItemDesignModel();
        // Dasselbe wie: public static ChatListItemDesignModel Instance {get {return new ChatListItemDesignModel();}} C#7


        #endregion

        #region Konstruktor

        public ChatListItemDesignModel()
        {
            Name = "Thomas Anderson";
            Status = "Online";
        }

        #endregion
    }
}
