using System;

namespace DontMergeMeYet.Data
{
    public class RepositoryInstallation
    {
        public int RepositoryId { get; set; }
        public string RepositoryFullName { get; set; }
        public int InstallationId { get; set; }
        public DateTime InstallationDate { get; set; }
    }
}
