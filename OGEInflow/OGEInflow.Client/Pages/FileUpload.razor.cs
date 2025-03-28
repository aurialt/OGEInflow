using OGEInflow.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace OGEInflow.Client.Pages;


public partial class FileUpload : ComponentBase
{
    public static string FileName { get; set; } = "";
    public static long FileSize { get; set; }
    public static string FileType { get; set; } = "";
    public static DateTimeOffset LastModified { get; set; }
    public static string ErrorMessage { get; set; } = "";
    
    const int MAX_FILESIZE = 5000 * 1024; // 2 MB
    
    public static async Task FileUploaded(InputFileChangeEventArgs e)
    {
        var browserFiles = e.GetMultipleFiles();

        foreach (var browserFile in browserFiles)
        {
            if (browserFile != null)
            {
                FileSize = browserFile.Size;
                FileType = browserFile.ContentType;
                FileName = browserFile.Name;
                LastModified = browserFile.LastModified;

                try
                {
                    var fileStream = browserFile.OpenReadStream(MAX_FILESIZE);

                    var projectRoot = Directory.GetCurrentDirectory();
                    var uploadFolder = Path.Combine(projectRoot, "Files");
                    Directory.CreateDirectory(uploadFolder);
                    var targetFilePath = Path.Combine(uploadFolder, FileName); 

                    var destinationStream = new FileStream(targetFilePath, FileMode.Create);
                    await fileStream.CopyToAsync(destinationStream);
                    destinationStream.Close();

                    Console.WriteLine("File uploaded Name:" + FileName);
                    Console.WriteLine("File project root" + projectRoot);
                    Console.WriteLine("Target file path: " + targetFilePath);
                    
                    PopulateReaderEvents(targetFilePath);
                    
                }
                catch (Exception exception)
                {
                    ErrorMessage = exception.Message;
                }
            }
        }
    }
    
    
    /* ReaderEvent Section */
    public static void PopulateReaderEvents(string file)
    {
        if (Path.Exists(file) && Path.GetExtension(file) == ".csv")
        {
            using (StreamReader sr = new StreamReader(file))
            {
                string str = sr.ReadLine();
                while ((str = sr.ReadLine()) != null) // Skips the first line since ReadLine was already Called
                {
                    List<string> fields = str.Split(',').ToList();
                    
                    if (fields.Count >= 6)
                    {
                        ReaderEvent re = new ReaderEvent(fields[0], fields[1], fields[2], fields[3], fields[4], fields[5]);
                        ReaderEvent.readerEventsList.Add(re);
                    }
                    else
                    {
                        Console.WriteLine($"Skipping invalid row: {str}");
                    }
                }
            }
            
            ReaderEvent.GenerateDictionaries();

            Console.WriteLine("Events populated");
            Console.WriteLine($"Total events: {ReaderEvent.readerEventsList.Count}");
        }
        else
        {
            ErrorMessage = $"File {file} does not exist or is not a .csv file.";
        }
    }
}