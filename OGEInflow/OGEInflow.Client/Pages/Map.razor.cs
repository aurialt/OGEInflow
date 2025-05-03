using Microsoft.AspNetCore.Components;
using OGEInflow.Services;

namespace OGEInflow.Client.Pages;

public partial class Map : ComponentBase
{

    public static MarkupString ReaderPin(ReaderEvent re)
    {
        string testString = "test";   
        string myMarkup = $"<div class=\"icon-container\" style=\"top: 320px; left: 30px;\">\n            " +
                          "<div class=\"pin\" style=\"--pin-color: rgb(0,200,100);\" @onclick=\"@TestOutputVal\"></div>\n" +
                          $"<div class=\"label\" >{testString}</div>\n</div>";
        return (MarkupString)myMarkup;
    }

    private int testoutput = 0;

    private  void TestOutputVal()
    {
        testoutput++;
    }
}