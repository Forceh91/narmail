using Windows.ApplicationModel;

namespace narmail.Mvvm
{
    public class FeedbackPageViewModel : BindableBase
    {
        private string _versionInfo = string.Empty;
        public string versionInfo { get { return _versionInfo; } set { SetProperty(ref _versionInfo, value); } }

        public FeedbackPageViewModel()
        {
            // get the version info
            Package currentPackage = Package.Current;
            PackageVersion packageVersion = currentPackage.Id.Version;

            // get the build string, assign it to the update notification thingy
            string build = string.Format("{0}.{1}.{2}.{3}", packageVersion.Major, packageVersion.Minor, packageVersion.Build, packageVersion.Revision);
            versionInfo = string.Format("Version: {0}", build);
        }
    }
}
