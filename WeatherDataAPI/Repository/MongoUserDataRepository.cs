using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using WeatherDataAPI.Models;

namespace WeatherDataAPI.Repository
{
    public class UserDataRepository : IUserDataRepository
    {
        private readonly IMongoCollection<UserData> _users;

        public UserDataRepository(IMongoDatabase database)
        {
            _users = database.GetCollection<UserData>("Users");
        }

        // Get User by the id field
        public UserData GetById(string id)
        {
            ObjectId objectId;
            if (!ObjectId.TryParse(id, out objectId))
            {
                return null;
            }
            var filter = Builders<UserData>.Filter.Eq(u => u._id, objectId);
            return _users.Find(filter).FirstOrDefault();
        }

        // Get all Users
        public IEnumerable<UserData> GetAll()
        {
            return _users.Find(_ => true).ToList();
        }

        // Create a single new User
        public bool Insert(UserData userData)
        {
            var filter = Builders<UserData>.Filter.Eq(u => u.Username, userData.Username);
            var existingUser = _users.Find(filter).FirstOrDefault();

            if (existingUser != null)
            {

                return false;
            }

            userData.LastLoginDate = DateTime.UtcNow;
            _users.InsertOne(userData);
            return true;
        }

        // Delete a single record based on the provided id 
        public void Delete(string id)
        {
            ObjectId objectId;
            if (!ObjectId.TryParse(id, out objectId))
            {
                return;
            }

            var filter = Builders<UserData>.Filter.Eq(u => u._id, objectId);
            _users.DeleteOne(filter);
        }

        // Delete multiple records between date range based on a select role
        public void DeleteUsersByRoleAndLastLoginDateRange(string role, DateTime startDate, DateTime endDate)
        {
            var filter = Builders<UserData>.Filter.And(
                Builders<UserData>.Filter.Eq(u => u.Role, role),
                Builders<UserData>.Filter.Gte(u => u.LastLoginDate, startDate),
                Builders<UserData>.Filter.Lte(u => u.LastLoginDate, endDate)
            );

            _users.DeleteMany(filter);
        }


        // Update all users role with the last login date between the given range
        public void UpdateAccessLevelForUsersInDateRange(DateTime startDate, DateTime endDate, string newRole)
        {
            var filter = Builders<UserData>.Filter.And(
                Builders<UserData>.Filter.Gte(u => u.LastLoginDate, startDate),
                Builders<UserData>.Filter.Lte(u => u.LastLoginDate, endDate)
            );
            var update = Builders<UserData>.Update.Set(u => u.Role, newRole);
            _users.UpdateMany(filter, update);
        }

        // Updates the last login field on a user record when required
        public void UpdateLoginTime(string apiKey)
        {
            var currentDate = System.DateTime.UtcNow;

            // Finding entry based on matching (or 'eq'uivalent) apiKey
            var filter = Builders<UserData>.Filter.Eq(u => u.ApiKey, apiKey);
            var update = Builders<UserData>.Update.Set(u => u.LastLoginDate, currentDate);
                                         
            _users.UpdateOne(filter, update);
        }

        // Helper method for authorization
        private bool IsAllowedRole(string userRole, UserRolesEnum[] requiredRoles)
        {
            if (!Enum.TryParse(userRole, out UserRolesEnum userRoleType))
            {
                return false;
            }
            foreach (var role in requiredRoles)
            {
                int userRoleNumber = (int)userRoleType;
                int requiredRoleNumber = (int)role;
                if (userRoleNumber == requiredRoleNumber)
                {
                    return true;
                }
            }

            // No matching role found, return false ( here because it stops VS2022 yelling at me)
            return false;
        }

        // Helper method that conducts the actual authorization
        public UserData AuthenticateUser(string apiKey, params UserRolesEnum[] requiredRoles)
        {
            var filter = Builders<UserData>.Filter.Eq(u =>u.ApiKey, apiKey);
            var user = _users.Find(filter).FirstOrDefault();

            if (user == null)
            {
                return null;
            }

            if (IsAllowedRole(user.Role, requiredRoles) == false) 
            {
                return null;
            }

            return user;
        }
    }
}
