﻿using Org.BouncyCastle.Bcpg;
using PgpCore.Abstractions;
using PgpCore.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PgpCore
{
    public partial class PGP : ISignSync
    {
        #region SignFile
        /// <summary>
        /// Sign the file pointed to by unencryptedFileInfo
        /// </summary>
        /// <param name="inputFile">Plain data file to be signed</param>
        /// <param name="outputFile">Output PGP signed file</param>
        /// <param name="armor">True, means a binary data representation as an ASCII-only text. Otherwise, false</param>
        public void SignFile(FileInfo inputFile, FileInfo outputFile,
            bool armor = true)
        {
            if (inputFile == null)
                throw new ArgumentException("InputFile");
            if (outputFile == null)
                throw new ArgumentException("OutputFile");
            if (EncryptionKeys == null)
                throw new ArgumentException("EncryptionKeys");

            if (!inputFile.Exists)
                throw new FileNotFoundException($"Input file [{inputFile.FullName}] does not exist.");

            using (Stream outputStream = outputFile.OpenWrite())
            {
                if (armor)
                {
                    using (ArmoredOutputStream armoredOutputStream = new ArmoredOutputStream(outputStream))
                    {
                        OutputSigned(inputFile, armoredOutputStream);
                    }
                }
                else
                    OutputSigned(inputFile, outputStream);
            }
        }

        #endregion SignFile

        #region SignStream
        /// <summary>
        /// Sign the stream pointed to by unencryptedFileInfo and
        /// </summary>
        /// <param name="inputStream">Plain data stream to be signed</param>
        /// <param name="outputStream">Output PGP signed stream</param>
        /// <param name="armor">True, means a binary data representation as an ASCII-only text. Otherwise, false</param>
        /// <param name="name">Name of signed file in message, defaults to the input file name</param>
        public void SignStream(Stream inputStream, Stream outputStream,
            bool armor = true, string name = DefaultFileName)
        {
            if (inputStream == null)
                throw new ArgumentException("InputStream");
            if (outputStream == null)
                throw new ArgumentException("OutputStream");
            if (EncryptionKeys == null)
                throw new ArgumentException("EncryptionKeys");
            if (inputStream.Position != 0)
                throw new ArgumentException("inputStream should be at start of stream");

            if (name == DefaultFileName && inputStream is FileStream fileStream)
            {
                string inputFilePath = fileStream.Name;
                name = Path.GetFileName(inputFilePath);
            }

            if (armor)
            {
                using (ArmoredOutputStream armoredOutputStream = new ArmoredOutputStream(outputStream))
                {
                    OutputSigned(inputStream, armoredOutputStream, name);
                }
            }
            else
                OutputSigned(inputStream, outputStream, name);
        }

        #endregion SignStream

        #region SignArmoredString
        /// <summary>
        /// Sign the string
        /// </summary>
        /// <param name="input">Plain string to be signed</param>
        /// <param name="name">Name of signed file in message, defaults to the input file name</param>
        public string SignArmoredString(string input, string name = DefaultFileName)
        {
            using (Stream inputStream = input.GetStream())
            using (Stream outputStream = new MemoryStream())
            {
                SignStream(inputStream, outputStream, true, name);
                outputStream.Seek(0, SeekOrigin.Begin);
                return outputStream.GetString();
            }
        }

        #endregion SignArmoredString

        #region ClearSignFile
        /// <summary>
        /// Clear sign the file pointed to by unencryptedFileInfo
        /// </summary>
        /// <param name="inputFile">Plain data file to be signed</param>
        /// <param name="outputFile">Output PGP signed file</param>
        public void ClearSignFile(FileInfo inputFile, FileInfo outputFile)
        {
            if (inputFile == null)
                throw new ArgumentException("InputFile");
            if (outputFile == null)
                throw new ArgumentException("OutputFile");
            if (EncryptionKeys == null)
                throw new ArgumentException("EncryptionKeys");

            if (!inputFile.Exists)
                throw new FileNotFoundException($"Input file [{inputFile.Name}] does not exist.");

            using (Stream outputStream = outputFile.OpenWrite())
            {
                OutputClearSigned(inputFile, outputStream);
            }
        }

        #endregion ClearSignFile

        #region ClearSignStream
        /// <summary>
        /// Clear sign the provided stream
        /// </summary>
        /// <param name="inputStream">Plain data stream to be signed</param>
        /// <param name="outputStream">Output PGP signed stream</param>
        public void ClearSignStream(Stream inputStream, Stream outputStream)
        {
            if (inputStream == null)
                throw new ArgumentException("InputStream");
            if (outputStream == null)
                throw new ArgumentException("OutputStream");
            if (EncryptionKeys == null)
                throw new ArgumentException("EncryptionKeys");
            if (inputStream.Position != 0)
                throw new ArgumentException("inputStream should be at start of stream");

            OutputClearSigned(inputStream, outputStream);
        }

        #endregion ClearSignStream

        #region ClearSignArmoredString
        /// <summary>
        /// Clear sign the provided string
        /// </summary>
        /// <param name="input">Plain string to be signed</param>
        public string ClearSignArmoredString(string input)
        {
            using (Stream inputStream = input.GetStream())
            using (Stream outputStream = new MemoryStream())
            {
                ClearSignStream(inputStream, outputStream);
                outputStream.Seek(0, SeekOrigin.Begin);
                return outputStream.GetString();
            }
        }

        #endregion ClearSignArmoredString
    }
}
