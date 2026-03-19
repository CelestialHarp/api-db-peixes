namespace API_DB_PESCES_em_C__bonitona.DTOs
{
    public record LoginDTO
    (
        string Username,
        string Password
    );
    public record RegistroDTO
    (
        string Username,
        string Password
    );
    public record UserResponseDTO
    (
        string Username,
        string Token,
        string Role
    );
}