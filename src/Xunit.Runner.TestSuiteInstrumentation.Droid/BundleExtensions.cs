using Android.OS;

namespace Xunit.Runners.TestSuiteInstrumentation
{
    public static class BundleExtensions
    {
        public static Bundle WithValue(this Bundle bundle, string key, string value)
        {
            bundle.PutString(key, value);
            return bundle;
        }

        public static Bundle WithValue(this Bundle bundle, string key, int value)
        {
            bundle.PutInt(key, value);
            return bundle;
        }
    }

}
