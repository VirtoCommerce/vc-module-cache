using System;
using System.IO;
using VirtoCommerce.CacheModule.Data.Services;
using VirtoCommerce.ContentModule.Data.Services;
using VirtoCommerce.Platform.Core.Assets;

namespace VirtoCommerce.CacheModule.Data.Decorators
{
    public class ContentBlobStorageProviderDecorator : IContentBlobStorageProvider
    {
        private readonly IContentBlobStorageProvider _contentBlobStorageProvider;
        private readonly IChangesTrackingService _changesTrackingService;

        public ContentBlobStorageProviderDecorator(IContentBlobStorageProvider contentBlobStorageProvider, IChangesTrackingService changesTrackingService)
        {
            _contentBlobStorageProvider = contentBlobStorageProvider;
            _changesTrackingService = changesTrackingService;
        }

        #region IStoreService Members
        public void CopyContent(string srcUrl, string destUrl)
        {
            _contentBlobStorageProvider.CopyContent(srcUrl, destUrl);
            UpdateChangesTracking();
        }

        public void CreateFolder(BlobFolder folder)
        {
            _contentBlobStorageProvider.CreateFolder(folder);
        }

        public BlobInfo GetBlobInfo(string blobUrl)
        {
            return _contentBlobStorageProvider.GetBlobInfo(blobUrl);
        }

        public void MoveContent(string srcUrl, string destUrl)
        {
            _contentBlobStorageProvider.MoveContent(srcUrl, destUrl);
            UpdateChangesTracking();
        }

        public Stream OpenRead(string blobUrl)
        {
            return _contentBlobStorageProvider.OpenRead(blobUrl);
        }

        public Stream OpenWrite(string blobUrl)
        {
            UpdateChangesTracking();
            return _contentBlobStorageProvider.OpenWrite(blobUrl);
        }

        public void Remove(string[] urls)
        {
            _contentBlobStorageProvider.Remove(urls);
            UpdateChangesTracking();
        }

        public BlobSearchResult Search(string folderUrl, string keyword)
        {
            return _contentBlobStorageProvider.Search(folderUrl, keyword);
        }
        #endregion

        private void UpdateChangesTracking()
        {
            _changesTrackingService.Update(null, DateTime.Now.AddMinutes(-1));
        }
    }
}
