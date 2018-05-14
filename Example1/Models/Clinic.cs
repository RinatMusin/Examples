using BookMedica.JsonResults;
using CityStar.Meta.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookMedica.Models
{
    /// <summary>
    /// Клиника
    /// </summary>
    public class Clinic
    {
        public int ID;
        /// <summary>
        /// Название медицинского учреждения
        /// </summary>
        public string Name;
        /// <summary>
        /// Адрес
        /// </summary>
        public string Address;
        /// <summary>
        /// Телефон
        /// </summary>
        public string Phone;
        /// <summary>
        /// Список телефонов
        /// </summary>
        public List<string> PhoneList;
        /// <summary>
        /// Описание оказываемых услуг
        /// </summary>
        public string Description;
        /// <summary>
        /// Рекламное описание
        /// </summary>
        public string AdvertisementDescription;
        /// <summary>
        /// Сайт
        /// </summary>
        public string Site;
        /// <summary>
        /// 
        /// </summary>
        public string Email;

        /// <summary>
        /// Тип лечения
        /// </summary>
        public int TreatmentTypeID;
        public string TreatmentType;
        /// <summary>
        /// Тип поликлиники
        /// </summary>
        public int TypeID;
        public string Type;
        /// <summary>
        /// Тип собственности
        /// </summary>
        public int PropertyID;
        public string Property;

        /// <summary>
        /// рейтинг врача
        /// </summary>
        public decimal Rating;
        /// <summary>
        /// Кол-во оценок
        /// </summary>
        public int RatingCount;
        /// <summary>
        /// кол-во отзывов
        /// </summary>
        public int ReviewCount;

        public string Services;
        public string Photo;
        public string Logo;
        public int CityID;
        public int UserID;
        public int OwnerID;
        private System.Data.DataRow _data;
        public decimal Longitude;
        public decimal Latitude;
        public decimal Distance;
        public int DoctorCount;
        public string Skype;
        /// <summary>
        /// Статус модерации
        /// </summary>
        public int Moderation;
        public DateTime CreateDate;
        public int SkipModeration;
        /// <summary>
        /// Время работы клиники
        /// </summary>
        public string WorkTime;
        public Clinic()
        {
        }
        public Clinic(System.Data.DataRow data)
        {
            _data = data;

            ID = DataRowHelper.GetIntValue(data, "id");

            Name = DataRowHelper.GetTextValue(data, "name");
            Address = DataRowHelper.GetTextValue(data, "address");
            //формирую список телефонов
            PhoneList = Utils.GetPhoneList(DataRowHelper.GetTextValue(data, "phone"));
            if (PhoneList.Count > 0)
                Phone = PhoneList[0];

            Email = DataRowHelper.GetTextValue(data, "email");
            Site = DataRowHelper.GetTextValue(data, "site");
            AdvertisementDescription = DataRowHelper.GetTextValue(data, "advertisement_description");
            Description = DataRowHelper.GetTextValue(data, "description");

            PropertyID = DataRowHelper.GetIntValue(data, "type_property_id");
            Property = DataRowHelper.GetTextValue(data, "type_property");
            TypeID = DataRowHelper.GetIntValue(data, "type_id");
            Type = DataRowHelper.GetTextValue(data, "type");
            TreatmentTypeID = DataRowHelper.GetIntValue(data, "treatment_type_id");
            TreatmentType = DataRowHelper.GetTextValue(data, "treatment_type");
            Services = DataRowHelper.GetTextValue(data, "services");
            Photo = DataRowHelper.GetTextValue(data, "photo");
            Logo = DataRowHelper.GetTextValue(data, "logo");
            Skype = DataRowHelper.GetTextValue(data, "skype");

            RatingCount = DataRowHelper.GetIntValue(data, "rating_count");
            ReviewCount = DataRowHelper.GetIntValue(data, "review_count");
            Rating = DataRowHelper.GetDecimalValue(data, "rating");
            CityID = DataRowHelper.GetIntValue(data, "city_id");
            UserID = DataRowHelper.GetIntValue(data, "user_id");
            OwnerID = DataRowHelper.GetIntValue(data, "owner_id");


            Latitude = DataRowHelper.GetDecimalValue(data, "latitude");
            Longitude = DataRowHelper.GetDecimalValue(data, "longitude");
            Distance = DataRowHelper.GetDecimalValue(data, "distance");

            DoctorCount = DataRowHelper.GetIntValue(data, "doctor_count");

            CreateDate = DataRowHelper.GetDateTimeValue(data, "create_date");
            Moderation = DataRowHelper.GetIntValue(data, "moderation");
            SkipModeration = DataRowHelper.GetIntValue(data, "skip_moderation");

            WorkTime = DataRowHelper.GetTextValue(data, "work_time");
        }


        public JSonClinicResult GetJSonResult()
        {
            JSonClinicResult res = new JSonClinicResult
            {
                Id = ID,
                Name = Name,
                Address = Address,
                Phone = Phone,
                Photo = Utils.GetClinicPreview(Photo),
                Rating = GetRating(),

                RatingPercent = GetPercent(),
                ReviewCount = ReviewCount,
                RatingCount = RatingCount,
            };
            return res;
        }

        public string GetRating()
        {
            return string.Format("{0:N1}", Rating);
        }
        public int GetPercent()
        {
            decimal percent = Rating * 100 / 5;
            if (percent > 100)
                percent = 100;
            return (int)percent;
        }
    }
}