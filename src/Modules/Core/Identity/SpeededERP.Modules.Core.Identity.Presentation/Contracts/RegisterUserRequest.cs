namespace SpeededERP.Modules.Core.Identity.Presentation.Contracts;

public sealed record RegisterUserRequest(string Email, string FirstName, string LastName, string InitialPassword);
