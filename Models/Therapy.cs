using System;
using System.Xml.Serialization;

namespace Veb_projekat.Models
{

    [Serializable]
    [XmlRoot("Therapies")]
    public class Therapy
    {
        [XmlElement("Id")]
        public int Id { get; set; }

        [XmlElement("Description")]
        public string Description { get; set; }

        [XmlElement("StartDate")]
        public DateTime StartDate { get; set; }

        [XmlElement("EndDate")]
        public DateTime EndDate { get; set; }

        [XmlElement("PatientUsername")]
        public string PatientUsername { get; set; }
    }
}
