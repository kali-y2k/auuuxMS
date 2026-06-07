using Microsoft.EntityFrameworkCore;
using auuuxMS.Data.Entities;

namespace auuuxMS.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Artist> Artists => Set<Artist>();
    public DbSet<Album> Albums => Set<Album>();
    public DbSet<Track> Tracks => Set<Track>();
    public DbSet<MoodTag> MoodTags => Set<MoodTag>();
    public DbSet<Follow> Follows => Set<Follow>();
    public DbSet<Connect> Connects => Set<Connect>();
    public DbSet<Rate> Rates => Set<Rate>();
    public DbSet<Moment> Moments => Set<Moment>();
    public DbSet<MyVinyl> MyVinyls => Set<MyVinyl>();
    public DbSet<MyVinylTrack> MyVinylTracks => Set<MyVinylTrack>();
    public DbSet<UserTopArtist> UserTopArtists => Set<UserTopArtist>();
    public DbSet<UserTopAlbum> UserTopAlbums => Set<UserTopAlbum>();

    protected override void OnModelCreating(ModelBuilder m)
    {
        // Follow — chiave composta
        m.Entity<Follow>().HasKey(f => new { f.FollowerId, f.FollowingId });

        // MyVinylTrack — chiave composta
        m.Entity<MyVinylTrack>().HasKey(t => new { t.UserId, t.Position });

        // UserTopArtist — chiave composta
        m.Entity<UserTopArtist>().HasKey(t => new { t.UserId, t.Position });

        // UserTopAlbum — chiave composta
        m.Entity<UserTopAlbum>().HasKey(t => new { t.UserId, t.Position });

        // Rate — unique per utente+album e utente+traccia
        m.Entity<Rate>()
            .HasIndex(r => new { r.UserId, r.AlbumId }).IsUnique();
        m.Entity<Rate>()
            .HasIndex(r => new { r.UserId, r.TrackId }).IsUnique();
    }
}
