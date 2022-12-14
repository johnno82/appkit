namespace AdMaiora.AppKit.IO
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.Linq;

    public enum UniversalFileMode
    {
        CreateNew = 1,
        Create = 2,
        Open = 3,
        OpenOrCreate = 4,
        Truncate = 5,
        Append = 6
    }

    public enum UniversalFileAccess
    {
        Read = 1,
        Write = 2,
        ReadWrite = 3
    }

    public enum UniversalFileShare
    {
        None,
        Read,
        Write,
        ReadWrite,
        Delete,
        Inheritable
    }

    public class UniversalFileInfo
    {
        public DateTime CreationTime
        {
            get;
            set;
        }

        public DateTime LastAccessTime
        {
            get;
            set;
        }

        public DateTime LastWriteTime
        {
            get;
            set;
        }

        public ulong Length
        {
            get;
            set;
        }
    }

    public class FileSystem
    {
        #region Constants & Fields

        protected IFileSystemPlatform _fileSystemPlatform;

        #endregion

        #region Constructors

        public FileSystem(IFileSystemPlatform fileSystemPlatform)
        {
            _fileSystemPlatform = fileSystemPlatform;
        }

        #endregion

        #region Public Methods

        public FileUri CreateFileUri(string uri)
        {
            return new FileUri(_fileSystemPlatform, uri);
        }

        public FileUri CreateFileUri(string path, StorageLocation location)
        {
            return new FileUri(_fileSystemPlatform, path, location);
        }

        public FolderUri CreateFolderUri(string uri)
        {
            return new FolderUri(_fileSystemPlatform, uri);
        }

        public FolderUri CreateFolderUri(string path, StorageLocation location)
        {
            return new FolderUri(_fileSystemPlatform, path, location);
        }

        public ulong GetAvailableDiskSpace(FolderUri uri)
        {
            return _fileSystemPlatform.GetAvailableDiskSpace(uri);
        }

        public ulong GetFileSize(FileUri uri)
        {
            return _fileSystemPlatform.GetFileSize(uri);
        }

        public UniversalFileInfo GetFileInfo(FileUri uri)
        {
            return _fileSystemPlatform.GetFileInfo(uri);
        }

        public bool FolderExists(FolderUri uri)
        {
            return _fileSystemPlatform.FolderExists(uri);
        }

        public void CreateFolder(FolderUri uri)
        {
            if (uri.Location == StorageLocation.Bundle)
                throw new InvalidOperationException("Unable to create folder inside the bundle.");

            _fileSystemPlatform.CreateFolder(uri);
        }

        public void DeleteFolder(FolderUri uri)
        {
            if (uri.Location == StorageLocation.Bundle)
                throw new InvalidOperationException("Unable to delete folder inside the bundle.");

            _fileSystemPlatform.DeleteFolder(uri);
        }

        public FileUri[] GetFolderFiles(FolderUri uri, string searchPattern, bool recursive)
        {
            string[] filePaths = _fileSystemPlatform.GetFolderFiles(uri, searchPattern, recursive);
            if (filePaths == null)
                return null;

            List<FileUri> fileUris = filePaths
                .Select(x => CreateFileUri(Path.Combine(uri.Uri, x.Replace(uri.AbsolutePath + "/", String.Empty))))
                .ToList();

            return fileUris.ToArray();
        }

        public bool FileExists(FileUri uri)
        {
            return _fileSystemPlatform.FileExists(uri);
        }

        public void CopyFile(FileUri sourceUri, FileUri destinationUri, bool overwrite)
        {
            if (destinationUri.Location == StorageLocation.Bundle)
                throw new InvalidOperationException("Unable to copy file inside the bundle.");

            _fileSystemPlatform.CopyFile(sourceUri, destinationUri, overwrite);
        }

        public void MoveFile(FileUri sourceUri, FileUri destinationUri)
        {
            if (destinationUri.Location == StorageLocation.Bundle)
                throw new InvalidOperationException("Unable to copy file inside the bundle.");

            _fileSystemPlatform.MoveFile(sourceUri, destinationUri);
        }

        public void DeleteFile(FileUri uri)
        {
            if (uri.Location == StorageLocation.Bundle)
                throw new InvalidOperationException("Unable to delete file inside the bundle.");

            _fileSystemPlatform.DeleteFile(uri);
        }

        public Stream OpenFile(FileUri uri, UniversalFileMode mode, UniversalFileAccess access, UniversalFileShare share)
        {
            return _fileSystemPlatform.OpenFile(uri, mode, access, share);
        }

        public Stream OpenFile(FileUri uri, UniversalFileMode mode, UniversalFileAccess access)
        {
            return OpenFile(uri, mode, access, UniversalFileShare.None);
        }

        public Stream OpenFile(FileUri uri)
        {
            return OpenFile(uri, UniversalFileMode.Open, UniversalFileAccess.Read);
        }

        public string GetMimeType(string fileUriOrExt)
        {
            if (String.IsNullOrWhiteSpace(fileUriOrExt))
                return null;
            
            return _fileSystemPlatform.GetMimeType(fileUriOrExt);
        }

        #endregion
    }
}
