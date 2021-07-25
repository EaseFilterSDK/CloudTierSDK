using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Threading;
using System.IO;
using System.Net;
using System.Linq;

using EaseFilter.GlobalObjects;

namespace EaseFilter.CloudManager
{

    // A modified version of the ProgressStream from http://blogs.msdn.com/b/paolos/archive/2010/05/25/large-message-transfer-with-wcf-adapters-part-1.aspx
    // This class allows progress changed events to be raised from the block upload/download.
    public class ProgressStream : Stream
    {
        #region Private Fields
        private Stream stream;
        private long bytesTransferred;
        private long totalLength;

        #endregion

        #region Public Handler
        public event EventHandler<ProgressChangedEventArgs> ProgressChanged;
        #endregion

        #region Public Constructor
        public ProgressStream(Stream file)
        {
            this.stream = file;
            this.totalLength = file.Length;
            this.bytesTransferred = 0;
        }
        #endregion

        #region Public Properties
        public override bool CanRead
        {
            get
            {
                return this.stream.CanRead;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return this.stream.CanSeek;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return this.stream.CanWrite;
            }
        }

        public override void Flush()
        {
            this.stream.Flush();
        }

        public override void Close()
        {
            this.stream.Close();
        }

        public override long Length
        {
            get
            {
                return this.stream.Length;
            }
        }

        public override long Position
        {
            get
            {
                return this.stream.Position;
            }
            set
            {
                this.stream.Position = value;
            }
        }


        #endregion

        #region Public Methods
        public override int Read(byte[] buffer, int offset, int count)
        {
            int result = 0;
            lock (this)
            {
                long currentOffset = stream.Position;

                if (stream.Position < stream.Length)
                {
                    result = stream.Read(buffer, offset, count);
                }
                else
                {
                    result = 0;
                }

            }

            bytesTransferred += result;
            if (ProgressChanged != null)
            {
                try
                {
                    OnProgressChanged(new ProgressChangedEventArgs(bytesTransferred, totalLength,result));
                }
                catch (Exception)
                {
                    ProgressChanged = null;
                }
            }
            return result;
        }

        protected virtual void OnProgressChanged(ProgressChangedEventArgs e)
        {
            if (ProgressChanged != null)
                ProgressChanged(this, e);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return this.stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            totalLength = value;
            //this.stream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            lock (this)
            {
               this.stream.Write(buffer, offset, count);
            }

            bytesTransferred += count;
            {
                try
                {
                    OnProgressChanged(new ProgressChangedEventArgs(bytesTransferred, totalLength,count));
                }
                catch (Exception)
                {
                    ProgressChanged = null;
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            stream.Dispose();
            base.Dispose(disposing);
        }

        #endregion
    }

  

    public class ProgressChangedEventArgs : EventArgs
    {
        #region Private Fields
        private long bytesRead;
        private long totalLength;
        private int bytesChanged;
        #endregion

        #region Public Constructor
        public ProgressChangedEventArgs(long bytesRead, long totalLength, int bytesChanged)
        {
            this.bytesRead = bytesRead;
            this.totalLength = totalLength;
            this.bytesChanged = bytesChanged;
        }
        #endregion

        #region Public properties

        public long BytesRead
        {
            get
            {
                return this.bytesRead;
            }
            set
            {
                this.bytesRead = value;
            }
        }

        public long TotalLength
        {
            get
            {
                return this.totalLength;
            }
            set
            {
                this.totalLength = value;
            }
        }

        public int BytesChanged
        {
            get
            {
                return this.bytesChanged;
            }
            set
            {
                this.bytesChanged = value;
            }
        }
        #endregion
    }

    public enum TransferTypeEnum
    {
        Download,
        Upload
    }

}
