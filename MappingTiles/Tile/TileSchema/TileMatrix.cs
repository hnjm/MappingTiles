﻿using System;
using System.Collections.ObjectModel;

namespace MappingTiles
{
    public class TileMatrix
    {
        private TileMatrix()
        { }

        public TileMatrix(double resolution, TileSchema tileScehma)
            : this(resolution, tileScehma, Utility.CreateUniqueId())
        { }

        public TileMatrix(double resolution, TileSchema tileSchema, string id)
        {
            this.ZoomLevel = new ZoomLevel(resolution);
            this.TileSchema = tileSchema;
            this.Id = id;

            this.TileWidth = 256;
            this.TileHeight = 256;
        }

        public string Id
        {
            get;
            private set;
        }

        public TileSchema TileSchema
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the resolution, scale of this tile matrix.
        /// </summary>
        public ZoomLevel ZoomLevel
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the width of the tile in the matrix.
        /// </summary>
        public int TileWidth
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the height of the tile in the matrix.
        /// </summary>
        public int TileHeight
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the width of the whole tile matrix, which covers the world boundingBox of the corresponding tile schema.
        /// </summary>
        public int Width
        {
            get
            {
                return (int)Math.Ceiling(TileSchema.MaxExtent.Width / ZoomLevel.Resolution);
            }
        }

        /// <summary>
        /// Gets or sets the height of the whole tile matrix, which covers the world boundingBox of the corresponding tile schema.
        /// </summary>
        public int Height
        {
            get
            {
                return (int)Math.Ceiling(TileSchema.MaxExtent.Height / ZoomLevel.Resolution);
            }
        }

        public Collection<TileInfo> GetTiles(BoundingBox boundingBox)
        {
            var tileWorldUnits = ZoomLevel.Resolution * TileWidth;

            Collection<TileInfo> tiles = new Collection<TileInfo>();

            var tileRange = new TileRange(-1, -1);
            if (TileSchema.IsYAxisReversed)
            {
                var firstCol = (int)Math.Floor((boundingBox.MinX - TileSchema.MaxExtent.MinX) / tileWorldUnits);
                var firstRow = (int)Math.Floor((-boundingBox.MaxY + TileSchema.MaxExtent.MaxY) / tileWorldUnits);
                var lastCol = (int)Math.Ceiling((boundingBox.MaxX - TileSchema.MaxExtent.MinX) / tileWorldUnits);
                var lastRow = (int)Math.Ceiling((-boundingBox.MinY + TileSchema.MaxExtent.MaxY) / tileWorldUnits);

                tileRange = new TileRange(firstCol, firstRow, lastCol - firstCol, lastRow - firstRow);
            }
            else
            {
                var firstCol = (int)Math.Floor((boundingBox.MinX - TileSchema.MaxExtent.MinX) / tileWorldUnits);
                var firstRow = (int)Math.Floor((boundingBox.MinY - TileSchema.MaxExtent.MaxY) / tileWorldUnits);
                var lastCol = (int)Math.Ceiling((boundingBox.MaxX - TileSchema.MaxExtent.MinX) / tileWorldUnits);
                var lastRow = (int)Math.Ceiling((boundingBox.MaxY - TileSchema.MaxExtent.MaxY) / tileWorldUnits);

                tileRange = new TileRange(firstCol, firstRow, lastCol - firstCol, lastRow - firstRow);
            }

            for (var x = tileRange.StartColumn; x < tileRange.StartColumn + tileRange.NumberOfColumns; x++)
            {
                for (var y = tileRange.StartRow; y < tileRange.StartRow + tileRange.NumberOfRows; y++)
                {
                    tiles.Add(new TileInfo(x, y, ZoomLevel.Resolution, TileSchema));
                }
            }

            return tiles;
        }

        public BoundingBox GetBoundingBox(TileRange range)
        {
            var resolution = ZoomLevel.Resolution;
            if (TileSchema.IsYAxisReversed)
            {
                var tileWorldUnits = resolution * TileWidth;
                var minX = range.StartColumn * tileWorldUnits + TileSchema.MaxExtent.MinX;
                var minY = -(range.StartRow + range.NumberOfRows) * tileWorldUnits + TileSchema.MaxExtent.MaxY;
                var maxX = (range.StartColumn + range.NumberOfColumns) * tileWorldUnits + TileSchema.MaxExtent.MinX;
                var maxY = -(range.StartRow) * tileWorldUnits + TileSchema.MaxExtent.MaxY;

                return new BoundingBox(minX, minY, maxX, maxY);
            }
            else
            {
                var tileWorldUnits = resolution * TileWidth;
                var minX = range.StartColumn * tileWorldUnits + TileSchema.MaxExtent.MinX;
                var minY = range.StartRow * tileWorldUnits + TileSchema.MaxExtent.MaxY;
                var maxX = (range.StartColumn + range.NumberOfColumns) * tileWorldUnits + TileSchema.MaxExtent.MinX;
                var maxY = (range.StartRow + range.NumberOfRows) * tileWorldUnits + TileSchema.MaxExtent.MaxY;

                return new BoundingBox(minX, minY, maxX, maxY);
            }
        }
    }
}
