﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PgpCore.Abstractions
{
    public interface ISignAsync : IDisposable
    {
        Task SignFileAsync(FileInfo inputFile, FileInfo outputFile, bool armor = true, string name = null, IDictionary<string, string> headers = null);

        Task SignStreamAsync(Stream inputStream, Stream outputStream, bool armor = true, string name = null, IDictionary<string, string> headers = null);

        Task<string> SignArmoredStringAsync(string input, bool armor = true, string name = null, IDictionary<string, string> headers = null);

        Task ClearSignFileAsync(FileInfo inputFile, FileInfo outputFile, IDictionary<string, string> headers = null);

        Task ClearSignStreamAsync(Stream inputStream, Stream outputStream, IDictionary<string, string> headers = null);

        Task<string> ClearSignArmoredStringAsync(string input, IDictionary<string, string> headers = null);
    }
}
