using ALIVEMusicAPI.Controllers;
using CollaborateMusicAPI.Contexts;
using CollaborateMusicAPI.Models.Entities;
using CollaborateMusicAPI.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ALIVEMusicAPI.Repositories;

public interface ITrackRepository : IRepository<Track, ApplicationDBContext>
{
    Task<Track> SaveTrack(Track track);
    Task<Track> GetTrack(int trackId);
    Task<Track> GetTrackByJobId(Guid jobId);
    Task<Track> UpdateTrack(Track track);
    Task<List<Track>> GetTracksByArtistId(int artistId);
    Task<List<Track>> GetAllTracks();
  
}

public class TrackRepository : Repository<Track, ApplicationDBContext>, ITrackRepository
{
    public TrackRepository(ApplicationDBContext context) : base(context)
    {
    }

    public async Task<Track> SaveTrack(Track track)
    {
        await _context.Tracks.AddAsync(track);
        await _context.SaveChangesAsync();
        return track;
    }

    public async Task<Track> UpdateTrack(Track track)
    {
        _context.Tracks.Update(track);
        await _context.SaveChangesAsync();
        return track;
    }

    public async Task<Track> GetTrack(int trackId)
    {
        return await _context.Tracks.FirstOrDefaultAsync(t => t.TrackID == trackId);
    }

    public async Task<Track> GetTrackByJobId(Guid jobId)
    {
        return await _context.Tracks.FirstOrDefaultAsync(t => t.JobID == jobId);
    }

    public async Task<List<Track>> GetTracksByArtistId(int artistId)
    {
        return await _context.Tracks
            .Where(t => t.ArtistID == artistId)
            .Include(t => t.Artist) 
            .Include(t => t.Album)
            .ToListAsync();
    }

    public async Task<List<Track>> GetAllTracks()
    {
        return await _context.Tracks
            .Include(t => t.Artist)
            .Include(t => t.Album)
            .ToListAsync();
    }

}
