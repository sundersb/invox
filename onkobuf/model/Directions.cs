using System.Collections.Generic;
using System.Xml;

namespace onkobuf.model {
    /// <summary>
    /// SLTRAVM.xml neo suspicion directions dictionary
    /// </summary>
    class Direction {
        int id;
        string title;

        public int ID { get { return id; } }
        public string Title { get { return title; } }
        public string Compound { get { return string.Format("{0} - {1}", id, title); } }

        public Direction(string anId, string aTitle) {
            id = 0;
            int.TryParse(anId, out id);
            title = aTitle;
        }

        /// <summary>
        /// Does this direction match search filter?
        /// </summary>
        /// <param name="value">Search template. Should be lowercased</param>
        public bool Matches(string value) {
            return title.ToLower().Contains(value);
        }
    }

    /// <summary>
    /// Dictionary of directions
    /// </summary>
    class Directions {
        const string XML_NAME = "SLTRAVM.xml";
        const int LIMIT = 50;

        static Directions instance;
        static object flock = new object();

        static Directions Instance {
            get {
                if (instance == null) lock (flock) {
                        if (instance == null) {
                            instance = new Directions(onkobuf.Options.ResourceDirectory + XML_NAME);
                        }
                    }
                return instance;
            }
        }

        /// <summary>
        /// Get all articles
        /// </summary>
        List<Direction> directions = null;

        public static List<Direction> All { get { return Instance.directions; } }

        Directions(string xmlName) {
            directions = new List<Direction>();
            XmlDocument xml = new XmlDocument();
            xml.Load(xmlName);
            XmlElement root = xml.DocumentElement;
            foreach (XmlNode node in root.SelectNodes("rec")) {
                string id = node.SelectSingleNode("id").InnerText;
                string title = node.SelectSingleNode("name").InnerText;
                directions.Add(new Direction(id, title));
            }
        }
    }
}
