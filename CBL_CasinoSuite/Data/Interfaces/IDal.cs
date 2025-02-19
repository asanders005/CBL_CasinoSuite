using CBL_CasinoSuite.Data.Models;

namespace CBL_CasinoSuite.Data.Interfaces
{
    public interface IDal
    {
        /// <summary>
        /// Retrieves the specified user from the database
        /// </summary>
        /// <param name="username">The username of the user to be retrieved</param>
        /// <returns>The user matching the specified username</returns>
        public User GetUser(string username);
        /// <summary>
        /// Retrieves every user in the database
        /// </summary>
        /// <returns>A list containing all users in the database</returns>
        public List<User> GetUsers();
        /// <summary>
        /// Creates a new user data entry in the database
        /// </summary>
        /// <param name="user">The user to add to the database</param>
        public void AddUser(User user);
        /// <summary>
        /// Updates all data for the specified user in the database
        /// </summary>
        /// <param name="username">The username of the user to update</param>
        /// <param name="user">A user instance containing the data to be changed</param>
        public void UpdateUser(string username, User user);
        /// <summary>
        /// Updates the current balance for the specified user in the database
        /// </summary>
        /// <param name="username">The username of the user to update</param>
        /// <param name="balance">The value that the user's current balance will be set to</param>
        public void UpdateUserBalance(string username, float balance);
        /// <summary>
        /// Updates the statistics of a single game for the specified user in the database
        /// </summary>
        /// <param name="username">The username of the user to update</param>
        /// <param name="gameStatistics">The GameStats variable detailing the game and statistics to be updated</param>
        public void UpdateUserStatistics(string username, GameStats gameStatistics);
        /// <summary>
        /// Removes the specified user from the database
        /// </summary>
        /// <param name="username">The username of the user to be removed</param>
        public void DeleteUser(string username);
    }
}
