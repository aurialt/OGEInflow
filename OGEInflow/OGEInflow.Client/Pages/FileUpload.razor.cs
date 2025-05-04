using OGEInflow.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using OGEInflow.Client.Services;
namespace OGEInflow.Client.Pages;


public partial class FileUpload : ComponentBase
{
    public static string FileName { get; set; } = "";
    public static long FileSize { get; set; }
    public static string FileType { get; set; } = "";
    public static DateTimeOffset LastModified { get; set; }
    public static string ErrorMessage { get; set; } = "";
    
    
    public static bool isFileUploaded { get; set; } = false;
    private bool showLoaderSpinner = false;
    
    const int MAX_FILESIZE = 5000 * 1024; // 5 MB
    
    [Inject]
    NavigationManager NavigationManager { get; set; }
    
    private async Task FileUploaded(InputFileChangeEventArgs e)
    {
        var browserFiles = e.GetMultipleFiles();
        showLoaderSpinner = true;

        ReaderEvent.MinDate = DateTime.MaxValue;
        ReaderEvent.MaxDate = DateTime.MinValue;
        WarningManager.ClearWarnings();
        
        foreach (var browserFile in browserFiles)
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
                isFileUploaded = true;
                showLoaderSpinner = false;
                NavigationManager.NavigateTo("/activity");
                    
            }
            catch (Exception exception)
            {
                ErrorMessage = exception.Message;
            }
        }
    }
    
    
    /* ReaderEvent Section */
    private void PopulateReaderEvents(string file)
    {
        if (Path.Exists(file) && Path.GetExtension(file) == ".csv")
        {
            using (StreamReader sr = new StreamReader(file))
            {
                ReaderEvent.readerEventsList.Clear();
                string str = sr.ReadLine();
                while ((str = sr.ReadLine()) != null) // Skips the first line since ReadLine was already Called
                {
                    List<string> fields = str.Split(',').ToList();
                    DateTime eventTime = DateTime.Parse(fields[0]);
                    if (fields.Count >= 6)
                    {
                        ReaderEvent re = new ReaderEvent(eventTime, fields[1], fields[2], fields[3], fields[4], fields[5]);
                        ReaderEvent.readerEventsList.Add(re);
                        
                        CheckMinMaxDate(eventTime);
                    }
                    else
                    {
                        Console.WriteLine($"Skipping invalid row: {str}");
                    }
                }
            }
            
            ReaderEvent.GenerateDictionaries();
            // ReaderEvent.printDescList();

            Console.WriteLine("Events populated");
            Console.WriteLine($"Total events: {ReaderEvent.readerEventsList.Count}");
            RunStartUpMethods();
            
        }
        else
        {
            ErrorMessage = $"File {file} does not exist or is not a .csv file.";
        }
    }
    
    
    
    /* Miscellaneous Section */
    private void CheckMinMaxDate(DateTime dateString)
    {
        try
        {
            DateTime fullDateTime = dateString.ToUniversalTime();
            DateTime dateOnly = fullDateTime.Date;
            
            if (dateOnly > ReaderEvent.MaxDate)
            {
                ReaderEvent.MaxDate = dateOnly;
                // Console.WriteLine($"Max date Updated: {ReaderEvent.MaxDate}");
            }
            else if (ReaderEvent.MinDate == DateTime.MinValue || dateOnly < ReaderEvent.MinDate)
            {
                ReaderEvent.MinDate = dateOnly;
                // Console.WriteLine($"Min date Updated: {ReaderEvent.MinDate}");
            }
        }
        catch (FormatException)
        {
            ErrorMessage = $"Invalid date format: {dateString}";
        }
    }
    
    //Start-ups
    private void RunStartUpMethods()
    {
        Activity.InitializeDateBounds(ReaderEvent.MinDate, ReaderEvent.MaxDate);
        WarningManager.LoadWarnings();
    }
}


