using System.Collections.Generic;

namespace RevitToVR
{
    // contains documents
    public class Database
    {
        public Dictionary<string, RevitDocument> OpenDocuments = new Dictionary<string, RevitDocument>();
    }
}