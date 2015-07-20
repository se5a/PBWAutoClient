using System;
using System.Collections.Generic;
using System.IO;
using SevenZip;

namespace PBW2AutoPlrClient
{
    class ArchiveHandler
    {   
        ///<summary>
        ///checks weather running 64 or 32
        ///and selecets the correct dll
        ///<\summery>
        ///
        public static int setLibPath()
        {


	        if (IntPtr.Size == 8) // 64-bit
	        {
		        SevenZipCompressor.SetLibraryPath("7z64.dll");
		        SevenZipExtractor.SetLibraryPath("7z64.dll");
            
	        }
	        else
	        {
		        SevenZipCompressor.SetLibraryPath("7z.dll");
		        SevenZipExtractor.SetLibraryPath("7z.dll");
	        }
            return 1;

        }

        public static int extractArchive(string inFileName, string extractPath)
        {
            // extract input archive
            FileStream fileStream;
            SevenZipExtractor extractor = null;
            try
            {
                extractor = new SevenZipExtractor(File.OpenRead(inFileName));

                foreach (var fileName in extractor.ArchiveFileNames)
                {
                    // you of course can extract to a file stream instead of a memory stream if you want :)
                    //var memoryStream = new MemoryStream();
                    //extractor.ExtractFile(fileName, memoryStream);
                    // do what you want with your file here :)
                    fileStream = File.Create(Path.Combine(extractPath, fileName));
                    


                    extractor.ExtractFile(fileName, fileStream);


                    int buffersize = 1024;
                    byte[] buffer = new byte[buffersize];
                    int bytesRead = 0;


                    while ((bytesRead = fileStream.Read(buffer, 0, buffersize)) != 0)
                    {
                        fileStream.Write(buffer, 0, bytesRead);
                    } // end while
                    fileStream.Close();
                }
                extractor.Dispose();
               
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Error: Could not open input archive: " + inFileName);
                Console.Error.WriteLine("\t" + ex.Message);
                
            }
            finally
            {
                
            }
            return 1;
        }

        public int CompressArchive(Dictionary<string, Stream> files, string outFileName)
        {
            
            // create output archive
			SevenZipCompressor compressor = new SevenZipCompressor();
			try
			{
				var dict = new Dictionary<string, Stream>();

				foreach (var fileName in files.Keys)
					dict.Add(fileName, files[fileName]);
				compressor.CompressStreamDictionary(dict, outFileName);
                return 0;

			}
			catch (Exception ex)
			{
				Console.Error.WriteLine("Error: Could not create output archive: " + outFileName);

				Console.Error.WriteLine("\t" + ex.Message);
				return 1;
			}
        }
    }
}
