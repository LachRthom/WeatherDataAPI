using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using WeatherDataAPI.AttributeTags;
using WeatherDataAPI.Models;
using WeatherDataAPI.Models.DTOs;
using WeatherDataAPI.Repository;

namespace WeatherDataAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserDataRepository _userDataRepository;

        public UserController(IUserDataRepository userDataRepository)
        {
            _userDataRepository = userDataRepository;
        }

        /// <summary>
        /// Retrieves a user by their ID.
        /// </summary>
        /// <param name="id">The ID of the user to retrieve.</param>
        /// <returns>An ActionResult containing the user if found, or NotFound if no user is found with the provided ID.</returns>
        // GET: api/UserData/5
        [HttpGet("{id}")]
        [ApiKey(UserRolesEnum.TEACHER)]
        [SwaggerResponse(200, "Returns the user if found", typeof(UserData))]
        [SwaggerResponse(401, "If user is unauthorized to create a user")]
        [SwaggerResponse(403, "If user role does not meet required level")]
        [SwaggerResponse(404, "If no user is found with the provided ID")]
        public IActionResult GetById(string id)
        {
            // Use the repository to retrieve the user by ID
            UserData user = _userDataRepository.GetById(id);

            // Check if the user was found
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="createUserDto">UserData object containing user information.</param>
        /// <returns>An ActionResult containing the newly created user, or an appropriate error response.</returns>
        // POST: api/User
        [HttpPost]
        [ApiKey(UserRolesEnum.TEACHER)]
        [SwaggerResponse(201, "Returns the newly created user", typeof(UserData))]
        [SwaggerResponse(400, "If invalid user role is provided")]
        [SwaggerResponse(403, "If user role does not meet required level")]
        [SwaggerResponse(500, "If an error occurs while processing the request")]
        public IActionResult CreateUser([FromBody] CreateUserDTO createUserDto)
        {
            try
            {
                // Convert CreateUserDTO to UserData
                var userData = new UserData
                {
                    Username = createUserDto.Username,
                    Password = createUserDto.Password,
                    Role = createUserDto.Role.ToUpper(), // Convert role to uppercase
                    Email = createUserDto.Email,
                    LastLoginDate = DateTime.UtcNow, // Set current UTC time as last login date
                    ApiKey = Guid.NewGuid().ToString() // Generate unique API key
                };

                _userDataRepository.Insert(userData);

                return CreatedAtAction(nameof(GetById), new { id = userData._id }, userData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing the request");
            }
        }




        /// <summary>
        /// Deletes a user by their ID.
        /// </summary>
        /// <param name="id">The ID of the user to delete.</param>
        /// <returns>An ActionResult containing a success message if the user is deleted successfully, or NotFound if no user is found with the provided ID.</returns>
        // DELETE: api/User/{id}
        [HttpDelete("{id}")]
        [EnableCors("Google")]
        [ApiKey(UserRolesEnum.TEACHER)]
        [SwaggerResponse(200, "Returns success message if the user is deleted successfully")]
        [SwaggerResponse(401, "If user is unauthorized to create a user")]
        [SwaggerResponse(403, "If user role does not meet required level")]
        [SwaggerResponse(404, "If no user is found with the provided ID")]
        public IActionResult DeleteUser(string id)
        {
            var existingUser = _userDataRepository.GetById(id);
            if (existingUser == null)
            {
                return NotFound();
            }
            _userDataRepository.Delete(id);
            return Ok("User deleted successfully");
        }

        /// <summary>
        /// Deletes users by their role and last login date range.
        /// </summary>
        /// <param name="role">The role of the users to delete.</param>
        /// <param name="startDate">The start date of the last login date range.</param>
        /// <param name="endDate">The end date of the last login date range.</param>
        /// <returns>An ActionResult containing a success message if users are deleted successfully, or an appropriate error response.</returns>
        // DELETE: api/User/delete-by-role/{role}/from/{startDate}/to/{endDate}
        [HttpDelete("delete-by-role/{role}/from/{startDate}/to/{endDate}")]
        [ApiKey(UserRolesEnum.TEACHER)]
        [SwaggerResponse(200, "Returns success message if users are deleted successfully")]
        [SwaggerResponse(400, "If invalid start date or end date format is provided")]
        [SwaggerResponse(401, "If user is unauthorized to create a user")]
        [SwaggerResponse(403, "If user role does not meet required level")]
        public IActionResult DeleteUsersByRoleAndLastLoginDateRange(string role, DateTime startDate, DateTime endDate)
        {
            // Check if startDate and endDate are provided and valid
            if (startDate == default || endDate == default)
            {
                return BadRequest("Both startDate and endDate must be provided and be valid format");
            }

            // Check if startDate is before endDate
            if (startDate >= endDate)
            {
                return BadRequest("startDate must be before endDate");
            }

            _userDataRepository.DeleteUsersByRoleAndLastLoginDateRange(role, startDate, endDate);
            return Ok($"Users with role '{role}' and last login date between '{startDate}' and '{endDate}' deleted successfully");
        }

        /// <summary>
        /// Updates access level for users in a specified date range to a new role.
        /// </summary>
        /// <param name="startDate">The start date of the date range.</param>
        /// <param name="endDate">The end date of the date range.</param>
        /// <param name="newRole">The new role to assign to the users.</param>
        /// <returns>An ActionResult containing a success message if access level is updated successfully.</returns>
        // PATCH: api/User/update-access-level/from/{startDate}/to/{endDate}/to-role/{newRole}
        [HttpPatch("update-access-level/from/{startDate}/to/{endDate}/to-role/{newRole}")]
        [ApiKey(UserRolesEnum.TEACHER)]
        [SwaggerResponse(200, "Returns success message if access level is updated successfully")]
        [SwaggerResponse(401, "If user is unauthorized to create a user")]
        [SwaggerResponse(403, "If user role does not meet required level")]
        public IActionResult UpdateAccessLevelForUsersInDateRange(DateTime startDate, DateTime endDate, string newRole)
        {
            _userDataRepository.UpdateAccessLevelForUsersInDateRange(startDate, endDate, newRole);
            return Ok($"Access level updated for users with last login date between '{startDate}' and '{endDate}' to role '{newRole}'");
        }
    }
}
