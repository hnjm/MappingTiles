﻿using System;
using System.Collections.ObjectModel;
using System.Globalization;

namespace MappingTiles
{
    public class OpenStreetMapTileSource : TileSource
    {
        private readonly Collection<string> OptionDomains = new Collection<string>() { "a", "b", "c" };

        private OpenStreetMapType mapType;

        public OpenStreetMapTileSource()
            : this(OpenStreetMapType.Standard)
        { }

        public OpenStreetMapTileSource(OpenStreetMapType mapType)
        {
            MapType = mapType;
            InitializeTileSchema(MapType);
        }

        public OpenStreetMapType MapType
        {
            get { return mapType; }
            set
            {
                if (mapType != value)
                {
                    InitializeTileSchema(value);

                    mapType = value;
                }
            }
        }

        protected override Uri GetUriCore(TileInfo tileInfo)
        {
            string requestUri = GetUriTemplate(MapType);

            requestUri.Replace("{s}", GetNextServerDomain(OptionDomains));
            requestUri.Replace("{z}", tileInfo.ZoomLevel.Id);
            requestUri.Replace("{x}", tileInfo.TileX.ToString(CultureInfo.InvariantCulture));
            requestUri.Replace("{y}", tileInfo.TileY.ToString(CultureInfo.InvariantCulture));

            return new Uri(requestUri);
        }

        private void InitializeTileSchema(OpenStreetMapType mapType)
        {
            Schema = new SphericalMercatorTileSchema();
            switch (mapType)
            {
                case OpenStreetMapType.CycleMap:
                    ((SphericalMercatorTileSchema)Schema).NumberOfZoomLevels = 17;
                    break;
                case OpenStreetMapType.TransportMap:
                    ((SphericalMercatorTileSchema)Schema).NumberOfZoomLevels = 19;
                    break;
                case OpenStreetMapType.Standard:
                    ((SphericalMercatorTileSchema)Schema).NumberOfZoomLevels = 19;
                    break;
                default:
                    break;
            }
        }

        private static string GetUriTemplate(OpenStreetMapType mapType)
        {
            string uriTemplate = "http://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png";
            switch (mapType)
            {
                case OpenStreetMapType.CycleMap:
                    uriTemplate = "http://{s}.tile.opencyclemap.org/cycle/{z}/{x}/{y}.png";
                    break;
                case OpenStreetMapType.TransportMap:
                    uriTemplate = "http://{s}.tile2.opencyclemap.org/transport/{z}/{x}/{y}.png";
                    break;
                case OpenStreetMapType.Standard:
                default:
                    break;
            }

            return uriTemplate;
        }
    }
}
