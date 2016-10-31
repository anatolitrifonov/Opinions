using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BestFor.Dto
{
    /// <summary>
    /// Used by BlobService to transfer data in and out.
    /// </summary>
    public class BlobDataDto
    {
        /// <summary>
        /// Blob file name. We only have blobs that are files for now.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Blob data
        /// </summary>
        public Stream Stream { get; set; }
    }
}
