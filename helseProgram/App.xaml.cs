using System.Windows;
using helseProgram.Data;
using Microsoft.EntityFrameworkCore;

namespace helseProgram;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        using var db = new AppDbContext();

        //Ado, migrerer databasen
        db.Database.Migrate();


        // Legger inn manglende medikamenter i databasen
        MedikamentData.Seed(db);
    }
}