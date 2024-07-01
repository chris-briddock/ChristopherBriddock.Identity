using Domain.Contracts;

namespace Application.Commands;

public sealed record ConfirmEmailCommand(string Email, string Code)
{ } 
