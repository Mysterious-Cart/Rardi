using System;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using System.Text.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Hosting;

using Radzen;

using CHKS.Models;

namespace CHKS.Services
{
    public partial class SecurityService
    {
        private readonly HttpClient httpClient;
        private readonly Uri baseUri;
        private readonly NavigationManager navigationManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly IWebHostEnvironment env;

        public ApplicationUser User { get; private set; } = new ApplicationUser { Name = "Anonymous" };

        public ClaimsPrincipal Principal { get; private set; }

        /// <summary>
        /// Creates a new security service instance.
        /// </summary>
        public SecurityService(
            NavigationManager navigationManager,
            IHttpClientFactory factory,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IWebHostEnvironment env)
        {
            httpClient = factory.CreateClient("CHKS");
            this.navigationManager = navigationManager;
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.env = env;

            baseUri = new Uri($"{navigationManager.BaseUri}odata/Identity/");

        }

        /// <summary>
        /// Attempts a user sign-in and returns the outcome and redirect target.
        /// </summary>
        public async Task<LoginResult> LoginAsync(string userName, string password, string redirectUrl)
        {
            var normalizedRedirect = NormalizeRedirectUrl(redirectUrl);

            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
            {
                return LoginResult.Failed("Username and password are required.", normalizedRedirect);
            }

            if (env.IsDevelopment() && userName == "admin" && password == "admin")
            {
                var adminUser = await userManager.FindByNameAsync("admin");
                if (adminUser == null)
                {
                    adminUser = new ApplicationUser
                    {
                        UserName = "admin",
                        Email = "admin@example.com",
                        EmailConfirmed = true
                    };

                    await userManager.CreateAsync(adminUser, "admin");
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, "admin"),
                    new Claim(ClaimTypes.Email, "admin@example.com")
                };

                roleManager.Roles.ToList().ForEach(role => claims.Add(new Claim(ClaimTypes.Role, role.Name)));
                await signInManager.SignInWithClaimsAsync(adminUser, isPersistent: false, claims);

                return LoginResult.Success(normalizedRedirect);
            }

            var result = await signInManager.PasswordSignInAsync(userName, password, false, false);
            if (result.Succeeded)
            {
                return LoginResult.Success(normalizedRedirect);
            }

            return LoginResult.Failed("Invalid username or password.", normalizedRedirect);
        }

        /// <summary>
        /// Normalizes a redirect URL to the application root when missing.
        /// </summary>
        private static string NormalizeRedirectUrl(string redirectUrl)
        {
            if (string.IsNullOrWhiteSpace(redirectUrl))
            {
                return "~/";
            }

            return redirectUrl.StartsWith("/") ? $"~{redirectUrl}" : redirectUrl.StartsWith("~") ? redirectUrl : $"~/{redirectUrl}";
        }

        /// <summary>
        /// Determines whether the current principal is in any of the supplied roles.
        /// </summary>
        public bool IsInRole(params string[] roles)
        {
#if DEBUG
            if (User.Name == "admin")
            {
                return true;
            }
#endif

            if (roles.Contains("Everybody"))
            {
                return true;
            }

            if (!IsAuthenticated())
            {
                return false;
            }

            if (roles.Contains("Authenticated"))
            {
                return true;
            }

            return roles.Any(role => Principal.IsInRole(role));
        }

        /// <summary>
        /// Returns true when the current principal is authenticated.
        /// </summary>
        public bool IsAuthenticated()
        {
            return Principal?.Identity.IsAuthenticated == true;
        }

        /// <summary>
        /// Initializes the service from an authentication state.
        /// </summary>
        public async Task<bool> InitializeAsync(AuthenticationState result)
        {
            Principal = result.User;
#if DEBUG
            if (Principal.Identity.Name == "admin")
            {
                User = new ApplicationUser { Name = "Admin" };

                return true;
            }
#endif
            var userId = Principal?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != null && User?.Id != userId)
            {
                User = await GetUserById(userId);
            }

            return IsAuthenticated();
        }


        /// <summary>
        /// Requests the current authentication state from the server.
        /// </summary>
        public async Task<ApplicationAuthenticationState> GetAuthenticationStateAsync()
        {
            var uri = new Uri($"{navigationManager.BaseUri}Account/CurrentUser");

            var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, uri));

            return await response.ReadAsync<ApplicationAuthenticationState>();
        }

        /// <summary>
        /// Redirects the user to the logout endpoint.
        /// </summary>
        public void Logout()
        {
            navigationManager.NavigateTo("Account/Logout", true);
        }

        /// <summary>
        /// Redirects the user to the login page.
        /// </summary>
        public void Login()
        {
            navigationManager.NavigateTo("Login", true);
        }

        /// <summary>
        /// Retrieves all roles from the identity API.
        /// </summary>
        public async Task<IEnumerable<ApplicationRole>> GetRoles()
        {
            var uri = new Uri(baseUri, $"ApplicationRoles");

            var response = await httpClient.GetAsync(uri);

            var result = await response.ReadAsync<List<ApplicationRole>>();

            return result;
        }

        /// <summary>
        /// Creates a new role through the identity API.
        /// </summary>
        public async Task<ApplicationRole> CreateRole(ApplicationRole role)
        {
            var uri = new Uri(baseUri, $"ApplicationRoles");

            // Use standard JSON serialization unless your API expects OData format
            var content = new StringContent(JsonSerializer.Serialize(role), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(uri, content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new ApplicationException($"Failed to create role: {error}");
            }

            // Only try to read the response if you expect the created role to be returned
            if (response.Content.Headers.ContentLength > 0)
            {
                return await response.ReadAsync<ApplicationRole>();
            }
            else
            {
                return null; // Or handle as appropriate
            }
        }

        /// <summary>
        /// Deletes a role by id.
        /// </summary>
        public async Task<HttpResponseMessage> DeleteRole(string id)
        {
            var uri = new Uri(baseUri, $"Applicatio nRoles('{id}')");

            return await httpClient.DeleteAsync(uri);
        }

        /// <summary>
        /// Retrieves all users from the identity API.
        /// </summary>
        public async Task<IEnumerable<ApplicationUser>> GetUsers()
        {
            var uri = new Uri(baseUri, $"ApplicationUsers");

            var response = await httpClient.GetAsync(uri);

            var result = await response.ReadAsync<List<ApplicationUser>>();

            return result;
        }

        /// <summary>
        /// Creates a new user through the identity API.
        /// </summary>
        public async Task<ApplicationUser> CreateUser(ApplicationUser user)
        {
            var uri = new Uri(baseUri, $"ApplicationUsers");

            var content = new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(uri, content);

            return await response.ReadAsync<ApplicationUser>();
        }

        /// <summary>
        /// Deletes a user by id.
        /// </summary>
        public async Task<HttpResponseMessage> DeleteUser(string id)
        {
            var uri = new Uri(baseUri, $"ApplicationUsers('{id}')");

            return await httpClient.DeleteAsync(uri);
        }

        /// <summary>
        /// Retrieves a user by id.
        /// </summary>
        public async Task<ApplicationUser> GetUserById(string id)
        {
            var uri = new Uri(baseUri, $"ApplicationUsers('{id}')?$expand=Roles");

            var response = await httpClient.GetAsync(uri);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            return await response.ReadAsync<ApplicationUser>();
        }

        /// <summary>
        /// Updates a user by id through the identity API.
        /// </summary>
        public async Task<ApplicationUser> UpdateUser(string id, ApplicationUser user)
        {
            var uri = new Uri(baseUri, $"ApplicationUsers('{id}')");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri)
            {
                Content = new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json")
            };

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await response.ReadAsync<ApplicationUser>();
        }
        /// <summary>
        /// Changes the current user's password.
        /// </summary>
        public async Task ChangePassword(string oldPassword, string newPassword)
        {
            var uri = new Uri($"{navigationManager.BaseUri}Account/ChangePassword");

            var content = new FormUrlEncodedContent(new Dictionary<string, string> {
                { "oldPassword", oldPassword },
                { "newPassword", newPassword }
            });

            var response = await httpClient.PostAsync(uri, content);

            if (!response.IsSuccessStatusCode)
            {
                var message = await response.Content.ReadAsStringAsync();

                throw new ApplicationException(message);
            }
        }
    }

    /// <summary>
    /// Represents the outcome of a login attempt.
    /// </summary>
    public sealed class LoginResult
    {
        /// <summary>
        /// Gets a value indicating whether the login succeeded.
        /// </summary>
        public bool IsSuccess { get; init; }

        /// <summary>
        /// Gets the error message when login fails.
        /// </summary>
        public string ErrorMessage { get; init; }

        /// <summary>
        /// Gets the normalized redirect URL for the login flow.
        /// </summary>
        public string RedirectUrl { get; init; }

        /// <summary>
        /// Creates a successful login result.
        /// </summary>
        public static LoginResult Success(string redirectUrl)
        {
            return new LoginResult { IsSuccess = true, RedirectUrl = redirectUrl };
        }

        /// <summary>
        /// Creates a failed login result.
        /// </summary>
        public static LoginResult Failed(string errorMessage, string redirectUrl)
        {
            return new LoginResult { IsSuccess = false, ErrorMessage = errorMessage, RedirectUrl = redirectUrl };
        }
    }
}