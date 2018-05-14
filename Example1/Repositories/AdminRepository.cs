using BookMedica.Models;
using CityStar.Core.Data;
using CityStar.Meta.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Web;

namespace BookMedica.Repositories
{
    public class AdminRepository : BaseRepository
    {
        #region Служебные
        private static void addLog(string text)
        {
            CityStar.Core.Logger.Log.Info(text);
        }
        private static DataRow executeDataRow(string commandText)
        {
            var db = DB.GetDB();
            var cmd = db.GetCommand(commandText);
            return executeDataRow(cmd);
        }
        private static DataRow executeDataRow(DbCommand cmd)
        {
            var db = DB.GetDB();
            DataTable data = db.ExecuteDataTable(cmd);
            if (data != null && data.Rows.Count > 0)
                return data.Rows[0];
            else
                return null;
        }
        private static DB db = DB.GetDB();
        #endregion


        #region Модерация отзывов
        public static int GetOpinionListCount(string searchText)
        {
            var db = DB.GetDB();
            QParamCollection paramCollection = new QParamCollection();
            string searchQuery = "";
            if (!string.IsNullOrEmpty(searchText))
            {
                //надо делить на слова
                string[] words = searchText.Split(' ');
                StringBuilder searchSelect = new StringBuilder();
                int wordIndex = 0;
                searchSelect.Append("AND (");
                foreach (var word in words)
                {
                    if (wordIndex > 0)
                        searchSelect.Append(" OR ");
                    searchSelect.AppendFormat("like_text ilike @st{0} OR dislike_text ilike @st{0}", wordIndex);
                    paramCollection.AddParam("st" + wordIndex, DbType.String, "%" + word + "%");
                    wordIndex++;
                }
                searchSelect.Append(")");
                searchQuery = searchSelect.ToString();

            }
            var cmd = db.GetCommand(string.Format(@"SELECT COUNT(o.id) FROM  bookmedica.opinion o
                LEFT JOIN bookmedica.doctor d ON d.id=o.on_id AND o.class_id={0}                
                WHERE o.moderation={1} AND d.moderation={1} {2}", Values.DOCTOR_CLASS_ID, Values.MODERATION_STATUS_OK, searchQuery));
            db.AddInParameters(cmd, paramCollection.ParamList);
            DataRow data = executeDataRow(cmd);
            return DataRowHelper.GetIntValue(data, "count");
        }
        public static List<AdminOpinion> GetOpinionList(int page, string searchText)
        {
            QParamCollection paramCollection = new QParamCollection();
            if (page < 1)
                page = 1;
            int offset = (page - 1) * Values.OPINION_PAGE_SIZE;

            List<AdminOpinion> res = new List<AdminOpinion>();
            var db = DB.GetDB();

            string searchQuery = "";
            if (!string.IsNullOrEmpty(searchText))
            {
                //надо делить на слова
                string[] words = searchText.Split(' ');
                StringBuilder searchSelect = new StringBuilder();
                int wordIndex = 0;
                searchSelect.Append("AND (");
                foreach (var word in words)
                {
                    if (wordIndex > 0)
                        searchSelect.Append(" OR ");
                    searchSelect.AppendFormat("like_text ilike @st{0} OR dislike_text ilike @st{0}", wordIndex);
                    paramCollection.AddParam("st" + wordIndex, DbType.String, "%" + word + "%");
                    wordIndex++;
                }
                searchSelect.Append(")");
                searchQuery = searchSelect.ToString();

            }
            //20160112- отзывы только по врачам, клиник нет
            /*var cmd = db.GetCommand(string.Format(@"SELECT c.name AS clinic_name,c.photo AS clinic_photo, d.full_name AS doctor_name,d.photo AS doctor_photo,o.* FROM  bookmedica.opinion o
                LEFT JOIN bookmedica.doctor d ON d.id=o.on_id AND o.class_id={1}
                LEFT JOIN bookmedica.clinic c ON c.id=o.on_id AND o.class_id={2}
                WHERE c.city_id={0} OR d.city_id={0} ORDER BY id DESC LIMIT {3} OFFSET {4}", cityID, Values.DOCTOR_CLASS_ID, Values.CLINIC_CLASS_ID, Values.OPINION_PAGE_SIZE, offset));
            */
            var cmd = db.GetCommand(string.Format(@"SELECT d.specialization AS doctor_specialization, d.full_name AS doctor_name,d.photo AS doctor_photo,o.* FROM  bookmedica.opinion o
                LEFT JOIN bookmedica.doctor d ON d.id=o.on_id AND o.class_id={0}                
                WHERE o.moderation={4} AND d.moderation={4} {5} ORDER BY create_date DESC LIMIT {2} OFFSET {3}", Values.DOCTOR_CLASS_ID, Values.CLINIC_CLASS_ID, Values.OPINION_PAGE_SIZE, offset, Values.MODERATION_STATUS_OK,
                searchQuery));
            db.AddInParameters(cmd, paramCollection.ParamList);
            DataTable data = db.ExecuteDataTable(cmd);
            if (data != null)
                foreach (DataRow item in data.Rows)
                {
                    res.Add(new AdminOpinion(item));

                }
            return res;
        }
        /// <summary>
        /// Получить список неотмодерированных отзывов
        /// </summary>
        /// <returns></returns>
        public static List<Opinion> GetNotModerationOpinionList()
        {
            var cmd = db.GetCommand("SELECT * FROM bookmedica.opinion WHERE moderation IN (0,1) ORDER BY create_date LIMIT(1);");
            DataTable data = db.ExecuteDataTable(cmd);
            List<Opinion> res = new List<Opinion>();
            if (data != null)
                foreach (DataRow item in data.Rows)
                    res.Add(new Opinion(item));
            return res;
        }
        /// <summary>
        /// Получить неотмодерированное объявление
        /// </summary>
        /// <returns></returns>
        public static Opinion GetNotModerationOpinion()
        {
            var cmd = db.GetCommand(string.Format("SELECT * FROM bookmedica.opinion WHERE moderation IN ({0},{1}) ORDER BY create_date LIMIT(1);", Values.MODERATION_STATUS_NEW, Values.MODERATION_STATUS_EDIT));
            DataRow data = executeDataRow(cmd);
            Opinion res = new Opinion(data);
            return res;
        }
        /// <summary>
        /// Обновить данные по модерации
        /// </summary>
        /// <param name="opinionID"></param>
        /// <param name="moderation"></param>
        /// <param name="userID"></param>
        internal static void UpdateOpinionModeration(int opinionID, int moderation, int moderatorID)
        {
            var cmd = db.GetCommand("UPDATE bookmedica.opinion SET moderation=@moderation,moderator_id=@moderatorID,moderation_date=now() WHERE id=@id;");
            db.AddInParameter(cmd, "moderation", moderation);
            db.AddInParameter(cmd, "moderatorID", moderatorID);
            db.AddInParameter(cmd, "id", opinionID);
            db.ExecuteNonQuery(cmd);
        }


        /// <summary>
        /// Получить кол-во неотмодерированных отзывов
        /// </summary>
        /// <returns></returns>
        internal static int GetNotModerationOpinionCount()
        {
            var cmd = db.GetCommand(string.Format("SELECT COUNT(id) FROM bookmedica.opinion WHERE moderation IN  ({0},{1});", Values.MODERATION_STATUS_NEW, Values.MODERATION_STATUS_EDIT));
            DataRow data = executeDataRow(cmd);
            return DataRowHelper.GetIntValue(data, "count");
        }

        /// <summary>
        /// Обновить кол-во рекомендаций по врачу
        /// </summary>
        /// <param name="p"></param>
        internal static void UpdateDoctorRecomendation(int doctorID)
        {
            var cmd = db.GetCommand(string.Format("SELECT bookmedica.update_doctor_recomendation({0});", doctorID));
            db.ExecuteNonQuery(cmd);
        }

        #endregion

        #region Модерация врачей
        internal static int GetNotModerationDoctorCount()
        {
            var cmd = db.GetCommand(string.Format("SELECT COUNT(id) FROM bookmedica.doctor WHERE moderation IN  ({0},{1});", Values.MODERATION_STATUS_NEW, Values.MODERATION_STATUS_EDIT));
            DataRow data = executeDataRow(cmd);
            return DataRowHelper.GetIntValue(data, "count");
        }
        internal static Doctor GetNotModerationDoctor()
        {
            var cmd = db.GetCommand(string.Format("SELECT * FROM bookmedica.doctor WHERE moderation IN ({0},{1}) ORDER BY skip_moderation NULLS FIRST,create_date LIMIT(1);", Values.MODERATION_STATUS_NEW, Values.MODERATION_STATUS_EDIT));
            DataRow data = executeDataRow(cmd);
            Doctor res = new Doctor(data);
            return res;
        }
        internal static void UpdateDoctorModeration(int doctorID, int moderation, int moderatorID)
        {
            var cmd = db.GetCommand("UPDATE bookmedica.doctor SET moderation=@moderation,moderator_id=@moderatorID,moderation_date=now() WHERE id=@id;");
            db.AddInParameter(cmd, "moderation", moderation);
            db.AddInParameter(cmd, "moderatorID", moderatorID);
            db.AddInParameter(cmd, "id", doctorID);
            db.ExecuteNonQuery(cmd);
        }
        /// <summary>
        /// Обновить кол-во врачей в клинике
        /// </summary>
        /// <param name="p"></param>
        internal static void UpdateClinicDoctorCount(string clinicIDList)
        {
            if (!string.IsNullOrEmpty(clinicIDList))
            {
                var db = DB.GetDB();
                string[] list = clinicIDList.Split(',');
                foreach (var sID in list)
                {
                    var cmd = db.GetCommand(string.Format("SELECT bookmedica.update_clinic_doctor_count({0});", sID));
                    db.ExecuteNonQuery(cmd);
                }
            }
        }
        /// <summary>
        /// Получить список похожих врачей
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        internal static List<Doctor> GetSimilarDoctorList(Doctor doctor)
        {
            var cmd = db.GetCommand("SELECT * FROM bookmedica.doctor WHERE moderation=@moderation AND (LOWER(full_name)=@fullName OR LOWER(surname)=@surname)AND id<>@doctorID ORDER BY full_name");
            db.AddInParameter(cmd, "fullName", doctor.FullName.ToLower());
            db.AddInParameter(cmd, "surName", doctor.Surname.ToLower());
            db.AddInParameter(cmd, "doctorID", doctor.ID);
            db.AddInParameter(cmd, "moderation", Values.MODERATION_STATUS_OK);
            DataTable data = db.ExecuteDataTable(cmd);
            if (data != null)
            {
                var res = new List<Doctor>();
                foreach (DataRow item in data.Rows)
                {
                    res.Add(new Doctor(item));
                }
                return res;
            }
            else
                return null;
        }
        /// <summary>
        /// Пропустить модерацию врача
        /// </summary>
        /// <param name="p"></param>
        internal static void SkipDoctorModeration(int doctorID, int skipModeration)
        {
            var cmd = db.GetCommand("UPDATE bookmedica.doctor SET skip_moderation=@skipModeration WHERE id=@id;");
            db.AddInParameter(cmd, "id", doctorID);
            db.AddInParameter(cmd, "skipModeration", skipModeration);
            db.ExecuteNonQuery(cmd);
        }
        #endregion


        #region Модерация клиник
        internal static int GetNotModerationClinicCount()
        {
            var cmd = db.GetCommand(string.Format("SELECT COUNT(id) FROM bookmedica.clinic WHERE moderation IN  ({0},{1});", Values.MODERATION_STATUS_NEW, Values.MODERATION_STATUS_EDIT));
            DataRow data = executeDataRow(cmd);
            return DataRowHelper.GetIntValue(data, "count");
        }
        internal static Clinic GetNotModerationClinic()
        {
            var cmd = db.GetCommand(string.Format("SELECT * FROM bookmedica.clinic WHERE moderation IN ({0},{1}) ORDER BY skip_moderation NULLS FIRST,create_date LIMIT(1);", Values.MODERATION_STATUS_NEW, Values.MODERATION_STATUS_EDIT));
            DataRow data = executeDataRow(cmd);
            Clinic res = new Clinic(data);
            return res;
        }




        internal static void UpdateClinicModeration(int clinicID, int moderation, int moderatorID)
        {
            var cmd = db.GetCommand("UPDATE bookmedica.clinic SET moderation=@moderation,moderator_id=@moderatorID,moderation_date=now() WHERE id=@id;");
            db.AddInParameter(cmd, "moderation", moderation);
            db.AddInParameter(cmd, "moderatorID", moderatorID);
            db.AddInParameter(cmd, "id", clinicID);
            db.ExecuteNonQuery(cmd);
        }

        internal static void UpdateClinicDoctorCount(int clinicID)
        {
            var db = DB.GetDB();
            var cmd = db.GetCommand(string.Format("SELECT bookmedica.update_clinic_doctor_count({0});", clinicID));
            db.ExecuteNonQuery(cmd);


        }
        /// <summary>
        /// Добавить врача в клинику
        /// </summary>
        /// <param name="doctorID"></param>
        /// <param name="clinicID"></param>
        internal static void AddDoctorClinic(int doctorID, int clinicID)
        {
            var cmd = db.GetCommand(string.Format("INSERT INTO bookmedica.doctor_clinic(doctor_id, clinic_id)VALUES ({0},{1});", doctorID, clinicID));
            db.ExecuteNonQuery(cmd);
        }
        /// <summary>
        /// Обновить данные по клиникам у врача
        /// </summary>
        /// <param name="doctorID"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        internal static void UpdateDoctorClinic(int doctorID, string clinic, string clinicList)
        {
            var cmd = db.GetCommand("UPDATE bookmedica.doctor SET clinic=@clinic,clinic_list=@clinicList WHERE id=@id;");
            db.AddInParameter(cmd, "clinic", clinic);
            db.AddInParameter(cmd, "clinicList", clinicList);
            db.AddInParameter(cmd, "id", doctorID);
            db.ExecuteNonQuery(cmd);
        }
        /// <summary>
        /// Пропустить модерацию клиники
        /// </summary>
        /// <param name="p"></param>
        internal static void SkipClinicModeration(int clinicID, int skipModeration)
        {
            var cmd = db.GetCommand("UPDATE bookmedica.clinic SET skip_moderation=@skipModeration WHERE id=@id;");
            db.AddInParameter(cmd, "id", clinicID);
            db.AddInParameter(cmd, "skipModeration", skipModeration);
            db.ExecuteNonQuery(cmd);
        }
        internal static void UpdateClinicRating(int clinicID)
        {
            var db = DB.GetDB();
            var cmd = db.GetCommand(string.Format("SELECT bookmedica.update_clinic_rating({0});", clinicID));
            db.ExecuteNonQuery(cmd);
        }
        /// <summary>
        /// Получить список похожих клиник по названию или адресу
        /// </summary>
        /// <param name="clinic"></param>
        /// <returns></returns>
        internal static List<Clinic> GetSimilarClinicList(Clinic clinic)
        {
            var cmd = db.GetCommand("SELECT * FROM bookmedica.clinic WHERE moderation=@moderation AND (LOWER(name)=@name OR LOWER(address)=@address)AND id<>@clinicID ORDER BY name");
            db.AddInParameter(cmd, "name", clinic.Name.ToLower());
            db.AddInParameter(cmd, "address", clinic.Address.ToLower());
            db.AddInParameter(cmd, "clinicID", clinic.ID);
            db.AddInParameter(cmd, "moderation", Values.MODERATION_STATUS_OK);
            DataTable data = db.ExecuteDataTable(cmd);
            if (data != null)
            {
                var res = new List<Clinic>();
                foreach (DataRow item in data.Rows)
                {
                    res.Add(new Clinic(item));
                }
                return res;
            }
            else
                return null;
        }
        #endregion



        /// <summary>
        /// Перенос отзывов от одного врача другому
        /// </summary>
        /// <param name="p"></param>
        /// <param name="joinDoctor"></param>
        internal static void TransferOpinions(int targetDoctorID, int sourceDoctorID)
        {
            var cmd = db.GetCommand(string.Format("UPDATE bookmedica.opinion SET on_id={0} WHERE on_id={1}", targetDoctorID, sourceDoctorID));
            db.ExecuteNonQuery(cmd);
        }


        #region Модерация специализаций
        /// <summary>
        /// Кол-во не проверенных специализаций
        /// </summary>
        /// <returns></returns>
        internal static int GetNotModerationSpecializationCount()
        {
            var cmd = db.GetCommand(string.Format("SELECT COUNT(id) FROM bookmedica.valueset WHERE moderation IN ({0},{1}) AND type=1;", Values.MODERATION_STATUS_NEW, Values.MODERATION_STATUS_EDIT));
            DataRow data = executeDataRow(cmd);
            return DataRowHelper.GetIntValue(data, "count");
        }

        /// <summary>
        /// Обновление модерации у специализаций
        /// </summary>
        /// <param name="specializationID"></param>
        /// <param name="moderation"></param>
        /// <param name="moderatorID"></param>
        internal static void UpdateSpecializationModeration(int specializationID, int moderation, int moderatorID)
        {
            var cmd = db.GetCommand("UPDATE bookmedica.valueset SET moderation=@moderation,moderator_id=@moderatorID,moderation_date=now() WHERE id=@id;");
            db.AddInParameter(cmd, "moderation", moderation);
            db.AddInParameter(cmd, "moderatorID", moderatorID);
            db.AddInParameter(cmd, "id", specializationID);
            db.ExecuteNonQuery(cmd);
        }
        #endregion
    }
}