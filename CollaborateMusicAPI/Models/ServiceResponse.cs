using CollaborateMusicAPI.Enums;

namespace CollaborateMusicAPI.Models;

public class ServiceResponse<T>
{
    public StatusCode StatusCode { get; set; } 
    public T? Content { get; set; }
    public string Message { get; set; } = null!;
}
