using System.Net.Http.Headers;
using ALIVEMusicAPI.Models.DTOs;
using ALIVEMusicAPI.Repositories;
using CollaborateMusicAPI.Models.Entities;
using Newtonsoft.Json.Linq;

namespace ALIVEMusicAPI.Services;

public interface ITrackService
{
    Task<(Track track, Guid jobId)> UploadTrackAsync(TrackUploadDTO trackUploadDTO);
    Task<Track> GetTrack(int trackId);
    Task<Track> GetTrackByJobId(Guid jobId);
    Task<List<Track>> GetAllTracksByArtistId(int artistId);
    Task<List<Track>> GetAllTracks();
}

public class TrackService : ITrackService
{
    private readonly ITrackRepository _trackRepository;
    private readonly IAzureBlobService _azureBlobService;

    public TrackService(ITrackRepository trackRepository, IAzureBlobService azureBlobService)
    { 

        _trackRepository = trackRepository;
        _azureBlobService = azureBlobService;
   
    }

    public async Task<(Track track, Guid jobId)> UploadTrackAsync(TrackUploadDTO trackUploadDTO)
    {
        Guid jobId = Guid.Empty;
        try
        {
            using var trackStream = new MemoryStream();
            await trackUploadDTO.TrackFile.CopyToAsync(trackStream);

            // Reset the position of the MemoryStream to the start
            trackStream.Position = 0;

            var trackFilePath = await _azureBlobService.UploadFileAsync("tracks", $"{Guid.NewGuid()}.mp3", trackStream);

            var track = new Track
            {
                TrackName = trackUploadDTO.TrackName,
                Duration = trackUploadDTO.Duration,
                TrackNumber = trackUploadDTO.TrackNumber,
                Lyrics = trackUploadDTO.Lyrics,
                AlbumID = trackUploadDTO.AlbumID,
                ArtistID = trackUploadDTO.ArtistID,
                TrackFilePath = trackFilePath
            };

            await _trackRepository.SaveTrack(track);

            // Reset the position of the MemoryStream to the start again
            trackStream.Position = 0;


            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Authorization", "2555cad4-34a6-427a-a2e8-965f848f69fc");



                // Request the upload and download URLs
                var uploadUrlResponse = await client.GetAsync("https://api.music.ai/api/upload");
                var uploadUrlResponseContent = await uploadUrlResponse.Content.ReadAsStringAsync();

                // Print out the entire response
                Console.WriteLine(uploadUrlResponseContent);

                var uploadUrlResponseJson = JObject.Parse(uploadUrlResponseContent);
                var uploadUrl = uploadUrlResponseJson["uploadUrl"].ToString();
                var downloadUrl = uploadUrlResponseJson["downloadUrl"].ToString();

                // Upload the track file to the upload URL
                using (var uploadContent = new StreamContent(trackStream))
                {
                    uploadContent.Headers.ContentType = new MediaTypeHeaderValue("audio/mpeg");
                    var uploadResponse = await client.PutAsync(uploadUrl, uploadContent);
                }

                // Create a job to analyze the track
                var jobResponse = await client.PostAsJsonAsync("https://api.music.ai/api/job", new
                {
                    name = "My job 123",
                    workflow = "workflows",
                    @params = new { inputUrl = downloadUrl }
                });

                if (!jobResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Status code: {jobResponse.StatusCode}");
                    Console.WriteLine($"Error message: {await jobResponse.Content.ReadAsStringAsync()}");
                }
                if (jobResponse.IsSuccessStatusCode)
                {
                    var jobResponseContent = await jobResponse.Content.ReadAsStringAsync();
                    Console.WriteLine(jobResponseContent); // Print out the entire response
                    var jobResponseJson = JObject.Parse(jobResponseContent);
                     jobId = Guid.Parse(jobResponseJson["id"].ToString()); // replace "id" with the actual property name in the JSON response

                    // Store the jobId in the track
                    track.JobID = jobId;

                    // Update the track with the jobId
                    await _trackRepository.UpdateTrack(track);

                    var jobStatus = "pending";
                    while (jobStatus == "pending")
                    {
                        var response = await client.GetAsync($"https://api.music.ai/api/job/{jobId}");
                        var job = await response.Content.ReadAsAsync<JObject>();
                        jobStatus = job["status"].Value<string>();
                        await Task.Delay(1000);
                    }
                }
            }



            return (track, jobId);
        }

        catch (Exception e)
        {
            Console.WriteLine($"Exception: {e.Message}");
            Console.WriteLine($"Stack trace: {e.StackTrace}");
            throw;
        }
    }

    public async Task<Track> GetTrack(int trackId)
    {
        return await _trackRepository.GetTrack(trackId);
    }

    public async Task<Track> GetTrackByJobId(Guid jobId)
    {
        return await _trackRepository.GetTrackByJobId(jobId);
    }

    public async Task<List<Track>> GetAllTracksByArtistId(int artistId)
    {
        return await _trackRepository.GetTracksByArtistId(artistId);
    }

    public async Task<List<Track>> GetAllTracks()
    {
        return await _trackRepository.GetAllTracks();
    }

}
