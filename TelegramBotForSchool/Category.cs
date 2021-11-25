using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace TelegramBotForSchool
{
    public class Category
    {
        public string Name { get; private set; }
        private List<Document> documents;

        public Category(string name, List<Document> documents)
        {
            Name = name;
            this.documents = documents;
        }

        public string[] GetDocumentsNames()
        {
            string[] documentsNames = new string[documents.Count];

            for (int i = 0; i < documents.Count; i++)
            {
                documentsNames[i] = documents[i].Name;
            }

            return documentsNames;
        }
        public bool TryGetDocumentWithName(string documentName, out Document document)
        {
            document = null;
            foreach (Document doc in documents)
            {
                if (doc.Name == documentName)
                {
                    document = doc;
                    break;
                }
            }

            if (document == null)
                return false;
            else 
                return true;
        }
    }
}

