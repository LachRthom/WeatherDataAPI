using WeatherDataAPI.Models;

namespace WeatherDataAPI.Repository
{
    public interface IUserDataRepository
    {
        UserData AuthenticateUser(string apiKey, params UserRolesEnum[] requiredRoles);
        void UpdateLoginTime(string apiKey);
        UserData GetById(string id);
        IEnumerable<UserData> GetAll();
        bool Insert(UserData userData);
        void Delete(string id);
        void DeleteUsersByRoleAndLastLoginDateRange(string role, DateTime startDate, DateTime endDate);
        void UpdateAccessLevelForUsersInDateRange(DateTime startDate, DateTime endDate, string newRole);
    }
}