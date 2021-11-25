using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace TelegramBotForSchool
{
    public class CategoryCompiler
    {
        public static CategoryCompiler Singleton { get; private set; }

        private string xmlFile = "documents.xml";
        XmlDocument xDoc = new XmlDocument();

        public CategoryCompiler()
        {
            if (Singleton == null)
                Singleton = this;
        }

        public CategoryCollection GetCategories()
        {
            List<Category> categories = new List<Category>();

            xDoc.Load(xmlFile);
            XmlElement xRoot = xDoc.DocumentElement;

            if (xRoot != null)
            {
                var categoriesNodes = xRoot.ChildNodes;
                foreach (XmlElement categoryNode in categoriesNodes)
                {
                    string categoryName = categoryNode.Attributes.GetNamedItem("name").Value;
                    List<Document> documents = new List<Document>();
                    foreach (XmlNode documentNode in categoryNode.ChildNodes)
                    {
                        var documentAttributes = documentNode.Attributes;

                        string documentName = documentAttributes.GetNamedItem("name").Value;
                        string statesString = documentAttributes.GetNamedItem("info").Value;
                        var documentStates = ParseStates(statesString);
                        string headEmail = documentAttributes.GetNamedItem("email").Value;

                        Document newDocument = new Document(documentName, documentStates, headEmail);
                        documents.Add(newDocument);
                    }

                    Category newCategory = new Category(categoryName, documents);
                    categories.Add(newCategory);
                }
            }

            return new CategoryCollection(categories);
        }

        private List<State> ParseStates(string statesString)
        {
            List<State> states = new List<State>();
            var stateNumbers = statesString.Split(' ');
            foreach (string stateNumber in stateNumbers)
            {
                int intNumber;
                if (int.TryParse(stateNumber, out intNumber))
                    states.Add(GetStateWithNumber(intNumber));
            }
            return states;
        }

        private State GetStateWithNumber(int number)
        {
            State state = number switch
            {
                1 => State.FIO,
                2 => State.POSITION,
                3 => State.PREDOSTAVLENIE_V,
                4 => State.YEAR,
                5 => State.NUMBER_OF_COPIES,
                _ => State.FIO
            };
            return state;
        }
    }
}
