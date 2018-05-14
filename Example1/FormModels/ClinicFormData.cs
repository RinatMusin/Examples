using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookMedica.FormModels
{
    public class ClinicFormData
    {
        public string WorkTime { get; set; }
        public int ClinicID { get; set; }
        public int CityID { get; set; }
        public string Address { get; set; }
        public string AdvertisementDescription { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Skype { get; set; }
        public string RelativePath { get; set; }

        public string LogoRelativePath { get; set; }
        public string Site { get; set; }
        public int UserID { get; set; }
        public string UserIDHash { get; set; }
        public int PropertyID { get; set; }
        public int TreatmentTypeId { get; set; }
        public int TypeId { get; set; }

        public List<PhotoFormModel> PhotoList { get; set; }

        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }

        public List<ClinicServiceGroupFormData> ServiceList { get; set; }
    }
}