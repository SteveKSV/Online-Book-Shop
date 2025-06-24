using Identity.Data;
using Identity.Entities;
using Identity.Managers.Interfaces;
using Identity.Models;
using Microsoft.EntityFrameworkCore;
using Identity.Helpers;
using AutoMapper;
using Identity.Helpers.Catalog.Helpers;

namespace Identity.Managers
{
    public class AuthManager : IAuthManager
    {
        private readonly AppDbContext _context;
        private readonly JwtTokenGenerator _jwt;
        private readonly IMapper _mapper;
        public AuthManager(AppDbContext context, JwtTokenGenerator jwt, IMapper mapper)
        {
            _context = context;
            _jwt = jwt;
            _mapper = mapper;
        }

        public async Task<AuthResponse> SignUp(RegisterDTO registerInfo)
        {
            try
            {
                if (await _context.Users.AnyAsync(u => u.Email == registerInfo.Email))
                {
                    return new AuthResponse { StatusCode = "409", StatusMessage = "Email already exists." };
                }

                var user = new User
                {
                    Id = Guid.NewGuid(),
                    UserName = registerInfo.UserName,
                    Email = registerInfo.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(registerInfo.Password),
                    UserTypeId = _context.UserTypes.First(ut => ut.Name == "User").Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                var token = _jwt.GenerateToken(user);

                return new AuthResponse
                {
                    Token = token,
                    StatusCode = "200",
                    StatusMessage = "User registered successfully."
                };
            }
            catch (Exception ex)
            {
                return new AuthResponse
                {
                    StatusCode = "500",
                    StatusMessage = $"An error occurred during registration: {ex.Message}"
                };
            }
        }

        public async Task<AuthResponse> SignIn(LoginDTO loginInfo)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.UserType)
                    .FirstOrDefaultAsync(u => u.Email == loginInfo.Email);

                if (user == null || !BCrypt.Net.BCrypt.Verify(loginInfo.Password, user.Password))
                {
                    return new AuthResponse
                    {
                        StatusCode = "401",
                        StatusMessage = "Invalid credentials."
                    };
                }

                var token = _jwt.GenerateToken(user);

                return new AuthResponse
                {
                    Token = token,
                    StatusCode = "200",
                    StatusMessage = "Login successful."
                };
            }
            catch (Exception ex)
            {
                return new AuthResponse
                {
                    StatusCode = "500",
                    StatusMessage = $"An error occurred during login: {ex.Message}"
                };
            }
        }
        public async Task<PagedList<UserDTO>> GetAllUsersAsync(int pageNumber, int pageSize)
        {
            try
            {
                var query = _context.Users
                    .Include(u => u.UserType)
                    .AsQueryable();

                var dtoQuery = _mapper.ProjectTo<UserDTO>(query);
                return PagedList<UserDTO>.ToPagedList(dtoQuery, pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetAllUsersAsync] Error: {ex.Message}");
                throw new InvalidOperationException("An error occurred. Details: " + ex.Message);
            }
        }

        public async Task<UserDTO?> GetUserByIdAsync(Guid userId)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.UserType)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                return user != null ? _mapper.Map<UserDTO>(user) : null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetUserByIdAsync] Error: {ex.Message}");
                throw new InvalidOperationException("An error occurred. Details: " + ex.Message);
            }
        }

        public async Task<AuthResponse> UpdateUserAsync(UpdateUserDTO userDto)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.UserType)
                    .FirstOrDefaultAsync(u => u.Id == userDto.Id);

                if (user == null)
                    return new AuthResponse { StatusCode = "404", StatusMessage = "User not found." };

                if (!string.IsNullOrWhiteSpace(userDto.UserName) && user.UserName != userDto.UserName)
                {
                    user.UserName = userDto.UserName;
                }

                if (!string.IsNullOrWhiteSpace(userDto.Email) && user.Email != userDto.Email)
                {
                    user.Email = userDto.Email;
                }

                if (!string.IsNullOrWhiteSpace(userDto.Password))
                {
                    user.Password = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
                }

                user.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                var token = _jwt.GenerateToken(user);

                return new AuthResponse
                {
                    Token = token,
                    StatusCode = "200",
                    StatusMessage = "User updated successfully."
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UpdateUserAsync] Error: {ex.Message}");
                return new AuthResponse
                {
                    StatusCode = "500",
                    StatusMessage = "An error occurred during update: " + ex.Message
                };
            }
        }


        public async Task<bool> DeleteUserAsync(Guid userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                    return false;

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DeleteUserAsync] Error: {ex.Message}");
                throw;
            }
        }

    }
}
