using System;
using System.Collections.Generic;
using System.Text;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.Enums;

namespace Avalanche.Services
{
    class MediaFile : IMediaFile
    {
        public MediaFileType Type { get; set; }
        public ResourceAvailability Availability { get; set; }
        public IMediaFileMetadata Metadata { get; set; }
        public string Url { get; set; }
        public bool MetadataExtracted { get; set; }

        public event MetadataUpdatedEventHandler MetadataUpdated;
    }
}
