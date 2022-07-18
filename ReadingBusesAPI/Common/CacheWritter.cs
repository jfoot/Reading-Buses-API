// Copyright (c) Jonathan Foot. All Rights Reserved. 
// Licensed under the GNU Affero General Public License, Version 3.0 
// See the LICENSE file in the project root for more information.

using System;
using System.IO;

namespace ReadingBusesAPI.Common
{

	/// <summary>
	/// Used to write a cache files to the disk.
	/// </summary>
	internal class CacheWritter
	{
		/// <summary>
		/// Saves a cache file to local disk, and hides the folder so the user cannot see it.
		/// </summary>
		/// <param name="fileLoc">The location for the cache file.</param>
		/// <param name="fileName">The name of the file</param>
		/// <param name="content">The contents of the file.</param>
		public static void WriteToCache(string fileLoc, string fileName, string content)
		{
			//Checks that the directory exists or not, if not make it.
			if (!Directory.Exists(fileLoc))
			{
				Directory.CreateDirectory(fileLoc);
				_ = new DirectoryInfo(fileLoc) { Attributes = FileAttributes.Hidden };
			}
			//Then actually save the file.
			File.WriteAllText(fileLoc + "/" + fileName, content);
		}
	}
}
