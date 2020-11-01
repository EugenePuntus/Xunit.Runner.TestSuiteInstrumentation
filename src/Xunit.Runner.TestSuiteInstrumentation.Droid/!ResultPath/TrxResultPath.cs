using Android.App;
using Android.OS;
using System.IO;

namespace Xunit.Runners.TestSuiteInstrumentation
{
    public class TrxResultPath : IResultPath
    {
        public string Path()
        {
            Java.IO.File externalFilesDir = GetExternalFilesDir();
            bool num = externalFilesDir?.Exists() ?? false;
            string text = num ? externalFilesDir.AbsolutePath : System.IO.Path.Combine(Application.Context!.FilesDir!.AbsolutePath, ".__override__");
            if (!num && !Directory.Exists(text))
            {
                Directory.CreateDirectory(text);
            }
            return System.IO.Path.Combine(text, "TestResults.trx");
        }

        private Java.IO.File GetExternalFilesDir()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.Kitkat)
            {
                return null;
            }
            string text = null;
            text = Android.OS.Environment.DirectoryDocuments;
            return Application.Context!.GetExternalFilesDir(text);
        }
    }
}
