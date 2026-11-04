using System.Windows;
using helseProgram.Data;

namespace helseProgram;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        using var db = new AppDbContext();

        // Legger inn manglende medikamenter i databasen
        MedikamentData.Seed(db);
    }
}