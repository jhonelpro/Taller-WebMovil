using api.src.DTOs.User;
using api.src.Models.User;

namespace api.src.Mappers
{
    /// <summary>
    /// Clase que proporciona métodos de mapeo entre entidades de usuario y sus DTOs correspondientes.
    /// </summary>
    public class UserMapper
    {
        /// <summary>
        /// Mapea un objeto AppUser a un objeto UserDto.
        /// </summary>
        /// <param name="user">Parametro de tipo AppUser que representa el usuario a mapear.</param>
        /// <returns>Un objeto de tipo UserDto que representa la información del usuario mapeada.</returns>
        public static UserDto MapUserToUserDto(AppUser user)
        {
            return new UserDto
            {
                UserName = user.UserName ?? string.Empty,
                Rut = user.Rut,
                Name = user.Name,
                DateOfBirth = user.DateOfBirth,
                Email = user.Email,
                Gender = user.Gender,
                IsActive = user.IsActive
            };
        }
    }
}