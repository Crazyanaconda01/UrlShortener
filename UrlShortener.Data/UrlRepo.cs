using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace UrlShortener.Data
{
    public interface IUrlRepo
    {
        Task<IEnumerable<TheUrl>> GetAllUrls();

        Task<TheUrl> GetUrl(string id);

        Task<IEnumerable<TheUrl>> GetUrls(string bodyText, DateTime updatedFrom);

        Task<TheUrl> GetUrlByShortUrl(string shortUrl);

        Task<TheUrl> GetUrlByOriginalUrl(string originalUrl);

        Task AddUrl(TheUrl item);

        Task<bool> RemoveUrl(string id);

        Task<bool> UpdateUrl(string id, long counter, DateTime lastAccessed);

        Task<bool> UpdateUrl(string id, TheUrl item);

        Task<bool> RemoveAllUrls();
    }

    public class UrlRepo : IUrlRepo
    {
        private readonly UrlDbContext _context = null;
        private ILogger _log = null;

        public UrlRepo(IOptions<Settings> settings, ILogger<UrlRepo> logger)
        {
            _context = new UrlDbContext(settings);
            _log = logger;
        }

        public async Task<IEnumerable<TheUrl>> GetAllUrls()
        {
            try
            {
                return await _context.Urls
                        .Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                _log.LogCritical("GetAllUrls failed", ex);
                throw ex;
            }
        }

        public async Task<TheUrl> GetUrl(string id)
        {
            #region
            var collection = _context.Urls.Database.GetCollection<TheUrl>("UrlsDb");
            var test = collection.AsQueryable<TheUrl>().ToList();
            #endregion

            try
            {
                ObjectId internalId = GetInternalId(id);
                return await _context.Urls
                                .Find(Url => Url.Id == id)
                                .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _log.LogCritical($"GetUrl with id {id} failed", ex);
                throw ex;
            }
        }

        public async Task<IEnumerable<TheUrl>> GetUrls(string orginalUrl, DateTime updatedFrom)
        {
            try
            {
                var query = _context.Urls.Find(Url => Url.OriginalUrl.Contains(orginalUrl) &&
                                       Url.CreatedDate >= updatedFrom);

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                _log.LogCritical($"GetUrl failed with params {orginalUrl} and {updatedFrom}", ex);
                throw ex;
            }
        }

        public async Task<TheUrl> GetUrlByShortUrl(string shortUrl)
        {
            try
            {
                return await _context.Urls
                                .Find(Url => Url.ShortUrl == shortUrl)
                                .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _log.LogCritical($"GetUrlByShortUrl with shorturl {shortUrl} failed", ex);
                throw ex;
            }
        }

        public async Task<TheUrl> GetUrlByOriginalUrl(string originalUrl)
        {
            try
            {
                return await _context.Urls
                                .Find(Url => Url.OriginalUrl == originalUrl)
                                .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _log.LogCritical($"GetUrlByShortUrl with shorturl {originalUrl} failed", ex);
                throw ex;
            }
        }

        private ObjectId GetInternalId(string id)
        {
            ObjectId internalId;
            if (!ObjectId.TryParse(id, out internalId))
                internalId = ObjectId.Empty;

            return internalId;
        }

        public async Task AddUrl(TheUrl item)
        {
            try
            {
                await _context.Urls.InsertOneAsync(item);
            }
            catch (Exception ex)
            {
                _log.LogCritical($"Add failed for item {item.ToString()}", ex);
                throw ex;
            }
        }

        public async Task<bool> RemoveUrl(string id)
        {
            ObjectId objectId = new ObjectId(id);

            #region test
            var filter = new BsonDocument();
            var url = Builders<TheUrl>.Filter.Where(x => x.Id == id || x.InternalId == objectId);
            #endregion

            try
            {
                DeleteResult actionResult
                    = await _context.Urls.DeleteOneAsync(
                Builders<TheUrl>.Filter.Where(x => x.Id == id || x.InternalId == objectId));

                return actionResult.IsAcknowledged && actionResult.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                _log.LogCritical($"RemoveUrl failed for id {id}", ex);
                throw ex;
            }
        }

        public async Task<bool> UpdateUrl(string id, long counter, DateTime lastAccessed)
        {
            var filter = Builders<TheUrl>.Filter.Eq(s => s.Id, id);
            var update = Builders<TheUrl>.Update
                            .Set(s => s.Counter, counter)
                            .Set(s => s.LastAccessed, lastAccessed)
                            .CurrentDate(s => s.CreatedDate);

            try
            {
                UpdateResult actionResult
                    = await _context.Urls.UpdateOneAsync(filter, update);

                return actionResult.IsAcknowledged
                    && actionResult.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                _log.LogCritical($"UpdateUrl failed for id {id}", ex);
                throw ex;
            }
        }

        public async Task<bool> UpdateUrl(string id, TheUrl item)
        {
            try
            {
                ReplaceOneResult actionResult
                    = await _context.Urls
                                    .ReplaceOneAsync(n => n.Id.Equals(id)
                                            , item
                                            , new UpdateOptions { IsUpsert = true });
                return actionResult.IsAcknowledged
                    && actionResult.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                _log.LogCritical($"UpdateUrl failed for id {id} and item: {item.ToString()}", ex);
                throw ex;
            }
        }

        public async Task<bool> RemoveAllUrls()
        {
            try
            {
                DeleteResult actionResult
                    = await _context.Urls.DeleteManyAsync(new BsonDocument());

                return actionResult.IsAcknowledged
                    && actionResult.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                _log.LogCritical($"RemoveAllUrls failed", ex);
                throw ex;
            }
        }

    }
}
