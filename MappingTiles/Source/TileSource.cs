﻿using System;
using System.Collections.ObjectModel;
using System.Net;

namespace MappingTiles
{
    public abstract class TileSource : Source
    {
        private readonly object counterLocker = new object();
        private readonly ITileCache<byte[]> tileCache;
        private readonly TileDownloader tileDownloader;

        private int counter;

        protected TileSource()
        { }

        protected TileSource(string id)
            : this(new Wgs84TileSchema())
        { }

        protected TileSource(TileSchema tileSchema)
            : this(tileSchema, null, null)
        { }

        protected TileSource(TileSchema tileSchema, TileDownloader tileDownloaer)
            : this(tileSchema, null, tileDownloaer)
        { }

        protected TileSource(TileSchema tileSchema, ITileCache<byte[]> tileCache, TileDownloader tileDownloader)
            : this(tileSchema, tileCache, tileDownloader, Utility.CreateUniqueId())
        { }

        protected TileSource(TileSchema tileSchema, ITileCache<byte[]> tileCache, TileDownloader tileDownloader, string id)
            : base(id)
        {
            this.Schema = tileSchema;
            this.tileCache = tileCache ?? new MemoryTileCache<byte[]>();
            this.tileDownloader = this.tileDownloader ?? new ImageTileDownloader(tileSchema);
        }

        public TileSchema Schema
        {
            get;
            protected set;
        }

        public TileFormat Format
        {
            get
            {
                return Schema.TileFormat;
            }
            set
            {
                Schema.TileFormat = value;
            }
        }

        public ITileCache<byte[]> TileCache
        {
            get
            {
                return tileCache;
            }
        }

        public virtual void DownloadTile(TileInfo tileInfo, AsyncTileRequestCompletedHandler callback)
        {
            Uri tileUri = GetUri(tileInfo);
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(tileUri);

            this.tileDownloader.Download(tileInfo, this, callback);
        }

        public Uri GetUri(TileInfo tileInfo)
        {
            return GetUriCore(tileInfo);
        }

        protected abstract Uri GetUriCore(TileInfo tileInfo);

        internal string GetNextServerDomain(Collection<string> serverDomains)
        {
            lock (counterLocker)
            {
                var selectedDomain = serverDomains[counter++];
                if (counter >= serverDomains.Count)
                {
                    counter = 0;
                }

                return selectedDomain;
            }
        }
    }
}
