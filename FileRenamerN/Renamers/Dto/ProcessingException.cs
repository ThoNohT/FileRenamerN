// ---------------------------------------------------------------------------------------------------------------------
//  <copyright file="ProcessingException.cs" company="Prodrive B.V.">
//      Copyright (c) Prodrive B.V. All rights reserved.
//  </copyright>
// ---------------------------------------------------------------------------------------------------------------------
namespace FileRenamerN.Renamers.Dto
{
    using System;

    /// <summary>
    /// An exception that occurs during processing of a file by a renamer.
    /// </summary>
    public class ProcessingException : Exception
    {
        public ProcessingException(string message) : base(message)
        {
        }
    }
}