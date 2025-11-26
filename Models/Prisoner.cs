using System;

namespace PrisonApp.Models
{
    /// <summary>
    /// يمثل السجين داخل النظام (الوحدة: إدارة السجناء).
    /// </summary>
    public class Prisoner
    {
        // البيانات الأساسية
        public int PrisonerId { get; set; }          // رقم السجين (معرف داخلي في النظام)
        public string PrisonerNumber { get; set; }   // رقم السجين الظاهر في السجن (إن احتجت)
        public string NationalId { get; set; }       // رقم الهوية / الرقم الوطني
        public string FullName { get; set; }         // الاسم الرباعي
        public string Gender { get; set; }           // ذكر / أنثى
        public DateTime BirthDate { get; set; }      // تاريخ الميلاد
        public string Nationality { get; set; }      // الجنسية
        public string LegalStatus { get; set; }      // موقوف / محكوم / مفرج عنه
        public DateTime EntryDate { get; set; }      // تاريخ دخول السجن
        public DateTime? ReleaseDate { get; set; }   // تاريخ الإفراج (إن وجد)
        public string CrimeDescription { get; set; } // الوصف الجنائي / سبب التوقيف

        // بيانات مرتبطة بالسجن
        public string CurrentBlock { get; set; }     // العنبر الحالي
        public string CurrentRoom { get; set; }      // الغرفة الحالية
        public DateTime? LastMovementDate { get; set; } // تاريخ آخر حركة (للعرض فقط غالباً)

        // معلومات أخرى
        public string CreatedBy { get; set; }        // الموظف المسجّل
        public string Notes { get; set; }            // ملاحظات عامة

        // مرفقات (هنا فقط نخزن المسارات أو معرفات، التنفيذ الفعلي يكون بوحدة أخرى)
        public string AttachmentsPath { get; set; }  // مسار مجلد المرفقات إن وجد
    }
}
