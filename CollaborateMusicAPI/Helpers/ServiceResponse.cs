using System;

namespace ALIVEMusicAPI.Helpers;

public class ServiceResponse
{
    public CollaborateMusicAPI.Enums.StatusCode StatusCode { get; set; }
    public string Message { get; set; }
    public object Data { get; set; }
}