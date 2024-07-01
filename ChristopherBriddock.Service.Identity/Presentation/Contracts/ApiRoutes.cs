namespace Presentation.Contracts;

public static class ApiRoutes
{
    public static class BaseRoute
    {
        public const string Name = "api/v{version:apiVersion}/";
    }

    public static class OAuth
    {
        public const string Authorize = "oauth2/authorize";
        public const string Token = "oauth2/token";
        public const string Logout = "oauth2/logout";
    }

    public static class Users
    {
        public const string ReadById = "users/{userId}";
        public const string ReadByEmail = "users/{userEmail}";
        public const string Create = "users";
        public const string UpdateById = "users/{userId}";
        public const string UpdateByEmail = "users/{userEmail}";
        public const string DeleteById = "users/{userId}";
        public const string DeleteByEmail = "users/{userEmail}";
    }

    public static class Applications
    {
        public const string ReadById = "applications/{appId}";
        public const string ReadByName = "applications/{name}";
        public const string Create = "applications";
        public const string UpdateById = "applications/{userId}";
        public const string UpdateByEmail = "applications/{userEmail}";
        public const string DeleteById = "applications/{userId}";
        public const string DeleteByEmail = "applications/{userEmail}";
    }

    public static class Groups
    {
        public const string GetById = "groups/{groupId}";
        public const string Create = "groups";
        public const string Update = "groups/{groupId}";
        public const string Delete = "groups/{groupId}";
    }
}
