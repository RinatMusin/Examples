using CityStar.Meta.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace BookMedica.Models
{
    /// <summary>
    /// Привязка врача к клинике
    /// </summary>
    public class DoctorClinicRelation
    {
        public int ID;
        public int DoctorID;
        public int ClinicID;
        public DoctorClinicRelation(DataRow data)
        {
            ID = DataRowHelper.GetIntValue(data, "id");
            DoctorID = DataRowHelper.GetIntValue(data, "doctor_id");
            ClinicID = DataRowHelper.GetIntValue(data, "clinic_id");
        }
    }
    /// <summary>
    /// Врач
    /// </summary>
    public class Doctor
    {
        private DataRow _data;
        public Doctor(DataRow data)
        {
            Name = DataRowHelper.GetTextValue(data, "name");
            Surname = DataRowHelper.GetTextValue(data, "surname");
            Patronymic = DataRowHelper.GetTextValue(data, "patronymic");
            FullName = DataRowHelper.GetTextValue(data, "full_name");
            Photo = DataRowHelper.GetTextValue(data, "photo");


            Specialization = DataRowHelper.GetTextValue(data, "specialization");
            SpecializationList = DataRowHelper.GetTextValue(data, "specialization_list");
            Position = DataRowHelper.GetTextValue(data, "position");
            AcademicDegree = DataRowHelper.GetTextValue(data, "academic_degree");
            Category = DataRowHelper.GetTextValue(data, "category");
            Clinic = DataRowHelper.GetTextValue(data, "clinic");
            ClinicList = DataRowHelper.GetTextValue(data, "clinic_list");
            SpecializationText = DataRowHelper.GetTextValue(data, "specialization_text");
            Description = DataRowHelper.GetTextValue(data, "description");

            //сначала пробую поле doctor_id
            ID = DataRowHelper.GetIntValue(data, "doctor_id");
            if (ID == 0)
                ID = DataRowHelper.GetIntValue(data, "id");

            PositionID = DataRowHelper.GetIntValue(data, "position_id");
            AcademicDegreeID = DataRowHelper.GetIntValue(data, "academic_degree_id");
            CategoryID = DataRowHelper.GetIntValue(data, "category_id");
            WorkExperience = DataRowHelper.GetIntValue(data, "work_experience");

            RatingCount = DataRowHelper.GetIntValue(data, "rating_count");
            ReviewCount = DataRowHelper.GetIntValue(data, "review_count");
            Rating = DataRowHelper.GetDecimalValue(data, "rating");
            Price = DataRowHelper.GetIntValue(data, "price");

            CityID = DataRowHelper.GetIntValue(data, "city_id");
            UserID = DataRowHelper.GetIntValue(data, "user_id");
            OwnerID = DataRowHelper.GetIntValue(data, "owner_id");
            FlowerBouquetCount = DataRowHelper.GetIntValue(data, "flower_bouquet_count");
            RecomendationCount = DataRowHelper.GetIntValue(data, "recomendation_count");



            Email = DataRowHelper.GetTextValue(data, "email");
            Site = DataRowHelper.GetTextValue(data, "site");
            Skype = DataRowHelper.GetTextValue(data, "skype");

            CreateDate = DataRowHelper.GetDateTimeValue(data, "create_date");
            Moderation = DataRowHelper.GetIntValue(data, "moderation");
            SkipModeration = DataRowHelper.GetIntValue(data, "skip_moderation");


            //Source = ("BookMedica.ru, "+DataRowHelper.GetTextValue(data, "source")).Trim().TrimEnd(',');
            Source = DataRowHelper.GetTextValue(data, "source");

        }

        public Doctor()
        {
            // TODO: Complete member initialization
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
        public string Description;
        public string SpecializationText;
        public int Price;

        public int FlowerBouquetCount;
        public int UserID;
        public int OwnerID;

        public string Email;
        public string Site;
        public string Skype;


        public string Photo;
        public int ID;
        /// <summary>
        /// Имя врача
        /// </summary>
        public string Name;
        /// <summary>
        /// Фамилия врача
        /// </summary>
        public string Surname;
        /// <summary>
        /// Отчество врача
        /// </summary>
        public string Patronymic;
        /// <summary>
        /// Полное имя врача
        /// </summary>
        public string FullName;
        /// <summary>
        /// Врачебная специализация
        /// </summary>
        public string Specialization;
        public string SpecializationList;

        public string Clinic;
        public string ClinicList;
        /// <summary>
        /// Должность
        /// </summary>
        public string Position;
        public int PositionID;
        /// <summary>
        /// Ученая степень
        /// </summary>
        public string AcademicDegree;
        public int AcademicDegreeID;
        /// <summary>
        /// Врачебная категория
        /// </summary>
        public string Category;
        public int CategoryID;

        /// <summary>
        /// Стаж работы, лет
        /// </summary>
        public int WorkExperience;
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
        public int CityID;
        /// <summary>
        /// Кол-во рекомендаций от пользователей по отзывам
        /// </summary>
        public int RecomendationCount;

        /// <summary>
        /// Статус модерации
        /// </summary>
        public int Moderation;
        public int SkipModeration;

        public DateTime CreateDate;
        public string Source;
    }
}